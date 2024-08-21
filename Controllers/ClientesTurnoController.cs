using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoruCosmetica.Models.ViewModel;
using KoruCosmetica.Models;
using KoruCosmetica.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KoruCosmetica.Controllers
{
    public class ClientesTurnoController : Controller
    {
        private readonly KoruCosmeticaContext _context;

        public ClientesTurnoController(KoruCosmeticaContext context)
        {
            _context = context;
        }

        public IActionResult Index(Turnos turnos)
        {
            if (TempData["ClienteID"] is int clienteId)
            {
                turnos.ClienteID = clienteId;
                var dataTurno = (from c in _context.Clientes
                            join t in _context.Turnos on c.ClienteID equals t.ClienteID
                            join s in _context.Turnos on t.ServicioID equals s.ServicioID
                            where c.ClienteID == clienteId
                            select new ClientesTurnos
                            {
                                Nombre = c.Nombre,
                                Apellido = c.Apellido,
                                Fecha = t.Fecha.ToDateTime(TimeOnly.MinValue),
                                Hora = t.Hora,
                                Servicio = s.Servicio.Nombre
                            }).ToList();

                return View(dataTurno);
            }
            var data = (from c in _context.Clientes
                        join t in _context.Turnos on c.ClienteID equals t.ClienteID
                        join s in _context.Turnos on t.ServicioID equals s.ServicioID
                        select new ClientesTurnos
                        {
                            Nombre = c.Nombre,
                            Apellido = c.Apellido,
                            Fecha = t.Fecha.ToDateTime(TimeOnly.MinValue),
                            Hora = t.Hora,
                            Servicio = s.Servicio.Nombre
                        }).ToList();
            return View(data);
        }
    }
}