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
        public byte[] Arquivo { get; set; }
    }
}