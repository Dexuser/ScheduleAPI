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

    public AppointmentServices(IAppointmentsRepository appointmentsRepository, AppointmentValidator appointmentValidator, ILogger<AppointmentServices> logger, IEmailSender emailSender, IUserRepository userRepository, IShiftRepository shiftRepository)
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
                userWhoRequested, appointment.ShiftId);

            throw new ValidationException("We couldn't create the appointment, someone else booked before you");
        }

        _logger.LogInformation(
            "The user {userWhoRequested} successfully created a new appointment" +
            " to the shift of ID: {appointment.ShiftId}",
            userWhoRequested, appointment.ShiftId);

        var user = await _userRepository.GetUserByUserNameAsync(userWhoRequested);
        var shift = await _shiftRepository.GetShiftByIdAsync(appointment.ShiftId);
   
        
        await _emailSender.SendEmailAsync(
            user.Email,
            $"Now you have a appointment in the shift #{appointment.ShiftId}",
            $"We just booked a appointment to the shift #{appointment.ShiftId} in the date: {shift.Date} at your name, {userWhoRequested}");

        _logger.LogInformation(
            "We just sent a email to the user: {userWhoRequested} about the creation of their appointment",
            userWhoRequested);
        
    }

    public async Task UpdateStateAsync(string userWhoRequested, int userId, AppointmentPatch appointment)
    {
        await _appointmentValidator.ValidateUpdate(userWhoRequested, userId, appointment.ShiftId);

        if (appointment.State.HasValue)
        {
            await _appointmentsRepository.UpdateStateAsync(userId, appointment.ShiftId, appointment.State.Value);

            _logger.LogInformation(
                "The user {userWhoRequested} updated the state of his appointment to {state}. Shift: {appointment.ShiftId}",
                userWhoRequested,
                appointment.State,
                appointment.ShiftId);

            var user = await _userRepository.GetUserByUserNameAsync(userWhoRequested);
            var shift = await _shiftRepository.GetShiftByIdAsync(appointment.ShiftId);
           
            await _emailSender.SendEmailAsync(
                user.Email,
                $"The state of your appointment has changed.",
                $"{userWhoRequested}. Your appointment status has changed to {appointment.State}. Shift ID: {appointment.ShiftId}, date: {shift.Date}");

            _logger.LogInformation(
                "We just sent a email to the user: {userWhoRequested} " +
                "about the Status change of his appointment at the shift {appointment.ShiftId}. ",
                userWhoRequested,
                appointment.ShiftId);
            
        }
    }
}