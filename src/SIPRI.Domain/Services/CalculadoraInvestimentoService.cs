using SIPRI.Domain.Contexts;
using SIPRI.Domain.Interfaces.Services;

namespace SIPRI.Domain.Services;

/// <summary>
/// Orquestra o cálculo do investimento
/// Este serviço usa o Padrão Strategy (com um toque de Factory)
/// para selecionar e executar a regra de cálculo correta
/// com base no Tipo do Produto.
/// </summary>
public sealed class CalculadoraInvestimentoService : ICalculadoraInvestimentoService
{
    /// <summary>
    /// Mapeia o "TipoProduto" (string) para a sua
    /// implementação de regra (IRegraCalculoInvestimento).
    /// </summary>
    private readonly IReadOnlyDictionary<string, IRegraCalculoInvestimento> _regras;

    /// <summary>
    /// Injeta todas as regras de cálculo (qualquer classe que
    /// implemente IRegraCalculoInvestimento) registradas na
    /// Injeção de Dependência.
    /// </summary>
    public CalculadoraInvestimentoService(IEnumerable<IRegraCalculoInvestimento> regras)
    {
        ArgumentNullException.ThrowIfNull(regras);

        if (!regras.Any())
        {
            throw new ArgumentException("Nenhuma regra de cálculo foi injetada.", nameof(regras));
        }

        // Constrói o dicionário para lookup rápido (O(1)).
        // Usa ToUpperInvariant() para garantir que a busca não seja case-sensitive.
        _regras = regras.ToDictionary(r => r.TipoProduto.ToUpperInvariant(), r => r);
    }

    /// <summary>
    /// Seleciona e executa a regra de cálculo correta.
    /// </summary>
    public decimal Calcular(CalculoInvestimentoContexto contexto)
    {
        ArgumentNullException.ThrowIfNull(contexto.Produto);

        var tipoProdutoKey = contexto.Produto.Tipo.ToUpperInvariant();

        // 1. Seleciona a Estratégia
        if (!_regras.TryGetValue(tipoProdutoKey, out var regra))
        {
            // Se não houver regra específica, lança uma exceção.
            // Tratar isso na camada de Application
            throw new NotSupportedException($"O tipo de produto '{contexto.Produto.Tipo}' não possui uma regra de cálculo definida.");
        }

        // 2. Executa a Estratégia
        return regra.Calcular(contexto);
    }
}