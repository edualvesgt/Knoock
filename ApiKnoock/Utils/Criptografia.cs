namespace ApiKnoock.Utils
{
    public class Criptografia
    {
        public static string Hash(string senha)
        {
           return BCrypt.Net.BCrypt.HashPassword(senha);
        }

        public static bool CompareHash (string senhaForms, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(senhaForms, hash);
        }
    }
}
