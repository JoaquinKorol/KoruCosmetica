using KoruCosmetica.Models.ViewModel;

namespace KoruCosmetica.Services
{
    public interface IServicioClienteTurno
    {
        List<ClientesTurnos> ObtenerTurnosPorCliente(int clienteId);
    }
}
