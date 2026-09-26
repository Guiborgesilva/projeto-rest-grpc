using System.Net;
using System.Text.Json;
using RestauranteApi.Domain.Exceptions;

namespace RestauranteApi.Infrastructure.Exceptions;

public sealed class DomainExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (DomainException exception)
        {
            var statusCode = exception switch
            {
                ClienteNaoEncontradoException => HttpStatusCode.NotFound,
                PedidoNaoEncontradoException => HttpStatusCode.NotFound,
                ClienteInativoException => HttpStatusCode.Conflict,
                EmailDuplicadoException => HttpStatusCode.Conflict,
                ClienteComPedidosException => HttpStatusCode.Conflict,
                ValidacaoDomainException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.BadRequest
            };

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/problem+json";

            var body = new
            {
                title = "Erro de domínio",
                status = (int)statusCode,
                detail = exception.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(body));
        }
    }
}
