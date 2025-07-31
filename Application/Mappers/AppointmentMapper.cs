using Application.Dtos;
using ProyectoFinal.Models;

namespace Application.Mappers;

public static class AppointmentMapper
{
    public static Appointment toEntity(AppointmentCreate create)
    {
        return new Appointment()
        {
            ShiftId = create.ShiftId,
            UserId = create.UserId
        };
    } 
}