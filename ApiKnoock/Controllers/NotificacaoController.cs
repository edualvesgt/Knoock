using ApiKnoock.Domains;
using ApiKnoock.Interface;
using ApiKnoock.Repository;
using ApiKnoock.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiKnoock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacaoController : ControllerBase
    {

        private readonly INotificacaoRepository _notificacaoRepository;

        public NotificacaoController()
        {
            _notificacaoRepository = new NotificaoRepository();
        }

        /// <summary>
        /// Retorna todas as notificações registradas.
        /// </summary>
        /// <returns>Retorna uma lista de notificações ou uma mensagem de erro em caso de falha.</returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_notificacaoRepository.GetAllNotification());
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }

        /// <summary>
        /// Cria uma nova notificação com base nos dados fornecidos.
        /// </summary>
        /// <param name="notificacaoViewModel">Objeto contendo as informações da notificação.</param>
        /// <returns>Retorna o status da criação ou uma mensagem de erro em caso de falha.</returns>
        [HttpPost]
        public IActionResult Post(NotificacaoViewModel notificacaoViewModel)
        {
            try
            {
                _notificacaoRepository.Create(notificacaoViewModel);
                return StatusCode(201, "Notificação criada com sucesso.");
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }

    }
}
