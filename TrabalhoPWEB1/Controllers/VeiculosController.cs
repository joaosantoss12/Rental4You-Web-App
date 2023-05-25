using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrabalhoPWEB1.Data;
using TrabalhoPWEB1.Models;
using TrabalhoPWEB1.Models.ViewModels;

namespace TrabalhoPWEB1.Controllers
{
    public class VeiculosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VeiculosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Resultados pesquisa para Search.cshtml
        [HttpPost]  // Sem isto é método GET
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Search([Bind("pesquisarTexto")]
        PesquisaVeiculoViewModel pesquisaVeiculo, int pesquisaLocalizacao,
            DateTime pesquisaLevantamento, DateTime pesquisaEntrega,
            string ordenarResultados, string filtro, string filtroTipo, string filtroEstado,
            string ordenarResultados2)
        {
            ViewData["EmpresaId"] = new SelectList(_context.Empresas.ToList(), "Id", "Nome");
            ViewData["LocalizacaoId"] = new SelectList(_context.Localizacoes.ToList(), "Id", "Nome");
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.ToList(), "Id", "Nome");

            if (pesquisaLevantamento == DateTime.Parse("01/01/0001 00:00:00"))
            {
                pesquisaLevantamento = DateTime.Now;
            }
            if (pesquisaEntrega == DateTime.Parse("01/01/0001 00:00:00"))
            {
                pesquisaEntrega = DateTime.Now;
            }

            var query = _context.Veiculos
                        .Include(p => p.Empresa)
                        .Include(p => p.Localizacao)
                        .Include(p => p.Categoria)
                        .Where(p => p.Disponivel == true && p.Empresa.EstadoSubscricao == "Válida");


            if (string.IsNullOrEmpty(pesquisaVeiculo.pesquisarTexto))
            {
                query = query.OrderBy(v => v.Preco);
            }
            else
            {
                query = query
                    .Where(c => (
                        c.Nome.Contains(pesquisaVeiculo.pesquisarTexto)
                        || c.Marca.Contains(pesquisaVeiculo.pesquisarTexto)
                        || c.Localizacao.Nome.Contains(pesquisaVeiculo.pesquisarTexto)
                        || c.Categoria.Nome.Contains(pesquisaVeiculo.pesquisarTexto)
                        ));

                /*
                 * if(pesqusiaCategoria != 0){
                 * 
                 * }
                 */

            }

            if (User.IsInRole("Funcionario") || User.IsInRole("Gestor"))
            {
                if (!string.IsNullOrEmpty(filtroEstado))
                {
                    foreach (var veiculo in _context.Veiculos)
                    {
                        query = query.Where(c => c.Estado == filtroEstado);
                    }
                }
            }

            if(User.IsInRole("Funcionario") || User.IsInRole("Gestor"))
            {
                if (!string.IsNullOrEmpty(ordenarResultados2))
                {
                    if (ordenarResultados == "UpC")
                    {
                        query = query.OrderBy(v => v.Categoria.Nome);
                    }
                    else if (ordenarResultados == "DownC")
                    {
                        query = query.OrderByDescending(v => v.Categoria.Nome);
                    }
                    else if (ordenarResultados == "UpE")
                    {
                        query = query.OrderBy(v => v.Estado);
                    }
                    else if (ordenarResultados == "DownE")
                    {
                        query = query.OrderByDescending(v => v.Estado);
                    }
                }
            }
            
            if (!string.IsNullOrEmpty(ordenarResultados))
            {
                if (ordenarResultados == "UpP")
                {
                    query = query.OrderBy(v => v.Preco);
                }
                else if (ordenarResultados == "DownP")
                {
                    query = query.OrderByDescending(v => v.Preco);
                }
                else if (ordenarResultados == "UpC")
                {
                    query = query.OrderBy(v => v.Empresa.Avaliacao);
                }
                else if (ordenarResultados == "DownC")
                {
                    query = query.OrderByDescending(v => v.Empresa.Avaliacao);
                }
            }

            if (!string.IsNullOrEmpty(filtro))
            {
                foreach (var empresa in _context.Empresas)
                {
                    query = query.Where(c => c.Empresa.Id.ToString() == filtro);
                }
            }
            if (!string.IsNullOrEmpty(filtroTipo))
            {
                foreach (var veiculo in _context.Veiculos)
                {
                    query = query.Where(c => c.Categoria.Id.ToString() == filtroTipo);
                }
            }

            if (User.IsInRole("Funcionario") || User.IsInRole("Gestor"))
            {
                int? userEmpresaId = (await _userManager.GetUserAsync(User)).EmpresaId;

                query = query.Where(v => v.EmpresaId == userEmpresaId);
            }


            pesquisaVeiculo.ListaDeVeiculos = await query.ToListAsync();

            foreach (var veiculo in pesquisaVeiculo.ListaDeVeiculos.ToList())
            {
                if (veiculo.dataEntrega != null && veiculo.dataLevantamento != null)
                {
                    if (veiculo.dataEntrega >= pesquisaLevantamento && veiculo.dataEntrega <= pesquisaEntrega)
                    {
                        pesquisaVeiculo.ListaDeVeiculos.Remove(veiculo);
                    }
                    if (veiculo.dataLevantamento >= pesquisaLevantamento && veiculo.dataLevantamento <= pesquisaEntrega)
                    {
                        pesquisaVeiculo.ListaDeVeiculos.Remove(veiculo);
                    }
                }

            }

            pesquisaVeiculo.nResultados = pesquisaVeiculo.ListaDeVeiculos.Count;

