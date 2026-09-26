# Relatório de andamento — Restaurante API

## 1. Objetivo

Desenvolver uma API que disponibilize os mesmos casos de uso por REST e gRPC, utilizando uma arquitetura em camadas e mantendo as regras de negócio concentradas no Domínio.

## 2. Domínio escolhido

Foi escolhido o domínio de **pedidos de um restaurante**, por ser simples de entender e permitir demonstrar uma relação entre entidades com regra de negócio real.

As duas entidades principais definidas nesta etapa são:

- **Cliente**: representa o consumidor do restaurante.
- **Pedido**: representa um pedido realizado por um cliente.

A relação é de um cliente para vários pedidos.

## 3. Regra de negócio

A principal regra definida é:

> Um pedido só pode ser criado ou alterado para um cliente que esteja ativo.

Essa regra exige consultar a entidade `Cliente` durante uma operação relacionada a `Pedido`, caracterizando uma regra que cruza os dois agregados.

Também foi definida uma regra complementar:

> Um cliente que já possui pedidos não pode ser excluído.

## 4. Tecnologias definidas

- **.NET 10 / ASP.NET Core** para a API.
- **gRPC + Protobuf** para a interface gRPC.
- **Entity Framework Core** para persistência.
- **PostgreSQL** como banco de dados relacional.
- **Docker Compose** para executar a aplicação e o banco.

A escolha do .NET 10 foi feita por ser uma versão LTS atualmente suportada. O contrato gRPC será mantido em arquivo `.proto`, seguindo a abordagem contract-first.

## 5. Estrutura de camadas

```text
Controllers/       -> apresentação REST
Grpc/              -> apresentação gRPC
Domain/Entities/   -> entidades
Domain/Services/   -> regras de negócio e interface do serviço
Domain/Repositories/ -> contratos de acesso a dados
Infrastructure/Repositories/ -> acesso ao PostgreSQL via EF Core
Infrastructure/Data/ -> DbContext e inicialização do banco
Protos/            -> contrato gRPC
```

REST e gRPC utilizam a mesma interface `IRestauranteService`. Com isso, a regra de negócio não é repetida nos dois meios de comunicação.

## 6. Persistência

Os dados serão persistidos em PostgreSQL.

A aplicação utiliza Entity Framework Core para acessar o banco. O PostgreSQL é executado como um serviço separado no `docker-compose.yml` e os dados ficam em um volume Docker chamado `postgres_data`.

Na primeira execução, o esquema é criado automaticamente e são inseridos dois clientes de teste: um ativo e um inativo, permitindo demonstrar a regra de negócio.

## 7. Comunicação

Foram reservadas duas portas:

- **8080** para REST/HTTP.
- **5000** para gRPC/HTTP2.

A separação de portas facilita os testes no Postman e com `grpcurl` durante o desenvolvimento.

## 8. Estado atual

Nesta etapa foram definidos e estruturados:

- domínio da aplicação;
- entidades `Cliente` e `Pedido`;
- regra de negócio entre as entidades;
- interface única `IRestauranteService`;
- contratos dos repositórios;
- repositórios usando Entity Framework Core;
- endpoints REST iniciais;
- contrato gRPC em `.proto`;
- serviço gRPC reutilizando o mesmo domínio;
- tratamento centralizado das exceções;
- configuração inicial do PostgreSQL;
- Dockerfile e Docker Compose;
- README com instruções de execução e exemplos.

A próxima etapa de desenvolvimento é executar a solução no ambiente local, validar a criação do banco e testar os mesmos casos de uso via REST e gRPC.

## 9. Requisitos já contemplados na estrutura

- [x] Camadas separadas.
- [x] Duas entidades relacionadas.
- [x] Regra de negócio cruzando entidades.
- [x] Mesmo serviço de domínio para REST e gRPC.
- [x] Contrato gRPC via `.proto`.
- [x] Exceções de domínio com mapeamento específico.
- [x] PostgreSQL para persistência.
- [x] Dockerfile e docker-compose.
- [x] README inicial.

## 10. Pendências

- Validar a compilação no ambiente de desenvolvimento.
- Executar a aplicação com Docker.
- Testar os endpoints REST no Postman.
- Testar as operações gRPC com Postman ou `grpcurl`.
- Revisar os detalhes finais com os integrantes do grupo e adaptar a implementação ao conteúdo cobrado pelo professor.
