using TrabalhoPWEB1.Data;
using TrabalhoPWEB1.Models;
using TrabalhoPWEB1.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace TrabalhoPWEB1.Controllers
{
    public class RolesManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesManagerController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {

            var roles = await _roleManager.Roles.ToListAsync();

            return View(roles);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AddRole(string roleName)
        {
            IdentityRole newRole = new IdentityRole();
            newRole.Name = roleName;

            if (!String.IsNullOrEmpty(roleName))
            {
                await _roleManager.CreateAsync(newRole);
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Remove(string? id)
        {
            await _roleManager.DeleteAsync(await _roleManager.FindByIdAsync(id));

            return RedirectToAction("Index");
        }
    }
}