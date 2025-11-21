namespace SIPRI.Application.Exceptions;

/// <summary>
/// Exceção customizada para ser lançada quando uma entidade
/// ou recurso não é encontrado.
/// A camada de Presentation (API) deve ser configurada
/// para capturar esta exceção e retornar um HTTP 404.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException()
        : base()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public NotFoundException(string name, object key)
        : base($"Entidade \"{name}\" ({key}) não foi encontrada.")
    {
    }
}
