using ApiKnoock.Context;
using ApiKnoock.Domains;
using ApiKnoock.Interface;
using ApiKnoock.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace ApiKnoock.Repository
{
    public class VeiculoRepository : IVeiculoRepository
    {
        KnoockContext _context = new KnoockContext();
        public async Task Create(VeiculoViewModel veiculoViewModel)
        {
            try
            {
                var veiculo = new Veiculo
                {
                    TipoUsuarioId = veiculoViewModel.TipoUsuarioId,
                    EntregaId = veiculoViewModel.EntregaId, 
                    Marca = veiculoViewModel.Marca,
                    Modelo = veiculoViewModel.Modelo,
                    Placa = veiculoViewModel.Placa,
                    Ano = veiculoViewModel.Ano
                };

                // Adiciona o veículo ao contexto e salva
                _context.Veiculos.Add(veiculo);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<Veiculo> GetAllVehicles()
        {
            try
            {
                return _context.Veiculos.ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

       
    }
}

