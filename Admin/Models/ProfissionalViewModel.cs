using System.Collections.Generic;

namespace Admin.Models
{
    public class ProfissionalViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Especialidade { get; set; }
        public string Telefone { get; set; }
        public string DataNascimento { get; set; }
        public string Email { get; set; }
        public Endereco Endereco { get; set; }
        public List<DocumentoViewModel> Documentos { get; set; }
    }
}