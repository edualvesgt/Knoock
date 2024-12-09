using ApiKnoock.Domains;
using ApiKnoock.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

public class Monitoramento
{
    //Fazer o Agente Monitor
    //Fazer o Hub para aviso pro client
    //Fazer logica de regras de negocio
    //Configurar monitoramento 




    private readonly IEntregaRepository _entregaRepository;
    private readonly IAfiliadosRepository _afiliadosRepository;
    private readonly IHubContext<EntregasHub> _hubContext;

    public Monitoramento(IEntregaRepository entregaRepository, IHubContext<EntregasHub> hubContext, IAfiliadosRepository afiliados)
    {
        _entregaRepository = entregaRepository;
        _hubContext = hubContext;
        _afiliadosRepository = afiliados;
    }

    [FunctionName("MonitorarAfiliados")]
    public async Task Run([TimerTrigger("*/5 * * * * *")] TimerInfo myTimer, ILogger log)
    {
        log.LogInformation($"Monitorando entregas pendentes �s {DateTime.Now}");

        // Verifica entregas pendentes
        var entregasPendentes = await _entregaRepository.GetEntregasPendentesAsync();
        if (!entregasPendentes.Any())
        {
            log.LogInformation("Nenhuma entrega pendente encontrada.");
            return;
        }

        // Obt�m afiliados online
        var afiliadosOnline = await _afiliadosRepository.GetAfiliadosOnlineAsync();
        if (!afiliadosOnline.Any())
        {
            log.LogInformation("Nenhum afiliado online dispon�vel.");
            return;
        }

        // Adiciona afiliados online � fila
        foreach (var afiliado in afiliadosOnline)
        {
            await AdicionarAfiliadoNaFilaAsync(afiliado.Id);
        }

        // Processa a fila
        await ProcessarFilaAsync(entregasPendentes.FirstOrDefault(), log);
    }

    private async Task ProcessarFilaAsync(Entrega entrega, ILogger log)
    {
        var proximoAfiliado = await ObterProximoAfiliadoAsync();
        if (proximoAfiliado == null)
        {
            log.LogWarning("Nenhum afiliado na fila.");
            return;
        }

        // Envia notifica��o para o afiliado
        await _hubContext.Clients.User(proximoAfiliado.Id.ToString())
            .SendAsync("ReceberMensagem", $"Voc� deseja aceitar a entrega {entrega.Id}?");

        // Atualiza o status e remove da fila
        await _entregaRepository.AtualizarStatusEntregaAsync(entrega.Id, "Em Curso");
        await _entregaRepository.RemoverAfiliadoDaFilaAsync(proximoAfiliado.Id);

        log.LogInformation($"Afiliado {proximoAfiliado.Id} aceitou a entrega {entrega.Id}");
    }

    private async Task AdicionarAfiliadoNaFilaAsync(Guid afiliadoId)
    {
        // Implementar a l�gica para adicionar afiliado na fila
    }

    private async Task<Afiliado?> ObterProximoAfiliadoAsync()
    {
        var afiliadosOnline = await _afiliadosRepository.GetAfiliadosOnlineAsync();
        return afiliadosOnline.FirstOrDefault();
    }
}
