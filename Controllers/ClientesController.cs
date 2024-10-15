using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KoruCosmetica.Data;
using KoruCosmetica.Models;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using KoruCosmetica.Services;
using System.Security.Policy;
using KoruCosmetica.Models.ViewModel;

namespace KoruCosmetica.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IServicioCliente _servicioCliente;
        private readonly IServicioRecuperar _servicioRecuperar;

        public ClientesController(IServicioCliente servicioCliente, IServicioRecuperar servicioRecuperar)
        {
            _servicioCliente = servicioCliente;
            _servicioRecuperar = servicioRecuperar;
        }


        // GET: Clientes/Create
        public IActionResult Registrar()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("clientes/registrar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar([Bind("ClienteID,Nombre,Apellido,Mail,Telefono,Contraseña")] Clientes clientes)
        {
            if (ModelState.IsValid)
            {
                var errorMessage = await _servicioCliente.RegistrarAsync(clientes);
                if (errorMessage != null)
                {
                    ViewBag.ErrorMessage = errorMessage;
                    return View(clientes); // Vuelve a mostrar la vista con el mensaje de error
                }

                // Redirigir al índice de la página principal
                return RedirectToAction("Index", "Home");
            }

            // Si el modelo no es válido, volver a mostrar la vista
            return View(clientes);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("clientes/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Mail,Contraseña")] Clientes clientes)
        {

            var resultado = await _servicioCliente.LoginAsyc(clientes);
            if (resultado.exitoso != true)
            {
                ViewBag.ErrorMessage = resultado.mensaje;
                return View(clientes);
            }
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            HttpContext.Response.Cookies.Append("jwtToken", resultado.token, cookieOptions);

            // Redirigir al usuario
            return RedirectToAction("Index", "Home");


        }

        
        public async Task<IActionResult> Logout()
        {
            await _servicioCliente.LogoutAsync(HttpContext);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult RecuperarContraseña()
        {
            return View();
        }

        public IActionResult MensajeEnviado()
        {
            return View();
        }
        public IActionResult ContraseñaActualizada()
        {
            return View(); 
        }

        public IActionResult RestablecerContraseña()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EnviarRecuperacion(Clientes clientes)
        {
            var resultado = await _servicioRecuperar.EnviarRecuperacionAsync(clientes, HttpContext);
            if (resultado)
            {
                return RedirectToAction("MensajeEnviado", "Clientes");
            }
            ModelState.AddModelError(string.Empty, "No se encontró ningún usuario con ese correo electrónico.");
            return View(clientes);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarContraseña(string email, CambiarContraseña model)
        {
            var resultado = await _servicioRecuperar.ActualizarContraseña(email, model);

            if (resultado.resultado)
            {
                // Contraseña actualizada con éxito, redirigir o mostrar un mensaje
                TempData["MensajeExito"] = resultado.mensaje; // Usar TempData para mensajes de éxito
                return RedirectToAction("ContraseñaActualizada", "Clientes");

            }
            else
            {
                // Mostrar un mensaje de error en la vista
                ModelState.AddModelError(string.Empty, resultado.mensaje);
                return View(model); // Volver a mostrar el formulario con el mensaje de error
            }
        }

    }
}



      