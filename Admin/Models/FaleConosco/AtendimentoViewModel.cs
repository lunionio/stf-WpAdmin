using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models
{
    public class AtendimentoViewModel : Base
    {
        public AtendimentoViewModel(string email, string origem, string categoria, string numero, string descricao, int ticketId, int ticketStatusId)
        {
            Email = email;
            Origem = origem;
            Categoria = categoria;
            Numero = numero;
            Descricao = descricao;
            TicketId = ticketId;
            TicketStatusId = ticketStatusId;
        }

        public AtendimentoViewModel()
        {

        }

        public string Email { get; set; }
        public string Origem { get; set; }
        public string Resposta { get; set; }
        public string Categoria { get; set; }
        public string Numero { get; set; }
        public int TicketId { get; set; }
        public int TicketStatusId { get; set; }
    }
}