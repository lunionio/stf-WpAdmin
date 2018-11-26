using System;

namespace Admin.Models.FaleConosco
{
    public class Atendimento : Base
    {
        public Atendimento(int idUserAdmin, string resposta, int ticketID)
        {
            IdUserAdmin = idUserAdmin;
            Resposta = resposta;
            TicketID = ticketID;
        }

        public Atendimento()
        {

        }

        public int IdUserAdmin { get; set; }
        public string Resposta { get; set; }
        public int TicketID { get; set; }
        public Ticket Ticket { get; set; }
    }
}
