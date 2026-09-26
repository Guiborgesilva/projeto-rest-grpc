namespace RestauranteApi.Contracts;

public sealed record CriarPedidoRequest(Guid ClienteId, string Descricao, decimal ValorTotal);
public sealed record AtualizarPedidoRequest(Guid ClienteId, string Descricao, decimal ValorTotal);
