using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KoruCosmetica.Data;
using KoruCosmetica.Models;

namespace KoruCosmetica.Controllers
{
    public class TurnosController : Controller
    {
        private readonly KoruCosmeticaContext _context;

        public TurnosController(KoruCosmeticaContext context)
        {
            _context = context;
        }

        // GET: Turnos
        public async Task<IActionResult> Index()
        {
            var koruCosmeticaContext = _context.Turnos.Include(t => t.Cliente).Include(t => t.Profesional).Include(t => t.Servicio);
            return View(await koruCosmeticaContext.ToListAsync());
        }

        // GET: Turnos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var turnos = await _context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Profesional)
                .Include(t => t.Servicio)
                .FirstOrDefaultAsync(m => m.TurnosId == id);
            if (turnos == null)
            {
                return NotFound();
            }

            return View(turnos);
        }

        // GET: Turnos/Create
        public IActionResult Create()
        {
            ViewData["ClienteID"] = new SelectList(_context.Clientes, "ClienteID", "ClienteID");
            ViewData["ProfesionalID"] = new SelectList(_context.Set<Profesionales>(), "ProfesionalID", "Nombre");
            ViewData["ServicioID"] = new SelectList(_context.Set<Servicios>(), "ServicioID", "Nombre");
            return View();
        }

        // POST: Turnos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TurnosId,Fecha,Hora,Estado,ClienteID ,ServicioID,ProfesionalID")] Turnos turnos)
        {
            if (ModelState.IsValid && TempData["ClienteID"] is int clienteId)
            {
                turnos.ClienteID = clienteId;
                _context.Add(turnos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteID"] = new SelectList(_context.Clientes, "ClienteID", "ClienteID", turnos.ClienteID);
            ViewData["ProfesionalID"] = new SelectList(_context.Set<Profesionales>(), "ProfesionalID", "ProfesionalID", turnos.ProfesionalID);
            ViewData["ServicioID"] = new SelectList(_context.Set<Servicios>(), "ServicioID", "ServicioID", turnos.ServicioID);
            
            return View(turnos);
        }

        // GET: Turnos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var turnos = await _context.Turnos.FindAsync(id);
            if (turnos == null)
            {
                return NotFound();
            }
            ViewData["ClienteID"] = new SelectList(_context.Clientes, "ClienteID", "ClienteID", turnos.ClienteID);
            ViewData["ProfesionalID"] = new SelectList(_context.Set<Profesionales>(), "ProfesionalID", "ProfesionalID", turnos.ProfesionalID);
            ViewData["ServicioID"] = new SelectList(_context.Set<Servicios>(), "ServicioID", "ServicioID", turnos.ServicioID);
            return View(turnos);
        }

        // POST: Turnos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TurnosId,Fecha,Hora,Estado,ClienteID,ServicioID,ProfesionalID")] Turnos turnos)
        {
            if (id != turnos.TurnosId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(turnos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TurnosExists(turnos.TurnosId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteID"] = new SelectList(_context.Clientes, "ClienteID", "ClienteID", turnos.ClienteID);
            ViewData["ProfesionalID"] = new SelectList(_context.Set<Profesionales>(), "ProfesionalID", "ProfesionalID", turnos.ProfesionalID);
            ViewData["ServicioID"] = new SelectList(_context.Set<Servicios>(), "ServicioID", "ServicioID", turnos.ServicioID);
            return View(turnos);
        }

        // GET: Turnos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var turnos = await _context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Profesional)
                .Include(t => t.Servicio)
                .FirstOrDefaultAsync(m => m.TurnosId == id);
            if (turnos == null)
            {
                return NotFound();
            }

            return View(turnos);
        }

        // POST: Turnos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var turnos = await _context.Turnos.FindAsync(id);
            if (turnos != null)
            {
                _context.Turnos.Remove(turnos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TurnosExists(int id)
        {
            return _context.Turnos.Any(e => e.TurnosId == id);
        }
    }
}
