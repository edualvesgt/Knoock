using Twilio;

using Twilio.Rest.Api.V2010.Account;

namespace ApiKnoock.Utils.Sms
{
    public class SmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;

        public SmsService(IConfiguration configuration)
        {
            //Associacao do acesso a API de SMS
            _accountSid = configuration["Twilio:AccountSid"];
            _authToken = configuration["Twilio:AuthToken"];
            _fromPhoneNumber = configuration["Twilio:FromPhoneNumber"];

            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task SendSmsAsync(string toPhoneNumber, string message)
        {
            try
            {
                await MessageResource.CreateAsync(
                    to: new Twilio.Types.PhoneNumber(toPhoneNumber),
                    from: new Twilio.Types.PhoneNumber(_fromPhoneNumber),
                    body: message
                );
            }
            catch (Exception error)
            {
                throw new ApplicationException("Erro ao enviar o SMS.", error);
            }
        }
    }
}

