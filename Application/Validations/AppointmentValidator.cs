using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Domain.Interfaces;
using ProyectoFinal.Models;

namespace Application.Validations;

public class AppointmentValidator
{
    private readonly IAppointmentsRepository _appointmentsRepository;
    private readonly IShiftRepository _shiftsRepository;

    public AppointmentValidator(IAppointmentsRepository appointmentsRepository, IShiftRepository shiftsRepository)
    {
        _appointmentsRepository = appointmentsRepository;
        _shiftsRepository = shiftsRepository;
    }

    private async Task ThatShiftExists(int shiftId)
    {
        if (! await _shiftsRepository.ThatShiftExists(shiftId))
        {
            throw new ApplicationException("That shift doesn't exist");
        }
    }

    public async Task ValidateCreate(AppointmentCreate create)
    {
       await this.ThatShiftExists(create.ShiftId);
        
        bool haveAnotherAppointment =
            _appointmentsRepository.UserHaveAnotherAppointmentsOnThatDay(create.UserId, create.ShiftId);
        if (haveAnotherAppointment)
        {
            throw new ValidationException("User has another appointment on that same day");    
        }
    }

    public async Task ValidateUpdate(int userId, int shiftId )
    {
        // La idea es validar que quien quiere modificar la cita sea el usuario dueño de esa cita.
        // Teniendo en cuenta que un usuario solamente puede tener una cita por dia, basta con que
        // se recupere el unico registro que tiene el mismo usuario y el mismo Turno. para verificar que es del usuario.
        
        // en pocas palabras, el unico turno que tenga mi ID y este Turno, es el unico appointment que tengo y que 
        // puedo modificar su estado
        var appointment = await _appointmentsRepository.GetAppointmentByIdAndShiftIdAsync(userId, shiftId);
        if (appointment == null)
        {
            throw new ApplicationException("That appointment doesn't exist");
        }
        
        
    }
    
}