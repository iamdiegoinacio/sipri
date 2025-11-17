namespace SIPRI.Domain.Interfaces
{
    /// <summary>
    /// Define o padrão Unit of Work para garantir a atomicidade
    /// </summary>
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}