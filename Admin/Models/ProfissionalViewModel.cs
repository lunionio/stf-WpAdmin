using System;
using System.Collections.Generic;

namespace Admin.Models
{
    public class ProfissionalViewModel
    {
        public ProfissionalViewModel(int id, string nome, string especialidade, string telefone, int telefoneId, string dataNascimento, string email, Endereco endereco)
        {
            Id = id;
            Nome = nome;
            Especialidade = especialidade;
            Telefone = telefone;
            TelefoneId = TelefoneId;
            DataNascimento = dataNascimento;
            Email = email;
            Endereco = endereco;
        }

        public ProfissionalViewModel(int iD, string nome)
        {

        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Especialidade { get; set; }
        public string Telefone { get; set; }
        public int TelefoneId { get; set; }
        public DateTime DataCriacao { get; set; }
        public string DataNascimento { get; set; }
        public string Email { get; set; }
        public Endereco Endereco { get; set; }
        public List<DocumentoViewModel> Documentos { get; set; }
    }
}