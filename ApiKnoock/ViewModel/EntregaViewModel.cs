using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiKnoock.ViewModel
{
    public class EntregaViewModel
    {
        public Guid TipoUsuarioId { get; set; }
        
        public DateTime DataRegistro { get; set; }
        public string? Status { get; set; }
        public string? FotoProduto { get; set; }
        public bool NotificacaoMorador { get; set; }
        public string? PinRetirada { get; set; }
        public DateTime? DataRetirada { get; set; }
        public bool FgEntrega { get; set; }
        public DateTime DataNotificacao { get; set; }
        public string? Observacao { get; set; }
        public string? Origem { get; set; }

        [NotMapped]
        [JsonIgnore]
        public IFormFile? Arquivo { get; set; }
    }
}
