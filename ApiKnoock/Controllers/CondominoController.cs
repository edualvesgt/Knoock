using ApiKnoock.Interface;
using ApiKnoock.Repository;
using ApiKnoock.Utils.Blob;
using ApiKnoock.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ApiKnoock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CondominoController : ControllerBase
    {
        private readonly ICondominoRepository _condominoRepository;

        public CondominoController()
        {
            _condominoRepository = new CondominoRepository();
        }


        /// <summary>
        /// Obtém uma lista de todos os moradores cadastrados.
        /// </summary>
        /// <returns>Retorna uma lista de moradores ou uma mensagem de erro em caso de exceção.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_condominoRepository.GetAllResident());
            }
            catch (Exception error)
            {

                return BadRequest(error.Message);
            }
        }

        /// <summary>
        /// Obtém um morador específico pelo ID.
        /// </summary>
        /// <param name="id">ID do morador a ser pesquisado.</param>
        /// <returns>Retorna os detalhes do morador ou NotFound caso o morador não seja encontrado.</returns>
        [HttpGet("Id")]
        public IActionResult GetId(Guid id)
        {
            try
            {
                return Ok(_condominoRepository.SearchById(id));
            }
            catch (Exception)
            {
                return NotFound();
            }
        }



        [HttpPost]
        public async Task<IActionResult> Post([FromForm] CondominoViewModel condominoViewModel)
        {
            try
            {
                string? imageUrl = null;

                // Tenta realizar o upload da imagem
                if (condominoViewModel.Arquivo != null)
                {
                    try
                    {
                        // Configurações do Azure Blob Storage
                        var connectionString = "DefaultEndpointsProtocol=https;AccountName=knoock;AccountKey=hsiILkQLXswnSJNg4ZPz1ziySFejny2gijodiuiCgNdCTYOtz6YhRZPD4CVzxQU4NHCKg7CReQ8s+AStJr5BzA==;EndpointSuffix=core.windows.net";
                        var containerName = "knoockcontainer";
                        imageUrl = await AzureBlobStorage.UploadImageBlobAsync(condominoViewModel.Arquivo, connectionString, containerName);
                    }
                    catch (Exception blobEx)
                    {
                        return BadRequest($"Erro ao fazer upload da imagem: {blobEx.Message}");
                    }
                }

                // Chama o método do repositório com a URL da imagem
                await _condominoRepository.Create(condominoViewModel, imageUrl);

                return StatusCode(201, "Condomino criado com sucesso.");
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }

    }
}
