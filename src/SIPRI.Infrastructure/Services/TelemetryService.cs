using System.Collections.Concurrent;
using SIPRI.Application.DTOs.Telemetria;
using SIPRI.Application.Interfaces;

namespace SIPRI.Infrastructure.Services;

/// <summary>
/// Implementação em memória do serviço de telemetria.
/// </summary>
public class TelemetryService : ITelemetryService
{
    // Armazena os dados: Chave = Nome do Endpoint, Valor = Estatísticas
    private readonly ConcurrentDictionary<string, EndpointStats> _stats = new();
    
    // Data de início da coleta (quando a aplicação subiu)
    private readonly DateOnly _dataInicio;

    public TelemetryService()
    {
        _dataInicio = DateOnly.FromDateTime(DateTime.UtcNow);
    }

    /// <summary>
    /// Método chamado pelo Handler
    /// </summary>
    public Task<TelemetriaDto> GetMetricsAsync()
    {
        var servicosDto = _stats.Select(kvp => new ServicoTelemetriaDto
        {
            Nome = kvp.Key,
            QuantidadeChamadas = kvp.Value.Count,
            // Evita divisão por zero se count for 0
            MediaTempoRespostaMs = kvp.Value.Count == 0 ? 0 : kvp.Value.TotalTimeMs / kvp.Value.Count
        }).ToList();

        var dto = new TelemetriaDto
        {
            Servicos = servicosDto,
            Periodo = new PeriodoTelemetriaDto
            {
                Inicio = _dataInicio,
                Fim = DateOnly.FromDateTime(DateTime.UtcNow)
            }
        };

        return Task.FromResult(dto);
    }

    /// <summary>
    /// Método utilitário para registrar uma nova métrica.
    /// Será chamado pelo Middleware de Telemetria (Presentation).
    /// </summary>
    public void RecordRequest(string endpointName, long elapsedMs)
    {
        // Adiciona ou Atualiza atomicamente
        _stats.AddOrUpdate(
            key: endpointName,
            addValueFactory: _ => new EndpointStats { Count = 1, TotalTimeMs = elapsedMs },
            updateValueFactory: (_, current) =>
            {
                // Incrementa de forma segura (lock-free logic simplificada aqui para o objeto)
                current.Count++;
                current.TotalTimeMs += elapsedMs;
                return current;
            });
    }

    // Classe interna para guardar o estado acumulado
    private class EndpointStats
    {
        public int Count { get; set; }
        public long TotalTimeMs { get; set; }
    }
}