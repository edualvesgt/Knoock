using Microsoft.AspNetCore.SignalR;

public class EntregasHub : Hub
{
    // Cliente envia resposta sobre a entrega
    public async Task RespostaAfiliado(Guid entregaId, bool aceitou)
    {
        // Processar resposta do afiliado
        if (aceitou)
        {
            Console.WriteLine($"Afiliado aceitou a entrega {entregaId}");
        }
        else
        {
            Console.WriteLine($"Afiliado recusou a entrega {entregaId}");
        }
    }
}
