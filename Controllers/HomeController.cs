using KoruCosmetica.Data;
using KoruCosmetica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace KoruCosmetica.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _secret;
        private readonly KoruCosmeticaContext _context;

        public HomeController(ILogger<HomeController> logger, IOptions<AppSettings> appSettings, KoruCosmeticaContext context)
        {
            _logger = logger;
            _secret = appSettings.Value.Secret;
            _context = context;

        }

        public IActionResult Index()
        {
            var token = HttpContext.Request.Query["token"].ToString();
            if (!string.IsNullOrEmpty(token))
            {
                var servicios = _context.Servicios.ToList();
                var usuario = ValidateToken(token); // Método para validar el token
                if (usuario != null)
                {
                    // Inicia sesión con el usuario
                    HttpContext.Session.SetString("UserId", usuario.ClienteID.ToString());
                    // O utiliza algún sistema de autenticación
                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public Clientes ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero // No permite una tolerancia de tiempo
            };

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // Extraer el UserId u otra información del token
                var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    // Aquí deberías obtener el usuario desde tu base de datos
                    var userId = userIdClaim.Value;
                    return ObtenerUsuarioPorId(userId); // Implementa este método para obtener el usuario
                }
            }
            catch (SecurityTokenException)
            {
                // El token no es válido
                return null;
            }
            catch (Exception)
            {
                // Maneja cualquier otro error
                return null;
            }

            return null;
        }

        private Clientes ObtenerUsuarioPorId(string userId)
        {
            return _context.Clientes.Find(userId);
        }
    }
}
