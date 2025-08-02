using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using ProyectoFinal.Models;

namespace Application.Validations;

public class AppointmentValidator
{
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IShiftRepository _shiftsRepository;
    private readonly ILogger<AppointmentValidator> _logger;

    public AppointmentValidator(IAppointmentsRepository appointmentsRepository, IShiftRepository shiftsRepository,
        ILogger<AppointmentValidator> logger)
    {
        _appointmentsRepository = appointmentsRepository;
        _shiftsRepository = shiftsRepository;
        _logger = logger;
    }

    private async Task ThatShiftExists(string userWhoRequested, int shiftId)
    {
        if (!await _shiftsRepository.ThatShiftExists(shiftId))
        {
            _logger.LogError(
                "The user {userWhoRequested}, failed to create / update a appointment (That shift doesn't exist)",
                userWhoRequested);
            throw new ValidationException("That Shift doesn't exist");
        }
    }

    public async Task ValidateCreateAsync(string userWhoRequested, AppointmentCreate create)
    {
        await this.ThatShiftExists(userWhoRequested, create.ShiftId);

        bool haveAnotherAppointment =
            _appointmentsRepository.UserHaveAnotherAppointmentsOnThatDay(create.UserId, create.ShiftId);
        if (haveAnotherAppointment)
        {
            _logger.LogError(
                "The user {userWhoRequested}, failed to create a appointment (User has another appointment on that day)",
                userWhoRequested);

            throw new ValidationException("User has another appointment on that same day");
        }

        if (! await _shiftsRepository.ThatShiftStillAcceptsAppointments(create.ShiftId))
        {
            _logger.LogError(
                "The user {userWhoRequested}, failed to create a appointment (That shift have reached the limit of appointments)",
                userWhoRequested);

            throw new ValidationException("That shift have reached the limit of appointments");
        }
    }

    public async Task ValidateUpdate(string userWhoRequested, int userId, int shiftId)
    {
        // Nota, aquí validamos solamente que exista dicho appointment
        await this.ThatShiftExists(userWhoRequested, shiftId);

        // Aquí validamos que ese appointment sea de ese usuario.
        // Teniendo en cuenta que un usuario solamente puede tener un appointment por día, basta con
        // encontrar la unica cita en donde coincida su UserId y el ShiftID a la que está suscrito su appointment.

        var appointment = await _appointmentsRepository.GetAppointmentByIdAndShiftIdAsync(userId, shiftId);
        if (appointment == null)
        {
            _logger.LogError(
                "The user {userWhoRequested}, failed to update the appointment of ID: {shiftId} " +
                "(That appointment doesn't exist or the user isn't the owner of that appointment)",
                userWhoRequested,
                shiftId);

            throw new ValidationException("That appointment doesn't exist");
        }
    }
}