using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Application.Mappers;
using Application.Validations;
using Domain.Interfaces;
using ProyectoFinal.Models;

namespace Application.Services;

public class AppointmentServices  
{
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly AppointmentValidator _appointmentValidator;

    public AppointmentServices(IAppointmentsRepository appointmentsRepository, AppointmentValidator appointmentValidator)
    {
        _appointmentsRepository = appointmentsRepository;
        _appointmentValidator = appointmentValidator;
    }


    public async Task CreateAppointmentAsync(AppointmentCreate appointment)
    {
        await _appointmentValidator.ValidateCreate(appointment);
        bool wasBooked = await _appointmentsRepository.CreateAppointmentAsync(AppointmentMapper.toEntity(appointment));
        if (!wasBooked)
        {
            throw new ValidationException("We couldn't create appointment, someone else booked before you");
        }
    }

    public async Task UpdateStateAsync(int userId, AppointmentPatch appointment)
    {
        await _appointmentValidator.ValidateUpdate(userId, appointment.ShiftId);
        if (appointment.State.HasValue)
        {
           await _appointmentsRepository.UpdateStateAsync(appointment.ShiftId,appointment.ShiftId, appointment.State.Value);
        } 
    }
}