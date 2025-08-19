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
    private readonly IEmailSender _emailSender;
    private readonly IUserRepository _userRepository;
    private readonly IShiftRepository _shiftRepository;

    public AppointmentServices( 
        IAppointmentsRepository appointmentsRepository,
        AppointmentValidator appointmentValidator, 
        ILogger<AppointmentServices> logger,
        IEmailSender emailSender,
        IUserRepository userRepository,
        IShiftRepository shiftRepository)
    {
        _appointmentsRepository = appointmentsRepository;
        _appointmentValidator = appointmentValidator;
        _logger = logger;
        _emailSender = emailSender;
        _userRepository = userRepository;
        _shiftRepository = shiftRepository;
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
                userWhoRequested, appointment.SlotId);

            throw new ValidationException("We couldn't create the appointment, someone else booked before you");
        }

        _logger.LogInformation(
            "The user {userWhoRequested} successfully created a new appointment" +
            " to the shift of ID: {appointment.ShiftId}",
            userWhoRequested, appointment.SlotId);

        var user = await _userRepository.GetUserByUserNameAsync(userWhoRequested);
        var slot = await _shiftRepository.GetSlotAndShiftBySlotIdAsync(appointment.SlotId);

        await _emailSender.SendEmailAsync(
            user.Email,
            "Your appointment has been created",
            $"""
             Hello, {user.UserName}!, whe just booked a appointment at your name. Here it's information:
             
             Shift ID: {slot.Shift.Id}.
             Day: {slot.Shift.Date}.
             Between: {slot.StartTime} to {slot.EndTime}.
             """ 
            );

        _logger.LogInformation(
            "We just sent a email to the user: {userWhoRequested} about the creation of their appointment",
            userWhoRequested);
        
    }

    public async Task UpdateStateAsync(string userWhoRequested, int userId, AppointmentPatch appointment)
    {
        await _appointmentValidator.ValidateUpdate(userWhoRequested, userId, appointment.SlotId);

        if (appointment.State.HasValue)
        {
            await _appointmentsRepository.UpdateStateAsync(userId, appointment.SlotId, appointment.State.Value);

            _logger.LogInformation(
                "The user {userWhoRequested} updated the state of his appointment to {state}. Shift: {appointment.ShiftId}",
                userWhoRequested,
                appointment.State,
                appointment.SlotId);

            var user = await _userRepository.GetUserByUserNameAsync(userWhoRequested);
            var slot = await _shiftRepository.GetSlotAndShiftBySlotIdAsync(appointment.SlotId);
           
            
            await _emailSender.SendEmailAsync(
                user.Email,
                "Your appointment has been updated",
                $"""
                 Hello, {user.UserName}!, whe just updated the state of your appointment  Here it's information:

                 Shift ID: {slot.Shift.Id}.
                 Day: {slot.Shift.Date}.
                 Appointment ID: {slot.Appointment.Id}. 
                 Between: {slot.StartTime} to {slot.EndTime}.
                 New state of your appointment:  {appointment.State}: 
                 """ 
            );
            
            _logger.LogInformation(
                "We just sent a email to the user: {userWhoRequested} " +
                "about the Status change of his appointment at the shift {appointment.ShiftId}. ",
                userWhoRequested,
                appointment.SlotId);
            
        }
    }
}