using NuGet.Protocol.Core.Types;
using System.ComponentModel.DataAnnotations;

namespace KoruCosmetica.Models
{
    public class HorariosDisponibles
    {
        [Key]
        public int HorarioID { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
        public string? Estado { get; set; }

        public int DiaID {  get; set; }
        public int? ProfesionalID {  get; set; }

        public Profesionales? Profesional { get; set; }
    }
}
