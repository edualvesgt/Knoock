using ApiKnoock.Context;
using ApiKnoock.Domains;
using ApiKnoock.Utils.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiKnoock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecoveryPasswordController : ControllerBase
    {
        private readonly KnoockContext _context; // Contexto do banco de dados
        private readonly EmailSendingService _emailSendingService; // Serviço de envio de e-mails

        public RecoveryPasswordController(KnoockContext context, EmailSendingService emailSendingService)
        {
            _context = context; // Inicializa o contexto do banco de dados
            _emailSendingService = emailSendingService; // Inicializa o serviço de envio de e-mails
        }


        /// <summary>
        /// Envia um código de recuperação de senha para o email do usuário.
        /// </summary>
        /// <param name="email">Email do usuário que solicitou a recuperação de senha.</param>
        /// <returns>Retorna sucesso se o código for enviado ou uma mensagem de erro se o usuário não for encontrado.</returns>
        [HttpPost]
        public async Task<IActionResult> SendRecoveryCodePassword(string email)
        {
            try
            {
                // Busca o usuário pelo e-mail no banco de dados
                var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return NotFound("Usuario Nao Encontrado "); // Retorna um erro 404 se o usuário não for encontrado
                }

                // Gera um código de recuperação de senha aleatório
                Random random = new Random();
                int RecoveryCode = random.Next(1000, 9999);

                // Armazena o código de recuperação de senha no usuário
                user.CodigoRecuperacao = RecoveryCode;

                await _context.SaveChangesAsync(); // Salva as alterações no banco de dados

                // Envia o código de recuperação de senha por e-mail
                await _emailSendingService.SendRecoveryPassword(RecoveryCode, user.Email!);

                return Ok("Codigo Enviado Com Sucesso "); // Retorna uma resposta de sucesso
            }
            catch (Exception error)
            {
                return BadRequest($"Erro Ao enviar o Codigo: {error.Message}"); // Retorna um erro 400 em caso de falha
            }
        }


        /// <summary>
        /// Valida o código de recuperação de senha enviado ao usuário.
        /// </summary>
        /// <param name="email">Email do usuário.</param>
        /// <param name="code">Código de recuperação enviado ao email.</param>
        /// <returns>Retorna sucesso se o código for válido ou uma mensagem de erro em caso de falha.</returns>
        [HttpPost("RecoveryPassword")]
        public async Task<IActionResult> ValidatePasswordRecoveryCode(string email, int code)
        {
            try
            {
                // Busca o usuário pelo e-mail no banco de dados
                var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return NotFound("Usuario Nao Encontrado "); // Retorna um erro 404 se o usuário não for encontrado
                }

                if (user.CodigoRecuperacao != code)
                {
                    return BadRequest("Codigo Invalido"); // Retorna um erro 400 se o código for inválido
                }

                user.CodigoRecuperacao = null; // Limpa o código de recuperação de senha do usuário

                await _context.SaveChangesAsync(); // Salva as alterações no banco de dados

                return Ok("Codigo de Recuperacao Valido"); // Retorna uma resposta de sucesso
            }
            catch (Exception error)
            {
                return BadRequest(error.Message); // Retorna um erro 400 em caso de falha
            }
        }
    }
}

