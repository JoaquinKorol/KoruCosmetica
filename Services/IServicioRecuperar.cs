using Azure.Core;
using KoruCosmetica.Models;
using KoruCosmetica.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;

namespace KoruCosmetica.Services
{
    public interface IServicioRecuperar
    {
        Task<bool> EnviarRecuperacionAsync(Clientes clientes, HttpContext httpContext);
        Task<(bool resultado, string mensaje)> ActualizarContraseña(string mail, CambiarContraseña model);


    }
}
