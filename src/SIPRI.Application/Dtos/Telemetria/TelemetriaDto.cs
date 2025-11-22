namespace SIPRI.Application.DTOs.Telemetria;

/// <summary>
/// DTO para a resposta da consulta de Telemetria.
/// </summary>
public class TelemetriaDto
{
    public List<ServicoTelemetriaDto> Servicos { get; set; } = [];
    public PeriodoTelemetriaDto Periodo { get; set; } = new();
}

/// <summary>
/// Parte do DTO de Telemetria.
/// </summary>
public class ServicoTelemetriaDto
{
    public string Nome { get; set; } = string.Empty;
    public int QuantidadeChamadas { get; set; }
    public long MediaTempoRespostaMs { get; set; }
}

/// <summary>
/// Parte do DTO de Telemetria.
/// </summary>
public class PeriodoTelemetriaDto
{
    public DateOnly Inicio { get; set; }
    public DateOnly Fim { get; set; }
}
