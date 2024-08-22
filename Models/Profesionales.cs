using System.ComponentModel.DataAnnotations;

namespace KoruCosmetica.Models
{
    public class Profesionales
    {
        [Key]
        public int ProfesionalID {  get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        public ICollection<Turnos> Turnos { get; }
       
    }
}