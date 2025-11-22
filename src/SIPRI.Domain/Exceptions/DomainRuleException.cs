namespace SIPRI.Domain.Exceptions;

/// <summary>
/// Lançada pela camada de Domínio quando uma regra de negócio central (invariante)
/// é violada. (Ex: Tentar criar um PerfilRisco com pontuação negativa,
/// ou simular um investimento com valor zero).
/// A camada de Presentation deve capturar e retornar um HTTP 400 Bad Request
/// ou HTTP 409 Conflict, dependendo da semântica da regra.
/// </summary>
public class DomainRuleException : Exception
{
    public DomainRuleException(string message)
        : base(message)
    {
    }

    public DomainRuleException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}