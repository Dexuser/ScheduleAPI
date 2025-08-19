using Application.Dtos;
using ProyectoFinal.Models;

namespace Application.Mappers;

public static class ShiftMapper
{

    public static Shift ToEntity(ShiftCreate shift)
    {
        return new Shift()
        {
            Date = shift.Date,
            ServicesSlots = shift.ServicesSlots,
            MeetingDurationOnMinutes = shift.MeetingDurationOnMinutes,
            ScheduleId = shift.ScheduleId,
        };
    }
    
    public static ShiftDto ToDto(Shift entity)
    {

        var dto = new ShiftDto();
        dto.Id = entity.Id;
        dto.Date = entity.Date;
        dto.ServicesSlots = entity.ServicesSlots;
        dto.MeetingDurationOnMinutes = entity.MeetingDurationOnMinutes;
        dto.Schedule = new ScheduleDto()
        {
            Id = entity.Schedule.Id,
            StartTime = entity.Schedule.StartTime,
            EndTime = entity.Schedule.EndTime,
            Description = entity.Schedule.Description,
        };
        if (entity.Slots != null)
        {
            dto.Slots = SlotsMapper.ToDto(entity.Slots);
        }
        return dto;

    }
    
    public static IEnumerable<ShiftDto> ToDto(IEnumerable<Shift> entities)
    {
        List<ShiftDto> dtos = new List<ShiftDto>();
        foreach (var entity in entities)
        {
            dtos.Add(ToDto(entity));
        }
        return dtos;
    }
}