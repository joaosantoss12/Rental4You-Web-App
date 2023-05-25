using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrabalhoPWEB1.Data;
using TrabalhoPWEB1.Models;

namespace TrabalhoPWEB1.Controllers
{
    public class LocalizacoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocalizacoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Localizacoes
        public async Task<IActionResult> Index()
        {
              return View(await _context.Localizacoes.ToListAsync());
        }

        // GET: Localizacoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Localizacoes == null)
            {
                return NotFound();
            }

            var localizacoes = await _context.Localizacoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (localizacoes == null)
            {
                return NotFound();
            }

            return View(localizacoes);
        }

        // GET: Localizacoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Localizacoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome")] Localizacoes localizacoes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(localizacoes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(localizacoes);
        }

        // GET: Localizacoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Localizacoes == null)
            {
                return NotFound();
            }

            var localizacoes = await _context.Localizacoes.FindAsync(id);
            if (localizacoes == null)
            {
                return NotFound();
            }
            return View(localizacoes);
        }

        // POST: Localizacoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome")] Localizacoes localizacoes)
        {
            if (id != localizacoes.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(localizacoes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocalizacoesExists(localizacoes.Id))
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
            return View(localizacoes);
        }

        // GET: Localizacoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Localizacoes == null)
            {
                return NotFound();
            }

            var localizacoes = await _context.Localizacoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (localizacoes == null)
            {
                return NotFound();
            }

            return View(localizacoes);
        }

        // POST: Localizacoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Localizacoes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Localizacoes'  is null.");
            }
            var localizacoes = await _context.Localizacoes.FindAsync(id);
            if (localizacoes != null)
            {
                _context.Localizacoes.Remove(localizacoes);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocalizacoesExists(int id)
        {
          return _context.Localizacoes.Any(e => e.Id == id);
        }
    }
}
