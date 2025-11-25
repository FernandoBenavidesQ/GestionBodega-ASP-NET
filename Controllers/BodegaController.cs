using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GestionBodega.Models;
using GestionBodega.Dtos;

namespace GestionBodega.Controllers
{
    [Authorize]
    public class BodegaController : Controller
    {
        private readonly GestionBodegaDbContext _context;

        public BodegaController(GestionBodegaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
           
            ViewBag.Categorias = _context.Categorias.OrderBy(c => c.Nombre).ToList();
            ViewBag.Personal = _context.Personals.Where(p => p.Activo).OrderBy(p => p.Nombre).ToList();

            var inventario = _context.Materials
                .Include(m => m.Categoria)
                .OrderBy(m => m.Nombre)
                .ToList();

            return View(inventario);
        }

        [HttpGet]
        public IActionResult ObtenerCatalogoPorCategoria(int categoriaId)
        {
            var datosRaw = _context.Catalogos
                .Where(c => c.CategoriaId == categoriaId)
                .Select(c => new { c.Id, c.Modelo, c.Marca, c.Tipo })
                .ToList();

            var productos = datosRaw
                .Select(c => new {
                    c.Id,
                    Texto = $"{c.Modelo} - {c.Marca} ({c.Tipo})"
                })
                .OrderBy(c => c.Texto)
                .ToList();

            return Json(productos);
        }

