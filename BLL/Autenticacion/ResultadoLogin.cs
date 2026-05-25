using System;

namespace BLL.Autenticacion
{
    public class ResultadoLogin
    {
        private ResultadoLogin(bool loginExitoso, Guid usuarioId, string nombreUsuario, bool debeCambiarPassword)
        {
            LoginExitoso = loginExitoso;
            UsuarioId = usuarioId;
            NombreUsuario = nombreUsuario;
            DebeCambiarPassword = debeCambiarPassword;
        }

        public bool LoginExitoso { get; private set; }

        public Guid UsuarioId { get; private set; }

        public string NombreUsuario { get; private set; }

        public bool DebeCambiarPassword { get; private set; }

        public static ResultadoLogin Fallido()
        {
            return new ResultadoLogin(false, Guid.Empty, null, false);
        }

        public static ResultadoLogin Exitoso(Guid usuarioId, string nombreUsuario, bool debeCambiarPassword)
        {
            return new ResultadoLogin(true, usuarioId, nombreUsuario, debeCambiarPassword);
        }
    }
}
