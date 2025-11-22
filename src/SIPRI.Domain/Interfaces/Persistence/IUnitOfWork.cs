namespace SIPRI.Domain.Interfaces.Persistence;

/// <summary>
/// Define o padrão Unit of Work para garantir a atomicidade
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}