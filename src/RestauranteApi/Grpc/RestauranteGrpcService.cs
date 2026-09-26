using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using RestauranteApi.Domain.Entities;
using RestauranteApi.Domain.Exceptions;
using RestauranteApi.Domain.Services;

namespace RestauranteApi.Grpc;

public sealed class RestauranteGrpcService(IRestauranteService service)
    : RestauranteService.RestauranteServiceBase
{
    public override async Task<ListaClientesResponse> ListarClientes(
        Empty request,
        ServerCallContext context)
    {
        var clientes = await service.ListarClientesAsync(context.CancellationToken);
        var response = new ListaClientesResponse();

        response.Clientes.AddRange(clientes.Select(MapCliente));
        return response;
    }

    public override async Task<ClienteResponse> ObterCliente(
        IdRequest request,
        ServerCallContext context)
    {
        var cliente = await service.ObterClienteAsync(ParseGuid(request.Id), context.CancellationToken);
        return MapCliente(cliente);
    }

    public override async Task<ClienteResponse> CriarCliente(
        CriarClienteRequest request,
        ServerCallContext context)
    {
        var cliente = await service.CriarClienteAsync(
            request.Nome,
            request.Email,
            request.Ativo,
            context.CancellationToken);

        return MapCliente(cliente);
    }

    public override async Task<ClienteResponse> AtualizarCliente(
        AtualizarClienteRequest request,
        ServerCallContext context)
    {
        var cliente = await service.AtualizarClienteAsync(
            ParseGuid(request.Id),
            request.Nome,
            request.Email,
            request.Ativo,
            context.CancellationToken);

        return MapCliente(cliente);
    }

    public override async Task<Empty> ExcluirCliente(
        IdRequest request,
        ServerCallContext context)
    {
        await service.ExcluirClienteAsync(ParseGuid(request.Id), context.CancellationToken);
        return new Empty();
    }

    public override async Task<ListaPedidosResponse> ListarPedidos(
        Empty request,
        ServerCallContext context)
    {
        var pedidos = await service.ListarPedidosAsync(context.CancellationToken);
        var response = new ListaPedidosResponse();

        response.Pedidos.AddRange(pedidos.Select(MapPedido));
        return response;
    }

    public override async Task<PedidoResponse> ObterPedido(
        IdRequest request,
        ServerCallContext context)
    {
        var pedido = await service.ObterPedidoAsync(ParseGuid(request.Id), context.CancellationToken);
        return MapPedido(pedido);
    }

    public override async Task<PedidoResponse> CriarPedido(
        CriarPedidoRequest request,
        ServerCallContext context)
    {
        var pedido = await service.CriarPedidoAsync(
            ParseGuid(request.ClienteId),
            request.Descricao,
            CentavosParaDecimal(request.ValorCentavos),
            context.CancellationToken);

        return MapPedido(pedido);
    }

    public override async Task<PedidoResponse> AtualizarPedido(
        AtualizarPedidoRequest request,
        ServerCallContext context)
    {
        var pedido = await service.AtualizarPedidoAsync(
            ParseGuid(request.Id),
            ParseGuid(request.ClienteId),
            request.Descricao,
            CentavosParaDecimal(request.ValorCentavos),
            context.CancellationToken);

        return MapPedido(pedido);
    }

    public override async Task<Empty> ExcluirPedido(
        IdRequest request,
        ServerCallContext context)
    {
        await service.ExcluirPedidoAsync(ParseGuid(request.Id), context.CancellationToken);
        return new Empty();
    }

    private static ClienteResponse MapCliente(Cliente cliente) => new()
    {
        Id = cliente.Id.ToString(),
        Nome = cliente.Nome,
        Email = cliente.Email,
        Ativo = cliente.Ativo
    };

    private static PedidoResponse MapPedido(Pedido pedido) => new()
    {
        Id = pedido.Id.ToString(),
        ClienteId = pedido.ClienteId.ToString(),
        Descricao = pedido.Descricao,
        ValorCentavos = DecimalParaCentavos(pedido.ValorTotal),
        CriadoEmUtc = Timestamp.FromDateTime(DateTime.SpecifyKind(pedido.CriadoEmUtc, DateTimeKind.Utc))
    };

    private static Guid ParseGuid(string value)
    {
        if (Guid.TryParse(value, out var id))
            return id;

        throw new ValidacaoDomainException($"ID inválido: '{value}'.");
    }

    private static decimal CentavosParaDecimal(long valorCentavos)
        => valorCentavos / 100m;

    private static long DecimalParaCentavos(decimal valor)
        => decimal.ToInt64(decimal.Round(valor * 100m, 0, MidpointRounding.AwayFromZero));
}
