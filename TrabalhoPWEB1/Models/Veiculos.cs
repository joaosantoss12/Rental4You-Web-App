using System.ComponentModel.DataAnnotations;
using System.Data;
using TrabalhoPWEB1.Models;

namespace TrabalhoPWEB1.Models
{
    public class Veiculos
    {
        public int Id { get; set; }
        public bool Disponivel { get; set; }
        public string Nome { get; set; }
        public string Marca { get; set; }
        //public string Categoria { get; set; }
        public decimal Preco { get; set; }
        public string Estado { get; set; }

        public int anoProducao { get; set; }

        [DataType(DataType.Date)]
        public DateTime? dataLevantamento { get; set; }
        [DataType(DataType.Date)]
        public DateTime? dataEntrega { get; set; }

        // URL IMAGEM
        public string? url { get; set; }

        public int nKm { get; set; }

        public int EmpresaId { get; set; } 
        public Empresas Empresa { get; set; }

        public int LocalizacaoId { get; set; }
        public Localizacoes Localizacao { get; set; }

        public int CategoriaId { get; set; }
        public Categorias Categoria { get; set; }

    }
}
