using ProyectoFinal.Models;

namespace Application.Services;

public interface ITokenProvider
{
    public string Create(User user);
}