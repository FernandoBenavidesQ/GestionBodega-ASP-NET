using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GestionBodega.Models;
using System.Linq;

namespace GestionBodega.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly GestionBodegaDbContext _context;

        public TicketsController(GestionBodegaDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var historial = _context.Movimientos
                .Include(m => m.Personal)
                .Where(m => m.NroCotizacion != null)
                .AsEnumerable()
                .GroupBy(m => m.NroCotizacion)
                .Select(g => new TicketResumenViewModel
                {
                    NroTicket = g.Key,
                    Fecha = g.First().Fecha,
                    Tipo = g.First().Tipo,
                    NombreTecnico = g.First().Personal?.Nombre + " " + g.First().Personal?.Apellido,
                    RutTecnico = g.First().Personal?.Rut,
                    Proyecto = g.First().Proyecto,
                    TotalItems = g.Count(),
                    TotalPiezas = g.Sum(x => x.Cantidad)
                })
                .OrderByDescending(t => t.Fecha)
                .ToList();

            return View(historial);
        }

        public IActionResult Detalle(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var movimientos = _context.Movimientos
                .Include(m => m.Material)
                .Include(m => m.Personal)
                .Where(m => m.NroCotizacion == id)
                .ToList();

            if (!movimientos.Any()) return RedirectToAction(nameof(Index));

            return View(movimientos);
        }
    }
}