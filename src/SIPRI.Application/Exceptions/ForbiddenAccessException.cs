namespace SIPRI.Application.Exceptions;

/// <summary>
/// Exceção customizada para ser lançada quando um usuário está
/// autenticado (possui um token válido), mas não possui autorização
/// (permissão) para executar uma ação ou acessar um recurso específico.
/// Ex: Cliente "123" tentando acessar "GET /investimentos/456".
/// A camada de Presentation (API) deve ser configurada para capturar
/// esta exceção e retornar um HTTP 403 Forbidden.
/// </summary>
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException()
        : base("O usuário não tem permissão para executar esta ação.")
    {
    }

    public ForbiddenAccessException(string message)
        : base(message)
    {
    }
}
