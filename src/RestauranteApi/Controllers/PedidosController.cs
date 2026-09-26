using Microsoft.AspNetCore.Mvc;
using RestauranteApi.Contracts;
using RestauranteApi.Domain.Entities;
using RestauranteApi.Domain.Services;

namespace RestauranteApi.Controllers;

[ApiController]
[Route("api/pedidos")]
public sealed class PedidosController(IRestauranteService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Pedido>>> Listar(CancellationToken cancellationToken)
        => Ok(await service.ListarPedidosAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Pedido>> Obter(Guid id, CancellationToken cancellationToken)
        => Ok(await service.ObterPedidoAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<Pedido>> Criar(
        [FromBody] CriarPedidoRequest request,
        CancellationToken cancellationToken)
    {
        var pedido = await service.CriarPedidoAsync(
            request.ClienteId,
            request.Descricao,
            request.ValorTotal,
            cancellationToken);

        return CreatedAtAction(nameof(Obter), new { id = pedido.Id }, pedido);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Pedido>> Atualizar(
        Guid id,
        [FromBody] AtualizarPedidoRequest request,
        CancellationToken cancellationToken)
        => Ok(await service.AtualizarPedidoAsync(
            id,
            request.ClienteId,
            request.Descricao,
            request.ValorTotal,
            cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken cancellationToken)
    {
        await service.ExcluirPedidoAsync(id, cancellationToken);
        return NoContent();
    }
}
