namespace ApiKnoock.ViewModel
{
    public class NotificacaoViewModel
    {
        public Guid TipoUsuarioId { get; set; }
        public string? Mensagem { get; set; }
        public DateTime DataNotificacao { get; set; }
        public string? Status { get; set; }
        public string? Tipo { get; set; }
        public string? ImagemAviso { get; set; }
        public Guid? EntregaId { get; set; }

    }
}
