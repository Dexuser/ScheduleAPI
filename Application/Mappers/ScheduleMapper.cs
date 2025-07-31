using Application.Dtos;
using ProyectoFinal.Models;

namespace Application.Mappers;

public static class ScheduleMapper
{
    public static Schedule ToEntity(ScheduleCreate scheduleCreate)
    {
        return new Schedule()
        {
            StartTime = scheduleCreate.StartTime,
            EndTime = scheduleCreate.EndTime,
            Description = scheduleCreate.Description,
        };
    }

    public static Schedule ToEntity(ScheduleDto dto)
    {
        return new Schedule()
        {
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Description = dto.Description,
        };
    }

    public static ScheduleDto ToDto(Schedule entity)
    {
        return new ScheduleDto()
        {
            Id = entity.Id,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            Description = entity.Description,
        };
    }

    public static IEnumerable<ScheduleDto> ToDto(IEnumerable<Schedule> entities)
    {
        List<ScheduleDto> dtos = new List<ScheduleDto>();
        foreach (var entity in entities)
        {
            dtos.Add(ToDto(entity));
        }

        return dtos;
    }
}