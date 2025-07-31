using ProyectoFinal.Models;

namespace Domain.Interfaces;

public interface IEnabledDateRepository
{
    Task<IEnumerable<EnabledDate>> GetEnabledDatesAsync();
    Task DeleteEnabledDateAsync(int id);
    Task CreateEnabledDateAsync(EnabledDate enabledDate);
}