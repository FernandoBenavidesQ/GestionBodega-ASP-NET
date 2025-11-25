using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionBodega.Models
{
    public partial class Material
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [StringLength(200)]
        public string? Descripcion { get; set; }

        [Required]
        [Range(0, 999999)]
        public int Stock { get; set; }

        public int Largo { get; set; }

        [Required]
        [StringLength(20)]
        public string Unidad { get; set; } = "Unidad";

        [ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; } = null!;
    }
}