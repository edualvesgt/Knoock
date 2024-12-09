using ApiKnoock.Context;
using ApiKnoock.Domains;
using ApiKnoock.Interface;
using ApiKnoock.Utils;
using ApiKnoock.Utils.Blob;
using ApiKnoock.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace ApiKnoock.Repository
{
    public class CondominoRepository : ICondominoRepository
    {
        KnoockContext _context = new KnoockContext();



        public async Task Create(CondominoViewModel condominoViewModel, string? imageUrl)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // Criação do usuário
                var usuario = new Usuario
                {
                    Nome = condominoViewModel.Nome!,
                    Telefone = condominoViewModel.Telefone!,
                    Email = condominoViewModel.Email!,
                    Senha = Criptografia.Hash(condominoViewModel.Senha!),
                    DataNascimento = condominoViewModel.DataNacimento,
                    FotoUsuario = imageUrl
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Busca o tipo 'Condomino'
                var tipoAfiliado = _context.Tipos.FirstOrDefault(t => t.Tipo1 == "Condomino");
                if (tipoAfiliado == null)
                    throw new Exception("Tipo 'Condomino' não encontrado.");

                // Associa o usuário ao tipo
                var tipoUsuario = new TipoUsuario
                {
                    IdUsuario = usuario.Id,
                    IdTipo = tipoAfiliado.Id
                };

                _context.TipoUsuarios.Add(tipoUsuario);
                await _context.SaveChangesAsync();

                // Criação do Condomino
                var condomino = new Condomino
                {
                    TipoUsuarioId = tipoUsuario.Id,
                    DeliveryPin = condominoViewModel.DeliveryPin,
                    Pin = condominoViewModel.Pin,
                    Bloco = condominoViewModel.Bloco,
                    Apartamento = condominoViewModel.Apartamento
                };

                _context.Condominos.Add(condomino);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Erro ao criar o condomínio. Detalhes: " + ex.Message, ex);
            }
        }


        public List<Condomino> GetAllResident()
        {
            try
            {
                return _context.Condominos
                    .Include(x => x.TipoUsuario!.IdUsuarioNavigation)
                    .ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Condomino SearchById(Guid id)
        {
            try
            {
                return _context.Condominos.FirstOrDefault(x => x.Id == id)!;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
