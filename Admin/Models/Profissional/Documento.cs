using System;

namespace Admin.Models
{
    public class Documento: Base
    {
        public string Arquivo { get; set; }
        public string Numero { get; set; }
        public int Tipo { get; set; }
        public bool Requerido { get; set; }
        public int CodigoExterno { get; set; }
        public int DocumentoStatusID { get; set; }
        public DocumentoTipo DocumentoTipo { get; set; }
        public DocumentoStatus DocumentoStatus { get; set; }
        public DocStatusObservacoes StatusObservacoes { get; set; }

        private byte[] _conteudo;
        public byte[] Conteudo
        {
            get => _conteudo;
            set => _conteudo = Convert.FromBase64String(Arquivo);
        }

        public Documento()
        {

        }
    }
}
