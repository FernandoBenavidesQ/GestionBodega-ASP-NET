using GestionBodega.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionBodega.Controllers
{
    [Authorize]
    public class PersonalController : Controller
    {
        private readonly GestionBodegaDbContext _context;

        public PersonalController(GestionBodegaDbContext context) { _context = context; }

        public IActionResult Index()
        {
            return View(_context.Personals.Where(p => p.Activo).ToList());
        }

        public IActionResult Crear() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Personal personal)
        {
            if (ModelState.IsValid)
            {
                personal.Activo = true;
                _context.Personals.Add(personal);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(personal);
        }

        public IActionResult Eliminar(int id)
        {
            var p = _context.Personals.Find(id);
            if (p != null)
            {
                p.Activo = false;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}