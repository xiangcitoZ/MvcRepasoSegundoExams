using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcRepasoSegundoExam.Models
{
    [Table("USUARIOS_REPASO")]
    public class Usuario
    {
        [Key]
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }

        [Column("NOMBRE")]
        public string Nombre { get; set; }

        [Column("APELLIDOS")]
        public string Apellidos { get; set; }

        [Column("EMAIL")]
        public string Email { get; set; }

        [Column("USERNAME")]
        public string UserName { get; set; }

        [Column("PASSWORD")]
        public string Password { get; set; }

        [Column("IMAGENPERFIL")]
        public string ImagenPerfil { get; set; }


    }
}
