using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ApiKnoock.ViewModel
{
    public class UsuarioViewModel
    {
        public string? Nome { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public DateOnly? DataNacimento { get; set; }
        public int? CodigoRecuperacao { get; set; }
        [NotMapped]
        [JsonIgnore]
        public IFormFile Arquivo { get; set; }


    }
}
