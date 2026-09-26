using Microsoft.AspNetCore.Mvc;
using RestauranteApi.Contracts;
using RestauranteApi.Domain.Entities;
using RestauranteApi.Domain.Services;

namespace RestauranteApi.Controllers;

[ApiController]
[Route("api/clientes")]
public sealed class ClientesController(IRestauranteService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Cliente>>> Listar(CancellationToken cancellationToken)
        => Ok(await service.ListarClientesAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Cliente>> Obter(Guid id, CancellationToken cancellationToken)
        => Ok(await service.ObterClienteAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<Cliente>> Criar(
        [FromBody] CriarClienteRequest request,
        CancellationToken cancellationToken)
    {
        var cliente = await service.CriarClienteAsync(
            request.Nome,
            request.Email,
            request.Ativo,
            cancellationToken);

        return CreatedAtAction(nameof(Obter), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Cliente>> Atualizar(
        Guid id,
        [FromBody] AtualizarClienteRequest request,
        CancellationToken cancellationToken)
        => Ok(await service.AtualizarClienteAsync(
            id,
            request.Nome,
            request.Email,
            request.Ativo,
            cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken cancellationToken)
    {
        await service.ExcluirClienteAsync(id, cancellationToken);
        return NoContent();
    }
}
