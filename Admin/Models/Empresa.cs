using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Admin.Models
{
    public class Empresa : Base
    {
        public string CNAE_S { get; set; }
        public string RazaoSocial { get; set; }
        [Required]
        public string CNPJ { get; set; }
        public Endereco Endereco { get; set; }
        public IList<Contato> Contatos { get; set; }
    }
}
