using KoruCosmetica.Data;
using KoruCosmetica.Models.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace KoruCosmetica.Services
{
    public class ServiciosTurnosHorarios : IServicioTurnosHorarios
    {
        private readonly KoruCosmeticaContext _context;
        private readonly IEmailSender _emailSender;

        public ServiciosTurnosHorarios(KoruCosmeticaContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public async Task<bool> SacarTurno(TurnosHorarios viewModel, string clienteIdClaim)
        {
            if (string.IsNullOrEmpty(clienteIdClaim) || !int.TryParse(clienteIdClaim, out int clienteId))
                return false;

            viewModel.Turnos.ClienteID = clienteId;

            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null)
                return false;

            var destinatario = cliente.Mail;
            var asunto = "Turno Confirmado Koru Cosmética";
            var mensaje = $"Hola {cliente.Nombre}, se ha confirmado su turno para el {viewModel.Turnos.Fecha.ToShortDateString()} a las {viewModel.Turnos.Hora.ToString(@"HH\:mm")} en Koru Cosmética.";

            await _emailSender.SendEmailAsync(destinatario, asunto, mensaje);

            _context.Add(viewModel.Turnos);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelarTurno(int id)
        {
            var turno = await _context.Turnos
                .Include(t => t.Cliente)
                .FirstOrDefaultAsync(t => t.TurnosId == id);

            if (turno == null)
                return false;

            var destinatario = turno.Cliente.Mail;
            var asunto = "Turno Cancelado Koru Cosmética";
            var mensaje = $"Hola {turno.Cliente.Nombre}, se ha cancelado su turno para el {turno.Fecha.ToShortDateString()} a las {turno.Hora.ToString(@"HH\:mm")} en Koru Cosmética.";

            await _emailSender.SendEmailAsync(destinatario, asunto, mensaje);

            _context.Turnos.Remove(turno);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string[]> ObtenerHorariosDisponibles(DateTime fecha)
        {
            int diaID = (int)fecha.DayOfWeek;
            var dateOnlyFecha = DateOnly.FromDateTime(fecha);

            var horariosDisponibles = await _context.HorariosDisponibles
                .Where(h => h.DiaID == diaID)
                .Where(h => !_context.Turnos.Any(t => t.Fecha == dateOnlyFecha && t.Hora == h.HoraInicio))
                .Select(h => h.HoraInicio.ToString("HH:mm"))
                .ToArrayAsync();

            return horariosDisponibles;
        }

        
    }
}
