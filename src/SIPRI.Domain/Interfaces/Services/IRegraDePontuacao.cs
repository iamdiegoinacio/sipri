using SIPRI.Domain.Contexts;

namespace SIPRI.Domain.Interfaces.Services;

/// <summary>
/// Define o contrato para uma regra de negócio
/// que calcula uma parte da pontuação de risco.
/// </summary>
public interface IRegraDePontuacao
{
    /// <summary>
    /// Calcula a pontuação com base no contexto fornecido.
    /// </summary>
    /// <param name="contexto">O objeto com todos os dados necessários</param>
    /// <returns>A pontuação calculada pela regra.</returns>
    int CalcularPontuacao(CalculoRiscoContexto contexto);
}