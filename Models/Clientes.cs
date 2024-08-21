using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace KoruCosmetica.Models
{
    public class Clientes
    {
        [Key]
        public int ClienteID { get; set; }
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ]+(?:\s[A-Za-zÁÉÍÓÚáéíóúÑñ]+)?$", ErrorMessage = "Ingrese un nombre valido")]
        public string Nombre { get; set; }
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ]+(?:[-\s][A-Za-zÁÉÍÓÚáéíóúÑñ]+)?$", ErrorMessage = "Ingrese un apellido valido")]
        public string Apellido { get; set; }
        [EmailAddress]
        public string Mail { get; set; }
        [RegularExpression(@"^(\+54\s?)?(9\s?)?(11|[2368]\d{2})\s?\d{4}\s?\d{4}$", ErrorMessage = "Ingrese un teléfono válido")]
        public string Telefono { get; set; }
        public ICollection<Turnos>? Turnos{ get; }
        public Clientes()
        {
            
        }
    }
}
