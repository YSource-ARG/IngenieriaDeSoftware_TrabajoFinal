using System;
using BLL.Bitacora;
using DAL.Usuarios;
using SSL.Interfaces;

namespace BLL.Autenticacion
{
    public class LoginAppService
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISessionService _sessionService;
        private readonly IBitacoraService _bitacoraService;

        public LoginAppService(IUsuarioRepositorio usuarioRepositorio, IPasswordHasher passwordHasher, ISessionService sessionService, IBitacoraService bitacoraService)
        {
            if (usuarioRepositorio == null)
            {
                throw new ArgumentNullException(nameof(usuarioRepositorio));
            }

            if (passwordHasher == null)
            {
                throw new ArgumentNullException(nameof(passwordHasher));
            }

            if (sessionService == null)
            {
                throw new ArgumentNullException(nameof(sessionService));
            }

            if (bitacoraService == null)
            {
                throw new ArgumentNullException(nameof(bitacoraService));
            }

            _usuarioRepositorio = usuarioRepositorio;
            _passwordHasher = passwordHasher;
            _sessionService = sessionService;
            _bitacoraService = bitacoraService;
        }

        public ResultadoLogin IniciarSesion(string nombreUsuario, string passwordIngresada)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(passwordIngresada))
            {
                return ResultadoLogin.Fallido();
            }
            //por aca continua el caso de uso del login invocando el metodo de la DAL
            // y nos devuelve el objeto Usuario con sus datos 
            var usuario = _usuarioRepositorio.ObtenerPorNombreUsuario(nombreUsuario);

            if (usuario == null)//en el caso fallido lo registra en la bitacora
            {
                _bitacoraService.Registrar(
                    null,
                    nombreUsuario,
                    "Seguridad",
                    "LOGIN_FALLIDO",
                    "Intento de login con usuario inexistente.",
                    "WARN"
                );

                return ResultadoLogin.Fallido();
            }
            //en el camino feliz, con los datos del usuario, ahora se puede validar la contraseña
            bool passwordValida = _passwordHasher.VerificarPassword(passwordIngresada, usuario.PasswordHash);

            if (!passwordValida)
            {
                _bitacoraService.Registrar(
                    usuario.Id,
                    usuario.NombreUsuario,
                    "Seguridad",
                    "LOGIN_FALLIDO",
                    "Intento de login con contraseña incorrecta.",
                    "WARN"
                );

                return ResultadoLogin.Fallido();
            }
            //metodo para asignar al usuario como el actual
            _sessionService.IniciarSesion(usuario.Id, usuario.NombreUsuario);
            //metodo para guardar en base de daros los datos del ultimo acceso del usuario
            _usuarioRepositorio.ActualizarFechaUltimoAcceso(usuario.Id);

            _bitacoraService.Registrar(
                usuario.Id,
                usuario.NombreUsuario,
                "Seguridad",
                "LOGIN_EXITOSO",
                "El usuario inició sesión correctamente.",
                "INFO"
            );

            return ResultadoLogin.Exitoso(
                usuario.Id,
                usuario.NombreUsuario,
                usuario.DebeCambiarPassword
            );
        }
    }
}
