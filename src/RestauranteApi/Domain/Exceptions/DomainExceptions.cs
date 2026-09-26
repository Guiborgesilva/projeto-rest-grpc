namespace RestauranteApi.Domain.Exceptions;

public sealed class ClienteNaoEncontradoException(Guid id)
    : DomainException($"Cliente {id} não encontrado.");

public sealed class PedidoNaoEncontradoException(Guid id)
    : DomainException($"Pedido {id} não encontrado.");

public sealed class ClienteInativoException(Guid id)
    : DomainException($"O cliente {id} está inativo e não pode realizar pedidos.");

public sealed class EmailDuplicadoException(string email)
    : DomainException($"Já existe um cliente cadastrado com o e-mail '{email}'.");

public sealed class ClienteComPedidosException(Guid id)
    : DomainException($"O cliente {id} possui pedidos e não pode ser excluído.");

public sealed class ValidacaoDomainException(string message)
    : DomainException(message);
