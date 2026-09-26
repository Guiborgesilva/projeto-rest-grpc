using Microsoft.EntityFrameworkCore;
using RestauranteApi.Domain.Entities;

namespace RestauranteApi.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("clientes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(180).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Ativo).IsRequired();
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToTable("pedidos");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Descricao).HasMaxLength(300).IsRequired();
            entity.Property(x => x.ValorTotal).HasPrecision(12, 2).IsRequired();
            entity.Property(x => x.CriadoEmUtc).IsRequired();
            entity.HasIndex(x => x.ClienteId);
        });

        // Relação entre Pedido e Cliente sem navegação pública na entidade.
        modelBuilder.Entity<Pedido>()
            .HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(x => x.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
