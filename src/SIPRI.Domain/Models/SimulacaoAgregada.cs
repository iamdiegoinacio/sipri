namespace SIPRI.Domain.Models;

/// <summary>
/// Modelo de dados para o resultado da agregação
/// (GET /simulacoes/por-produto-dia)
/// </summary>
public class SimulacaoAgregada
{
    public string Produto { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public int QuantidadeSimulacoes { get; set; }
    public decimal MediaValorFinal { get; set; }
}