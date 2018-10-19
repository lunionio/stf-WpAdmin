namespace Admin.Models
{
    public class Servico: Base
    {
        public int ServicoTipoId { get; set; }
        public virtual ServicoTipo ServicoTipo { get; set; }
    }
}
