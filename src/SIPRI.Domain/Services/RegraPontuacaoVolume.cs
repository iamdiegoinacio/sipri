using SIPRI.Domain.Contexts;
using SIPRI.Domain.Interfaces.Services;

namespace SIPRI.Domain.Services;

/// <summary>
/// Regra de negócio para pontuar o Volume total investido.
/// </summary>
public sealed class RegraPontuacaoVolume : IRegraDePontuacao
{
    // --- Constantes encapsuladas, específicas desta regra ---
    private const int PontosVolumeMax = 20;
    private const decimal LimiteVolumeBaixo = 5000m;
    private const decimal LimiteVolumeMedio = 50000m;
    private const decimal PercentualVolumeBaixo = 0.25m;
    private const decimal PercentualVolumeMedio = 0.5m;

    /// <summary>
    /// Calcula a pontuação com base no contexto fornecido.
    /// </summary>
    public int CalcularPontuacao(CalculoRiscoContexto contexto)
    {
        // --- VALIDAÇÕES (GUARD CLAUSES) ---
        ArgumentNullException.ThrowIfNull(contexto.Investimentos);

        // --- LÓGICA ---
        decimal valorTotal = contexto.Investimentos.Sum(i => i.Valor);

        if (valorTotal <= LimiteVolumeBaixo) return (int)(PontosVolumeMax * PercentualVolumeBaixo);
        if (valorTotal <= LimiteVolumeMedio) return (int)(PontosVolumeMax * PercentualVolumeMedio);
        return PontosVolumeMax;
    }
}