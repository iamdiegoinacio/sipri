using SIPRI.Domain.Contexts;

namespace SIPRI.Domain.Interfaces.Services;

/// <summary>
/// Define o contrato para o serviço orquestrador
/// que calcula o resultado de uma simulação (RF-03).
/// </summary>
public interface ICalculadoraInvestimentoService
{
    /// <summary>
    /// Executa o cálculo da simulação, selecionando a
    /// estratégia correta com base no tipo de produto.
    /// </summary>
    /// <returns>O Valor Final calculado.</returns>
    decimal Calcular(CalculoInvestimentoContexto contexto);
}