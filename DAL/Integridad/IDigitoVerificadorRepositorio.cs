using BE;
using System;
using System.Collections.Generic;

namespace DAL.Integridad
{
    public interface IDigitoVerificadorRepositorio
    {
        List<Usuario> ListarUsuariosParaIntegridad();

        string ObtenerDigitoVerificadorVertical(string entidad);

        void ActualizarDigitoVerificadorHorizontal(Guid idUsuario, string valor);

        void GuardarDigitoVerificadorVertical(string entidad, string valor);

        void BloquearUsuariosPorIntegridadExceptoAdmin();

        void DesbloquearUsuariosPorIntegridad();
    }
}