using SIPRI.Domain.Entities;

namespace SIPRI.Domain.Contexts;

/// <summary>
/// Representa o "contexto" de dados necessário
/// para que as regras de pontuação possam operar.
/// Este objeto é imutável.
/// </summary>
public record CalculoRiscoContexto(
    IReadOnlyCollection<Investimento> Investimentos,
    IReadOnlyCollection<ProdutoInvestimento> Produtos,
    DateTime DataReferencia);