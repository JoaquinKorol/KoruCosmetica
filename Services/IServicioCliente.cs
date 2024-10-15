using KoruCosmetica.Models;
using Microsoft.AspNetCore.Mvc;

namespace KoruCosmetica.Services
{
    public interface IServicioCliente
    {
        Task<string> RegistrarAsync(Clientes clientes);
        Task<(string? mensaje, string? token, bool exitoso)> LoginAsyc(Clientes clientes);
        Task LogoutAsync(HttpContext httpContext);
    }
}
