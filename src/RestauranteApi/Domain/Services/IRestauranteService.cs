using RestauranteApi.Domain.Entities;

namespace RestauranteApi.Domain.Services;

public interface IRestauranteService
{
    Task<IReadOnlyList<Cliente>> ListarClientesAsync(CancellationToken cancellationToken = default);
    Task<Cliente> ObterClienteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Cliente> CriarClienteAsync(string nome, string email, bool ativo, CancellationToken cancellationToken = default);
    Task<Cliente> AtualizarClienteAsync(Guid id, string nome, string email, bool ativo, CancellationToken cancellationToken = default);
    Task ExcluirClienteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Pedido>> ListarPedidosAsync(CancellationToken cancellationToken = default);
    Task<Pedido> ObterPedidoAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Pedido> CriarPedidoAsync(Guid clienteId, string descricao, decimal valorTotal, CancellationToken cancellationToken = default);
    Task<Pedido> AtualizarPedidoAsync(Guid id, Guid clienteId, string descricao, decimal valorTotal, CancellationToken cancellationToken = default);
    Task ExcluirPedidoAsync(Guid id, CancellationToken cancellationToken = default);
}
