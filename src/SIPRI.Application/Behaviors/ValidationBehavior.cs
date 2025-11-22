using FluentValidation;
using MediatR;

namespace SIPRI.Application.Behaviors;

/// <summary>
/// Pipeline behavior do MediatR que intercepta todas as requests
/// e executa validadores FluentValidation antes de chegar ao handler.
/// Baseado no padrão do projeto Caixaverso.
/// </summary>
/// <typeparam name="TRequest">Tipo da request (Command ou Query)</typeparam>
/// <typeparam name="TResponse">Tipo da response</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Se não houver validadores registrados para esta request, continua
        if (!_validators.Any())
        {
            return await next();
        }

        // Cria o contexto de validação
        var context = new ValidationContext<TRequest>(request);

        // Executa todos os validadores em paralelo
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Coleta todos os erros de validação
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // Se houver erros, lança exceção do FluentValidation
        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        // Se a validação passar, continua para o handler principal
        return await next();
    }
}