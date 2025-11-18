using SIPRI.Domain.Entities;

namespace SIPRI.Domain.Interfaces;
public interface IInvestimentoRepository
{
    /// <summary>
    /// Busca o histórico de investimentos de um cliente.
    /// </summary>
    Task<IEnumerable<Investimento>> GetByClienteIdAsync(Guid clienteId);
}