using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

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

    private async Task ThatSlotExits(string userWhoRequested, int slotId)
    {
        if (!await _shiftsRepository.ThatSlotExists(slotId))
        {
            _logger.LogError(
                "The user {userWhoRequested}, failed to create / update a appointment (That slot doesn't exist)",
                userWhoRequested);
            throw new ValidationException("That slot doesn't exist");
        }
    }

    public async Task ValidateCreateAsync(string userWhoRequested, AppointmentCreate create)
    {
        await this.ThatSlotExits(userWhoRequested, create.SlotId);
        
        if (await _shiftsRepository.ThatSlotIsTaken(create.SlotId))
        {
            _logger.LogError(
                "The user {userWhoRequested}, failed to create a appointment (that slot has already been taken)",
                userWhoRequested);

            throw new ValidationException("that slot has already been taken ");

        }

        bool haveAnotherAppointment =
            _appointmentsRepository.UserHaveAnotherAppointmentsOnThatDay(create.UserId, create.SlotId );
        if (haveAnotherAppointment)
        {
            _logger.LogError(
                "The user {userWhoRequested}, failed to create a appointment (User has another appointment on that day)",
                userWhoRequested);

            throw new ValidationException("User has another appointment on that same day");
        }
   }

    public async Task ValidateUpdate(string userWhoRequested, int userId, int slotid)
    {
        // Nota, aquí validamos solamente que exista dicho appointment
        await this.ThatSlotExits(userWhoRequested, slotid);

        // Aquí validamos que ese appointment sea de ese usuario.
        // Teniendo en cuenta que un usuario solamente puede tener un appointment por día, basta con
        // encontrar la unica cita en donde coincida su UserId y el ShiftID a la que está suscrito su appointment.

        var appointment = await _appointmentsRepository.GetThatAppointmentByIdAndSlotId(userId, slotid);
        if (appointment == null)
        {
            _logger.LogError(
                "The user {userWhoRequested}, failed to update the appointment of ID: {shiftId} " +
                "(That appointment doesn't exist or the user isn't the owner of that appointment)",
                userWhoRequested,
                slotid);

            throw new ValidationException("That appointment doesn't exist");
        }
    }
}