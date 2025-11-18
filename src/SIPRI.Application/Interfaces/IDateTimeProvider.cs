namespace SIPRI.Application.Interfaces;

/// <summary>
/// Abstrai o acesso ao tempo do sistema (DateTime.Now/UtcNow).
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Obtém a data e hora atual em UTC.
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    /// Obtém a data atual (sem hora).
    /// </summary>
    DateTime Today { get; }
}