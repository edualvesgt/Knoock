using System.ComponentModel.DataAnnotations;

namespace ApiKnoock.ViewModel
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Informe a nova senha do usuário")]
        public string? NovaSenha { get; set; }
    }
}
