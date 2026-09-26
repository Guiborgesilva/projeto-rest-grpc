using Grpc.Core;
using Grpc.Core.Interceptors;
using RestauranteApi.Domain.Exceptions;

namespace RestauranteApi.Infrastructure.Exceptions;

public sealed class DomainExceptionInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (DomainException exception)
        {
            var statusCode = exception switch
            {
                ClienteNaoEncontradoException => StatusCode.NotFound,
                PedidoNaoEncontradoException => StatusCode.NotFound,
                ClienteInativoException => StatusCode.FailedPrecondition,
                EmailDuplicadoException => StatusCode.AlreadyExists,
                ClienteComPedidosException => StatusCode.FailedPrecondition,
                ValidacaoDomainException => StatusCode.InvalidArgument,
                _ => StatusCode.FailedPrecondition
            };

            throw new RpcException(new Status(statusCode, exception.Message));
        }
    }
}
