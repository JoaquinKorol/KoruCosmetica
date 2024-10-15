using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KoruCosmetica.Models.ViewModel;
using KoruCosmetica.Models;
using KoruCosmetica.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using KoruCosmetica.Services;
using Microsoft.Extensions.Options;

namespace KoruCosmetica.Controllers
{
    public class ClientesTurnoController : Controller
    {
        private readonly KoruCosmeticaContext _context;
        private readonly IServicioClienteTurno _servicioClienteTurno;
        private readonly IServicioAdminTurno _servicioAdminTurno;

        public ClientesTurnoController(KoruCosmeticaContext context ,IServicioClienteTurno servicioClienteTurno, IServicioAdminTurno servicioAdminTurno)
        { 
            _context = context;
            _servicioClienteTurno = servicioClienteTurno;
            _servicioAdminTurno = servicioAdminTurno;
        }

        [Authorize(Roles = "User")]
        public IActionResult IndexUser()
        {
            // Obtiene el identificador del usuario desde el token
            var clienteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (clienteIdClaim != null && int.TryParse(clienteIdClaim.Value, out int clienteId))
            {
                // Obtiene los turnos del cliente autenticado
                var dataTurno = _servicioClienteTurno.ObtenerTurnosPorCliente(clienteId);

                return View(dataTurno);
            }

            // Si el ClienteID no se encuentra, retornamos una vista vacía o un mensaje de error
            return View(new List<ClientesTurnos>());
        }



        [Authorize(Roles = "Admin")]
        public IActionResult IndexAdmin(string nombreBuscar, DateOnly? fechaDesde, DateOnly? fechaHasta, int? servicioBuscar, string reset)
        {
            var servicios = _context.Servicios.ToList();
            ViewBag.ServicioID = new SelectList(servicios, "ServicioID", "Nombre");

            // Usar el servicio para obtener los turnos filtrados
            var turnosFiltrados = _servicioAdminTurno.ObtenerTurnosFiltrados(nombreBuscar, fechaDesde, fechaHasta, servicioBuscar);

            if (!string.IsNullOrEmpty(reset))
            {
                return View(turnosFiltrados); // Devuelve la lista sin filtros
            }

            // Crear el ViewModel
            var viewModel = new TurnosViewModel
            {
                Turnos = turnosFiltrados
            };

            return View(viewModel);
        }



    }
}