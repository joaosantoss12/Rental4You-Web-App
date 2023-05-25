using System.ComponentModel.DataAnnotations;
using TrabalhoPWEB1.Models;

namespace TrabalhoPWEB1.Models
{
    public class Empresas
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        [Range (0,5, ErrorMessage = "Introduza um numero entre 0 e 5")]
        public int Avaliacao { get; set; }
        public string EstadoSubscricao { get; set; }
        public string? url { get; set; }

        public ICollection<Veiculos> Veiculos { get; set; }
    }
}
