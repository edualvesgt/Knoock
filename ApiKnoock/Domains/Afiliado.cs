using System;
using System.Collections.Generic;

namespace ApiKnoock.Domains;

public partial class Afiliado
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TipoUsuarioId { get; set; }

    public int KnookCoins { get; set; }

    public bool FgOnline { get; set; }

    public bool FgTransito { get; set; }

    public bool FgResgatado { get; set; }

    public virtual TipoUsuario TipoUsuario { get; set; } = null!;
}
