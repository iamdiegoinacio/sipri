using SIPRI.Domain.Contexts;
using SIPRI.Domain.Interfaces.Services;

namespace SIPRI.Domain.Services;

/// <summary>
/// Regra de negócio para pontuar a Frequência de transações.
/// </summary>
public sealed class RegraPontuacaoFrequencia : IRegraDePontuacao
{
    // --- Constantes encapsuladas, específicas desta regra ---
    private const int PontosBaixo = 10;
    private const int PontosMedio = 25;
    private const int PontosAlto = 40;

    private const int LimiteMesesFrequencia = -6;
    private const int LimiteFrequenciaBaixa = 2;
    private const int LimiteFrequenciaMedia = 6;

    /// <summary>
    /// Calcula a pontuação com base no contexto fornecido.
    /// </summary>
    public int CalcularPontuacao(CalculoRiscoContexto contexto)
    {
        // --- VALIDAÇÕES (GUARD CLAUSES) ---
        ArgumentNullException.ThrowIfNull(contexto.Investimentos);
        if (contexto.DataReferencia == DateTime.MinValue)
        {
            throw new ArgumentException("Data de referência inválida.", nameof(contexto.DataReferencia));
        }

        // --- LÓGICA ---
        var dataLimite = contexto.DataReferencia.AddMonths(LimiteMesesFrequencia);
        int numTransacoes = contexto.Investimentos.Count(i => i.Data >= dataLimite);

        if (numTransacoes <= LimiteFrequenciaBaixa) return PontosBaixo;
        if (numTransacoes <= LimiteFrequenciaMedia) return PontosMedio;
        return PontosAlto;
    }
}