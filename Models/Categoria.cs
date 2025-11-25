using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionBodega.Models
{
    public partial class Categoria
    {
        public Categoria()
        {
            Materials = new HashSet<Material>();
            Catalogos = new HashSet<Catalogo>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = null!;

        public virtual ICollection<Material> Materials { get; set; }
        public virtual ICollection<Catalogo> Catalogos { get; set; }
    }
}