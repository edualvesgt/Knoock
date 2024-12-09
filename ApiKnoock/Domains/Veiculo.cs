using System;
using System.Collections.Generic;

namespace ApiKnoock.Domains;

public partial class Veiculo
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TipoUsuarioId { get; set; }

    public Guid? EntregaId { get; set; }

    public string? Marca { get; set; }

    public string? Modelo { get; set; }

    public string? Placa { get; set; }

    public int? Ano { get; set; }

    public virtual Entrega? Entrega { get; set; }

    public virtual TipoUsuario TipoUsuario { get; set; } = null!;
}
