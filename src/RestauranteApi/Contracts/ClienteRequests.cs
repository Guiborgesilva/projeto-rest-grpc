namespace RestauranteApi.Contracts;

public sealed record CriarClienteRequest(string Nome, string Email, bool Ativo = true);
public sealed record AtualizarClienteRequest(string Nome, string Email, bool Ativo);
