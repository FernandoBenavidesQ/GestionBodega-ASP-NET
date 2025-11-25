using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionBodega.Models
{
    public partial class Catalogo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio.")]
        [StringLength(50)]
        public string Tipo { get; set; } = null!;

        [StringLength(50)]
        public string? Marca { get; set; }

        [Required(ErrorMessage = "El modelo es obligatorio.")]
        [StringLength(100)]
        public string Modelo { get; set; } = null!;

        public string? Detalle { get; set; }

        [Required]
        public string UnidadMedida { get; set; } = "Unidad";

        [ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; } = null!;
    }
}