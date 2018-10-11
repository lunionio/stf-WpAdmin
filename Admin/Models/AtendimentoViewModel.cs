using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models
{
    public class AtendimentoViewModel : Base
    {
        public int IdUserAdmin { get; set; }
        public string Resposta { get; set; }
        public int TicketID { get; set; }
    }
}