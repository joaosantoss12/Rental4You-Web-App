using System.Data;
using TrabalhoPWEB1.Models;

namespace TrabalhoPWEB1.Models
{
    public class Localizacoes
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public ICollection<Veiculos> Veiculos { get; set; }
    }
}
