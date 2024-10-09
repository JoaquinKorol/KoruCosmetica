using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KoruCosmetica.Models
{

    public class Clientes
    {
        [Key]
        public int ClienteID { get; set; }
        [Required(ErrorMessage = "El campo de nombre es obligatorio.")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ]+(?:\s[A-Za-zÁÉÍÓÚáéíóúÑñ]+)?$", ErrorMessage = "Ingrese un nombre valido.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo de apellido es obligatorio.")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ]+(?:[-\s][A-Za-zÁÉÍÓÚáéíóúÑñ]+)?$", ErrorMessage = "Ingrese un apellido valido.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El campo de contraseña es obligatorio.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "La contraseña debe tener al menos 8 caracteres, incluyendo mayúsculas, minúsculas y números.")]
        public string Contraseña { get; set; }

        [Required(ErrorMessage = "El campo de mail es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un email valido.")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "El campo de teléfono es obligatorio.")]
        [RegularExpression(@"^(\+54\s?)?(9\s?)?(11|[2368]\d{2})\s?\d{4}\s?\d{4}$", ErrorMessage = "Ingrese un teléfono válido.")]
        public string Telefono { get; set; }
        public ICollection<Turnos>? Turnos{ get; }
        public Clientes()
        {
            
        }
    }
}
