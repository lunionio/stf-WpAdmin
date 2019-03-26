using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models.Financeiro
{
    public class CreditoViewModel
    {
        public string Empresa { get; set; }
        public string Valor { get; set; }
        public string Natureza { get; set; }
        public string Descricao { get; set; }
        public decimal Taxa { get; set; }

        public int? EmpresaId { get; set; }
        public int? NaturezaId { get; set; }
    }
}