using RestauranteApi.Domain.Entities;

namespace RestauranteApi.Domain.Repositories;

public interface IClienteRepository
{
    Task<IReadOnlyList<Cliente>> ListarAsync(CancellationToken cancellationToken = default);
    Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExisteEmailAsync(string email, Guid? ignorarId = null, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task ExcluirAsync(Cliente cliente, CancellationToken cancellationToken = default);
}
