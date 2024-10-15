using Azure.Core;
using KoruCosmetica.Data;
using KoruCosmetica.Models;
using KoruCosmetica.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;

namespace KoruCosmetica.Services
{
    public class ServicioCliente : IServicioCliente
    {
        private readonly KoruCosmeticaContext _context;
        private readonly IEmailSender _sender;
        private readonly string _secret;

        public ServicioCliente(KoruCosmeticaContext context, IEmailSender emailSender, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _sender = emailSender;
            _secret = appSettings.Value.Secret;
        }

        public async Task<string> RegistrarAsync(Clientes clientes)
        {
            var usuarioExistente = await _context.Clientes.FirstOrDefaultAsync(u => u.Mail == clientes.Mail);
            if (usuarioExistente != null)
            {
                return "El Mail ya está registrado con otro usuario. Inicie sesión aquí.";
            }

            clientes.Contraseña = ContraseñaHasher.HashPassword(clientes.Contraseña);

            _context.Add(clientes);
            await _context.SaveChangesAsync();

            var destinatario = clientes.Mail;
            var asunto = "Confirmar registro Koru Cosmetica";
            var mensaje = $"Hola {clientes.Nombre}, se ha registrado correctamente en koru cosmetica.";

            await _sender.SendEmailAsync(destinatario, asunto, mensaje);

            return null;
        }

        public async Task<(string? mensaje, string? token, bool exitoso)> LoginAsyc(Clientes clientes)
        {
            var usuario = await _context.Clientes.FirstOrDefaultAsync(u => u.Mail == clientes.Mail);
            if (usuario != null)
            {
                if (BCrypt.Net.BCrypt.Verify(clientes.Contraseña, usuario.Contraseña))
                {
                    var token = GetToken(usuario);
                    return (null, token, true);
                    
                }
                else
                {
                    return ("contraseña incorrecta", null, false);
                }
            }
            else
            {
                return ("El mail con el que esta intentando acceder no se enuentra registrado", null, false);
            }
        }

        
        public async Task LogoutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            httpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
            httpContext.Response.Cookies.Delete("jwtToken");
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
