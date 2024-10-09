using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using KoruCosmetica.Data;
using System.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using KoruCosmetica.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using KoruCosmetica.Services;
namespace KoruCosmetica
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<KoruCosmeticaContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("KoruCosmeticaContext") ?? throw new InvalidOperationException("Connection string 'KoruCosmeticaContext' not found.")));

            var appSettingsSection = builder.Configuration.GetSection("AppSettings");
            var stmpSettingsSection = builder.Configuration.GetSection("SmtpSettings");

            builder.Services.Configure<AppSettings>(appSettingsSection);

            builder.Services.Configure<SmtpSettings>(stmpSettingsSection);

            builder.Services.AddTransient<IEmailSender, EmailSender>();


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(builder.Configuration["AppSettings:Secret"]);
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // Podrías deshabilitar esta validación temporalmente
                    ValidateAudience = false, // Podrías deshabilitar esta validación temporalmente
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            })
            .AddCookie(x =>
            {
                x.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // Expira en 30 minutos
                x.SlidingExpiration = true; // Renovar la cookie automáticamente si el usuario sigue activo
                x.LogoutPath = "/Clientes/Logout";
            });

            builder.Services.AddAuthorization(x =>
            {
                x.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Administrador"));
                x.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
            });


            builder.Services.AddHttpContextAccessor();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseMiddleware<JwtCookieMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
