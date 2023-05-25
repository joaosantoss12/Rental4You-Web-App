using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TrabalhoPWEB1.Models
{
        public class ApplicationUser : IdentityUser
        {
            public string PrimeiroNome { get; set; }
            public string UltimoNome { get; set; }
            [PersonalData]
            public DateTime DataNascimento { get; set; }
            [PersonalData]
            public int NIF { get; set; }

            public string EstadoConta { get; set; }

            public int? EmpresaId { get; set; }
            public Empresas? Empresa { get; set; }

        }

}
