using System;
using BLL.Bitacora;
using SSL.Interfaces;

namespace BLL.Autenticacion
{
    public class CerrarSesionAppService
    {
        private readonly ISessionService _sessionService;
        private readonly IBitacoraService _bitacoraService;

        public CerrarSesionAppService(ISessionService sessionService, IBitacoraService bitacoraService)
        {
            if (sessionService == null)
            {
                throw new ArgumentNullException(nameof(sessionService));
            }

            if (bitacoraService == null)
            {
                throw new ArgumentNullException(nameof(bitacoraService));
            }

            _sessionService = sessionService;
            _bitacoraService = bitacoraService;
        }

        public void CerrarSesion()
        {
            Guid usuarioId = _sessionService.UsuarioIdActual;
            string nombreUsuario = _sessionService.NombreUsuarioActual;

            if (_sessionService.HaySesionActiva)
            {
                _bitacoraService.Registrar(
                    usuarioId,
                    nombreUsuario,
                    "Seguridad",
                    "LOGOUT",
                    "El usuario cerró sesión correctamente.",
                    "INFO"
                );
            }

            _sessionService.CerrarSesion();
        }
    }
}