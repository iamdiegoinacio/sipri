using SIPRI.Application.Interfaces;

namespace SIPRI.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;

    // Retorna a data de hoje com a hora zerada (00:00:00)
    public DateTime Today => DateTime.Today;
}