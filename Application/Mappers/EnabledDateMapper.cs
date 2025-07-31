using Application.Dtos;
using ProyectoFinal.Models;

namespace Application.Mappers;

public static class EnabledDateMapper
{
    public static EnabledDate ToEntity(EnableDateCreate dto)
    {
        return new EnabledDate()
        {
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };
    }

    public static EnabledDateDto ToDto(EnabledDate entity)
    {
        return new EnabledDateDto()
        {
            Id = entity.Id,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate
        };
    }

    public static IEnumerable<EnabledDateDto> ToDto(IEnumerable<EnabledDate> entities)
    {
        List<EnabledDateDto> dtos = new List<EnabledDateDto>();
        foreach (var entity in entities)
        {
            dtos.Add(ToDto(entity));
        }

        return dtos;
    }
}