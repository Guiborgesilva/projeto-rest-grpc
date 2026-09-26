# Restaurante API — REST + gRPC

Projeto acadêmico de uma API com os mesmos casos de uso expostos por REST e gRPC, organizada nas camadas **Apresentação → Domínio → Repositório**.

## Tecnologias

- .NET 10 LTS
- ASP.NET Core
- REST
- gRPC + Protobuf (`.proto`)
- Entity Framework Core 10
- PostgreSQL
- Docker / Docker Compose

## Domínio escolhido

A aplicação representa um restaurante com duas entidades principais:

- `Cliente`
- `Pedido`

### Regra de negócio principal

Um pedido só pode ser criado ou alterado para um **cliente ativo**.

A regra exige consultar a entidade `Cliente` durante a operação de `Pedido`, portanto não é apenas uma validação de campo.

Também existe a regra de que um cliente que já possui pedidos não pode ser excluído.

## Arquitetura

```text
REST Controller ───┐
                   ├──> IRestauranteService ───> Repositories ───> EF Core ───> PostgreSQL
gRPC Service ──────┘
```

- `Controllers/`: endpoints REST.
- `Grpc/`: implementação dos serviços gRPC gerados a partir do `.proto`.
- `Domain/Entities/`: entidades de negócio.
- `Domain/Services/`: interface e regras de negócio.
- `Domain/Repositories/`: contratos dos repositórios.
- `Infrastructure/Repositories/`: implementação usando Entity Framework Core.
- `Infrastructure/Data/`: `DbContext` e inicialização do banco.
- `Infrastructure/Exceptions/`: tradução das exceções de domínio para HTTP e gRPC.
- `Protos/`: contrato gRPC.

O ponto importante é que **Controller e serviço gRPC dependem da mesma `IRestauranteService`**. Assim, a regra "cliente precisa estar ativo" não é duplicada.

## Portas

- REST: `http://localhost:8080`
- gRPC: `localhost:5000`
- PostgreSQL: `localhost:5432`

## Como executar

Na raiz do projeto:

```bash
docker compose up --build
```

A aplicação cria o banco/tabelas e dois clientes de teste automaticamente na primeira execução:

- João da Silva — ativo
- Maria Inativa — inativa

## REST — exemplos com curl

### Listar clientes

```bash
curl http://localhost:8080/api/clientes
```

### Criar cliente

```bash
curl -X POST http://localhost:8080/api/clientes \
  -H "Content-Type: application/json" \
  -d '{"nome":"Carlos","email":"carlos@teste.com","ativo":true}'
```

### Listar pedidos

```bash
curl http://localhost:8080/api/pedidos
```

### Criar pedido para cliente ativo

Substitua `<ID_DO_JOAO>` pelo ID retornado em `/api/clientes`:

```bash
curl -X POST http://localhost:8080/api/pedidos \
  -H "Content-Type: application/json" \
  -d '{"clienteId":"<ID_DO_JOAO>","descricao":"X-Burger + batata","valorTotal":35.90}'
```

### Testar a regra de negócio

Use o ID da Maria Inativa:

```bash
curl -i -X POST http://localhost:8080/api/pedidos \
  -H "Content-Type: application/json" \
  -d '{"clienteId":"<ID_DA_MARIA>","descricao":"Pedido inválido","valorTotal":20.00}'
```

O REST deve retornar `409 Conflict`.

## gRPC com grpcurl

Com `grpcurl` instalado:

### Listar clientes

```bash
grpcurl -plaintext localhost:5000 restaurante.RestauranteService/ListarClientes
```

### Listar pedidos

```bash
grpcurl -plaintext localhost:5000 restaurante.RestauranteService/ListarPedidos
```

### Criar pedido

O campo `valor_centavos` representa o valor em centavos.

```bash
grpcurl -plaintext \
  -import-path ./src/RestauranteApi/Protos \
  -proto restaurante.proto \
  -d '{"cliente_id":"<ID_DO_JOAO>","descricao":"X-Burger + batata","valor_centavos":3590}' \
  localhost:5000 restaurante.RestauranteService/CriarPedido
```

Para testar a regra, use o ID da Maria Inativa. O gRPC deve responder com `FailedPrecondition`.

## Tratamento de exceções

As regras de negócio lançam exceções específicas em `Domain/Exceptions`.

A camada REST usa `DomainExceptionMiddleware` para converter as exceções em HTTP. A camada gRPC usa `DomainExceptionInterceptor` para convertê-las em códigos gRPC.

Exemplos:

| Exceção | REST | gRPC |
|---|---:|---|
| Cliente não encontrado | 404 | NotFound |
| Pedido não encontrado | 404 | NotFound |
| Cliente inativo | 409 | FailedPrecondition |
| E-mail duplicado | 409 | AlreadyExists |
| Cliente com pedidos | 409 | FailedPrecondition |
| Validação de domínio | 400 | InvalidArgument |
