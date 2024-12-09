using System;
using System.Collections.Generic;

namespace ApiKnoock.Domains;

public partial class Entrega
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TipoUsuarioId { get; set; }

    public DateTime? DataRegistro { get; set; }

    public string Status { get; set; } = null!;

    public string? FotoProduto { get; set; }

    public bool? NotificacaoMorador { get; set; }

    public string? PinRetirada { get; set; }

    public DateTime? DataRetirada { get; set; }

    public DateTime? DataNotificacao { get; set; }

    public string? Observacao { get; set; }

    public string? Origem { get; set; }

    public virtual ICollection<NotificacaoEntrega> NotificacaoEntregas { get; set; } = new List<NotificacaoEntrega>();

    public virtual TipoUsuario TipoUsuario { get; set; } = null!;

    public virtual ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}
