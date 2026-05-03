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

            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                throw new ArgumentException("El nombre de usuario no puede estar vacío.", nameof(nombreUsuario));
            }

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
