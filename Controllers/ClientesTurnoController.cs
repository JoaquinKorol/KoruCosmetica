using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoruCosmetica.Models.ViewModel;
using KoruCosmetica.Models;
using KoruCosmetica.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KoruCosmetica.Controllers
{
    public class ClientesTurnoController : Controller
    {
        private readonly KoruCosmeticaContext _context;

        public ClientesTurnoController(KoruCosmeticaContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "User")]
        public IActionResult IndexUser()
        {
            // Obtiene el identificador del usuario desde el token
            var clienteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (clienteIdClaim != null && int.TryParse(clienteIdClaim.Value, out int clienteId))
            {
                // Obtiene los turnos del cliente autenticado
                var dataTurno = (from c in _context.Clientes
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

                return View(dataTurno);
            }

            // Si el ClienteID no se encuentra, retornamos una vista vacía o un mensaje de error
            return View(new List<ClientesTurnos>());
        }



        [Authorize(Roles = "Admin")]
        public IActionResult IndexAdmin(string nombreBuscar, DateOnly? fechaDesde, DateOnly? fechaHasta, int? servicioBuscar, string reset)
        {
            var servicios = _context.Servicios.ToList();

            // Agrega los servicios al ViewBag para que estén disponibles en la vista
            ViewBag.ServicioID = new SelectList(servicios, "ServicioID", "Nombre");

            // Obtén la lista de turnos desde la base de datos
            var turnos = _context.Turnos.Include(t => t.Cliente).Include(t => t.Servicio).ToList();

            // Filtra según el nombre del cliente
            if (!string.IsNullOrEmpty(nombreBuscar))
            {
                turnos = turnos.Where(t => t.Cliente.Nombre.Contains(nombreBuscar) || t.Cliente.Apellido.Contains(nombreBuscar)).ToList();
            }

            // Filtra por fechas
            if (fechaDesde.HasValue)
            {
                turnos = turnos.Where(t => t.Fecha >= fechaDesde.Value).ToList();
            }

            if (fechaHasta.HasValue)
            {
                turnos = turnos.Where(t => t.Fecha <= fechaHasta.Value).ToList();
            }

            if (servicioBuscar.HasValue)
            {
                turnos = turnos.Where(t => t.Servicio.ServicioID == servicioBuscar).ToList();
            }

            if (!string.IsNullOrEmpty(reset))
            {
                return View(turnos.ToList());
            }

            var viewModel = new TurnosViewModel
            {
                Turnos = turnos.Select(t => new ClientesTurnos
                {
                    ID = t.TurnosId,
                    Nombre = t.Cliente.Nombre,
                    Apellido = t.Cliente.Apellido,
                    Fecha = t.Fecha,
                    Hora = t.Hora,
                    Servicio = t.Servicio.Nombre
                }).ToList()
            };

            return View(viewModel);
        }



    }
}