using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrabalhoPWEB1.Data;
using TrabalhoPWEB1.Models;

namespace TrabalhoPWEB1.Controllers
{
    public class EmpresasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmpresasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Ativar(int? id)
        {
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Nome");

            if (id == null || _context.Empresas == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresas.FindAsync(id);

            ModelState.Remove(nameof(empresa.Veiculos));

            empresa.EstadoSubscricao = "Válida";

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empresa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpresasExists(empresa.Id))
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

            return View(empresa);
        }

        public async Task<IActionResult> Desativar(int? id)
        {
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Nome");

            if (id == null || _context.Empresas == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresas.FindAsync(id);

            ModelState.Remove(nameof(empresa.Veiculos));

            empresa.EstadoSubscricao = "Inválida";

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empresa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpresasExists(empresa.Id))
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

            return View(empresa);
        }

        // GET: Empresas
        public async Task<IActionResult> Index(int id, string filtroEmpresa, string filtroSubs, string ordenarResultados)
        {
            ViewData["Nome"] = new SelectList(_context.Empresas.ToList(), "Id", "Nome");
            ViewData["EstadoSubscricao"] = new SelectList(_context.Empresas.Select(e => e.EstadoSubscricao).Distinct().ToList());


            var query = await _context.Empresas
                .ToListAsync();

            if (!string.IsNullOrEmpty(filtroEmpresa))
            {
                foreach (var empresa in _context.Empresas)
                {
                    if(empresa.Id.ToString() != filtroEmpresa)
                    {
                       query.Remove(empresa);
                    }
                    
                }
            }

            if (!string.IsNullOrEmpty(filtroSubs))
            {
                foreach (var empresa in _context.Empresas)
                {
                    if (empresa.EstadoSubscricao != filtroSubs)
                    {
                        query.Remove(empresa);
                    }

                }
            }

            if (!string.IsNullOrEmpty(ordenarResultados))
            {
                if (ordenarResultados == "UpN")
                {
                    query = query.OrderBy(v => v.Nome).ToList();
                }
                else if (ordenarResultados == "DownN")
                {
                    query = query.OrderByDescending(v => v.Nome).ToList();
                }
                else if (ordenarResultados == "UpES")
                {
                    query = query.OrderBy(v => v.EstadoSubscricao).ToList();
                }
                else if (ordenarResultados == "DownES")
                {
                    query = query.OrderByDescending(v => v.EstadoSubscricao).ToList();
                }
            }

            return View(query);
        }

        // GET: Empresas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Empresas == null)
            {
                return NotFound();
            }

            var empresas = await _context.Empresas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresas == null)
            {
                return NotFound();
            }

            return View(empresas);
        }

        // GET: Empresas/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empresas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Avaliacao,EstadoSubscricao")] Empresas empresa)
        {
            ModelState.Remove("Veiculos");

            if (ModelState.IsValid)
            {
                _context.Add(empresa);
                await _context.SaveChangesAsync();

                //Adicionar Default User - Gestor da Empresa
                var defaultUserGestor = new ApplicationUser
                {
                    UserName = "gestor-" + empresa.Nome + "@localhost.com",
                    Email = "gestor-" + empresa.Nome + "@localhost.com",
                    PrimeiroNome = "Gestor",    // OU DEIXAR VAZIO " "
                    UltimoNome = empresa.Nome,  // OU DEIXAR VAZIO " "
                    EmpresaId = empresa.Id,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    EstadoConta = "Ativa"
                };
                var user = await _userManager.FindByEmailAsync(defaultUserGestor.Email);
                if (user == null)
                {
                    await _userManager.CreateAsync(defaultUserGestor, "1qazZAQ!");
                    await _userManager.AddToRoleAsync(defaultUserGestor,
                    "Gestor");
                }

                return RedirectToAction(nameof(Index));
            }
           
            return View(empresa);
        }

        // GET: Empresas/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            

            if (id == null || _context.Empresas == null)
            {
                return NotFound();
            }

            var empresas = await _context.Empresas.FindAsync(id);
            if (empresas == null)
            {
                return NotFound();
            }

            return View(empresas);
        }

        // POST: Empresas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Avaliacao,EstadoSubscricao")] Empresas empresas)
        {

            ModelState.Remove("Veiculos");

            if (id != empresas.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empresas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpresasExists(empresas.Id))
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
            return View(empresas);
        }


        // GET: Empresas/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Empresas == null)
            {
                return NotFound();
            }

            var empresas = await _context.Empresas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empresas == null)
            {
                return NotFound();
            }

            return View(empresas);
        }

        // POST: Empresas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Empresas == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Empresas'  is null.");
            }

            var empresas = await _context.Empresas.Include(v => v.Veiculos).Where(e => e.Id == id).FirstOrDefaultAsync();
            if (empresas != null && empresas.Veiculos.Count == 0)
            {
                _context.Empresas.Remove(empresas);
            }
 
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpresasExists(int id)
        {
          return _context.Empresas.Any(e => e.Id == id);
        }
    }
}
