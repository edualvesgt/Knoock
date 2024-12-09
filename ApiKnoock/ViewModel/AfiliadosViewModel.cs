namespace ApiKnoock.ViewModel
{
    public class AfiliadosViewModel
    {
        public string? Nome { get; set; }
 
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public string? FotoUrl { get; set; }
        public DateOnly DataNascimento { get; set; }
        public bool FgOnline { get; set; }
        public bool FgTransito { get; set; }
        public bool FgResgatado { get; set; }
        public int KnoockCoins { get; set; }

    }
}
