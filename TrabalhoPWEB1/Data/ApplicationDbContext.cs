using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrabalhoPWEB1.Models;

namespace TrabalhoPWEB1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Veiculos> Veiculos { get; set; }
        public DbSet<Empresas> Empresas { get; set; }
        public DbSet<Localizacoes> Localizacoes { get; set; }
        public DbSet<Categorias> Categorias { get; set; }
        public DbSet<TrabalhoPWEB1.Models.Reservas> Reservas { get; set; }
    }
}