using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace KoruCosmetica.Models
{
    public class Clientes
    {
        [Key]
        public int ClienteID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Mail { get; set; }
        public string Telefono { get; set; }
        public ICollection<Turnos>? Turnos{ get; }
        public Clientes()
        {
            
        }
    }
}
