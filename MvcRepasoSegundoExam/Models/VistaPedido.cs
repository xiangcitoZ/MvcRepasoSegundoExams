using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcRepasoSegundoExam.Models
{
    [Table("vistapedidos")]
    public class VistaPedido
    {
        [Key]
        [Column("idvistapedidos")]
        public Int64 IdVistaPedido { get; set; }

        [Column("idUsuario")]
        public int IdUsuario { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Apellidos")]
        public string Apellidos { get; set; }

        [Column("Titulo")]
        public string Titulo { get; set; }

        [Column("Precio")]
        public int Precio { get; set; }

        [Column("Portada")]
        public string Portada { get; set; }

        [Column("Fecha")]
        public DateTime Fecha { get; set; }

        [Column("Preciofinal")]
        public int PrecioFinal { get; set; }

    }
}