            return View(pesquisaVeiculo);
        }

        // GET: Veiculos
        public async Task<IActionResult> Index()
        {
            ViewData["EmpresaId"] = new SelectList(_context.Empresas.ToList(), "Id", "Nome");
            ViewData["LocalizacaoId"] = new SelectList(_context.Localizacoes.ToList(), "Id", "Nome");
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.ToList(), "Id", "Nome");

            if (User.IsInRole("Funcionario") || User.IsInRole("Gestor"))
            {
                int? userEmpresaId = (await _userManager.GetUserAsync(User)).EmpresaId;

                return View(await _context.Veiculos
                    .Include("Categoria")
                    .Include("Empresa").Where(v => v.EmpresaId == userEmpresaId)
                    .Include("Localizacao").ToListAsync());
            }
            else
                return View(await _context.Veiculos.Where(c => c.Disponivel == true).Include("Categoria").Include("Empresa").Include("Localizacao").ToListAsync());
        }

        // GET: Veiculos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewData["CategoriaId"] = new SelectList(_context.Empresas.ToList(), "Id", "Nome");

            if (id == null || _context.Veiculos == null)
            {
                return NotFound();
            }

            var veiculos = await _context.Veiculos
                .Include("Empresa")
                .Include("Localizacao")
                .Include("Categoria")
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculos == null)
            {
                return NotFound();
            }

            return View(veiculos);
        }

        // GET: Veiculos/Create
        [Authorize(Roles= "Funcionario, Gestor")]
        public async Task<IActionResult> Create()
        {
            int? userEmpresaId = (await _userManager.GetUserAsync(User)).EmpresaId;

            ViewData["EmpresaId"] = new SelectList(_context.Empresas.Where(e => e.Id == userEmpresaId).ToList(), "Id", "Nome");
            ViewData["LocalizacaoId"] = new SelectList(_context.Localizacoes.ToList(), "Id", "Nome");
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.ToList(), "Id", "Nome");

            return View();
        }

        // POST: Veiculos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles= "Funcionario, Gestor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,nKm,Marca,Tipo,EmpresaId,Preco,LocalizacaoId,Estado,anoProducao,dataLevantamento,dataEntrega,url,Disponivel")] Veiculos veiculos)
        {
            int? userEmpresaId = (await _userManager.GetUserAsync(User)).EmpresaId;

            ViewData["EmpresaId"] = new SelectList(_context.Empresas.Where(e => e.Id == userEmpresaId).ToList(), "Id", "Nome");
            ViewData["LocalizacaoId"] = new SelectList(_context.Localizacoes.ToList(), "Id", "Nome");
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.ToList(), "Id", "Nome");

            ModelState.Remove(nameof(veiculos.Empresa));
            ModelState.Remove(nameof(veiculos.Localizacao));
            ModelState.Remove(nameof(veiculos.Categoria));

            if (ModelState.IsValid)
            {
                _context.Add(veiculos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(veiculos);
        }

        // GET: Veiculos/Edit/5
        [Authorize(Roles= "Funcionario, Gestor")]
        public async Task<IActionResult> Edit(int? id)
        {
            int? userEmpresaId = (await _userManager.GetUserAsync(User)).EmpresaId;

            ViewData["EmpresaId"] = new SelectList(_context.Empresas.Where(e => e.Id == userEmpresaId).ToList(), "Id", "Nome");
            ViewData["LocalizacaoId"] = new SelectList(_context.Localizacoes.ToList(), "Id", "Nome");
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.ToList(), "Id", "Nome");

            if (id == null || _context.Veiculos == null)
            {
                return NotFound();
            }

            var veiculos = await _context.Veiculos.FindAsync(id);
            if (veiculos == null)
            {
                return NotFound();
            }
            return View(veiculos);
        }

        // POST: Veiculos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles= "Funcionario, Gestor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Marca,Tipo,EmpresaId,Preco,LocalizacaoId,Estado,anoProducao,dataLevantamento,dataEntrega,url,Disponivel,CategoriaId")] Veiculos veiculos)
        {
            ViewData["EmpresaId"] = new SelectList(_context.Empresas.ToList(), "Id", "Nome");
            ViewData["LocalizacaoId"] = new SelectList(_context.Localizacoes.ToList(), "Id", "Nome");
            ViewData["CategoriaId"] = new SelectList(_context.Categorias.ToList(), "Id", "Nome");

            ModelState.Remove(nameof(veiculos.Empresa));
            ModelState.Remove(nameof(veiculos.Localizacao));
            ModelState.Remove(nameof(veiculos.Categoria));

            if (id != veiculos.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(veiculos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeiculosExists(veiculos.Id))
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
            return View(veiculos);
        }

        // GET: Veiculos/Delete/5
        [Authorize(Roles= "Funcionario, Gestor")]
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null || _context.Veiculos == null)
            {
                return NotFound();
            }

            var veiculos = await _context.Veiculos
                .Include("Empresa")
                .Include("Localizacao")
                .Include("Categoria")
                .FirstOrDefaultAsync(m => m.Id == id);
            if (veiculos == null)
            {
                return NotFound();
            }

            return View(veiculos);
        }

        // POST: Veiculos/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles= "Funcionario, Gestor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            if (_context.Veiculos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Veiculos'  is null.");
            }
            var veiculos = await _context.Veiculos.FindAsync(id);
            if (veiculos != null)
            {
                _context.Veiculos.Remove(veiculos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VeiculosExists(int id)
        {
            return _context.Veiculos.Any(e => e.Id == id);
        }
    }
}
