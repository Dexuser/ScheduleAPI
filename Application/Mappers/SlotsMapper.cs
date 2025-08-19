using Application.Dtos;
using ProyectoFinal.Models;

namespace Application.Mappers;

public static class SlotsMapper
{
    public static SlotDto ToDto(Slot entity)
    {
        return new SlotDto()
        {
            Id = entity.Id,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            IsTaken = entity.isTaken
        };
    }

    public static IEnumerable<SlotDto> ToDto(IEnumerable<Slot> entities)
    {
        List<SlotDto> dtos = new List<SlotDto>();
        foreach (var entity in entities)
        {
            dtos.Add(ToDto(entity));
        }
        return dtos;
    }
}