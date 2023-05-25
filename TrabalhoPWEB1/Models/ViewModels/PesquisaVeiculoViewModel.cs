using TrabalhoPWEB1.Data;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TrabalhoPWEB1.Models.ViewModels
{
    public class PesquisaVeiculoViewModel
    {
        public List<Veiculos> ListaDeVeiculos { get; set; }
        public int nResultados { get; set; }

        [Display(Name = "Pesquisar veiculos:", Prompt = "Ex.: Lamborghini")]
        public string pesquisarTexto { get; set; }
    }
}
