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
        private readonly KoruCosmeticaContext _context;
        private readonly string _secret;
        private readonly IEmailSender _emailSender;

        public ClientesController(KoruCosmeticaContext context, IOptions<AppSettings> appSettings, IEmailSender emailSender)
        {
            _context = context;
            _secret = appSettings.Value.Secret;
            _emailSender = emailSender;
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
                // Verificar si el usuario ya existe
                var usuarioExistente = await _context.Clientes.FirstOrDefaultAsync(u => u.Mail == clientes.Mail);
                if (usuarioExistente != null)
                {
                    ViewBag.ErrorMessage = "El Mail ya está registrado con otro usuario. Inicie sesión <a href='" + Url.Action("Login", "Clientes") + "'>aquí</a>.";
                    return View(clientes); // Vuelve a mostrar la vista con el mensaje de error
                }

                // Hashear la contraseña antes de guardar
                clientes.Contraseña = HashPassword(clientes.Contraseña);

                // Agregar el nuevo cliente
                _context.Add(clientes);
                await _context.SaveChangesAsync();

                var destinatario = clientes.Mail;
                var asunto = "Confirmar registro Koru Cosmetica";
                var mensaje = $"Hola {clientes.Nombre}, se ha registrado correctamente en koru cosmetica.";

                await _emailSender.SendEmailAsync(destinatario, asunto, mensaje);

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
           
            var usuario = await _context.Clientes.FirstOrDefaultAsync(u => u.Mail == clientes.Mail);

            if (usuario != null)
            {
                if (BCrypt.Net.BCrypt.Verify(clientes.Contraseña, usuario.Contraseña))
                {
                    var token = GetToken(usuario); // Generar el token aquí

                    // Configurar la cookie con el token
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true, // Previene ataques XSS
                        Secure = true,   // Asegúrate de que esto sea true en producción (requiere HTTPS)
                        SameSite = SameSiteMode.Strict, // Previene ataques CSRF
                        Expires = DateTime.UtcNow.AddHours(1) // Duración de la cookie
                    };

                    // Almacenar el token en la cookie
                    HttpContext.Response.Cookies.Append("jwtToken", token, cookieOptions);

                    // Redirigir al usuario
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "La contraseña es incorrecta.");
                    return View(clientes);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "El usuario no existe.");
                return View(clientes);
            }
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
            return View(); // Asegúrate de que existe una vista ContraseñaActualizada
        }

        public IActionResult RestablecerContraseña()
        {
            // Aquí puedes agregar la lógica para restablecer la contraseña
            return View(); // Asegúrate de que existe una vista RestablecerContraseña
        }

        public async Task<IActionResult> EnviarRecuperacion(Clientes clientes)
        {
            var usuarioExistente = await _context.Clientes.FirstOrDefaultAsync(u => u.Mail == clientes.Mail);
            if (usuarioExistente != null)
            {
                var destinatario = clientes.Mail;
                var asunto = "Recuperar Contraseña Koru Cosmetica";

                // Crear un enlace de restablecimiento de contraseña (esto es solo un ejemplo)
                var resetLink = Url.Action("RestablecerContraseña", "Clientes", new { email = clientes.Mail }, Request.Scheme);

                var mensaje = $"Hola {usuarioExistente.Nombre}, se ha solicitado restablecer la contraseña en Koru Cosmetica. " +
                              $"Por favor, haga clic en el siguiente enlace para restablecer su contraseña: <a href=\"{resetLink}\">Restablecer Contraseña</a>";

                try
                {
                    await _emailSender.SendEmailAsync(destinatario, asunto, mensaje);
                    return RedirectToAction("MensajeEnviado", "Clientes");
                }
                catch (Exception ex)
                {
                    // Manejo de errores en el envío del correo
                    // Aquí podrías registrar el error o mostrar un mensaje al usuario
                    ModelState.AddModelError(string.Empty, "Hubo un error al enviar el correo. Intente nuevamente más tarde.");
                    return View(clientes);
                }
            }
            else
            {
                // El usuario no existe
                ModelState.AddModelError(string.Empty, "No se encontró ningún usuario con ese correo electrónico.");
                return View(clientes);
            }


        }

        [HttpPost]
        public async Task<IActionResult> ActualizarContraseña(string email, Clientes cliente, CambiarContraseña model)
        {
            // Asegúrate de que el cliente contenga la nueva contraseña
            
            // Busca el usuario por su correo electrónico
            var usuario = await _context.Clientes.FirstOrDefaultAsync(u => u.Mail == email);
            if (usuario != null)
            {
                // Actualiza la contraseña
                if(model.NuevaContraseña == model.ConfirmarContraseña)
                {
                    usuario.Contraseña = HashPassword(model.NuevaContraseña);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("ContraseñaActualizada", "Clientes");
                }
                else 
                {
                    ModelState.AddModelError(string.Empty, "Las contraseñas no coinciden.");
                    return View(model);
                }
                
            }

            // Si no se encuentra el usuario, redirige a la página de error
            return RedirectToAction("Error", "Home");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Eliminar las cookies manualmente
            HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
            HttpContext.Response.Cookies.Delete("jwtToken");

            return RedirectToAction("Index", "Home");
        }


        private string HashPassword(string password)
        {
            
            return BCrypt.Net.BCrypt.HashPassword(password);
        }


        private string GetToken(Clientes clientes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);

            // Configura los claims que serán parte del token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, clientes.ClienteID.ToString()),
                new Claim(ClaimTypes.Email, clientes.Mail),
                new Claim("nombre", clientes.Nombre)
            };

            // Agregar el rol de Admin si el email es el específico
            if (clientes.Mail == "detefi3132@heweek.com")
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(10), // Ajusta la expiración según sea necesario
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
