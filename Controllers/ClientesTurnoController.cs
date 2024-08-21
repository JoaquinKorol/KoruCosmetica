using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoruCosmetica.Models.ViewModel;
using KoruCosmetica.Models;
using KoruCosmetica.Data;

namespace KoruCosmetica.Controllers
{
    public class ClientesTurnoController : Controller
    {
        private readonly KoruCosmeticaContext _context;

        public ClientesTurnoController(KoruCosmeticaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var data = (from c in _context.Clientes
                        join t in _context.Turnos on c.ClienteID equals t.ClienteID
                        join s in _context.Turnos on t.ServicioID equals s.ServicioID // Corregido aquí
                        select new ClientesTurnos
                        {
                            Nombre = c.Nombre,
                            Apellido = c.Apellido,
                            Fecha = t.Fecha.ToDateTime(TimeOnly.MinValue), // Si 'Fecha' ya es DateTime, no necesitas ToDateTime
                            Hora = t.Hora,
                            Servicio = s.Servicio.Nombre // Usar la propiedad 'Nombre' del modelo 'Servicios'
                        }).ToList();

            return View(data);
        }
    }
}