using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MvcRepasoSegundoExam.Models
{
    [Table("LIBROS")]
    public class Libro
    {
        [Key]
        [Column("IDLIBRO")]
        public int IdLibro { get; set; }

        [Column("TITULO")]
        public string Titulo { get; set; }

        [Column("AUTOR")]
        public string Autor { get; set; }

        [Column("EDITORIAL")]
        public string Editorial { get; set; }

        [Column("PORTADA")]
        public string Portada { get; set; }

        [Column("RESUMEN")]
        public string Resumen { get; set; }

        [Column("PRECIO")]
        public int Precio { get; set; }

        [Column("IDGENERO")]
        public int IdGenero { get; set; }
    }
}
