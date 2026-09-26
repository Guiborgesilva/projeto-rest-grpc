using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using RestauranteApi.Domain.Repositories;
using RestauranteApi.Domain.Services;
using RestauranteApi.Infrastructure.Data;
using RestauranteApi.Infrastructure.Exceptions;
using RestauranteApi.Infrastructure.Repositories;
using RestauranteApi.Grpc;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // REST: HTTP/1.1
    options.ListenAnyIP(8080, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });

    // gRPC: HTTP/2 sem TLS, ideal para o ambiente de demonstração em Docker.
    options.ListenAnyIP(5000, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' não configurada.");

builder.Services.AddControllers();
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<DomainExceptionInterceptor>();
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IRestauranteService, RestauranteApi.Domain.Services.RestauranteService>();

var app = builder.Build();

app.UseMiddleware<DomainExceptionMiddleware>();

// Cria o esquema e os dados iniciais automaticamente na primeira execução.
await DatabaseInitializer.InitializeAsync(app.Services);

app.MapControllers();
app.MapGrpcService<RestauranteApi.Grpc.RestauranteGrpcService>();

app.MapGet("/", () => Results.Ok(new
{
    aplicacao = "Restaurante API",
    rest = "http://localhost:8080",
    grpc = "localhost:5000"
}));

app.Run();
