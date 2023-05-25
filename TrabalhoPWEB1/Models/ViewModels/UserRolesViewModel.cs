namespace TrabalhoPWEB1.Models.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string PrimeiroNome { get; set; }
        public string UltimoNome { get; set; }
        public string UserName { get; set; }

        public string EstadoConta { get; set; }
        public int? EmpresaId { get; set; }
        public Empresas? Empresa { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
