using System;
using System.Collections.Generic;

namespace ApiKnoock.Domains;

public partial class TipoUsuario
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid IdUsuario { get; set; }

    public Guid IdTipo { get; set; }

    public virtual ICollection<Afiliado> Afiliados { get; set; } = new List<Afiliado>();

    public virtual ICollection<Condomino> Condominos { get; set; } = new List<Condomino>();

    public virtual ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();

    public virtual Tipo IdTipoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Notificacao> Notificacaos { get; set; } = new List<Notificacao>();

    public virtual ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}
