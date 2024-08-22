using KoruCosmetica.Data;
using KoruCosmetica.Models.ViewModel;
using KoruCosmetica.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;

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
        ViewBag.ClienteID = new SelectList(_context.Clientes, "ClienteID", "Nombre"); // Ajuste para mostrar nombre del cliente
        ViewBag.ProfesionalID = new SelectList(_context.Set<Profesionales>(), "ProfesionalID", "Nombre");
        ViewBag.ServicioID = new SelectList(_context.Set<Servicios>(), "ServicioID", "Nombre");
        ViewBag.HorarioDisponible = new SelectList(_context.Set<HorariosDisponibles>(), "HoraInicio", "HoraInicio"); // Asegúrate de que 'HorarioID' sea el valor adecuado

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TurnosHorarios viewModel)
    {
        if (ModelState.IsValid)
        {
            if (TempData["ClienteID"] is int clienteId)
            {
                viewModel.Turnos.Hora = viewModel.HorariosDisponibles.HoraInicio;
                viewModel.Turnos.ClienteID = clienteId;
                _context.Add(viewModel.Turnos);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "ClientesTurno");
            }
        }

        // Vuelve a cargar los ViewBags en caso de error
        ViewBag.ClienteID = new SelectList(_context.Clientes, "ClienteID", "Nombre", viewModel.Turnos.ClienteID);
        ViewBag.ProfesionalID = new SelectList(_context.Set<Profesionales>(), "ProfesionalID", "Nombre", viewModel.Turnos.ProfesionalID);
        ViewBag.ServicioID = new SelectList(_context.Set<Servicios>(), "ServicioID", "Nombre", viewModel.Turnos.ServicioID);
        ViewBag.HorarioDisponible = new SelectList(_context.Set<HorariosDisponibles>(), "HorarioID", "HoraInicio", viewModel.HorariosDisponibles.HoraInicio);

        return View(viewModel);
    }
}
