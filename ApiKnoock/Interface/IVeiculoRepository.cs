using ApiKnoock.Domains;
using ApiKnoock.ViewModel;

namespace ApiKnoock.Interface
{
    public interface IVeiculoRepository
    {
        Task Create(VeiculoViewModel veiculoViewModel);

        void Delete(Guid id);

        List<Veiculo> GetAllVehicles();
    }
}
