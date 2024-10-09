using KoruCosmetica.Data;
using KoruCosmetica.Models.ViewModel;
using KoruCosmetica.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using KoruCosmetica.Services;


[Authorize]
public class TurnosHorariosController : Controller
{
    private readonly KoruCosmeticaContext _context;
    private readonly IEmailSender _emailSender;

    public TurnosHorariosController(KoruCosmeticaContext context, IEmailSender emailSender)
    {
        _context = context;
        _emailSender = emailSender;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Create()
    {
        // Cargar datos iniciales para los SelectLists
        ViewBag.ProfesionalID = new SelectList(_context.Set<Profesionales>(), "ProfesionalID", "Nombre");
        ViewBag.ServicioID = new SelectList(_context.Set<Servicios>(), "ServicioID", "Nombre");

        // Inicialmente, no se muestran horarios disponibles
        ViewBag.HorariosDisponibles = new SelectList(_context.Set<HorariosDisponibles>(), "HoraInicio", "HoraInicio");

        return View();
    }

    [Authorize(Roles = "User")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SacarTurno(TurnosHorarios viewModel)
    {
        if (ModelState.IsValid)
        {
            var clienteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Verificar que el Claim no sea nulo y que se pueda convertir a entero
            if (clienteIdClaim != null && int.TryParse(clienteIdClaim, out int clienteId))
            {
                // Asignar el ClienteID al modelo de turno
                viewModel.Turnos.ClienteID = clienteId;

                // Obtener el cliente para enviar el correo
                var cliente = await _context.Clientes.FindAsync(clienteId);
                if (cliente != null)
                {
                    var destinatario = cliente.Mail;
                    var asunto = "Turno Confirmado Koru Cosmética";
                    var mensaje = $"Hola {cliente.Nombre}, se ha confirmado su turno para el {viewModel.Turnos.Fecha.ToShortDateString()} a las {viewModel.Turnos.Hora.ToString(@"HH\:mm")} en Koru Cosmética.";

                    // Enviar el correo
                    await _emailSender.SendEmailAsync(destinatario, asunto, mensaje);

                    // Guardar el turno con el ClienteID correcto
                    _context.Add(viewModel.Turnos);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("IndexUser", "ClientesTurno");
                }
                else
                {
                    // Manejo de error si el cliente no se encuentra
                    ModelState.AddModelError(string.Empty, "El cliente no se encontró.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "No se pudo obtener el ClienteID.");
            }
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelarTurno(int id)
    {
        var turno = await _context.Turnos
            .Include(t => t.Cliente) 
            .FirstOrDefaultAsync(t => t.TurnosId == id);
        if (turno == null)
        {
            return NotFound(); // Manejo si no se encuentra el turno
        }
        var destinatario = turno.Cliente.Mail;
        var asunto = "Turno Cancelado Koru Cosmética";
        var mensaje = $"Hola {turno.Cliente.Nombre}, se ha cancelado su turno para el {turno.Fecha.ToShortDateString()} a las {turno.Hora.ToString(@"HH\:mm")} en Koru Cosmética.";

        // Enviar el correo
        await _emailSender.SendEmailAsync(destinatario, asunto, mensaje);

        // Aquí podrías agregar lógica adicional, como notificar al cliente

        _context.Turnos.Remove(turno);
        await _context.SaveChangesAsync();

        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("IndexAdmin", "ClientesTurno");
        }
        return RedirectToAction("IndexUser", "ClientesTurno");
    }


    [HttpGet]
    public async Task<IActionResult> ObtenerHorariosDisponibles(DateTime fecha)
    {
        int diaID = TurnosHorarios.GetDiaIDFromDayOfWeek(fecha.DayOfWeek);

        var dateOnlyFecha = DateOnly.FromDateTime(fecha);

        var horariosDisponibles = await _context.HorariosDisponibles
            .Where(h => h.DiaID == diaID)
            .Where(h => !_context.Turnos.Any(t => t.Fecha == dateOnlyFecha && t.Hora == h.HoraInicio))
            .Select(h => h.HoraInicio.ToString("HH:mm"))
            .ToListAsync();

        return Json(horariosDisponibles);
    }

    
} 

