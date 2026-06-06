using System;
using SSL.Interfaces;

namespace SSL.Sesion
{
    public sealed class SessionService : ISessionService
    {
        private static SessionService _instance;
        private static readonly object _lock = new object();

        public Guid UsuarioIdActual { get; private set; }

        public string NombreUsuarioActual { get; private set; }

        public bool HaySesionActiva => UsuarioIdActual != Guid.Empty;

        private SessionService()
        {
        }
        /// <summary>
        /// Singleton para obtener la instancia unica de sesion 
        /// </summary>
        /// <returns></returns>
        public static SessionService ObtenerSesion()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SessionService();
                    }
                }
            }

            return _instance;
        }

        public void IniciarSesion(Guid usuarioId, string nombreUsuario)
        {
            if (usuarioId == Guid.Empty)
            {
                throw new ArgumentException("El ID del usuario no es válido.", nameof(usuarioId));
            }
            // Validación defensiva: la UI ya valida este dato,
            // pero se protege el servicio ante llamadas inválidas desde otros puntos del sistema.
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                throw new ArgumentException("El nombre de usuario no puede estar vacío.", nameof(nombreUsuario));
            }
            // aca asigna al usuario como el actual
            UsuarioIdActual = usuarioId;
            NombreUsuarioActual = nombreUsuario;
        }

        public void CerrarSesion()
        {
            UsuarioIdActual = Guid.Empty;
            NombreUsuarioActual = null;
        }
    }
}
