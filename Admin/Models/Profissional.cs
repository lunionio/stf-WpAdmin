using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models
{
    public class Profissional : Base
    {
        public string Email { get; set; }
        public Endereco Endereco { get; set; }
        public Telefone Telefone { get; set; }
        public IList<ProfissionalFormacao> Formacoes { get; set; }

        public Profissional()
        { }

        public Profissional(int id, string nome, string telefone, string email, Endereco endereco)
        {
            ID = id;
            Nome = nome;
            Telefone = new Telefone() { ProfissionalId = id, Numero = telefone, Nome = nome };
            Email = email;
            Endereco = endereco;
        }
    }
}