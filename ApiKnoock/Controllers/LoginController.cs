using ApiKnoock.Domains;
using ApiKnoock.Interface;
using ApiKnoock.Repository;
using ApiKnoock.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiKnoock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public LoginController()
        {
            _usuarioRepository = new UsuarioRepository();
        }


        /// <summary>
        /// Realiza o login de um usuário com base no email e senha fornecidos.
        /// </summary>
        /// <param name="loginViewModel">Objeto contendo os dados de login do usuário.</param>
        /// <returns>Retorna os dados do usuário autenticado ou uma mensagem de erro em caso de falha.</returns>
        [HttpPost]
        public IActionResult Login(LoginViewModel loginViewModel)
        {
            try
            {
                Usuario searchUser = _usuarioRepository.Login(loginViewModel.Email!, loginViewModel.Senha!);
                if (searchUser == null)
                {
                    return StatusCode(401, "Email ou Senha Inválidos");
                }

                // Define a role com base no domínio do e-mail
                string role = DetermineUserRole(searchUser.Email!);

                //informações que serão fornecidas no token
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, searchUser.Email!),
                    new Claim(JwtRegisteredClaimNames.Name,searchUser.Nome!),

                    new Claim(JwtRegisteredClaimNames.Jti, searchUser.Id.ToString()),
                    new Claim("role", role),
                };

                //chave de segurança
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("knoock-webapi-chave-symmetricsecuritykey"));

                //credenciais
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                //token
                var meuToken = new JwtSecurityToken(
                        issuer: "knoock-WebAPI",
                        audience: "knoock-WebAPI",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: creds
                    );
              
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(meuToken),
                    

                    usuario = new
                    {
                        searchUser.Id,
                        searchUser.Nome,
                        searchUser.Email,
                        searchUser.Telefone,
                        searchUser.DataNascimento,
                        searchUser.FotoUsuario,
                        
                        TipoUsuarios = searchUser.TipoUsuarios.Select(t => new
                        {
                            t.Id,
                            t.IdTipo,                           
                            
                            Tipo = new
                            {
                                t.IdTipoNavigation?.Id,
                                t.IdTipoNavigation?.Tipo1
                            }
                        })
                    }
                });



            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }

        // Método auxiliar para determinar a role com base no e-mail
        private string DetermineUserRole(string email)
        {
            if (email.EndsWith("@porteiro.com"))
            {
                return "Porteiro";
            }
            else if (email.EndsWith("@adm.com"))
            {
                return "Administrador";
            }
            else if (email.EndsWith("@afiliados.com"))
            {
                return "Afiliado";
            }
            else
            {
                return "Usuário"; // Role padrão
            }
        }
    }
}



