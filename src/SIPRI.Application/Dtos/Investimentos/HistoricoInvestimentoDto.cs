namespace SIPRI.Application.DTOs.Investimentos;

/// <a/summary>
/// DTO para a resposta da consulta de Histórico de Investimentos.
/// </summary>
public class HistoricoInvestimentoDto
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal Rentabilidade { get; set; }
    public DateTime Data { get; set; }
}