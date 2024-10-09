namespace KoruCosmetica.Models.ViewModel
{
    public class TurnosViewModel
    {
        public List<ClientesTurnos> Turnos { get; set; } = new List<ClientesTurnos>();
        public DateOnly? FechaFiltro { get; set; }
        public int? ClienteIDFiltro { get; set; }
        public List<Clientes> Clientes { get; set; } = new List<Clientes>();
    }

}
