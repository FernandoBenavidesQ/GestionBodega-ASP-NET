using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using GestionBodega.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionBodega.Controllers
{
    public class AccesoController : Controller
    {
        private readonly GestionBodegaDbContext _context;
        public AccesoController(GestionBodegaDbContext context) { _context = context; }

        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Bodega");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            string passHash = ConvertirSha256(password);
            var usuario = _context.Usuarios.AsNoTracking().FirstOrDefault(u => u.Email == email && u.Password == passHash);

            if (usuario != null)
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Role, usuario.Rol)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
                return RedirectToAction("Index", "Bodega");
            }
            ViewData["Mensaje"] = "Credenciales inválidas";
            return View();
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult Registrar()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Bodega");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(Usuario usuario)
        {
            if (_context.Usuarios.Any(u => u.Email == usuario.Email))
            {
                ViewData["Mensaje"] = "El correo ya está registrado.";
                return View();
            }
            if (ModelState.IsValid)
            {
                usuario.Password = ConvertirSha256(usuario.Password);
                usuario.Rol = "Usuario";

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                ViewData["Mensaje"] = "Cuenta creada. Inicie sesión.";
                return RedirectToAction("Login");
            }

            return View(usuario);
        }

        public static string ConvertirSha256(string texto)
        {
            using (SHA256 hash = SHA256.Create())
            {
                byte[] result = hash.ComputeHash(Encoding.UTF8.GetBytes(texto));
                return string.Concat(result.Select(b => b.ToString("x2")));
            }
        }
    }
}