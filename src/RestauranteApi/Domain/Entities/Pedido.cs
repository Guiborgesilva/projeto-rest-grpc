namespace RestauranteApi.Domain.Entities;

public class Pedido
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClienteId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public DateTime CriadoEmUtc { get; set; } = DateTime.UtcNow;
}
