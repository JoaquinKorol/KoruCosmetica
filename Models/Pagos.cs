using System.ComponentModel.DataAnnotations;

namespace KoruCosmetica.Models
{
    public class Pagos
    {
        [Key]
        public int PagosID { get; set; }
        public DateTime? Fecha { get; set; }
        public float? Monto { get; set; }
        public string Metodo_Pago { get; set; }
        public string Estado { get; set; }

        // Claves foráneas
        public int? ClienteID { get; set; }
        public int? TurnoID { get; set; }
        public Clientes Cliente { get; set; }
        public Turnos Turno { get; set; }

        public Pagos()
        {
            
        }
    }
}
