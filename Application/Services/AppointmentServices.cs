using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Application.Mappers;
using Application.Validations;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class AppointmentServices  
{
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly AppointmentValidator _appointmentValidator;
    private readonly ILogger<AppointmentServices> _logger;

    public AppointmentServices(IAppointmentsRepository appointmentsRepository, AppointmentValidator appointmentValidator, ILogger<AppointmentServices> logger)
    {
        _appointmentsRepository = appointmentsRepository;
        _appointmentValidator = appointmentValidator;
        _logger = logger;
    }


    public async Task CreateAppointmentAsync(string userWhoRequested, AppointmentCreate appointment)
    {
        await _appointmentValidator.ValidateCreateAsync(userWhoRequested, appointment);
        bool wasBooked = await _appointmentsRepository.CreateAppointmentAsync(AppointmentMapper.toEntity(appointment));
        if (!wasBooked)
        {
            _logger.LogInformation(
                "The user {userWhoRequested} failed to suscribe appointment " +
                "to the shift of ID: {appointment.ShiftId}",
                userWhoRequested, appointment.ShiftId);
            
            throw new ValidationException("We couldn't create the appointment, someone else booked before you");
        }
        
        _logger.LogInformation(
            "The user {userWhoRequested} successfully created a new appointment" +
            " to the shift of ID: {appointment.ShiftId}",
            userWhoRequested, appointment.ShiftId);
    }

    public async Task UpdateStateAsync(string userWhoRequested,int userId, AppointmentPatch appointment)
    {
        await _appointmentValidator.ValidateUpdate(userWhoRequested, userId, appointment.ShiftId);
        if (appointment.State.HasValue)
        {
           await _appointmentsRepository.UpdateStateAsync(userId,appointment.ShiftId, appointment.State.Value);
           
           _logger.LogInformation(
               "The user {userWhoRequested} updated the state of his appointment to {state}. Shift: {appointment.ShiftId}",
               userWhoRequested,
               appointment.State,
               appointment.ShiftId);
        } 
    }
}