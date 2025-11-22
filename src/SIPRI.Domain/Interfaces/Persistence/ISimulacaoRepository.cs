using SIPRI.Domain.Entities;
using SIPRI.Domain.Models;

namespace SIPRI.Domain.Interfaces.Persistence;

public interface ISimulacaoRepository
{
    /// <summary>
    /// Adiciona uma nova simulação ao banco.
    /// </summary>
    Task AddAsync(Simulacao simulacao);

    /// <summary>
    /// Busca simulações específicas de um cliente.
    /// </summary>
    Task<IEnumerable<Simulacao>> GetByClienteIdAsync(Guid clienteId);

    /// <summary>
    /// Busca dados agregados das simulações.
    /// </summary>
    Task<IEnumerable<SimulacaoAgregada>> GetAgregadoPorDiaAsync();
}