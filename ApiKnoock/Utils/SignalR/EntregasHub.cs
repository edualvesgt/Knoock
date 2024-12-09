using ApiKnoock.Domains;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class EntregasHub : Hub
{
    // Armazena as respostas dos afiliados
    private static readonly ConcurrentDictionary<Guid, bool?> RespostasEntregas = new();

    /// Notifica um afiliado sobre uma nova entrega
    public async Task NotificarEntrega(Guid afiliadoId, Guid entregaId)
    {
        await Clients.User(afiliadoId.ToString()).SendAsync("ReceberEntrega", entregaId);
        Console.WriteLine($"Notificação enviada para o afiliado {afiliadoId} sobre a entrega {entregaId}");
    }

    /// Afiliado responde à entrega
    public async Task ResponderEntrega(Guid entregaId, bool aceitou)
    {
        // Obtém o afiliadoId do contexto da conexão
        var afiliadoId = Context.Items["afiliadoId"] as string;

        if (afiliadoId == null)
        {
            Console.WriteLine("Usuário não autenticado ao responder entrega.");
            throw new HubException("Usuário não autenticado.");
        }

        // Registra a resposta no dicionário
        RespostasEntregas[entregaId] = aceitou;

        Console.WriteLine($"Afiliado {afiliadoId} respondeu para a entrega {entregaId}: {aceitou}");

        if (aceitou)
        {
            await Clients.All.SendAsync("EntregaAceita", afiliadoId, entregaId);
        }
        else
        {
            await Clients.All.SendAsync("EntregaRecusada", afiliadoId, entregaId);
        }
    }

    /// Limpa a resposta associada a uma entrega
    public static void LimparRespostaEntrega(Guid entregaId)
    {
        RespostasEntregas.TryRemove(entregaId, out _);
    }

    /// Busca a resposta registrada para uma entrega
    public static bool? ObterRespostaEntrega(Guid entregaId)
    {
        RespostasEntregas.TryGetValue(entregaId, out var resposta);
        return resposta;
    }

    /// Evento disparado quando um cliente se conecta ao Hub
    public override async Task OnConnectedAsync()
    {
        // Verifica se o usuário está autenticado
        var afiliadoId = Context.User.Identity?.Name;
        if (!string.IsNullOrEmpty(afiliadoId))
        {
            // Armazena o afiliadoId no contexto da conexão
            Context.Items["afiliadoId"] = afiliadoId;

            Console.WriteLine($"Afiliado conectado: {afiliadoId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, "Afiliados");
        }
        else
        {
            Console.WriteLine("Tentativa de conexão sem autenticação.");
        }

        await base.OnConnectedAsync();
    }

    /// Evento disparado quando um cliente se desconecta do Hub
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Remove o afiliadoId armazenado no contexto
        if (Context.Items.TryGetValue("afiliadoId", out var afiliadoId))
        {
            Console.WriteLine($"Afiliado desconectado: {afiliadoId}");
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Afiliados");
        await base.OnDisconnectedAsync(exception);
    }
}
