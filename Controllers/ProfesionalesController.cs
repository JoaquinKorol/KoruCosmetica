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
    public class ProfesionalesController : Controller
    {
        private readonly KoruCosmeticaContext _context;

        public ProfesionalesController(KoruCosmeticaContext context)
        {
            _context = context;
        }

        // GET: Profesionales
        public async Task<IActionResult> Index()
        {
            return View(await _context.Profesionales.ToListAsync());
        }

        // GET: Profesionales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profesionales = await _context.Profesionales
                .FirstOrDefaultAsync(m => m.ProfesionalID == id);
            if (profesionales == null)
            {
                return NotFound();
            }

            return View(profesionales);
        }

        // GET: Profesionales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Profesionales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProfesionalID,Nombre,Apellido")] Profesionales profesionales)
        {
            if (ModelState.IsValid)
            {
                _context.Add(profesionales);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(profesionales);
        }

        // GET: Profesionales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profesionales = await _context.Profesionales.FindAsync(id);
            if (profesionales == null)
            {
                return NotFound();
            }
            return View(profesionales);
        }

        // POST: Profesionales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProfesionalID,Nombre,Apellido")] Profesionales profesionales)
        {
            if (id != profesionales.ProfesionalID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profesionales);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfesionalesExists(profesionales.ProfesionalID))
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
            return View(profesionales);
        }

        // GET: Profesionales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profesionales = await _context.Profesionales
                .FirstOrDefaultAsync(m => m.ProfesionalID == id);
            if (profesionales == null)
            {
                return NotFound();
            }

            return View(profesionales);
        }

        // POST: Profesionales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profesionales = await _context.Profesionales.FindAsync(id);
            if (profesionales != null)
            {
                _context.Profesionales.Remove(profesionales);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfesionalesExists(int id)
        {
            return _context.Profesionales.Any(e => e.ProfesionalID == id);
        }
    }
}
