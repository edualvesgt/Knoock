using ApiKnoock.Context;
using ApiKnoock.Domains;
using ApiKnoock.Interface;
using ApiKnoock.Utils;
using ApiKnoock.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace ApiKnoock.Repository
{
    public class AfiliadosRepository : IAfiliadosRepository
    {
        
        private static readonly Queue<Afiliado> _queue = new Queue<Afiliado>();
        private readonly KnoockContext _context;

        public AfiliadosRepository( KnoockContext context)
        {
            
            _context = context;
        }
        public void Create(AfiliadosViewModel afiliadosViewModel)
        {
            try
            {

                // Validação de email com domínio @afiliados
                if (!afiliadosViewModel.Email.EndsWith("@afiliados.com", StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("O email deve pertencer ao domínio '@afiliados'.");
                }


                // Cria o Usuario 
                var usuario = new Usuario
                {
                    Nome = afiliadosViewModel.Nome!,
                    Telefone = afiliadosViewModel.Telefone!,
                    Email = afiliadosViewModel.Email!,
                    Senha = Criptografia.Hash(afiliadosViewModel.Senha!),
                    FotoUsuario = afiliadosViewModel.FotoUrl,
                    DataNascimento = afiliadosViewModel.DataNascimento
                };

                // Salva as informações
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                // Busca o tipo 'Afiliado' na tabela Tipo
                var tipoAfiliado = _context.Tipos.FirstOrDefault(t => t.Tipo1 == "Afiliado");
                if (tipoAfiliado == null) throw new Exception("Tipo 'Afiliado' não encontrado.");

                // Associa o usuário ao tipo 'Afiliado' em Tipo_Usuario
                var tipoUsuario = new TipoUsuario
                {
                    IdUsuario = usuario.Id,
                    IdTipo = tipoAfiliado.Id
                };

                // Salva as informações
                _context.TipoUsuarios.Add(tipoUsuario);
                _context.SaveChanges();

                // Cria o afiliado
                var afiliado = new Afiliado
                {
                    TipoUsuarioId = tipoUsuario.Id,
                    FgOnline = afiliadosViewModel.FgOnline,
                    FgTransito = afiliadosViewModel.FgTransito,
                    KnookCoins = afiliadosViewModel.KnoockCoins
                };

                _context.Afiliados.Add(afiliado);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Afiliado> GetAllAffiliate()
        {
            try
            {
                return _context.Afiliados
                    .Include(x => x.TipoUsuario!.IdUsuarioNavigation)
                    .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<Afiliado>> GetAfiliadosOnlineAsync()
        {
            return await _context.Afiliados.Where(a => a.FgOnline).ToListAsync();
        }

       

        public async Task<Afiliado?> GetAfiliadoByIdAsync(Guid afiliadoId)
        {
            return await _context.Afiliados.FirstOrDefaultAsync(a => a.Id == afiliadoId);
        }

        public void AtualizarStatusAfiliado(Guid afiliadoId, bool transito)
        {
            try
            {
                var afiliado = _context.Afiliados.Find(afiliadoId);
                if (afiliado == null)
                {
                    throw new Exception($"Afiliado com ID {afiliadoId} não encontrado.");
                }

                afiliado.FgTransito = transito;

                _context.Entry(afiliado).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Erro ao atualizar o status do afiliado: {ex.Message}");
                throw; 
            }
        }

        
        public async Task AdicionarAfiliadoNaFilaAsync(Guid afiliadoId)
        {
            // Verifica se o afiliado já está na fila (para evitar duplicatas)
            var afiliado = await GetAfiliadoByIdAsync(afiliadoId);
            if (afiliado == null)
            {
                throw new Exception("Afiliado não encontrado.");
            }

            // Adiciona na fila estática em memória (apenas para demonstração)
            _queue.Enqueue(afiliado);
        }

        // Método auxiliar para obter o próximo afiliado da fila
        public async Task<Afiliado?> ObterProximoAfiliadoDaFilaAsync()
        {
            // Retira o próximo afiliado da fila (se disponível)
            if (_queue.Count > 0)
            {
                return _queue.Dequeue();
            }
            return null;
        }


    }
}
