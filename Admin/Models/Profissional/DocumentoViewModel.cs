namespace Admin.Models
{
    public class DocumentoViewModel
    {
        public int Id { get; set; }
        public string TipoNome { get; set; }
        public int TipoId { get; set; }
        public int IdStatus { get; set; }
        public string Status { get; set; }
        public string Data  { get; set; }
        public string Arquivo { get; set; }
        public string Observacoes { get; set; }

        public DocumentoViewModel()
        {

        }

        public DocumentoViewModel(int id, string tipoNome, int tipoId, int idStatus, string status, string data, string arquivo)
        {
            Id = id;
            TipoNome = tipoNome;
            TipoId = tipoId;
            IdStatus = idStatus;
            Status = status;
            Data = data;
            Arquivo = arquivo;
        }
    }
}