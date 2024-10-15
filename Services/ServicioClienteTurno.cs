using KoruCosmetica.Data;
using KoruCosmetica.Models.ViewModel;
using System.Security.Claims;

namespace KoruCosmetica.Services
{
    public class ServicioClienteTurno : IServicioClienteTurno
    {
        private readonly KoruCosmeticaContext _context;

        public ServicioClienteTurno(KoruCosmeticaContext context)
        {
            _context = context;
        }
        public List<ClientesTurnos> ObtenerTurnosPorCliente(int clienteId)
        {
            return (from c in _context.Clientes
                   join t in _context.Turnos on c.ClienteID equals t.ClienteID
                   where t.ClienteID == clienteId // Filtrar por el cliente logueado
                   select new ClientesTurnos
                   {
                       ID = t.TurnosId,
                       Nombre = c.Nombre,
                       Apellido = c.Apellido,
                       Fecha = t.Fecha,
                       Hora = t.Hora,
                       Servicio = t.Servicio.Nombre // Asegúrate de que esta propiedad sea correcta
                   }).ToList();
        }
    }
}
