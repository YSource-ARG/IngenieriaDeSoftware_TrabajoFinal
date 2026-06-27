using System;

namespace BLL.Integridad
{
    public interface IIntegridadService
    {
        ResultadoVerificacionIntegridad VerificarIntegridadUsuarios();

        void RecalcularDigitosUsuarios(Guid? usuarioId, string usuario);

        void RecalcularDigitosPermisos();

        void DesbloquearUsuariosPorIntegridad(Guid? usuarioId, string usuario);
    }
}
