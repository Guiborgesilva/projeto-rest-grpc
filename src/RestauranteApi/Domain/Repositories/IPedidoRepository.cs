using RestauranteApi.Domain.Entities;

namespace RestauranteApi.Domain.Repositories;

public interface IPedidoRepository
{
    Task<IReadOnlyList<Pedido>> ListarAsync(CancellationToken cancellationToken = default);
    Task<Pedido?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistePorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Pedido pedido, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Pedido pedido, CancellationToken cancellationToken = default);
    Task ExcluirAsync(Pedido pedido, CancellationToken cancellationToken = default);
}
