using SIPRI.Domain.Contexts;
using SIPRI.Domain.Interfaces.Services;

namespace SIPRI.Domain.Services;

/// <summary>
/// Regra de negócio para pontuar a Preferência de Risco (RMP).
/// </summary>
public sealed class RegraPontuacaoPreferencia : IRegraDePontuacao
{
    // --- Constantes encapsuladas, específicas desta regra ---
    private const int PontosBaixo = 10;
    private const int PontosMedio = 25;
    private const int PontosAlto = 40;

    private const decimal LimiteRmpMedio = 1.5m;
    private const decimal LimiteRmpAlto = 2.5m;

    /// <summary>
    /// Calcula a pontuação com base no contexto fornecido.
    /// </summary>
    public int CalcularPontuacao(CalculoRiscoContexto contexto)
    {
        // --- VALIDAÇÕES (GUARD CLAUSES) ---
        ArgumentNullException.ThrowIfNull(contexto.Investimentos);
        ArgumentNullException.ThrowIfNull(contexto.Produtos);

        // --- LÓGICA ---
        decimal valorTotalCarteira = contexto.Investimentos.Sum(i => i.Valor);
        if (valorTotalCarteira == 0) return PontosBaixo;

        decimal rmpAcumulado = 0m;

        // Criar um lookup (dicionário) para performance
        var produtoLookup = contexto.Produtos.ToDictionary(p => p.Id, p => p.NivelRisco);

        foreach (var investimento in contexto.Investimentos)
        {
            if (produtoLookup.TryGetValue(investimento.ProdutoId, out int nivelRisco))
            {
                decimal peso = investimento.Valor / valorTotalCarteira;
                rmpAcumulado += peso * nivelRisco;
            }
        }

        // Mapeia o RMP para os pontos
        if (rmpAcumulado < LimiteRmpMedio) return PontosBaixo;
        if (rmpAcumulado < LimiteRmpAlto) return PontosMedio;
        return PontosAlto;
    }
}