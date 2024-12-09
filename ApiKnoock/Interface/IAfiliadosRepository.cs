using ApiKnoock.Domains;
using ApiKnoock.ViewModel;

namespace ApiKnoock.Interface
{
    public interface IAfiliadosRepository
    {
        void Create(AfiliadosViewModel afiliadoViewModel);

        List<Afiliado> GetAllAffiliate();

        Task<Afiliado?> GetAfiliadoByIdAsync(Guid afiliadoId);
        Task<List<Afiliado>> GetAfiliadosOnlineAsync();
        
        void AtualizarStatusAfiliado(Guid afiliadoId, bool transito);

        Task AdicionarAfiliadoNaFilaAsync(Guid afiliadoId);
        Task<Afiliado?> ObterProximoAfiliadoDaFilaAsync();
    }
}
