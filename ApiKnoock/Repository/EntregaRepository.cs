using ApiKnoock.Context;
using ApiKnoock.Domains;
using ApiKnoock.Interface;
using ApiKnoock.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace ApiKnoock.Repository
{
    public class EntregaRepository : IEntregaRepository
    {
        KnoockContext _context = new KnoockContext();

        private readonly IVeiculoRepository _veiculoRepository;

        // Injeta o repositório de Veículo no construtor
        public EntregaRepository(IVeiculoRepository veiculoRepository)
        {
            _veiculoRepository = veiculoRepository;
        }

        public void Create(Entrega entrega)
        {
            try
            {
                _context.Add(entrega);
                _context.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }


        public async Task CreateHeavyDeliveries(EntregaViewModel entregaViewModel, VeiculoViewModel veiculoViewModel)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {

                try
                {
                    //Cria um Objeto para mapear os dados que chegaram da controller
                    var entrega = new Entrega
                    {
                        TipoUsuarioId = entregaViewModel.TipoUsuarioId,
                        DataRegistro = entregaViewModel.DataRegistro,
                        Status = entregaViewModel.Status!,
                        FotoProduto = entregaViewModel.FotoProduto,
                        NotificacaoMorador = entregaViewModel.NotificacaoMorador,
                        PinRetirada = entregaViewModel.PinRetirada,
                        DataRetirada = entregaViewModel.DataRetirada,
                        DataNotificacao = entregaViewModel.DataNotificacao,
                        Observacao = entregaViewModel.Observacao,
                        Origem = entregaViewModel.Origem,

                    };

                    // Adiciona a entrega ao contexto e salva
                    _context.Entregas.Add(entrega);
                    await _context.SaveChangesAsync();

                    // Validacao para garantir que o veiculo chegou
                    if (veiculoViewModel != null)
                    {
                        veiculoViewModel.EntregaId = entrega.Id; // Associa o ID da entrega ao veículo

                        // Chama o repositório de Veículo para salvar o veículo
                        _veiculoRepository.Create(veiculoViewModel);
                    }

                    // Commit da transação
                    transaction.Commit();
                }
                catch (Exception)
                {
                    // Rollback em caso de erro
                    transaction.Rollback();
                    throw;
                }
            }
        }



        public List<Entrega> GetAllDelivery()
        {
            try
            {
                return _context.Entregas

                    .Include(x => x.TipoUsuario.IdTipoNavigation)
                    .Include(x => x.TipoUsuario)
                    .ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task GenerateDeliveryPinAsync(Guid entregaId)
        {

            var entrega = await _context.Entregas
                .Include(e => e.TipoUsuario)
                .ThenInclude(tu => tu.Condominos) // Inclui os Condôminos
                .FirstOrDefaultAsync(e => e.Id == entregaId);

            if (entrega == null)
                throw new ApplicationException("Entrega não encontrada.");

            // Selecionar o Condomino relacionado ao TipoUsuario
            var morador = entrega.TipoUsuario?.Condominos.FirstOrDefault(); // Use um critério se necessário

            if (morador == null)
                throw new ApplicationException("Condomino associado ao TipoUsuario não encontrado.");


            var random = new Random();
            string pin = random.Next(1000, 9999).ToString();

            // Atualizar o PIN no registro do Condomino
            morador.Pin = pin;

            // Salvar as alterações no banco de dados
            await _context.SaveChangesAsync();
        }


        public async Task<bool> ValidatePinAsync(Guid entregaId, string pin)
        {
            // Carregar a entrega e incluir os relacionamentos necessários
            var entrega = await _context.Entregas
                .Include(e => e.TipoUsuario)
                .ThenInclude(tu => tu!.Condominos) // Inclui os Condôminos
                .FirstOrDefaultAsync(e => e.Id == entregaId);

            if (entrega == null)
                throw new ApplicationException("Entrega não encontrada.");

            // Selecionar o Condomino relacionado ao TipoUsuario
            var morador = entrega.TipoUsuario?.Condominos.FirstOrDefault();

            if (morador == null)
                throw new ApplicationException("Condomino associado ao TipoUsuario não encontrado.");

            // Comparar o PIN fornecido com o PIN armazenado
            if (morador.Pin == pin)
            {
                // Atualizar o status da entrega para "Concluído"
                entrega.Status = "Concluído"; // Certifique-se de que o campo Status existe e está configurado corretamente
                await _context.SaveChangesAsync(); // Persistir a alteração no banco de dados

                return true;
            }

            return false;
        }

        public async Task<List<Entrega>> GetEntregasPendentesAsync()
        {
            return await _context.Entregas
                .Where(e => e.Status == "Pendente") 
                .ToListAsync();
        }

        public async Task AtualizarStatusEntregaAsync(Guid entregaId, string status)
        {
            var entrega = await _context.Entregas.FindAsync(entregaId);
            if (entrega != null)
            {
                entrega.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public Task RemoverAfiliadoDaFilaAsync(Guid afiliadoId)
        {
            throw new NotImplementedException();
        }

        //public async Task RemoverAfiliadoDaFilaAsync(Guid afiliadoId)
        //{
        //    var fila = await _context.Filas.FirstOrDefaultAsync(f => f.AfiliadoId == afiliadoId);
        //    if (fila != null)
        //    {
        //        _context.Filas.Remove(fila);
        //        await _context.SaveChangesAsync();
        //    }
        //}



    }
}
