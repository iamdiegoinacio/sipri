using SIPRI.Domain.Entities;
using SIPRI.Domain.ValueObjects;

namespace SIPRI.Domain.Interfaces.Services;

/// <summary>
/// Define o contrato para o serviço orquestrador
/// que calcula o perfil de risco do cliente.
/// </summary>
public interface IMotorPerfilRiscoServico
{
    /// <summary>
    /// Calcula o perfil de risco com base na carteira de investimentos e produtos.
    /// </summary>
    /// <param name="investimentos">A carteira de investimentos do cliente</param>
    /// <param name="produtos">O catálogo de produtos disponíveis</param>
    /// <param name="dataReferencia">A data base para o cálculo (usado para regras de tempo)</param>
    /// <returns>Um Value Object 'PerfilRisco' 100% válido.</returns>
    PerfilRisco CalcularPerfil(
        IReadOnlyCollection<Investimento> investimentos,
        IReadOnlyCollection<ProdutoInvestimento> produtos,
        DateTime dataReferencia);
}