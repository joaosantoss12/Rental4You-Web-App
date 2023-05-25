using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using TrabalhoPWEB1.Data;
using TrabalhoPWEB1.Models;
using TrabalhoPWEB1.Models.ViewModels;

namespace TrabalhoPWEB1.Controllers
{
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> EntregarVeiculo(int? id)
        {
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Nome");

            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.Include(r => r.veiculo).Where(e => e.Id == id).FirstOrDefaultAsync();

            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EntregarVeiculo([Bind("Id,DataInicioReserva,DataFimReserva,DuracaoEmDias,DuracaoEmHoras,PrecoReserva,DataHoraDoPedido,Estado,veiculoEntregue,veiculoRecebido,obs,emailFuncionario,VeiculoId,ApplicationUserId")] Reservas entregaVeiculoReserva, string danosVeiculos, string obs)
        {
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Nome");

            var userGestor = await _userManager.GetUserAsync(User);

            ModelState.Remove(nameof(entregaVeiculoReserva.veiculo));
            ModelState.Remove(nameof(entregaVeiculoReserva.ApplicationUser));

            entregaVeiculoReserva.veiculoEntregue = true;
            entregaVeiculoReserva.emailFuncionario = userGestor.Email;

            if(danosVeiculos == "Sim")
            {
                entregaVeiculoReserva.danosVeiculos = true;
            }
            else
            {
                entregaVeiculoReserva.danosVeiculos = false;
            }

            entregaVeiculoReserva.obs = obs;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entregaVeiculoReserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservasExists(entregaVeiculoReserva.Id))
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

            return View(entregaVeiculoReserva);
        }

        public async Task<IActionResult> DevolverVeiculo(int? id)
        {
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Nome");

            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.Include(r => r.veiculo).Where(e => e.Id == id).FirstOrDefaultAsync();

            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DevolverVeiculo([Bind("Id,DataInicioReserva,DataFimReserva,DuracaoEmDias,DuracaoEmHoras,PrecoReserva,DataHoraDoPedido,Estado,veiculoEntregue,veiculoRecebido,obs,obsCliente,emailFuncionario,VeiculoId,ApplicationUserId")] Reservas entregaVeiculoReserva, string danosVeiculos, string obsCliente)
        {
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Nome");

            ModelState.Remove(nameof(entregaVeiculoReserva.veiculo));
            ModelState.Remove(nameof(entregaVeiculoReserva.ApplicationUser));

            var veiculo = await _context.Veiculos.FindAsync(entregaVeiculoReserva.VeiculoId);

            entregaVeiculoReserva.veiculoRecebido = true;
            veiculo.Disponivel = true;

            if (danosVeiculos == "Sim")
            {
                entregaVeiculoReserva.danosVeiculos = true;
            }
            else
            {
                entregaVeiculoReserva.danosVeiculos = false;
            }

            entregaVeiculoReserva.obs = obsCliente;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entregaVeiculoReserva);
                    _context.Update(veiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservasExists(entregaVeiculoReserva.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MinhasReservas));
            }

