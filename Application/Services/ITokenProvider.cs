using ProyectoFinal.Models;

namespace Application.Interfaces;

public interface ITokenProvider
{
    public string Create(User user);
}