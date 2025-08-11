using ProyectoFinal.Models;

namespace Domain.Interfaces;

public interface IScheduleRepository
{
    Task<IEnumerable<Schedule>> GetAllSchedulesAsync();
    Task CreateScheduleAsync(Schedule schedule);
    Task DeleteScheduleAsync(int id);
    Task UpdateScheduleAsync(int id, Schedule schedule);
    Task<Schedule?> FindByIdAsync(int id);
    Task<bool> ExistsAsync(int id);

    Task<Schedule?> FindByStartTimeAndEndTime(TimeOnly startTime, TimeOnly endTime);
}