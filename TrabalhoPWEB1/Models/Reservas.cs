using System.ComponentModel.DataAnnotations;
using System.Data;
using TrabalhoPWEB1.Models;

namespace TrabalhoPWEB1.Models
{
    public class Reservas
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataInicioReserva { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataFimReserva { get; set; }
        public decimal DuracaoEmDias { get; set; }
        public decimal DuracaoEmHoras { get; set; }
        public decimal PrecoReserva { get; set; }
        public DateTime DataHoraDoPedido { get; set; }

        public string? Estado { get; set; }

        public bool veiculoEntregue { get; set; }
        public bool veiculoRecebido { get; set; }

        public bool danosVeiculos { get; set; }
        public string? obs { get; set; }    // OBSERVAÇÕES DO FUNCIONARIO / GESTOR
        public string? obsCliente { get; set; }

        public string? emailFuncionario { get; set; }

        public int VeiculoId { get; set; }
        public Veiculos veiculo { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}