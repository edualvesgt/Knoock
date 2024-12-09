using ApiKnoock.Interface;
using ApiKnoock.Repository;
using ApiKnoock.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiKnoock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AfiliadosController : ControllerBase
    {
        private readonly IAfiliadosRepository _afiliadosRepository;

        public AfiliadosController(IAfiliadosRepository afiliadosRepository)
        {
            _afiliadosRepository = afiliadosRepository;
        }


        /// <summary>
        /// Obtém uma lista de todos os afiliados.
        /// </summary>
        /// <returns>Retorna uma lista de afiliados cadastrados ou uma mensagem de erro caso ocorra uma exceção.</returns>
        [HttpGet]
        //[Authorize(Roles ="Afiliado")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_afiliadosRepository.GetAllAffiliate());
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }

        /// <summary>
        /// Cria um novo afiliado com base nas informações fornecidas.
        /// </summary>
        /// <param name="afiliadosViewModel">Objeto que contém os dados do afiliado.</param>
        /// <returns>Retorna o status de criação do afiliado ou uma mensagem de erro em caso de falha.</returns>

        [HttpPost]
        public IActionResult Post(AfiliadosViewModel afiliadosViewModel)
        {
            try
            {
                _afiliadosRepository.Create(afiliadosViewModel);
                return StatusCode(201, "Afiliado criado com sucesso.");
            }
            catch (Exception error)
            {
                return BadRequest(error.Message);
            }
        }
    }
}
