using ApiKnoock.Context;
using ApiKnoock.Domains;
using ApiKnoock.Interface;
using ApiKnoock.Utils;
using Microsoft.EntityFrameworkCore;

namespace ApiKnoock.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {

        KnoockContext _context = new KnoockContext();
        public bool ChangePassword(string email, string senha)
        {
            try
            {
                var user = _context.Usuarios.FirstOrDefault(x => x.Email == email);

                if (user == null)
                {
                    return false;
                }

                user.Senha = Criptografia.Hash(senha);

                _context.Update(user);
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Create(Usuario usuario)
        {
            try
            {
                // Hash da senha
                usuario.Senha = Criptografia.Hash(usuario.Senha!);

                // Adiciona o usuário à tabela principal
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                // Verifica o domínio do e-mail
                var emailDomain = usuario.Email!.Split('@')[1].ToLower(); // Obtém o domínio do e-mail

                // Define o tipo de usuário baseado no domínio do e-mail
                string tipoUsuarioNome;
                if (emailDomain.Contains("porteiro"))
                {
                    tipoUsuarioNome = "Porteiro";
                }
                else
                {
                    tipoUsuarioNome = "Administrador";
                }


                // Busca o tipo correspondente na tabela 'Tipos'
                var tipo = _context.Tipos.FirstOrDefault(t => t.Tipo1 == tipoUsuarioNome);
                if (tipo == null) throw new Exception($"Tipo '{tipoUsuarioNome}' não encontrado.");

                // Associa o usuário ao tipo correspondente na tabela 'TipoUsuario'
                var tipoUsuario = new TipoUsuario
                {
                    IdUsuario = usuario.Id,
                    IdTipo = tipo.Id
                };

                // Salva a associação
                _context.TipoUsuarios.Add(tipoUsuario);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao criar o usuário.", ex);
            }
        }

        public Usuario Login(string email, string senha)
        {
            try
            {
                // Busca o usuário com as informações necessárias
                var user = _context.Usuarios
                .Include(u => u.TipoUsuarios) // Inclui a lista de TipoUsuarios
                .ThenInclude(tu => tu.IdTipoNavigation)
                .FirstOrDefault(u => u.Email == email);


                if (user == null)
                {
                    return null!;
                }

                if (!Criptografia.CompareHash(senha, user.Senha))
                {
                    return null!;
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao realizar login", ex);
            }
        }


        public Usuario SearchById(Guid id)
        {
            try
            {
                return _context.Usuarios
                    .Where(u => u.Id == id)
                    .Select(u => new Usuario
                    {
                        Id = u.Id,
                        Nome = u.Nome,
                        DataNascimento = u.DataNascimento,
                        Telefone = u.Telefone,
                        Email = u.Email,
                        FotoUsuario = u.FotoUsuario,
                        TipoUsuarios = u.TipoUsuarios.Select(tu => new TipoUsuario
                        {
                            IdTipo = tu.IdTipo, // Aqui você mantém apenas o ID
                            IdTipoNavigation = new Tipo
                            {
                                Tipo1 = tu.IdTipoNavigation.Tipo1 // Aqui você acessa o Tipo1 do objeto relacionado
                            }
                        }).ToList()
                    })
                    .FirstOrDefault()!;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task GenerateRecoveryCodeAsync(string phoneNumber)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Telefone == phoneNumber);

            if (user == null)
                throw new ApplicationException("Usuário não encontrado.");

            Random random = new Random();
            int recoveryCode = random.Next(1000, 9999);

            user.CodigoRecuperacao = recoveryCode;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ValidateRecoveryCodeAsync(string phoneNumber, int code)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Telefone == phoneNumber);

            if (user == null)
                throw new ApplicationException("Usuário não encontrado.");

            if (user.CodigoRecuperacao == code)
            {
                user.CodigoRecuperacao = null; // Limpa o código após validação
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task<Usuario?> SearchByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Telefone == phoneNumber);
        }
    }
}
