﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Admin.Models
{
    public class DocStatusObservacoes
    {
        public int ID { get; set; }
        public int DocID { get; set; }
        public string Observacoes { get; set; }

        public DocStatusObservacoes()
        {

        }

        public DocStatusObservacoes(int docID, string observacoes)
        {
            DocID = docID;
            Observacoes = observacoes;
        }
    }
}
