using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TrabalhoPWEB1.Models.ViewModels;
using TrabalhoPWEB1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TrabalhoPWEB1.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Numerics;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TrabalhoPWEB1.Controllers
{
    public class UserRolesManagerController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        public UserRolesManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager, IUserStore<ApplicationUser> userStore)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;

            _emailStore = GetEmailStore();
            _userStore = userStore;
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        // GET: Veiculos/Create
        [Authorize(Roles = "Gestor")]
        public async Task<IActionResult> CreateGestor()
        {
            ViewData["EmpresaId"] = new SelectList(_context.Empresas.ToList(), "Id", "Nome");

            return View();
            
        }

        // POST: Veiculos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Gestor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGestor([Bind("PrimeiroNome,UltimoNome,Email")] ApplicationUser newuser)
        {
            ViewData["EmpresaId"] = new SelectList(_context.Empresas.ToList(), "Id", "Nome");

            Empresas? empresaAtual = (await _userManager.GetUserAsync(User)).Empresa;

            //Adicionar - Gestor da Empresa
            var defaultUserGestor = new ApplicationUser
            {
                UserName = newuser.Email,
                Email = newuser.Email,
                PrimeiroNome = newuser.PrimeiroNome,
                UltimoNome = newuser.UltimoNome,
                EmpresaId = empresaAtual.Id,
                EstadoConta = "Ativa",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
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

        // GET: Veiculos/Create
        [Authorize(Roles = "Gestor")]
        public async Task<IActionResult> CreateFuncionario()
        {
            ViewData["EmpresaId"] = new SelectList(_context.Empresas.ToList(), "Id", "Nome");

            return View();

        }

        // POST: Veiculos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Gestor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFuncionario([Bind("PrimeiroNome,UltimoNome,Email")] ApplicationUser newuser)
        {
            ViewData["EmpresaId"] = new SelectList(_context.Empresas.ToList(), "Id", "Nome");

            Empresas? empresaAtual = (await _userManager.GetUserAsync(User)).Empresa;

            //Adicionar - Gestor da Empresa
            var defaultUserGestor = new ApplicationUser
            {
                UserName = newuser.Email,
                Email = newuser.Email,
                PrimeiroNome = newuser.PrimeiroNome,
                UltimoNome = newuser.UltimoNome,
                EmpresaId = empresaAtual.Id,
                EstadoConta = "Ativa",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var user = await _userManager.FindByEmailAsync(defaultUserGestor.Email);
            if (user == null)
            {
                await _userManager.CreateAsync(defaultUserGestor, "1qazZAQ!");
                await _userManager.AddToRoleAsync(defaultUserGestor,
                "Funcionario");
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles="Administrador, Gestor")]
        public async Task<IActionResult> AtivarConta(string userId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            user.EstadoConta = "Ativa";
            user.LockoutEnabled = false;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExist(user.Id))
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


            return View(user);
        }

        [Authorize(Roles = "Administrador, Gestor")]
        public async Task<IActionResult> DesativarConta(string userId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            user.EstadoConta = "Inativa";
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTime.Now.AddYears(100);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExist(user.Id))
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


            return View(user);
        }

        [Authorize(Roles = "Administrador, Gestor")]
        public async Task<IActionResult> Index()
        {
            ViewData["EmpresaId"] = new SelectList(_context.Empresas.ToList(), "Id", "Nome");

            if (User.IsInRole("Gestor"))
            {
                int? empresaGestor = (await _userManager.GetUserAsync(User)).EmpresaId;
                string idGestor = (await _userManager.GetUserAsync(User)).Id;

                var usersG = await _context.Users.Include(e => e.Empresa).ToListAsync();

                List<UserRolesViewModel> userRolesManagerViewModelG = new List<UserRolesViewModel>();

   

                foreach (var user in usersG)
                {
                    if(user.EmpresaId == empresaGestor && user.Id != idGestor)
                    {
                        UserRolesViewModel userRolesViewModelG = new UserRolesViewModel();

                        userRolesViewModelG.UserId = user.Id;
                        userRolesViewModelG.UserName = user.UserName;
                        userRolesViewModelG.PrimeiroNome = user.PrimeiroNome;
                        userRolesViewModelG.UltimoNome = user.UltimoNome;
                        userRolesViewModelG.Empresa = user.Empresa;
                        userRolesViewModelG.EmpresaId = user.EmpresaId;
                        userRolesViewModelG.EstadoConta = user.EstadoConta;

                        userRolesViewModelG.Roles = await _userManager.GetRolesAsync(user);

                        userRolesManagerViewModelG.Add(userRolesViewModelG);
                    }
                    
                }
                return View(userRolesManagerViewModelG);
            }


            var users = await _context.Users.Include(e => e.Empresa).ToListAsync();

            List<UserRolesViewModel> userRolesManagerViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                UserRolesViewModel userRolesViewModel = new UserRolesViewModel();

                userRolesViewModel.UserId = user.Id;
                userRolesViewModel.UserName = user.UserName;
                userRolesViewModel.PrimeiroNome = user.PrimeiroNome;
                userRolesViewModel.UltimoNome = user.UltimoNome;
                userRolesViewModel.Empresa = user.Empresa;
                userRolesViewModel.EmpresaId = user.EmpresaId;
                userRolesViewModel.EstadoConta = user.EstadoConta;

                userRolesViewModel.Roles = await _userManager.GetRolesAsync(user);

                userRolesManagerViewModel.Add(userRolesViewModel);
            }
            return View(userRolesManagerViewModel);
        }

        [Authorize(Roles = "Administrador")]
        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }


        // EDITAR UTILIZADOR
        // GET: Utilizadores/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(string userId)
        {

            if (userId == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            UserRolesViewModel userRolesViewModel = new UserRolesViewModel();

            userRolesViewModel.UserId = user.Id;
            userRolesViewModel.UserName = user.UserName;
            userRolesViewModel.PrimeiroNome = user.PrimeiroNome;
            userRolesViewModel.UltimoNome = user.UltimoNome;
            userRolesViewModel.Empresa = user.Empresa;
            userRolesViewModel.EmpresaId = user.EmpresaId;
            userRolesViewModel.EstadoConta = user.EstadoConta;

            userRolesViewModel.Roles = await _userManager.GetRolesAsync(user);

            return View(userRolesViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("UserId,PrimeiroNome,UltimoNome,UserName,EmpresaId")] UserRolesViewModel userr)
        {

            ViewData["EmpresaId"] = new SelectList(_context.Empresas.ToList(), "Id", "Nome");

            ModelState.Remove(nameof(userr.Roles));
            ModelState.Remove(nameof(userr.Empresa));

            var user = await _userManager.FindByIdAsync(userr.UserId);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            user.Id = userr.UserId;
            user.UserName = userr.UserName;
            user.PrimeiroNome = userr.PrimeiroNome;
            user.UltimoNome = userr.UltimoNome;
            user.Empresa = userr.Empresa;
            user.EmpresaId = userr.EmpresaId;
            user.EstadoConta = userr.EstadoConta;


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExist(userr.UserId))
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


            return View(user);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(string userId)
        {

            if (userId == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.userId = userId;
            ViewData["UserName"] = user.UserName;

            List<ManageUserRolesViewModel> roles = new List<ManageUserRolesViewModel>();
            var userRoles = await _userManager.GetRolesAsync(await _userManager.Users.Where(u => u.Id == userId).FirstAsync());

            var listRoles = await _roleManager.Roles.ToListAsync();

            foreach (var role in listRoles)
            {
                ManageUserRolesViewModel roleViewModel = new ManageUserRolesViewModel();
                roleViewModel.RoleId = role.Id;
                roleViewModel.RoleName = role.Name;
                roleViewModel.Selected = userRoles.Contains(role.Name);

                roles.Add(roleViewModel);
            }

            return View(roles);
        }
        [HttpPost]
        public async Task<IActionResult> Details(List<ManageUserRolesViewModel> model,
       string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user,
                model.Where(x => x.Selected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        // GET: Utilizadores/Delete/5
        [Authorize(Roles = "Gestor")]
        public async Task<IActionResult> Delete(string? userId)
        {
            if (userId == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Utilizadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Gestor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string userId)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ApplicationDbContext.AspNetUsers'  is null.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool UsersExist(string id)
        {
            return _userManager.Users.Any(e => e.Id == id);
        }
    }
}
