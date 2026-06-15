using BE;
using System.Collections.Generic;

namespace SSL.Interfaces
{
    public interface IDigitoVerificadorService
    {
        string CalcularDigitoHorizontalUsuario(Usuario usuario);

        string CalcularDigitoVerticalUsuarios(List<Usuario> usuarios);
    }
}