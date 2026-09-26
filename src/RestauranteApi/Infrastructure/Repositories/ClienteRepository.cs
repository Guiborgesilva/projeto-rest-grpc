using Microsoft.EntityFrameworkCore;
using RestauranteApi.Domain.Entities;
using RestauranteApi.Domain.Repositories;
using RestauranteApi.Infrastructure.Data;

namespace RestauranteApi.Infrastructure.Repositories;

public sealed class ClienteRepository(AppDbContext db) : IClienteRepository
{
    public async Task<IReadOnlyList<Cliente>> ListarAsync(CancellationToken cancellationToken = default)
        => await db.Clientes
            .AsNoTracking()
            .OrderBy(x => x.Nome)
            .ToListAsync(cancellationToken);

    public Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        => db.Clientes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExisteEmailAsync(
        string email,
        Guid? ignorarId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var query = db.Clientes.AsQueryable();

        query = query.Where(x => x.Email == normalizedEmail);

        if (ignorarId.HasValue)
            query = query.Where(x => x.Id != ignorarId.Value);

        return query.AnyAsync(cancellationToken);
    }

    public async Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        db.Clientes.Add(cliente);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        db.Clientes.Update(cliente);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task ExcluirAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        db.Clientes.Remove(cliente);
        await db.SaveChangesAsync(cancellationToken);
    }
}
