namespace ApiKnoock.Utils.Sms
{
    public interface ISmsRepository
    {
        Task<string> SendSmsAsync(string toPhoneNumber, string message);
        Task SendRecoveryCodeAsync(string toPhoneNumber, int recoveryCode);

    }
}
