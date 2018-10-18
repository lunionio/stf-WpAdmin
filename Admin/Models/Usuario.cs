namespace Admin.Models
{
    public class Usuario : Base
    {
        public string Login { get; set; }
        public string SobreNome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string VAdmin { get; set; }
        public string Avatar { get; set; }
        public int IdEmpresa { get; set; }
    }

    public enum Status
    {
        Ativo = 1,
        Inativo = 0,
    }
}
