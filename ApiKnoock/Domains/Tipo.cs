using System;
using System.Collections.Generic;

namespace ApiKnoock.Domains;

public partial class Tipo
{
    public Guid Id { get; set; }

    public string Tipo1 { get; set; } = null!;

    public virtual ICollection<TipoUsuario> TipoUsuarios { get; set; } = new List<TipoUsuario>();
}
