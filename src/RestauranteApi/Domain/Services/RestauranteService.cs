using RestauranteApi.Domain.Entities;
using RestauranteApi.Domain.Exceptions;
using RestauranteApi.Domain.Repositories;

namespace RestauranteApi.Domain.Services;

public sealed class RestauranteService(
    IClienteRepository clienteRepository,
    IPedidoRepository pedidoRepository) : IRestauranteService
{
    public Task<IReadOnlyList<Cliente>> ListarClientesAsync(CancellationToken cancellationToken = default)
        => clienteRepository.ListarAsync(cancellationToken);

    public async Task<Cliente> ObterClienteAsync(Guid id, CancellationToken cancellationToken = default)
        => await clienteRepository.ObterPorIdAsync(id, cancellationToken)
           ?? throw new ClienteNaoEncontradoException(id);

    public async Task<Cliente> CriarClienteAsync(
        string nome,
        string email,
        bool ativo,
        CancellationToken cancellationToken = default)
    {
        ValidarTexto(nome, "Nome");
        ValidarTexto(email, "E-mail");

        if (await clienteRepository.ExisteEmailAsync(email, cancellationToken: cancellationToken))
            throw new EmailDuplicadoException(email);

        var cliente = new Cliente
        {
            Nome = nome.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Ativo = ativo
        };

        await clienteRepository.AdicionarAsync(cliente, cancellationToken);
        return cliente;
    }

    public async Task<Cliente> AtualizarClienteAsync(
        Guid id,
        string nome,
        string email,
        bool ativo,
        CancellationToken cancellationToken = default)
    {
        ValidarTexto(nome, "Nome");
        ValidarTexto(email, "E-mail");

        var cliente = await ObterClienteAsync(id, cancellationToken);

        if (await clienteRepository.ExisteEmailAsync(email, id, cancellationToken))
            throw new EmailDuplicadoException(email);

        cliente.Nome = nome.Trim();
        cliente.Email = email.Trim().ToLowerInvariant();
        cliente.Ativo = ativo;

        await clienteRepository.AtualizarAsync(cliente, cancellationToken);
        return cliente;
    }

    public async Task ExcluirClienteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cliente = await ObterClienteAsync(id, cancellationToken);

        if (await pedidoRepository.ExistePorClienteAsync(id, cancellationToken))
            throw new ClienteComPedidosException(id);

        await clienteRepository.ExcluirAsync(cliente, cancellationToken);
    }

    public Task<IReadOnlyList<Pedido>> ListarPedidosAsync(CancellationToken cancellationToken = default)
        => pedidoRepository.ListarAsync(cancellationToken);

    public async Task<Pedido> ObterPedidoAsync(Guid id, CancellationToken cancellationToken = default)
        => await pedidoRepository.ObterPorIdAsync(id, cancellationToken)
           ?? throw new PedidoNaoEncontradoException(id);

    public async Task<Pedido> CriarPedidoAsync(
        Guid clienteId,
        string descricao,
        decimal valorTotal,
        CancellationToken cancellationToken = default)
    {
        ValidarPedido(descricao, valorTotal);
        await ValidarClientePodePedirAsync(clienteId, cancellationToken);

        var pedido = new Pedido
        {
            ClienteId = clienteId,
            Descricao = descricao.Trim(),
            ValorTotal = valorTotal,
            CriadoEmUtc = DateTime.UtcNow
        };

        await pedidoRepository.AdicionarAsync(pedido, cancellationToken);
        return pedido;
    }

    public async Task<Pedido> AtualizarPedidoAsync(
        Guid id,
        Guid clienteId,
        string descricao,
        decimal valorTotal,
        CancellationToken cancellationToken = default)
    {
        ValidarPedido(descricao, valorTotal);

        var pedido = await ObterPedidoAsync(id, cancellationToken);
        await ValidarClientePodePedirAsync(clienteId, cancellationToken);

        pedido.ClienteId = clienteId;
        pedido.Descricao = descricao.Trim();
        pedido.ValorTotal = valorTotal;

        await pedidoRepository.AtualizarAsync(pedido, cancellationToken);
        return pedido;
    }

    public async Task ExcluirPedidoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pedido = await ObterPedidoAsync(id, cancellationToken);
        await pedidoRepository.ExcluirAsync(pedido, cancellationToken);
    }

    private async Task ValidarClientePodePedirAsync(Guid clienteId, CancellationToken cancellationToken)
    {
        var cliente = await ObterClienteAsync(clienteId, cancellationToken);

        if (!cliente.Ativo)
            throw new ClienteInativoException(cliente.Id);
    }

    private static void ValidarPedido(string descricao, decimal valorTotal)
    {
        ValidarTexto(descricao, "Descrição");

        if (valorTotal <= 0)
            throw new ValidacaoDomainException("O valor total do pedido deve ser maior que zero.");
    }

    private static void ValidarTexto(string valor, string campo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ValidacaoDomainException($"O campo {campo} é obrigatório.");
    }
}
