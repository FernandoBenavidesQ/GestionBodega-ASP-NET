using System;
using System.Collections.Generic;

namespace GestionBodega.Models
{
    public class TicketResumenViewModel
    {
        public string NroTicket { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = null!;
        public string NombreTecnico { get; set; } = null!;
        public string? RutTecnico { get; set; }
        public string? Proyecto { get; set; }
        public int TotalItems { get; set; }
        public int TotalPiezas { get; set; }
    }

    public class DetalleTicketViewModel
    {
        public string NroTicket { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = null!;
        public string NombreTecnico { get; set; } = null!;
        public string RutTecnico { get; set; } = null!;
        public string Cargo { get; set; } = null!;
        public string Proyecto { get; set; } = null!;
        public List<ItemDetalle> Items { get; set; } = new List<ItemDetalle>();
    }

    public class ItemDetalle
    {
        public string NombreMaterial { get; set; } = null!;
        public string Marca { get; set; } = null!;
        public string Unidad { get; set; } = null!;
        public int Cantidad { get; set; }
    }
}