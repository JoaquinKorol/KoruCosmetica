using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoruCosmetica.Models;

namespace KoruCosmetica.Data
{
    public class KoruCosmeticaContext : DbContext
    {
        public KoruCosmeticaContext (DbContextOptions<KoruCosmeticaContext> options)
            : base(options)
        {
        }

        public DbSet<KoruCosmetica.Models.Clientes> Clientes { get; set; } = default!;
        public DbSet<KoruCosmetica.Models.Turnos> Turnos { get; set; } = default!;
        public DbSet<KoruCosmetica.Models.Profesionales> Profesionales { get; set; } = default!;
        public DbSet<KoruCosmetica.Models.Servicios> Servicios { get; set; } = default!;

        public DbSet<KoruCosmetica.Models.HorariosDisponibles> HorariosDisponibles { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Clientes>()
                .HasMany(e => e.Turnos)
                .WithOne(e => e.Cliente)
                .HasForeignKey(e => e.ClienteID)
                .HasPrincipalKey(e => e.ClienteID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Turnos>()
            .HasOne(t => t.Servicio)
            .WithMany(s => s.Turnos)
            .HasForeignKey(t => t.ServicioID)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
