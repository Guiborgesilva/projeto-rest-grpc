using Microsoft.EntityFrameworkCore;
using RestauranteApi.Domain.Entities;
using RestauranteApi.Domain.Repositories;
using RestauranteApi.Infrastructure.Data;

namespace RestauranteApi.Infrastructure.Repositories;

public sealed class PedidoRepository(AppDbContext db) : IPedidoRepository
{
    public async Task<IReadOnlyList<Pedido>> ListarAsync(CancellationToken cancellationToken = default)
        => await db.Pedidos
            .AsNoTracking()
            .OrderByDescending(x => x.CriadoEmUtc)
            .ToListAsync(cancellationToken);

    public Task<Pedido?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        => db.Pedidos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistePorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
        => db.Pedidos.AnyAsync(x => x.ClienteId == clienteId, cancellationToken);

    public async Task AdicionarAsync(Pedido pedido, CancellationToken cancellationToken = default)
    {
        db.Pedidos.Add(pedido);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Pedido pedido, CancellationToken cancellationToken = default)
    {
        db.Pedidos.Update(pedido);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task ExcluirAsync(Pedido pedido, CancellationToken cancellationToken = default)
    {
        db.Pedidos.Remove(pedido);
        await db.SaveChangesAsync(cancellationToken);
    }
}
