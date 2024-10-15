using KoruCosmetica.Data;
using KoruCosmetica.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.CodeDom;

namespace KoruCosmetica.Services
{
    public class ServicioAdminTurno : IServicioAdminTurno
    {
        private readonly KoruCosmeticaContext _context;

        public ServicioAdminTurno(KoruCosmeticaContext context)
        {
            _context = context;
        }

        public List<ClientesTurnos> ObtenerTurnosFiltrados(string nombreBuscar, DateOnly? fechaDesde, DateOnly? fechaHasta, int? servicioBuscar)
        {
            var turnos = _context.Turnos.Include(t => t.Cliente).Include(t => t.Servicio).ToList();

            // Filtrar por nombre del cliente
            if (!string.IsNullOrEmpty(nombreBuscar))
            {
                turnos = turnos.Where(t => t.Cliente.Nombre.Contains(nombreBuscar) || t.Cliente.Apellido.Contains(nombreBuscar)).ToList();
            }

            // Filtrar por rango de fechas
            if (fechaDesde.HasValue)
            {
                turnos = turnos.Where(t => t.Fecha >= fechaDesde.Value).ToList();
            }

            if (fechaHasta.HasValue)
            {
                turnos = turnos.Where(t => t.Fecha <= fechaHasta.Value).ToList();
            }

            // Filtrar por servicio
            if (servicioBuscar.HasValue)
            {
                turnos = turnos.Where(t => t.Servicio.ServicioID == servicioBuscar).ToList();
            }

            return turnos.Select(t => new ClientesTurnos
            {
                ID = t.TurnosId,
                Nombre = t.Cliente.Nombre,
                Apellido = t.Cliente.Apellido,
                Fecha = t.Fecha,
                Hora = t.Hora,
                Servicio = t.Servicio.Nombre
            }).ToList();
        }
    }
}
