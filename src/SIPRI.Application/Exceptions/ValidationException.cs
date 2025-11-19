using System.Collections.Generic;

namespace SIPRI.Application.Exceptions;

/// <summary>
/// Lançada quando a validação da lógica de negócio na camada de Aplicação falha.
/// (Ex: "O produto 'XYZ' não é válido para simulação").
/// A camada de Presentation deve capturar e retornar um HTTP 400 Bad Request.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Coleção de erros de validação, onde a 'key' é o nome do campo
    /// (ou uma chave genérica) e o 'value' é um array de mensagens
    /// de erro para aquele campo.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Construtor para um único erro de validação.
    /// </summary>
    /// <param name="field">O nome do campo que falhou na validação.</param>
    /// <param name="message">A mensagem de erro.</param>
    public ValidationException(string field, string message)
        : base("A requisição falhou na validação.")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { message } }
        };
    }

    /// <summary>
    /// Construtor que aceita um dicionário de múltiplos erros de validação.
    /// </summary>
    /// <param name="errors">Um dicionário de erros de validação.</param>
    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("A requisição falhou na validação.")
    {
        Errors = errors;
    }
}