using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models.Relatorio
{
    public class RelatorioExtratoViewModel
    {
        public string Codigo { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string Descricao { get; set; }
        public string Valor { get; set; }
        public string NaturezaOperacao { get; set; }
        public string Usuario { get; set; }
        public string Data { get; set; }
        public string Hora { get; set; }
        public string Status { get; set; }
    }
}