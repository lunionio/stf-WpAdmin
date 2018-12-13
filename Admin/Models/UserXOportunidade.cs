using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Admin.Models
{
    public class UserXOportunidade
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public int OportunidadeId { get; set; }
        public Status Status { get; set; }
        public int StatusID { get; set; }


        public string EmailContratante { get; set; }
        public string EmailContratado { get; set; }
        public string NomeContratado { get; set; }
    }
}