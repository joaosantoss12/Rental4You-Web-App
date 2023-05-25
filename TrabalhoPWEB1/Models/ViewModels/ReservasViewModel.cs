using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TrabalhoPWEB1.Models.ViewModels
{
    public class ReservasViewModel
    {
        [Display(Name = "Data de Início da Reserva", Prompt = "yyyy-mm-dd")]
        [DataType(DataType.Date)]
        public DateTime DataInicioReserva { get; set; }
        [Display(Name = "Data de Fim da Reserva", Prompt = "yyyy-mm-dd")]
        [DataType(DataType.Date)]
        public DateTime DataFimReserva { get; set; }

        public int VeiculoId { get; set; }
    }
}
