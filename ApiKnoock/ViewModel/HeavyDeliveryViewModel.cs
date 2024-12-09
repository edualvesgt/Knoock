namespace ApiKnoock.ViewModel
{
    public class HeavyDeliveryViewModel
    {
        public Guid TipoUsuarioId { get; set; }
        public Guid EntregaId { get; set; }
        public string? FotoProduto { get; set; }
        public bool NotificacaoMorador { get; set; }
        public string? Status {  get; set; }
        public string? PinRetirada { get; set; }
        public DateTime DataRetirada { get; set; }
        public DateTime DataRegistro { get; set; }
        public DateTime DataNotificacao { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Placa { get; set; }
        public int Ano { get; set; }
    }
}
