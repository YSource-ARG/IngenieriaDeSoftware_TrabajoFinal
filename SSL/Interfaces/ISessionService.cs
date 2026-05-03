using System;

namespace SSL.Interfaces
{
    public interface ISessionService
    {
        Guid UsuarioIdActual { get; }
        string NombreUsuarioActual { get; }
        bool HaySesionActiva { get; }
        void IniciarSesion(Guid usuarioId, string nombreUsuario);
        void CerrarSesion();
    }
}
