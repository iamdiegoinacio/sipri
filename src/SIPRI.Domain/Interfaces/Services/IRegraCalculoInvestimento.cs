using SIPRI.Domain.Contexts;

namespace SIPRI.Domain.Interfaces.Services;

/// <summary>
/// Define o contrato para uma regra de negócio (Estrategy)
/// que calcula o resultado de uma simulação de investimento.
/// </summary>
public interface IRegraCalculoInvestimento
{
    /// <summary>
    /// O tipo de produto que esta regra sabe como calcular.
    /// (ex: "CDB", "Fundo", "LCI")
    /// </summary>
    string TipoProduto { get; }

    /// <summary>
    /// Calcula o valor final do investimento com base no contexto.
    /// </summary>
    /// <param name="contexto">Os dados de entrada para o cálculo.</param>
    /// <returns>O Valor Final calculado.</returns>
    decimal Calcular(CalculoInvestimentoContexto contexto);
}