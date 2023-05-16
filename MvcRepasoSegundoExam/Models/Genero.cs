using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcRepasoSegundoExam.Models
{
    [Table("Generos")]
    public class Genero
    {
        [Key]
        [Column("IdGenero")]
        public int IdGenero { get; set; }
        [Column("Nombre")]
        public string Nombre { get; set; }
    }
}
