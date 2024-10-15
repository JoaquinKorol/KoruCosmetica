using KoruCosmetica.Models.ViewModel;

namespace KoruCosmetica.Services
{
    public interface IServicioAdminTurno
    {
        List<ClientesTurnos> ObtenerTurnosFiltrados(string nombreBuscar, DateOnly? fechaDesde, DateOnly? fechaHasta, int? servicioBuscar);
    }
}
