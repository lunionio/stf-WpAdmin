using Newtonsoft.Json;
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
        public DateTime Data { get; set; }
        public TimeSpan Hora { get; set; }
        public string Status { get; set; }

        [JsonIgnore]
        public string DataExtrato
        {
            get => Data.ToString("dd/MM/yyyy");
        }

        [JsonIgnore]
        public string HoraExtrato
        {
            get => Hora.ToString(@"hh\:mm");
        }
    }
}