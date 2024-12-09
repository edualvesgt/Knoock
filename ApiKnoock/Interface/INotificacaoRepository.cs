using ApiKnoock.Domains;
using ApiKnoock.ViewModel;

namespace ApiKnoock.Interface
{
    public interface INotificacaoRepository
    {
        void Create(NotificacaoViewModel notificacaoViewModel);

        List<Notificacao> GetAllNotification();
    }
}
