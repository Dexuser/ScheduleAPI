using ProyectoFinal.Models;

namespace Domain.Interfaces;

public interface IEnabledDateRepository
{
    Task<IEnumerable<EnabledDate>> GetEnabledDatesAsync();
    Task DeleteEnabledDateAsync(int id);
    Task CreateEnabledDateAsync(EnabledDate enabledDate);
    
    Task<bool> AlreadyExistsThatDateRange(DateOnly starDate, DateOnly endDate);
    Task<EnabledDate?> GetEnabledDateByIdAsync(int id);
}