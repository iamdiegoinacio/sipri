namespace SIPRI.Application.DTOs.Simulacao;

/// <summary>
/// DTO para a resposta da consulta de Simulações Agregadas.
/// </summary>
public class SimulacaoAgregadaDto
{
    public string Produto { get; set; } = string.Empty;
    public DateOnly Data { get; set; }
    public int QuantidadeSimulacoes { get; set; }
    public decimal MediaValorFinal { get; set; }
}