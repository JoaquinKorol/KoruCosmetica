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
using Azure.Identity;

// TODO: Hacer servicio de este controlador.
[Authorize]
public class TurnosHorariosController : Controller
{
    private readonly KoruCosmeticaContext _context;
    private readonly IEmailSender _emailSender;
    private readonly IServicioTurnosHorarios _servicioTurnosHorarios;

    public TurnosHorariosController(KoruCosmeticaContext context, IEmailSender emailSender, IServicioTurnosHorarios turnosService)
    {
        _context = context;
        _emailSender = emailSender;
        _servicioTurnosHorarios = turnosService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Create()
    {
        // Cargar datos iniciales para los SelectLists
        ViewBag.ProfesionalID = new SelectList(_context.Profesionales, "ProfesionalID", "Nombre");
        ViewBag.ServicioID = new SelectList(_context.Servicios, "ServicioID", "Nombre");

        // Inicialmente, no se muestran horarios disponibles
        ViewBag.HorariosDisponibles = new SelectList(_context.HorariosDisponibles, "HoraInicio", "HoraInicio");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SacarTurno(TurnosHorarios viewModel)
    {
        if (ModelState.IsValid)
        {
            var clienteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var exito = await _servicioTurnosHorarios.SacarTurno(viewModel, clienteIdClaim);
            if (exito)
            {
                return RedirectToAction("IndexUser", "ClientesTurno");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "No se pudo sacar el turno.");
            }
        }

        // Recargar datos iniciales para los SelectLists en caso de error
        ViewBag.ProfesionalID = new SelectList(_context.Profesionales, "ProfesionalID", "Nombre");
        ViewBag.ServicioID = new SelectList(_context.Servicios, "ServicioID", "Nombre");
        ViewBag.HorariosDisponibles = new SelectList(_context.HorariosDisponibles, "HoraInicio", "HoraInicio");

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelarTurno(int id)
    {
        var exito = await _servicioTurnosHorarios.CancelarTurno(id);
        if (!exito)
        {
            return NotFound();
        }

        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("IndexAdmin", "ClientesTurno");
        }

        return RedirectToAction("IndexUser", "ClientesTurno");
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerHorariosDisponibles(DateTime fecha)
    {
        var horarios = await _servicioTurnosHorarios.ObtenerHorariosDisponibles(fecha);
        return Json(horarios);
    }


} 

