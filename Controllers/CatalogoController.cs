using GestionBodega.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GestionBodega.Controllers
{
    [Authorize]
    public class CatalogoController : Controller
    {
        private readonly GestionBodegaDbContext _context;

        public CatalogoController(GestionBodegaDbContext context) { _context = context; }

        public IActionResult Index(string buscar)
        {
            var query = _context.Catalogos.Include(c => c.Categoria).AsQueryable();
            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(c => c.Modelo.Contains(buscar) || c.Marca.Contains(buscar));
            }
            return View(query.ToList());
        }

        public IActionResult Crear()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Catalogo catalogo)
        {
            ModelState.Remove("Categoria");
            ModelState.Remove("Detalle");

            if (ModelState.IsValid)
            {
                catalogo.Marca = catalogo.Marca?.ToUpper();
                _context.Catalogos.Add(catalogo);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", catalogo.CategoriaId);
            return View(catalogo);
        }

        public IActionResult Editar(int? id)
        {
            if (id == null) return NotFound();
            var catalogo = _context.Catalogos.Find(id);
            if (catalogo == null) return NotFound();
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", catalogo.CategoriaId);
            return View(catalogo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Catalogo catalogo)
        {
            if (id != catalogo.Id) return NotFound();
            ModelState.Remove("Categoria");
            ModelState.Remove("Detalle");

            if (ModelState.IsValid)
            {
                _context.Update(catalogo);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", catalogo.CategoriaId);
            return View(catalogo);
        }

        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            var item = _context.Catalogos.Find(id);
            if (item != null) { _context.Catalogos.Remove(item); _context.SaveChanges(); }
            return RedirectToAction(nameof(Index));
        }
    }
}