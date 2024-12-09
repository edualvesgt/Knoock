using ApiKnoock.Domains;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace ApiKnoock.Interface
{
    public interface IUsuarioRepository
    {
        void Create(Usuario usuario);

        Usuario SearchById(Guid id);

        Usuario Login(string email, string senha);

        bool ChangePassword(string email, string senha);
        Task GenerateRecoveryCodeAsync(string phoneNumber); 
        Task<bool> ValidateRecoveryCodeAsync(string phoneNumber, int code); 

        Task<Usuario?> SearchByPhoneNumberAsync(string phoneNumber);

    }
}
