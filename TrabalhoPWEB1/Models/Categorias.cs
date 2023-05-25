using System.ComponentModel.DataAnnotations;
using TrabalhoPWEB1.Models;

namespace TrabalhoPWEB1.Models
{
    public class Categorias
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public ICollection<Veiculos> Veiculos { get; set; }
    }
}
