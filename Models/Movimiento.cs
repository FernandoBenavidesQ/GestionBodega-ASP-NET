using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionBodega.Models
{
    public partial class Movimiento
    {
        [Key]
        public int Id { get; set; }

        public int MaterialId { get; set; }
        public int PersonalId { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        [StringLength(20)]
        public string Tipo { get; set; } = null!;

        [StringLength(20)]
        public string Estado { get; set; } = "ABIERTO";
        public DateTime Fecha { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? NroCotizacion { get; set; }
        [StringLength(100)]
        public string? Proyecto { get; set; }

        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; } = null!;

        [ForeignKey("PersonalId")]
        public virtual Personal Personal { get; set; } = null!;
    }
}