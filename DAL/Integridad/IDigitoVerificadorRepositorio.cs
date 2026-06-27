using BE;
using BE.Permisos;
using System;
using System.Collections.Generic;

namespace DAL.Integridad
{
    public interface IDigitoVerificadorRepositorio
    {
        List<Usuario> ListarUsuariosParaIntegridad();

        List<ComponentePermisos> ListarComponentesPermisosParaIntegridad();

        List<RolComponente> ListarRolComponentesParaIntegridad();

        List<UsuarioPermisoComponente> ListarUsuarioPermisoComponentesParaIntegridad();

        string ObtenerDigitoVerificadorVertical(string entidad);

        void ActualizarDigitoVerificadorHorizontal(
            Guid idUsuario,
            string valor);

        void ActualizarDigitoVerificadorHorizontalComponentePermisos(
            Guid idComponente,
            string valor);

        void ActualizarDigitoVerificadorHorizontalRolComponente(
            Guid idRol,
            Guid idComponenteHijo,
            string valor);

        void ActualizarDigitoVerificadorHorizontalUsuarioPermisoComponente(
            Guid idUsuario,
            Guid idComponente,
            string valor);

        void GuardarDigitoVerificadorVertical(string entidad, string valor);

        void BloquearUsuariosPorIntegridadExceptoAdmin();

        void DesbloquearUsuariosPorIntegridad();
    }
}
