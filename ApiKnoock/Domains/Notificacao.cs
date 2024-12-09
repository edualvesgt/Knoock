using System;
using System.Collections.Generic;

namespace ApiKnoock.Domains;

public partial class Notificacao
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TipoUsuarioId { get; set; }

    public string? Mensagem { get; set; }

    public DateTime? DataNotificacao { get; set; }

    public string? Status { get; set; }

    public string? Tipo { get; set; }

    public string? ImagemAviso { get; set; }

    public virtual NotificacaoEntrega? NotificacaoEntrega { get; set; }

    public virtual TipoUsuario TipoUsuario { get; set; } = null!;
}
