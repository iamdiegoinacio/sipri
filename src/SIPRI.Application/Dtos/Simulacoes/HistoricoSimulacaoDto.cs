namespace SIPRI.Application.DTOs.Simulacoes;

/// <a/summary>
/// DTO para a resposta da consulta de Histórico de Simulações.
/// </summary>
public class HistoricoSimulacaoDto
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public string Produto { get; set; } = string.Empty;
    public decimal ValorInvestido { get; set; }
    public decimal ValorFinal { get; set; }
    public int PrazoMeses { get; set; }
    public DateTime DataSimulacao { get; set; }
}