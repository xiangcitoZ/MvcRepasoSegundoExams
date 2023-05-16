using MvcRepasoSegundoExam.Models;
using Microsoft.AspNetCore.Mvc;
using MvcRepasoSegundoExam.Services;

namespace MvcRepasoSegundoExam.Controllers
{
    public class TiendaController : Controller
    {
        private ServiceUsuarios service;
        private ServiceStorageBlobs serviceBlob;
        public TiendaController(ServiceUsuarios service, ServiceStorageBlobs serviceAzureBlob)
        {
            this.service = service;
            this.serviceBlob = serviceAzureBlob;
        }

        public async Task<IActionResult> Index(int? pagina)
        {

            int elementosPorPagina = 6; // Cantidad de elementos que deseas mostrar en cada página
            int paginaActual = pagina ?? 1; // Si no se proporciona una página, mostrar la primera por defecto

            // Obtener todos los productos
            List<Libro> productos = await this.service.GetProductosAsync();

            // Calcular la cantidad total de páginas y asegurarse de que la página actual sea válida
            int totalElementos = productos.Count;
            int totalPaginas = (int)Math.Ceiling((double)totalElementos / elementosPorPagina);
            paginaActual = paginaActual < 1 ? 1 : paginaActual;
            paginaActual = paginaActual > totalPaginas ? totalPaginas : paginaActual;

            // Obtener los elementos para la página actual
            int indiceInicio = (paginaActual - 1) * elementosPorPagina;
            List<Libro> productosPaginaActual = productos.Skip(indiceInicio).Take(elementosPorPagina).ToList();

            // Obtener las imágenes de portada para los productos
            List<BlobModel> listBlobs = await this.serviceBlob.GetBlobsAsync("imageneslibros");

            // Pasar los productos, las imágenes de portada y la información de paginación a la vista
            ViewData["Productos"] = productosPaginaActual;
            ViewData["Portadas"] = listBlobs;
            ViewData["PaginaActual"] = paginaActual;
            ViewData["TotalPaginas"] = totalPaginas;

            return View();

            //List<Libro> productos = await this.service.GetProductosAsync();
            //List<BlobModel> listBlobs = await this.serviceBlob.GetBlobsAsync("imageneslibros");
            //ViewData["PORTADAS"] = listBlobs;
            //return View(productos);
        }

        public async Task<IActionResult> Details(int id)
        {
            Libro producto = await this.service.FindProductoAsync(id);
            List<BlobModel> listBlobs = await this.serviceBlob.GetBlobsAsync("imageneslibros");
            ViewData["PORTADAS"] = listBlobs;
            return View(producto);
        }

        public async Task<IActionResult> RealizarPedido(int idLibro)
        {
            string token = HttpContext.Session.GetString("TOKEN");
            Usuario user = await this.service.GetPerfilUsuarioAsync(token);

            Libro producto = await this.service.FindProductoAsync(idLibro);
            await this.service.CrearPedido(producto, user, token);
            return RedirectToAction("Pedidos", "Usuarios");
        }
    }
}
