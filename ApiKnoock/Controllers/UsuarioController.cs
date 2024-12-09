using ApiKnoock.Domains;
using ApiKnoock.Interface;
using ApiKnoock.Repository;
using ApiKnoock.Utils.Blob;
using ApiKnoock.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiKnoock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioController()
        {
            _usuarioRepository = new UsuarioRepository();
        }


        /// <summary>
        /// Cria um novo usuário com base nos dados fornecidos.
        /// </summary>
        /// <param name="usuarioViewModel">Objeto contendo as informações do usuário.</param>
        /// <returns>Retorna o status de criação do usuário ou uma mensagem de erro caso o email já esteja em uso.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] UsuarioViewModel usuarioViewModel)
        {
            try
            {
                // Configurações do Azure Blob Storage
                var connectionString = "DefaultEndpointsProtocol=https;AccountName=knoock;AccountKey=hsiILkQLXswnSJNg4ZPz1ziySFejny2gijodiuiCgNdCTYOtz6YhRZPD4CVzxQU4NHCKg7CReQ8s+AStJr5BzA==;EndpointSuffix=core.windows.net";
                var containerName = "knoockcontainer";

                string? imageUrl = null;

                // Upload da foto (se existir)
                if (usuarioViewModel.Arquivo != null)
                {
                    imageUrl = await AzureBlobStorage.UploadImageBlobAsync(usuarioViewModel.Arquivo, connectionString, containerName);
                }

                Usuario usuario = new Usuario
                {
                    Nome = usuarioViewModel.Nome!,
                    Telefone = usuarioViewModel.Telefone!,
                    Email = usuarioViewModel.Email!,
                    Senha = usuarioViewModel.Senha!,
                    DataNascimento = usuarioViewModel.DataNacimento!,
                    CodigoRecuperacao = usuarioViewModel.CodigoRecuperacao!,
                    FotoUsuario = imageUrl
                };

                _usuarioRepository.Create(usuario);

                return StatusCode(201, new { Message = "Usuário criado com sucesso.", Usuario = usuario });
            }
            catch (Exception error)
            {
                if (error.Message.Contains("Email já cadastrado."))
                {
                    return BadRequest("O email informado já está em uso.");
                }

                return BadRequest(error.Message);
            }
        }


        /// <summary>
        /// Obtém um usuário específico pelo ID.
        /// </summary>
        /// <param name="id">ID do usuário a ser pesquisado.</param>
        /// <returns>Retorna os detalhes do usuário ou NotFound caso não seja encontrado.</returns>
        [HttpGet("GetById")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                Usuario Busca = _usuarioRepository.SearchById(id);

                return Ok(Busca);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Atualiza a senha de um usuário com base no email fornecido.
        /// </summary>
        /// <param name="email">Email do usuário.</param>
        /// <param name="changePasswordViewModel">Objeto contendo a nova senha do usuário.</param>
        /// <returns>Retorna sucesso se a senha for alterada com sucesso ou uma mensagem de erro em caso de falha.</returns>
        [HttpPut("UpdatePassword")]
        public IActionResult UpdatePassword(string email, ChangePasswordViewModel changePasswordViewModel)
        {

            try
            {
                _usuarioRepository.ChangePassword(email, changePasswordViewModel.NovaSenha!);

                return Ok("Senha Alterada com sucesso ");
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }
    }
}
