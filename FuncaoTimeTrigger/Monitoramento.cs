using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ApiKnoock.Interface;
using Microsoft.AspNetCore.SignalR;
using ApiKnoock.Domains;
using Microsoft.Azure.Functions.Worker;

namespace ApiKnoock.Functions
{
    public class Monitoramento
    {
        private readonly IAfiliadosRepository _afiliadosRepository;
        private readonly IEntregaRepository _entregaRepository;
        private readonly IHubContext<EntregasHub> _hubContext;

        public Monitoramento(
            IAfiliadosRepository afiliadosRepository,
            IEntregaRepository entregaRepository,
            IHubContext<EntregasHub> hubContext)
        {
            _afiliadosRepository = afiliadosRepository;
            _entregaRepository = entregaRepository;
            _hubContext = hubContext;
        }

        [FunctionName("MonitorarFila")]
        public async Task Run([TimerTrigger("*/5 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Fila monitorada em: {DateTime.Now}");

            // Busca entregas pendentes
            var entregasPendentes = await _entregaRepository.GetEntregasPendentesAsync();
            if (!entregasPendentes.Any())
            {
                log.LogInformation("Nenhuma entrega pendente.");
                return;
            }

            // Busca afiliados online
            var afiliadosOnline = await _afiliadosRepository.GetAfiliadosOnlineAsync();
            if (!afiliadosOnline.Any())
            {
                log.LogInformation("Nenhum afiliado online disponível.");
                return;
            }

            // Atualiza a fila com afiliados online
            foreach (var afiliado in afiliadosOnline)
            {
                await _afiliadosRepository.AdicionarAfiliadoNaFilaAsync(afiliado.Id);
            }

            // Processa cada entrega pendente
            foreach (var entrega in entregasPendentes)
            {
                await ProcessarEntrega(entrega);
            }
        }

        private async Task ProcessarEntrega(Entrega entrega)
        {
            Afiliado? afiliadoAtual;

            // Tenta processar até que alguém aceite
            while ((afiliadoAtual = await _afiliadosRepository.ObterProximoAfiliadoDaFilaAsync()) != null)
            {
                // Envia notificação para o afiliado via SignalR
                await _hubContext.Clients.User(afiliadoAtual.TipoUsuarioId.ToString())
                    .SendAsync("NotificarEntrega", entrega.Id);

                // Aguarda resposta do afiliado (simulado aqui com um delay de 10s)
                var aceitou = await AguardaRespostaAfiliado(entrega.Id, afiliadoAtual.Id);

                if (aceitou)
                {
                    // Atualiza entrega e afiliado
                    await _entregaRepository.AtualizarStatusEntregaAsync(entrega.Id, "EmTransito");
                    _afiliadosRepository.AtualizarStatusAfiliado(afiliadoAtual.Id, true);

                    // Move afiliado para o final da fila
                    await _afiliadosRepository.AdicionarAfiliadoNaFilaAsync(afiliadoAtual.Id);
                    break;
                }
                else
                {
                    // Afiliado recusou; tenta o próximo
                    continue;
                }
            }
        }

        private async Task<bool> AguardaRespostaAfiliado(Guid entregaId, Guid afiliadoId)
        {
            var timeout = TimeSpan.FromSeconds(10);
            var startTime = DateTime.UtcNow;

            while (DateTime.UtcNow - startTime < timeout)
            {
                var resposta = EntregasHub.ObterRespostaEntrega(entregaId);
                if (resposta.HasValue)
                {
                    // Limpa a resposta para evitar reuso
                    EntregasHub.LimparRespostaEntrega(entregaId);
                    return resposta.Value;
                }
                await Task.Delay(1000);
            }

            return false;
        }
    }
}
