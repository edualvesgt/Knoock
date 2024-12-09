namespace ApiKnoock.Utils.Email
{
    public class EmailSendingService
    {
        private readonly IEmailService _emailService;

        public EmailSendingService(IEmailService service)
        {
            _emailService = service;
        }

    
        public async Task SendRecoveryPassword(int codigo, string email)
        {
            try
            {
                MailRequest request = new MailRequest
                {
                    ToEmail = email,
                    Subject = "Código de Recuperação",
                    Body = GetHtmlContentRecovery(codigo)
                };

                //Await retirado para teste do metodo, mas o problema esta por aqui
                 _emailService.SendEmailAsync(request);
            }
            catch (Exception)
            {
                throw;
            }
        }
      

        private string GetHtmlContentRecovery(int codigo)
        {
            string Response = @"
<div style=""width:100%; padding: 20px;"">
    <div style=""max-width: 600px; margin: 0 auto; border-radius: 10px; padding: 20px; background-color:#FFFFFF;"">
        <div style=""background: url('https://livefest.blob.core.windows.net/bloblivefestcontainer/Splash%20screen.png') no-repeat center center; background-size: cover; padding: 20px;"">
            <img src=""https://livefest.blob.core.windows.net/bloblivefestcontainer/NewLogoI%20-%20LiveFest.png"" alt=""Logotipo do LiveFest"" style=""display: block; margin: 0 auto; max-width: 200px;"" />
            <h1 style=""color: #ffffff; text-align: center;"">Recuperação de senha</h1>
            <p style=""color: #ffffff; font-size: 24px; text-align: center;"">Código de confirmação <strong>" + codigo + @"</strong></p>
        </div>
    </div>
</div>";

            return Response;
        }
    }
}
