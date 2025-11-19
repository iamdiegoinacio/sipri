namespace SIPRI.Application.Exceptions;

/// <summary>
/// Lançada pela camada de Aplicação ao capturar uma exceção de Infraestrutura
/// que indica uma falha externa (ex: timeout do banco de dados, API de 
/// terceiros fora do ar).
/// Isso permite que o middleware diferencie um bug (500) de uma
/// indisponibilidade de serviço (503).
/// </summary>
public class InfrastructureException : Exception
{
    public InfrastructureException(string serviceName, string message)
        : base($"O serviço externo '{serviceName}' falhou: {message}")
    {
    }

    public InfrastructureException(string serviceName, string message, Exception innerException)
        : base($"O serviço externo '{serviceName}' falhou: {message}", innerException)
    {
    }
}