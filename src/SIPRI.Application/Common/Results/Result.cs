namespace SIPRI.Application.Common.Results;

/// <summary>
/// Representa o resultado de uma operação que pode ter sucesso ou falha.
/// Baseado no Result Pattern do projeto Caixaverso.
/// </summary>
public class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        // Garante que o estado é consistente:
        // - Se sucesso, não pode ter erro
        // - Se falha, deve ter erro
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new InvalidOperationException("Estado inválido do Result: sucesso e erro são inconsistentes.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    public static Result Failure(Error error) => new(false, error);
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
}

/// <summary>
/// Representa o resultado de uma operação que pode ter sucesso ou falha,
/// com um valor de retorno tipado.
/// </summary>
/// <typeparam name="TValue">Tipo do valor retornado em caso de sucesso</typeparam>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Obtém o valor do resultado.
    /// Lança exceção se tentar acessar o valor de um resultado de falha.
    /// </summary>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("O valor de um resultado de falha não pode ser acessado.");
}

/// <summary>
/// Record que representa um erro com código e mensagem.
/// </summary>
/// <param name="Code">Código do erro (ex: "NotFound", "ValidationError")</param>
/// <param name="Message">Mensagem descritiva do erro</param>
public record Error(string Code, string Message)
{
    /// <summary>
    /// Representa a ausência de erro (usado em resultados de sucesso).
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty);
}
