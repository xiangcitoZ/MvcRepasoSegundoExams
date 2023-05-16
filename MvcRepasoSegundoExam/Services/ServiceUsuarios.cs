using Azure.Storage.Blobs;
using MvcRepasoSegundoExam.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System.Net.Http.Headers;
using System.Text;

#region PROCEDURES

//Create PROCEDURE SP_LIBROS_GENERO_PAGINAR
//    (@POSICION INT, @IDGENERO INT)
//    AS
//        SELECT POSICION, IdLibro, Titulo, Autor, Editorial, Portada, Resumen, Precio, IdGenero
//		FROM
//            (SELECT CAST(
//                ROW_NUMBER() OVER(ORDER BY Autor DESC) AS INT) AS POSICION,
//                IdLibro, Titulo, Autor, Editorial, Portada, Resumen, Precio, IdGenero
//            FROM LIBROS
//            WHERE IdGenero = @IDGENERO) AS QUERY
//        WHERE QUERY.POSICION >= @POSICION AND QUERY.POSICION<(@POSICION + 6)
//    GO

//create PROCEDURE SP_LIBROS_PAGINAR
//    (@POSICION INT)
//    AS
//        SELECT POSICION, IdLibro, Titulo, Autor, Editorial, Portada, Resumen, Precio, IdGenero
//		FROM
//            (SELECT CAST(
//                ROW_NUMBER() OVER(ORDER BY Autor DESC) AS INT) AS POSICION,
//                IdLibro, Titulo, Autor, Editorial, Portada, Resumen, Precio, IdGenero
//            FROM LIBROS) AS QUERY
//        WHERE QUERY.POSICION >= @POSICION AND QUERY.POSICION<(@POSICION + 6)
//    GO

#endregion

namespace MvcRepasoSegundoExam.Services
{
    public class ServiceUsuarios
    {
        private BlobServiceClient client;

        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApiUsuarios;

        public ServiceUsuarios(IConfiguration configuration, BlobServiceClient client)
        {
            this.UrlApiUsuarios =
                configuration.GetValue<string>("ApiUrls:ApiRepasoSegundoExam");
            this.Header =
                new MediaTypeWithQualityHeaderValue("application/json");

            this.client = client;
        }

        public async Task<string> GetTokenAsync(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/login";
                client.BaseAddress = new Uri(this.UrlApiUsuarios);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    UserName = username,
                    Password = password
                };

                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                if(response.IsSuccessStatusCode)
                {
                    string data =
                        await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(data);
                    string token =
                        jsonObject.GetValue("response").ToString();
                    return token;
                } else
                {
                    return null;
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiUsuarios);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data =
                        await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }

            }
        }

        private async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiUsuarios);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data =
                        await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task GetRegisterUserAsync
            (string nombre, string apellidos, string email, string username, string password, string imagen)
        {

            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/register";
                client.BaseAddress = new Uri(this.UrlApiUsuarios);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                Usuario usuario = new Usuario();
                usuario.IdUsuario = 0;
                usuario.Nombre = nombre;
                usuario.Apellidos = apellidos;
                usuario.Email = email;
                usuario.UserName = username;
                usuario.Password = password;
                usuario.ImagenPerfil = imagen;

                string json = JsonConvert.SerializeObject(usuario);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }


        //METODO PROTEGIDO PARA RECUPERAR EL PERFIL
        public async Task<Usuario> GetPerfilUsuarioAsync
            (string token)
        {
            string request = "/api/usuarios/perfilusuario";
            Usuario usuario = await
                this.CallApiAsync<Usuario>(request, token);
            return usuario;
        }


        public async Task<List<Libro>> GetProductosAsync()
        {
            string request = "/api/tienda/productos";
            List<Libro> libros = await
                this.CallApiAsync<List<Libro>>(request);
            return libros;
        }

        public async Task<Libro> FindProductoAsync(int idproducto)
        {
            string request = "/api/tienda/producto/" + idproducto;
            return await this.CallApiAsync<Libro>(request);
        }

        public async Task<List<VistaPedido>> GetPedidosAsync(string token)
        {
            string request = "/api/tienda/pedidos";
            return await this.CallApiAsync<List<VistaPedido>>(request, token);
        }

        public async Task CrearPedido(Libro libro, Usuario usuario, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/tienda/realizarpedido";
                client.BaseAddress = new Uri(this.UrlApiUsuarios);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);

                Pedido pedido = new Pedido();
                pedido.IdPedido = 0;
                pedido.IdFactura = 0;
                pedido.Fecha = DateTime.UtcNow;
                pedido.IdLibro = libro.IdLibro;
                pedido.IdUsuario = usuario.IdUsuario;
                pedido.Cantidad = 1;

                string json = JsonConvert.SerializeObject(pedido);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }


        public async Task SendMailAsync(string email, string asunto, string mensaje)
        {
            string urlEmail =
                "https://prod-121.westeurope.logic.azure.com:443/workflows/6887c1d9014d4ef6b74e454c205fe5d0/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=r-dPn0DV8nYjNBsc8cXTzRZY5ym8jczP6Gvkn8ZIZDo";
            var model = new
            {
                email = email,
                subject = asunto,
                mensaje = mensaje
            };

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                string json = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync(urlEmail, content);
            }
        }

    }
}
