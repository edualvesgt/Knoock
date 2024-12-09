using ApiKnoock.Interface;
using ApiKnoock.Utils.Sms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiKnoock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsController : ControllerBase
    {
         private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISmsRepository _smsRepository;

        public SmsController(IUsuarioRepository usuarioRepository, ISmsRepository smsRepository)
        {
            _usuarioRepository = usuarioRepository;
            _smsRepository = smsRepository;
        }

        /// <summary>
        /// Envia um código de recuperação por SMS
        /// </summary>
        /// <param name="phoneNumber">Número de telefone do usuário</param>
        [HttpPost("send-code")]
        public async Task<IActionResult> SendRecoveryCode(string phoneNumber)
        {
            try
            {
                // Gera o código de recuperação e o salva no banco
                await _usuarioRepository.GenerateRecoveryCodeAsync(phoneNumber);

                // Busca o usuário para garantir que o código foi gerado corretamente
                var user = await _usuarioRepository.SearchByPhoneNumberAsync(phoneNumber);
                if (user == null || !user.CodigoRecuperacao.HasValue)
                    return NotFound("Usuário não encontrado ou código não gerado.");

                // Envia o código via SMS
                await _smsRepository.SendRecoveryCodeAsync(phoneNumber, user.CodigoRecuperacao.Value);

                return Ok("Código de recuperação enviado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao enviar o código de recuperação: {ex.Message}");
            }
        }

        /// <summary>
        /// Valida o código de recuperação recebido via SMS
        /// </summary>
        /// <param name="phoneNumber">Número de telefone do usuário</param>
        /// <param name="code">Código de recuperação</param>
        [HttpPost("validate-code")]
        public async Task<IActionResult> ValidateRecoveryCode(string phoneNumber, int code)
        {
            try
            {
                bool isValid = await _usuarioRepository.ValidateRecoveryCodeAsync(phoneNumber, code);

                if (isValid)
                    return Ok("Código de recuperação validado com sucesso.");
                else
                    return BadRequest("Código de recuperação inválido.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao validar o código de recuperação: {ex.Message}");
            }
        }
    }
}
