using SIPRI.Domain.Entities;

namespace SIPRI.Domain.Contexts;

/// <summary>
/// Representa o "contexto" de dados necessário
/// para que as regras de cálculo de investimento possam operar.
/// Este objeto é imutável.
/// </summary>
public record CalculoInvestimentoContexto(
    decimal ValorInvestido,
    int PrazoMeses,
    ProdutoInvestimento Produto);