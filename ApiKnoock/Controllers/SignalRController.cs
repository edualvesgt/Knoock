using ApiKnoock.Interface;
using ApiKnoock.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api/[controller]")]
public class SignalRController : ControllerBase
{
    private readonly IAfiliadosRepository _afiliadosRepository;


    public SignalRController(IAfiliadosRepository afiliadosRepository)
    {
        _afiliadosRepository = afiliadosRepository;
    }

    // Adicionar afiliado na fila
    [HttpPost("adicionar-afiliado")]
    public async Task<IActionResult> AdicionarAfiliadoNaFila(Guid afiliadoId)
    {
        try
        {
            await _afiliadosRepository.AdicionarAfiliadoNaFilaAsync(afiliadoId);
            return Ok($"Afiliado {afiliadoId} adicionado na fila.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Obter próximo afiliado da fila
    [HttpGet("proximo-afiliado")]
    public async Task<IActionResult> ObterProximoAfiliado()
    {
        var afiliado = await _afiliadosRepository.ObterProximoAfiliadoDaFilaAsync();
        if (afiliado == null)
        {
            return NotFound("Nenhum afiliado na fila.");
        }
        return Ok(afiliado);
    }
}


