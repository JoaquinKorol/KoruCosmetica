using KoruCosmetica.Data;
using KoruCosmetica.Models.ViewModel;
using KoruCosmetica.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class TurnosHorariosController : Controller
{
    private readonly KoruCosmeticaContext _context;

    public TurnosHorariosController(KoruCosmeticaContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Create()
    {
        // Cargar datos iniciales para los SelectLists
        ViewBag.ClienteID = new SelectList(_context.Clientes, "ClienteID", "Nombre");
        ViewBag.ProfesionalID = new SelectList(_context.Set<Profesionales>(), "ProfesionalID", "Nombre");
        ViewBag.ServicioID = new SelectList(_context.Set<Servicios>(), "ServicioID", "Nombre");

        // Inicialmente, no se muestran horarios disponibles
        ViewBag.HorariosDisponibles = new SelectList(_context.Set<HorariosDisponibles>(), "HoraInicio", "HoraInicio");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TurnosHorarios viewModel)
    {
        if (ModelState.IsValid)
        {
            // Obtener el día de la semana desde la fecha del turno
            int diaID = TurnosHorarios.GetDiaIDFromDayOfWeek(viewModel.Turnos.Fecha.DayOfWeek);

            // Obtener los horarios disponibles para el día de la semana correspondiente
            var horariosDisponibles =  _context.Set<HorariosDisponibles>()
                .Where(h => h.DiaID == diaID)
                .Select(h => h.HoraInicio)
                .ToList();

            // Pasar los horarios disponibles a la vista
            ViewBag.HorariosDisponibles = horariosDisponibles;

            // Aquí puedes manejar el resto de la lógica para crear el turno
            viewModel.Turnos.ClienteID = TempData["ClienteID"] as int?;

            _context.Add(viewModel.Turnos);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "ClientesTurno");
        }


        return View(viewModel);

    }
    [HttpGet]
    public async Task<IActionResult> ObtenerHorariosDisponibles(DateTime fecha)
    {
        int diaID = TurnosHorarios.GetDiaIDFromDayOfWeek(fecha.DayOfWeek);

        var horariosDisponibles = await _context.Set<HorariosDisponibles>()
            .Where(h => h.DiaID == diaID)
            .Select(h => h.HoraInicio.ToString("HH:mm"))
            .ToListAsync();

        return Json(horariosDisponibles);
    }
} 

