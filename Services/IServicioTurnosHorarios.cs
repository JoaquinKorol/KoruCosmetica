using KoruCosmetica.Models.ViewModel;

namespace KoruCosmetica.Services
{
    public interface IServicioTurnosHorarios
    {
        Task<string[]> ObtenerHorariosDisponibles(DateTime fecha);
        Task<bool> SacarTurno(TurnosHorarios viewModel, string clienteIdClaim);
        Task<bool> CancelarTurno(int id);
    }
}
