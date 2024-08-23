using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;


namespace KoruCosmetica.Models
{
    public class Turnos
    {
        [Key]
        public int TurnosId { get; set; }
        public DateOnly Fecha { get; set; }
        public TimeOnly Hora { get;  set; }
        public string? Estado { get; private set; } = "Pendiente";

        // Foreign Keys
        // Las Foreign Keys se definen como nullable para que no sean obligatorias
        [ForeignKey("ClienteID")]
        public int? ClienteID { get; set; }
        [ForeignKey("ServicioID")]
        public int? ServicioID { get; set; }
        [ForeignKey("ProfesionalID")]
        public int? ProfesionalID { get; set; }

        // Propiedades de navegación opcionales si utilizas Entity Framework
        public Clientes? Cliente { get; set; }
        public Servicios? Servicio { get; set; }
        public Profesionales? Profesional { get; set; }

      
    }
}
