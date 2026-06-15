using System;

namespace BLL.Autenticacion
{
    public class ResultadoLogin
    {
        private ResultadoLogin(
            bool loginExitoso,
            bool usuarioBloqueado,
            DateTime? bloqueadoHasta,
            Guid usuarioId,
            string nombreUsuario,
            bool debeCambiarPassword,
            bool errorAccesoDatos)
        {
            LoginExitoso = loginExitoso;
            UsuarioBloqueado = usuarioBloqueado;
            BloqueadoHasta = bloqueadoHasta;
            UsuarioId = usuarioId;
            NombreUsuario = nombreUsuario;
            DebeCambiarPassword = debeCambiarPassword;
            ErrorAccesoDatos = errorAccesoDatos;
        }

        public bool LoginExitoso { get; private set; }

        public bool UsuarioBloqueado { get; private set; }

        public DateTime? BloqueadoHasta { get; private set; }

        public Guid UsuarioId { get; private set; }

        public string NombreUsuario { get; private set; }

        public bool DebeCambiarPassword { get; private set; }
        public bool ErrorAccesoDatos { get; private set; }

        public static ResultadoLogin Fallido()
        {
            return new ResultadoLogin(
                false,
                false,
                null,
                Guid.Empty,
                null,
                false,
                false
            );
        }
        public static ResultadoLogin ErrorBaseDatos()
        {
            return new ResultadoLogin(
                false,
                false,
                null,
                Guid.Empty,
                null,
                false,
                true
            );
        }
        public static ResultadoLogin Bloqueado(DateTime bloqueadoHasta)
        {
            return new ResultadoLogin(
                false,
                true,
                bloqueadoHasta,
                Guid.Empty,
                null,
                false,
                false
            );
        }

        public static ResultadoLogin Exitoso(
            Guid usuarioId,
            string nombreUsuario,
            bool debeCambiarPassword)
        {
            return new ResultadoLogin(
                true,
                false,
                null,
                usuarioId,
                nombreUsuario,
                debeCambiarPassword,
                false
            );
        }
    }
}