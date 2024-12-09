using ApiKnoock.Domains;
using ApiKnoock.ViewModel;

namespace ApiKnoock.Interface
{
    public interface IEntregaRepository
    {
        void Create(Entrega entrega);

        List<Entrega> GetAllDelivery();

        Task CreateHeavyDeliveries(EntregaViewModel entregaViewModel, VeiculoViewModel veiculoViewModel);
        Task GenerateDeliveryPinAsync(Guid entregaId);

        Task<bool> ValidatePinAsync(Guid entregaId, string pin);

        Task<List<Entrega>> GetEntregasPendentesAsync();

        Task AtualizarStatusEntregaAsync(Guid entregaId, string status);
        Task RemoverAfiliadoDaFilaAsync(Guid afiliadoId);




    }
}
