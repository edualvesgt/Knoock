using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

public class EntregasHub : Hub
{
    
    /// Método para notificar um afiliado sobre uma nova entrega.
    public async Task NotificarEntrega(Guid afiliadoId, Guid entregaId)
    {
        // Envia notificação ao afiliado via SignalR
        await Clients.User(afiliadoId.ToString())
            .SendAsync("ReceberEntrega", entregaId);

        Console.WriteLine($"Notificação enviada para o afiliado {afiliadoId} sobre a entrega {entregaId}");
    }

   
    /// Método para o afiliado responder à entrega.
    public async Task ResponderEntrega(Guid entregaId, bool aceitou)
    {
        // Identifica o afiliado conectado
        var afiliadoId = Context.User?.Identity?.Name;

        if (afiliadoId == null)
        {
            throw new HubException("Usuário não autenticado.");
        }

        Console.WriteLine($"Afiliado {afiliadoId} respondeu para a entrega {entregaId}: {aceitou}");

        // Notifica o back-end sobre a resposta do afiliado
        if (aceitou)
        {
            await Clients.All.SendAsync("EntregaAceita", afiliadoId, entregaId);
        }
        else
        {
            await Clients.All.SendAsync("EntregaRecusada", afiliadoId, entregaId);
        }
    }


    /// Método para conectar o afiliado.
    public override async Task OnConnectedAsync()
    {
        var afiliadoId = Context.User?.Identity?.Name;

        if (afiliadoId != null)
        {
            Console.WriteLine($"Afiliado conectado: {afiliadoId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, "Afiliados");
        }

        await base.OnConnectedAsync();
    }


    /// Método para desconectar o afiliado.
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var afiliadoId = Context.User?.Identity?.Name;

        if (afiliadoId != null)
        {
            Console.WriteLine($"Afiliado desconectado: {afiliadoId}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Afiliados");
        }

        await base.OnDisconnectedAsync(exception);
    }
}
