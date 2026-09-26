using Microsoft.EntityFrameworkCore;
using RestauranteApi.Domain.Entities;

namespace RestauranteApi.Infrastructure.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.EnsureCreatedAsync();

        if (await db.Clientes.AnyAsync())
            return;

        db.Clientes.AddRange(
            new Cliente
            {
                Nome = "João da Silva",
                Email = "joao@teste.com",
                Ativo = true
            },
            new Cliente
            {
                Nome = "Maria Inativa",
                Email = "maria@teste.com",
                Ativo = false
            });

        await db.SaveChangesAsync();
    }
}
