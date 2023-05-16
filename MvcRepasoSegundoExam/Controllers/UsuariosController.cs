using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcRepasoSegundoExam.Filters;
using MvcRepasoSegundoExam.Models;
using MvcRepasoSegundoExam.Services;

namespace MvcRepasoSegundoExam.Controllers
{
    public class UsuariosController : Controller
    {
        private ServiceUsuarios service;
        private ServiceStorageBlobs serviceblob;

        public UsuariosController(ServiceUsuarios service, ServiceStorageBlobs serviceblob)
        {
            this.service = service;
            this.serviceblob = serviceblob;
        }



        [AuthorizeUsuarios]
        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeUsuarios]
        public async Task<IActionResult> Perfil()
        {
            string token =
                HttpContext.Session.GetString("TOKEN");
            Usuario usuario = await
                this.service.GetPerfilUsuarioAsync(token);
            BlobModel blobPerfil = await this.serviceblob.FindBlobPerfil("imagenesperfil", usuario.ImagenPerfil, usuario.UserName);
            ViewData["IMAGEN_PERFIL"]= blobPerfil;
            return View(usuario);
        }

        public async Task<IActionResult> Pedidos()
        {

            string token = HttpContext.Session.GetString("TOKEN");
            List<VistaPedido> pedidos = await this.service.GetPedidosAsync(token);
            return View(pedidos);
        }

        public async Task<IActionResult> Mail()
        {
            string email = "EMAIL QUE ENVIA EL MENSAJE";
            string asunto = "Aprobar examen";
            string mensaje = "Si lees esto, el examen estará fácil";

            await this.service.SendMailAsync(email, asunto, mensaje);
            return View();
        }


    }
}