            return View(entregaVeiculoReserva);
        }

        [Authorize(Roles="Gestor,Funcionario")]
        public async Task<IActionResult> Confirmar(int? id)
        {
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Nome");

            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FindAsync(id);
            var veiculo = await _context.Veiculos.FindAsync(reserva.VeiculoId);

            ModelState.Remove(nameof(reserva.veiculo));
            ModelState.Remove(nameof(reserva.ApplicationUser));

            reserva.Estado = "Confirmada";
            veiculo.Disponivel = false;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    _context.Update(veiculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservasExists(reserva.Id))
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

            return View(reserva);
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Pedido()
        {
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Nome", "Preco");
            return View();
        }       

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Calcular([Bind("DataInicioReserva,DataFimReserva,VeiculoId,PrecoReserva")] ReservasViewModel reserva)
        {
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Nome", "Preco");

            decimal NrDias = 0;
            decimal NrHoras = 0;

            if (reserva.DataInicioReserva > reserva.DataFimReserva)
            {
                
            }
                
            if(reserva.DataInicioReserva < DateTime.Now)
            {
                
            }        

            var veiculo = _context.Veiculos.Find(reserva.VeiculoId);

            if (ModelState.IsValid)
            {
                NrDias = (decimal)(reserva.DataFimReserva - reserva.DataInicioReserva).TotalDays;
                NrHoras = (decimal)(reserva.DataFimReserva - reserva.DataInicioReserva).TotalHours;

                Reservas x = new Reservas();
                x.DataFimReserva = reserva.DataFimReserva;
                x.DataInicioReserva = reserva.DataInicioReserva;
                x.DuracaoEmDias = NrDias;
                x.DuracaoEmHoras = NrHoras;

                x.VeiculoId = reserva.VeiculoId;
                x.veiculo = veiculo;
                x.PrecoReserva = veiculo.Preco * NrDias;
                //x.ApplicationUserId = _userManager.GetUserId(User);

                return View("PedidoConfirmacao", x);

            }
            
            return View("reserva", reserva);
        }

        // GET: Reservas
        [Authorize(Roles="Gestor, Funcionario")]
        public async Task<IActionResult> Index(string pesquisarTexto, string filtroCategoriaReserva, DateTime pesquisaLevantamento, DateTime pesquisaEntrega)
        {

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome");
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Nome");

            var userGestor = await _userManager.GetUserAsync(User);

            if (pesquisaLevantamento == DateTime.Parse("01/01/0001 00:00:00"))
            {
                pesquisaLevantamento = DateTime.Now;
            }
            else
            {
                return View(await _context.Reservas.Where(e=>e.DataInicioReserva >= pesquisaLevantamento).Include(a => a.veiculo).Include(e => e.veiculo.Categoria).Include(e => e.veiculo.Empresa).Where(e => e.veiculo.EmpresaId == userGestor.EmpresaId).Include(u => u.ApplicationUser).ToListAsync());
            }
            if (pesquisaEntrega == DateTime.Parse("01/01/0001 00:00:00"))
            {
                pesquisaEntrega = DateTime.Now;
            }
            else
            {
                return View(await _context.Reservas.Where(e => e.DataFimReserva <= pesquisaEntrega).Include(a => a.veiculo).Include(e => e.veiculo.Categoria).Include(e => e.veiculo.Empresa).Where(e => e.veiculo.EmpresaId == userGestor.EmpresaId).Include(u => u.ApplicationUser).ToListAsync());
            }

          

           

            if (!string.IsNullOrEmpty(pesquisarTexto))
            {
                return View(await _context.Reservas.Include(a => a.veiculo).Where(e => e.veiculo.Nome.Contains(pesquisarTexto) || e.ApplicationUser.PrimeiroNome.Contains(pesquisarTexto)|| e.ApplicationUser.UltimoNome.Contains(pesquisarTexto)).Include(e => e.veiculo.Categoria).Include(e => e.veiculo.Empresa).Where(e => e.veiculo.EmpresaId == userGestor.EmpresaId).Include(u => u.ApplicationUser).ToListAsync());
            }

            if(!string.IsNullOrEmpty(filtroCategoriaReserva))
            {
                return View(await _context.Reservas.Include(a => a.veiculo).Include(e => e.veiculo.Categoria).Where(e => e.veiculo.CategoriaId.ToString() == filtroCategoriaReserva).Include(e => e.veiculo.Empresa).Where(e => e.veiculo.EmpresaId == userGestor.EmpresaId).Include(u => u.ApplicationUser).ToListAsync());
            }

            

            if (User.IsInRole("Gestor"))
            {
                return View(await _context.Reservas.Include(a => a.veiculo).Where(e => e.veiculo.EmpresaId == userGestor.EmpresaId).Include(e => e.veiculo.Categoria).Include(e => e.veiculo.Empresa).Where(e => e.veiculo.EmpresaId == userGestor.EmpresaId).Include(u => u.ApplicationUser).ToListAsync());
            }

            return View (await _context.Reservas.Include(a => a.veiculo).Include(e => e.veiculo.Empresa).Include(u => u.ApplicationUser).ToListAsync());
        }

        public async Task<IActionResult> MinhasReservas()
        {

            var applicationDbContext = _context.Reservas.Include(a => a.veiculo)
                .Include(u => u.ApplicationUser).
                Where(c => c.ApplicationUserId == _userManager.GetUserId(User));

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reservas = await _context.Reservas
                .Include(u => u.ApplicationUser)
                .Include(v => v.veiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservas == null)
            {
                return NotFound();
            }

            return View(reservas);
        }

        // GET: Reservas/Create
        [Authorize(Roles = "Cliente")]
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Create([Bind("Id,VeiculoId,Estado,veiculoEntregue,veiculoRecebido,DataReserva,DataInicioReserva,DataFimReserva,DuracaoEmDias,DuracaoEmHoras,obs,obsCliente,emailFuncionario,PrecoReserva,DataHoraDoPedido,ApplicationUserId")] Reservas reservas)
        {
            reservas.veiculoEntregue = false;
            reservas.veiculoRecebido = false;
            reservas.Estado = "Não Confirmada";

            reservas.DataHoraDoPedido = DateTime.Now;

            ModelState.Remove(nameof(reservas.PrecoReserva));
            ModelState.Remove(nameof(reservas.ApplicationUser));
            ModelState.Remove(nameof(reservas.ApplicationUserId));
            ModelState.Remove(nameof(reservas.veiculo));

            reservas.ApplicationUserId = _userManager.GetUserId(User);

            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Id", reservas.VeiculoId);

            if (ModelState.IsValid)
            {
                _context.Add(reservas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MinhasReservas));
            }
            
            return View(reservas);
        }

        [Authorize(Roles = "Gestor")]
        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reservas = await _context.Reservas.FindAsync(id);
            if (reservas == null)
            {
                return NotFound();
            }
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Id", reservas.VeiculoId);
            return View(reservas);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gestor")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataReserva,DataInicioReserva,DataFimReserva,Preco,DataHoraDoPedido,ApplicationUserId")] Reservas reservas)
        {
            if (id != reservas.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservasExists(reservas.Id))
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
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Id", reservas.VeiculoId);
            return View(reservas);
        }

        // GET: Reservas/Delete/5
        [Authorize(Roles = "Gestor,Funcionario,Cliente")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reservas = await _context.Reservas
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservas == null)
            {
                return NotFound();
            }

            return View(reservas);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gestor,Funcionario,Cliente")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string erro = "<script>alert('Não é possível cancelar esta Reserva. A data de inicio da Reserva não pode ser menor ou igual que a data atual!');</script>";

            if (_context.Reservas == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reservas'  is null.");
            }
            var reservas = await _context.Reservas.FindAsync(id);
            if (reservas != null)
            {
                _context.Reservas.Remove(reservas);
            }
            if(reservas.DataInicioReserva <= DateTime.Now)
            {
                
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MinhasReservas));
        }

        private bool ReservasExists(int id)
        {
          return _context.Reservas.Any(e => e.Id == id);
        }
    }
}
