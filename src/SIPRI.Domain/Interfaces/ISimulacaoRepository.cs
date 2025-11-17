using SIPRI.Domain.Entities;
using SIPRI.Domain.Models;

namespace SIPRI.Domain.Interfaces;

public interface ISimulacaoRepository
{
    /// <summary>
    /// Adiciona uma nova simulação ao banco.
    /// </summary>
    Task AddAsync(Simulacao simulacao);

    /// <summary>
    /// Busca o histórico de todas as simulações.
    /// </summary>
    Task<IEnumerable<Simulacao>> GetAllAsync();

    /// <summary>
    /// Busca dados agregados das simulações.
    /// </summary>
    Task<IEnumerable<SimulacaoAgregada>> GetAgregadoPorDiaAsync();
}