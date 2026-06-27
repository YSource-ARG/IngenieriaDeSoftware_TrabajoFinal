using BE;
using BE.Permisos;
using System.Collections.Generic;

namespace SSL.Interfaces
{
    public interface IDigitoVerificadorService
    {
        string CalcularDigitoHorizontalUsuario(Usuario usuario);

        string CalcularDigitoVerticalUsuarios(List<Usuario> usuarios);

        string CalcularDigitoHorizontalComponentePermisos(
            ComponentePermisos componente);

        string CalcularDigitoVerticalComponentesPermisos(
            List<ComponentePermisos> componentes);

        string CalcularDigitoHorizontalRolComponente(
            RolComponente relacion);

        string CalcularDigitoVerticalRolComponentes(
            List<RolComponente> relaciones);

        string CalcularDigitoHorizontalUsuarioPermisoComponente(
            UsuarioPermisoComponente asignacion);

        string CalcularDigitoVerticalUsuarioPermisoComponentes(
            List<UsuarioPermisoComponente> asignaciones);
    }
}
