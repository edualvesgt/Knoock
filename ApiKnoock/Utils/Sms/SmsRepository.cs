namespace ApiKnoock.Utils.Sms
{
    public class SmsRepository : ISmsRepository
    {
        private readonly SmsService _smsService;

        public SmsRepository(SmsService smsService)
        {
            _smsService = smsService;
        }

        /// <summary>
        /// Envia uma mensagem SMS .
        /// </summary>
        public async Task<string> SendSmsAsync(string toPhoneNumber, string message)
        {
            try
            {
                await _smsService.SendSmsAsync(toPhoneNumber, message);
                return "SMS enviado com sucesso.";
            }
            catch (Exception error)
            {
                throw new ApplicationException($"Erro ao enviar SMS: {error.Message}");
            }
        }

        /// <summary>
        /// Envia um código de recuperação.
        /// </summary>
        public async Task SendRecoveryCodeAsync(string toPhoneNumber, int recoveryCode)
        {
            string message = $"Seu código de recuperação é: {recoveryCode}.";
            await SendSmsAsync(toPhoneNumber, message);
        }
    }
    }
