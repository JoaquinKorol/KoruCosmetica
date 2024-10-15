using KoruCosmetica.Data;
using KoruCosmetica.Models;
using KoruCosmetica.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using Azure.Core;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using KoruCosmetica.Utilities;

namespace KoruCosmetica.Services
{
    public class ServicioRecuperar : IServicioRecuperar
    {
        private readonly KoruCosmeticaContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public ServicioRecuperar(KoruCosmeticaContext context, IEmailSender emailSender, IUrlHelperFactory urlHelperFactory)
        {
            _context = context;
            _emailSender = emailSender;
            _urlHelperFactory = urlHelperFactory;
        }

        public async Task<bool> EnviarRecuperacionAsync(Clientes clientes, HttpContext httpContext)
        {
            var usuarioExistente = await _context.Clientes.FirstOrDefaultAsync(u => u.Mail == clientes.Mail);
            if (usuarioExistente != null)
            {
                var destinatario = clientes.Mail;
                var asunto = "Recuperar Contraseña Koru Cosmetica";

                var actionContext = new ActionContext(httpContext, httpContext.GetRouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
                var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);

                // Crear un enlace de restablecimiento de contraseña
                var resetLink = urlHelper.Action("RestablecerContraseña", "Clientes", new { email = clientes.Mail }, httpContext.Request.Scheme);

                var mensaje = $"Hola {usuarioExistente.Nombre}, se ha solicitado restablecer la contraseña en Koru Cosmetica. " +
                              $"Por favor, haga clic en el siguiente enlace para restablecer su contraseña: <a href=\"{resetLink}\">Restablecer Contraseña</a>";

                await _emailSender.SendEmailAsync(destinatario, asunto, mensaje);
                return true;
            }
            return false;
        }



        public async Task<(bool resultado, string mensaje)> ActualizarContraseña(string email, CambiarContraseña model)
        {
            var usuario = await _context.Clientes.FirstOrDefaultAsync(u => u.Mail == email);

            if (usuario != null)
            {
                // Verificar si las contraseñas coinciden
                if (model.NuevaContraseña == model.ConfirmarContraseña)
                {
                    // Hashear la nueva contraseña y guardarla
                    usuario.Contraseña = ContraseñaHasher.HashPassword(model.NuevaContraseña);
                    await _context.SaveChangesAsync(); // Guardar cambios en la base de datos
                    return (true, "Contraseña actualizada con éxito");
                }
                else
                {
                    return (false, "Las contraseñas no coinciden");
                }
            }
            return (false, "El email no existe");
        }

    }
}
