using System.ComponentModel.DataAnnotations;

namespace KoruCosmetica.Models
{
    public class Servicios
    {
        [Key]
        public int ServicioID { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int Duracion {  get; set; }

        public ICollection<Turnos> Turnos { get; }
        public Servicios()
        {
            
        }
    }
}