        [HttpPost]
        public IActionResult GuardarIngreso(int catalogoId, int cantidad)
        {
            if (cantidad <= 0) return Json(new { success = false, message = "La cantidad debe ser mayor a 0." });

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var ficha = _context.Catalogos.Find(catalogoId);
                    if (ficha == null) throw new Exception("El producto no existe en el catálogo.");

                    var material = _context.Materials.FirstOrDefault(m => m.Nombre == ficha.Modelo && m.Descripcion == ficha.Marca);

                    if (material != null)
                    {
                        material.Stock += cantidad;
                        _context.Materials.Update(material);
                    }
                    else
                    {
                        material = new Material
                        {
                            CategoriaId = ficha.CategoriaId,
                            Nombre = ficha.Modelo,
                            Descripcion = ficha.Marca,
                            Unidad = ficha.UnidadMedida,
                            Stock = cantidad,
                            Largo = 0
                        };
                        _context.Materials.Add(material);
                    }
                    _context.SaveChanges();
                    _context.Movimientos.Add(new Movimiento
                    {
                        MaterialId = material.Id,
                        PersonalId = 1, 
                        Fecha = DateTime.Now,
                        Tipo = "ENTRADA",
                        Estado = "CERRADO", 
                        Cantidad = cantidad,
                        Proyecto = "Abastecimiento Bodega",
                        NroCotizacion = "ING-" + DateTime.Now.ToString("HHmmss")
                    });

                    _context.SaveChanges();
                    transaction.Commit();
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null) return NotFound();
            var material = await _context.Materials.Include(m => m.Categoria).FirstOrDefaultAsync(m => m.Id == id);
            if (material == null) return NotFound();
            return View(material);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Material material)
        {
            var dbMat = _context.Materials.Include(m => m.Categoria).FirstOrDefault(m => m.Id == id);
            if (dbMat != null)
            {
                dbMat.Stock = material.Stock;
                if (dbMat.Categoria.Nombre.Contains("Cable")) dbMat.Largo = material.Largo;

                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Eliminar(int id)
        {
            try
            {
                var mat = _context.Materials.Find(id);
                if (mat != null)
                {
                    _context.Materials.Remove(mat);
                    _context.SaveChanges();
                }
            }
            catch
            {
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult ObtenerMaterialesParaSalida(int categoriaId)
        {
            var materiales = _context.Materials
                .Where(m => m.CategoriaId == categoriaId && m.Stock > 0)
                .Select(m => new {
                    m.Id,
                    m.Nombre,
                    m.Descripcion,
                    m.Stock,
                    m.Unidad
                })
                .OrderBy(m => m.Nombre)
                .ToList();
            return Json(materiales);
        }
        [HttpGet]
        public IActionResult BuscarMaterial(string term)
        {
            var results = _context.Materials
                .Where(m => (m.Nombre.Contains(term) || m.Descripcion.Contains(term)) && m.Stock > 0)
                .Select(m => new {
                    id = m.Id,
                    label = $"{m.Nombre} - {m.Descripcion}",
                    stock = m.Stock
                })
                .Take(10).ToList();
            return Json(results);
        }
        [HttpPost]
        public IActionResult ProcesarSalida([FromBody] SalidaMaterialDto dto)
        {
            if (!ModelState.IsValid) return Json(new { success = false, message = "Datos incompletos." });

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    string ticket = $"COT-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";

                    foreach (var item in dto.Items)
                    {
                        var mat = _context.Materials.Find(item.MaterialId);

                        if (mat == null) throw new Exception($"Material ID {item.MaterialId} no encontrado.");
                        if (mat.Stock < item.Cantidad) throw new Exception($"Stock insuficiente para: {mat.Nombre}. Disponible: {mat.Stock}");

                        mat.Stock -= item.Cantidad;

                        _context.Movimientos.Add(new Movimiento
                        {
                            MaterialId = item.MaterialId,
                            PersonalId = dto.PersonalId,
                            Cantidad = item.Cantidad,
                            Tipo = "SALIDA",
                            Estado = "ABIERTO",
                            Fecha = DateTime.Now,
                            NroCotizacion = ticket,
                            Proyecto = dto.Proyecto
                        });
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                    return Json(new { success = true, message = $"Salida registrada. Ticket: {ticket}" });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        [HttpGet]
        public IActionResult ObtenerProyectosPorTecnico(int personalId)
        {
            var proyectos = _context.Movimientos
                .Where(m => m.PersonalId == personalId && m.Tipo == "SALIDA" && m.Estado == "ABIERTO")
                .Select(m => m.Proyecto)
                .Distinct()
                .ToList();
            return Json(proyectos);
        }

        [HttpGet]
        public IActionResult ObtenerMaterialesPendientes(int personalId, string proyecto)
        {
            var raw = _context.Movimientos
                .Include(m => m.Material)
                .Where(m => m.PersonalId == personalId && m.Proyecto == proyecto)
                .ToList();

            var pendientes = raw.GroupBy(m => m.MaterialId)
                .Select(g => new {
                    id = g.Key,
                    label = g.First().Material.Nombre,
                    pendiente = g.Where(x => x.Tipo == "SALIDA").Sum(x => x.Cantidad) - g.Where(x => x.Tipo == "ENTRADA").Sum(x => x.Cantidad)
                })
                .Where(x => x.pendiente > 0)
                .ToList();

            return Json(pendientes);
        }
        [HttpPost]
        public IActionResult ProcesarDevolucion([FromBody] SalidaMaterialDto dto)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    string ticketDev = $"DEV-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";

                    foreach (var item in dto.Items)
                    {
                        var mat = _context.Materials.Find(item.MaterialId);
                        if (mat != null)
                        {
                            mat.Stock += item.Cantidad;
                        }
                        _context.Movimientos.Add(new Movimiento
                        {
                            MaterialId = item.MaterialId,
                            PersonalId = dto.PersonalId,
                            Cantidad = item.Cantidad,
                            Tipo = "ENTRADA",
                            Estado = "CERRADO",
                            Fecha = DateTime.Now,
                            NroCotizacion = ticketDev,
                            Proyecto = dto.Proyecto
                        });
                    }

                    var salidasAbiertas = _context.Movimientos
                        .Where(m => m.PersonalId == dto.PersonalId && m.Proyecto == dto.Proyecto && m.Estado == "ABIERTO")
                        .ToList();

                    foreach (var salida in salidasAbiertas)
                    {
                        salida.Estado = "CERRADO"; 
                    }

                    _context.SaveChanges();
                    transaction.Commit();
                    return Json(new { success = true, message = "Devolución procesada. Proyecto cerrado/archivado." });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }
    }
}