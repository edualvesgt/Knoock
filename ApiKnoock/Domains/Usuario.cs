using System;
using System.Collections.Generic;

namespace ApiKnoock.Domains;

public partial class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Nome { get; set; } = null!;

    public string Telefone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly? DataNascimento { get; set; }

    public string? Endereco { get; set; }

    public string Senha { get; set; } = null!;

    public int? CodigoRecuperacao { get; set; }

    public string? FotoUsuario { get; set; }

    public virtual ICollection<TipoUsuario> TipoUsuarios { get; set; } = new List<TipoUsuario>();
}
