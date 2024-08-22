using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KoruCosmetica.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHorariosDisponibles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClienteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ClienteID);
                });

            migrationBuilder.CreateTable(
                name: "DiasSemana",
                columns: table => new
                {
                    DiaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiasSemana", x => x.DiaID);
                });

            migrationBuilder.CreateTable(
                name: "Profesionales",
                columns: table => new
                {
                    ProfesionalID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profesionales", x => x.ProfesionalID);
                });

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    ServicioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duracion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.ServicioID);
                });

            migrationBuilder.CreateTable(
                name: "HorariosDisponibles",
                columns: table => new
                {
                    HorarioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoraInicio = table.Column<TimeOnly>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeOnly>(type: "time", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaID = table.Column<int>(type: "int", nullable: true),
                    ProfesionalID = table.Column<int>(type: "int", nullable: true),
                    ProfesionalesProfesionalID = table.Column<int>(type: "int", nullable: true),
                    DiasSemanaDiaID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosDisponibles", x => x.HorarioID);
                    table.ForeignKey(
                        name: "FK_HorariosDisponibles_DiasSemana_DiasSemanaDiaID",
                        column: x => x.DiasSemanaDiaID,
                        principalTable: "DiasSemana",
                        principalColumn: "DiaID");
                    table.ForeignKey(
                        name: "FK_HorariosDisponibles_Profesionales_ProfesionalesProfesionalID",
                        column: x => x.ProfesionalesProfesionalID,
                        principalTable: "Profesionales",
                        principalColumn: "ProfesionalID");
                });

            migrationBuilder.CreateTable(
                name: "Turnos",
                columns: table => new
                {
                    TurnosId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Hora = table.Column<TimeOnly>(type: "time", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClienteID = table.Column<int>(type: "int", nullable: true),
                    ServicioID = table.Column<int>(type: "int", nullable: true),
                    ProfesionalID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turnos", x => x.TurnosId);
                    table.ForeignKey(
                        name: "FK_Turnos_Clientes_ClienteID",
                        column: x => x.ClienteID,
                        principalTable: "Clientes",
                        principalColumn: "ClienteID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Turnos_Profesionales_ProfesionalID",
                        column: x => x.ProfesionalID,
                        principalTable: "Profesionales",
                        principalColumn: "ProfesionalID");
                    table.ForeignKey(
                        name: "FK_Turnos_Servicios_ServicioID",
                        column: x => x.ServicioID,
                        principalTable: "Servicios",
                        principalColumn: "ServicioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HorariosDisponibles_DiasSemanaDiaID",
                table: "HorariosDisponibles",
                column: "DiasSemanaDiaID");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosDisponibles_ProfesionalesProfesionalID",
                table: "HorariosDisponibles",
                column: "ProfesionalesProfesionalID");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_ClienteID",
                table: "Turnos",
                column: "ClienteID");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_ProfesionalID",
                table: "Turnos",
                column: "ProfesionalID");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_ServicioID",
                table: "Turnos",
                column: "ServicioID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HorariosDisponibles");

            migrationBuilder.DropTable(
                name: "Turnos");

            migrationBuilder.DropTable(
                name: "DiasSemana");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Profesionales");

            migrationBuilder.DropTable(
                name: "Servicios");
        }
    }
}
