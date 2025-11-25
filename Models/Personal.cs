using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionBodega.Models
{
    public partial class Personal
    {
        public Personal()
        {
            Movimiento = new HashSet<Movimiento>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El RUT es obligatorio.")]
        [ValidaRut(ErrorMessage = "El RUT ingresado no es válido.")]
        [StringLength(12)]
        public string Rut { get; set; } = null!;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50)]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El cargo es obligatorio.")]
        [StringLength(50)]


        public string Cargo { get; set; } = null!;

        public bool Activo { get; set; } = true;

        public virtual ICollection<Movimiento> Movimiento { get; set; }
    }
}