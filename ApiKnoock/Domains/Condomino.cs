using System;
using System.Collections.Generic;

namespace ApiKnoock.Domains;

public partial class Condomino
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TipoUsuarioId { get; set; }

    public string? DeliveryPin { get; set; }

    public string? Pin { get; set; }

    public string? Bloco { get; set; }

    public string? Apartamento { get; set; }

    public virtual TipoUsuario TipoUsuario { get; set; } = null!;
}
