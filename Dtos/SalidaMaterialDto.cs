using System.ComponentModel.DataAnnotations;

namespace GestionBodega.Dtos
{
    public class SalidaMaterialDto
    {
        [Required(ErrorMessage = "Debe seleccionar un técnico responsable.")]
        public int PersonalId { get; set; }

        [Required(ErrorMessage = "Debe ingresar el nombre del proyecto u obra.")]
        [StringLength(100, ErrorMessage = "El nombre del proyecto es muy largo.")]
        public string Proyecto { get; set; } = null!;

        [MinLength(1, ErrorMessage = "Debe agregar al menos un material a la lista.")]
        public List<DetalleSalidaDto> Items { get; set; } = new List<DetalleSalidaDto>();
    }

    public class DetalleSalidaDto
    {
        public int MaterialId { get; set; }

        [Range(1, 999999, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; }
    }
}