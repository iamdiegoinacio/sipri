namespace SIPRI.Application.Exceptions;

/// <summary>
/// Exceção customizada para ser lançada quando uma solicitação válida
/// não pode ser completada devido a um conflito com o estado atual
/// do recurso no servidor.
/// Ex: Uma falha de concorrência otimista (quando um usuário tenta salvar
///    dados desatualizados que já foram modificados por outro usuário).
/// A camada de Presentation (API) deve ser configurada para capturar
/// esta exceção e retornar um HTTP 409 Conflict.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
