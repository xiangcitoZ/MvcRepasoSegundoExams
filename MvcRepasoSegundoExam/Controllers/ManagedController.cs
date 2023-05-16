using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcRepasoSegundoExam.Models;
using MvcRepasoSegundoExam.Services;
using System.Security.Claims;

namespace MvcRepasoSegundoExam.Controllers
{
    public class ManagedController : Controller
    {

        private ServiceUsuarios service;
        private ServiceStorageBlobs serviceblobs;

        public ManagedController(ServiceUsuarios service, ServiceStorageBlobs serviceblobs)
        {
            this.service = service;
            this.serviceblobs = serviceblobs;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            string token = await this.service.GetTokenAsync(username, password);

            if (token == null)
            {
                ViewData["MENSAJE"] = "Usuario/Password incorrectos";
            }
            else
            {
                HttpContext.Session.SetString("TOKEN", token);
                ClaimsIdentity identity =
                    new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, 
                    ClaimTypes.Name, ClaimTypes.Role);

                identity.AddClaim(new Claim(ClaimTypes.Name, username));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, password));
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        userPrincipal, new AuthenticationProperties
                        {
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                        });
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("TOKEN");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string nombre, string apellidos, string email, string username, string password, IFormFile file)
        {
            string blobName = file.FileName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.serviceblobs.UploadBlobAsync
                ("imagenesperfil", blobName, stream);

            }

            await this.service.GetRegisterUserAsync(nombre, apellidos, email, username, password, blobName);

            return RedirectToAction("Login");
        }
    }
}
