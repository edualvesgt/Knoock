using ApiKnoock.Context;
using ApiKnoock.Domains;
using ApiKnoock.Interface;
using ApiKnoock.Repository;
using ApiKnoock.Utils.Blob;
using ApiKnoock.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiKnoock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntregaController : ControllerBase
    {
        private readonly IEntregaRepository _entregaRepository;

        private readonly KnoockContext _context;

        public EntregaController(IEntregaRepository entregaRepository, KnoockContext context)
        {
            _entregaRepository = entregaRepository;
            _context = context;
        }

        /// <summary>
        /// Obtém uma lista de todas as entregas registradas.
        /// </summary>
        /// <returns>Retorna uma lista de entregas ou uma mensagem de erro em caso de exceção.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_entregaRepository.GetAllDelivery());
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }

        /// <summary>
        /// Cria uma nova entrega com os dados fornecidos.
        /// </summary>
        /// <param name="entregaViewModel">Objeto contendo os dados da entrega.</param>
        /// <returns>Retorna o status da criação da entrega ou uma mensagem de erro em caso de falha.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] EntregaViewModel entregaViewModel)
        {
            try
            {
                // Validação do tipo de usuário
                var tipoUsuario = _context.TipoUsuarios
                    .Include(t => t.IdTipoNavigation)
                    .Where(t => t.Id == entregaViewModel.TipoUsuarioId)
                    .Select(t => new
                    {
                        t.Id,
                        t.IdUsuario,
                        t.IdTipo,
                        TipoNome = t.IdTipoNavigation!.Tipo1
                    })
                    .FirstOrDefault();

                if (tipoUsuario == null)
                {
                    return BadRequest("Tipo de usuário inválido.");
                }

                // Verifica permissões baseadas no tipo de usuário
                if (tipoUsuario.TipoNome == "Porteiro")
                {
                    // Porteiros devem obrigatoriamente enviar uma foto
                    if (entregaViewModel.Arquivo == null)
                    {
                        return BadRequest("Usuários do tipo 'Porteiro' devem enviar uma foto do produto.");
                    }
                }
                else if (tipoUsuario.TipoNome == "Condomino")
                {
                    // Condominos não podem enviar fotos
                    if (entregaViewModel.Arquivo != null)
                    {
                        return BadRequest("Usuários do tipo 'Condomino' não podem enviar fotos do produto.");
                    }
                }
                else
                {
                    return BadRequest("Apenas usuários dos tipos 'Porteiro' e 'Condomino' podem cadastrar entregas.");
                }

                // Configurações do Azure Blob Storage
                var connectionString = "DefaultEndpointsProtocol=https;AccountName=knoock;AccountKey=hsiILkQLXswnSJNg4ZPz1ziySFejny2gijodiuiCgNdCTYOtz6YhRZPD4CVzxQU4NHCKg7CReQ8s+AStJr5BzA==;EndpointSuffix=core.windows.net";
                var containerName = "knoockcontainer";

                string? imageUrl = null;

                // Upload da foto (se existir)
                if (entregaViewModel.Arquivo != null)
                {
                    imageUrl = await AzureBlobStorage.UploadImageBlobAsync(entregaViewModel.Arquivo, connectionString, containerName);
                }

                // Criação do objeto Entrega
                var entrega = new Entrega
                {
                    TipoUsuarioId = entregaViewModel.TipoUsuarioId,
                    DataRegistro = entregaViewModel.DataRegistro,
                    Status = entregaViewModel.Status!,
                    FotoProduto = imageUrl, // Foto será null para Condominos
                    NotificacaoMorador = entregaViewModel.NotificacaoMorador,
                    PinRetirada = entregaViewModel.PinRetirada,
                    DataRetirada = entregaViewModel.DataRetirada,
                    DataNotificacao = entregaViewModel.DataNotificacao,
                    Observacao = entregaViewModel.Observacao,
                    Origem = entregaViewModel.Origem,
                };

                // Chamada ao repositório para salvar a entrega
                _entregaRepository.Create(entrega);

                return StatusCode(201, "Entrega criada com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }





        /// <summary>
        /// Cria uma nova entrega de grande porte com detalhes adicionais do veículo.
        /// </summary>
        /// <param name="heavyDeliveryViewModel">Objeto contendo os dados da entrega de grande porte.</param>
        /// <returns>Retorna o status da criação da entrega ou uma mensagem de erro em caso de falha.</returns>
        [HttpPost("create-heavy-delivery")]
        public async Task<IActionResult> CreateHeavyDelivery(HeavyDeliveryViewModel heavyDeliveryViewModel)
        {
            if (heavyDeliveryViewModel == null)
                return BadRequest("Dados inválidos.");

            try
            {
                //Mapeamento dos dados que chega do json
                var entregaViewModel = new EntregaViewModel
                {
                    TipoUsuarioId = heavyDeliveryViewModel.TipoUsuarioId,
                    DataRegistro = heavyDeliveryViewModel.DataRegistro,
                    Status = heavyDeliveryViewModel.Status,
                    FotoProduto = heavyDeliveryViewModel.FotoProduto,
                    NotificacaoMorador = heavyDeliveryViewModel.NotificacaoMorador,
                    PinRetirada = heavyDeliveryViewModel.PinRetirada,
                    DataRetirada = heavyDeliveryViewModel.DataRetirada,
                    DataNotificacao = heavyDeliveryViewModel.DataNotificacao
                };

                var veiculoViewModel = new VeiculoViewModel
                {
                    TipoUsuarioId = heavyDeliveryViewModel.TipoUsuarioId,
                    Marca = heavyDeliveryViewModel.Marca,
                    Modelo = heavyDeliveryViewModel.Modelo,
                    Placa = heavyDeliveryViewModel.Placa,
                    Ano = heavyDeliveryViewModel.Ano
                };

                // Envia o mapeamento para o metodo
                await _entregaRepository.CreateHeavyDeliveries(entregaViewModel, veiculoViewModel);


                return StatusCode(201, "Entrega de grande porte criada com sucesso.");
            }
            catch (Exception error)
            {

                return BadRequest(error.Message);
            }
        }


        /// <summary>
        /// Finaliza a entrega especificada pelo ID e gera um PIN para a mesma.
        /// </summary>
        /// <param name="entregaId">ID da entrega que será finalizada.</param>
        /// <returns>Retorna uma mensagem de sucesso se a entrega for finalizada e o PIN gerado com sucesso; caso contrário, retorna uma mensagem de erro.</returns>
        [HttpPost("finalizar-entrega/{entregaId}")]
        public async Task<IActionResult> FinalizarEntrega(Guid entregaId)
        {
            try
            {
                // Gera o PIN para a entrega
                await _entregaRepository.GenerateDeliveryPinAsync(entregaId);

                return Ok("Entrega finalizada e PIN gerado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Valida o PIN fornecido para uma entrega específica.
        /// </summary>
        /// <param name="entregaId">ID da entrega para a qual o PIN será validado.</param>
        /// <param name="pin">PIN a ser validado.</param>
        /// <returns>Retorna uma mensagem de sucesso se o PIN for válido; caso contrário, retorna uma mensagem de erro informando que o PIN é inválido.</returns>
        [HttpPost("validar-pin/{entregaId}")]
        public async Task<IActionResult> ValidarPin(Guid entregaId, string pin)
        {
            try
            {
                var isValid = await _entregaRepository.ValidatePinAsync(entregaId, pin);

                if (isValid)
                    return Ok("PIN válido. Entrega concluída.");

                return BadRequest("PIN inválido.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }

}

