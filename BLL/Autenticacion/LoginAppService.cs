using System;
using BLL.Bitacora;
using DAL.Usuarios;
using DAL.Excepciones;
using SSL.Interfaces;

namespace BLL.Autenticacion
{
    public class LoginAppService
    {
        private const int CantidadMaximaIntentosFallidos = 3;
        private const int MinutosBloqueo = 5;

        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISessionService _sessionService;
        private readonly IBitacoraService _bitacoraService;

        public LoginAppService(
            IUsuarioRepositorio usuarioRepositorio,
            IPasswordHasher passwordHasher,
            ISessionService sessionService,
            IBitacoraService bitacoraService)
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
            try
            {
                if (string.IsNullOrWhiteSpace(nombreUsuario) ||
                    string.IsNullOrWhiteSpace(passwordIngresada))
                {
                    return ResultadoLogin.Fallido();
                }

                var usuario =
                    _usuarioRepositorio.ObtenerPorNombreUsuario(nombreUsuario);

                if (usuario == null)
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

                // Si la integridad fue vulnerada, los usuarios comunes quedan bloqueados.
                // El administrador se mantiene habilitado para poder ingresar,
                // recalcular los dígitos verificadores y desbloquear el acceso.
                if (usuario.BloqueadoPorIntegridad &&
                    !string.Equals(usuario.NombreUsuario, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    _bitacoraService.Registrar(
                        usuario.Id,
                        usuario.NombreUsuario,
                        "Seguridad",
                        "LOGIN_BLOQUEADO_INTEGRIDAD",
                        "Intento de acceso bloqueado por falla de integridad.",
                        "WARN"
                    );

                    return ResultadoLogin.BloqueadoPorFallaIntegridad();
                }

                DateTime fechaActual = DateTime.Now;

                if (usuario.BloqueadoHasta.HasValue &&
                    usuario.BloqueadoHasta.Value > fechaActual)
                {
                    _bitacoraService.Registrar(
                        usuario.Id,
                        usuario.NombreUsuario,
                        "Seguridad",
                        "LOGIN_BLOQUEADO",
                        "Intento de acceso mientras el usuario se encuentra bloqueado.",
                        "WARN"
                    );

                    return ResultadoLogin.Bloqueado(
                        usuario.BloqueadoHasta.Value
                    );
                }

                if (usuario.BloqueadoHasta.HasValue)
                {
                    _usuarioRepositorio.RestablecerIntentosFallidosLogin(
                        usuario.Id
                    );

                    usuario.IntentosFallidosLogin = 0;
                    usuario.BloqueadoHasta = null;
                }

                bool passwordValida =
                    _passwordHasher.VerificarPassword(
                        passwordIngresada,
                        usuario.PasswordHash
                    );

                if (!passwordValida)
                {
                    int intentosFallidos =
                        usuario.IntentosFallidosLogin + 1;

                    DateTime? bloqueadoHasta = null;

                    if (intentosFallidos >= CantidadMaximaIntentosFallidos)
                    {
                        bloqueadoHasta =
                            fechaActual.AddMinutes(MinutosBloqueo);
                    }

                    _usuarioRepositorio.ActualizarIntentosFallidosLogin(
                        usuario.Id,
                        intentosFallidos,
                        bloqueadoHasta
                    );

                    if (bloqueadoHasta.HasValue)
                    {
                        _bitacoraService.Registrar(
                            usuario.Id,
                            usuario.NombreUsuario,
                            "Seguridad",
                            "LOGIN_BLOQUEADO",
                            "El usuario fue bloqueado temporalmente por intentos fallidos.",
                            "WARN"
                        );

                        return ResultadoLogin.Bloqueado(
                            bloqueadoHasta.Value
                        );
                    }

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

                if (usuario.IntentosFallidosLogin > 0 ||
                    usuario.BloqueadoHasta.HasValue)
                {
                    _usuarioRepositorio.RestablecerIntentosFallidosLogin(
                        usuario.Id
                    );
                }

                _sessionService.IniciarSesion(
                    usuario.Id,
                    usuario.NombreUsuario
                );

                _usuarioRepositorio.ActualizarFechaUltimoAcceso(
                    usuario.Id
                );

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
            catch (AccesoDatosException ex)
            {
                _bitacoraService.Registrar(
                    null,
                    nombreUsuario,
                    "Seguridad",
                    "ERROR_BASE_DATOS",
                    "No se pudo acceder a la base de datos durante el inicio de sesión. " + ex.Message,
                    "ERROR"
                );

                return ResultadoLogin.ErrorBaseDatos();
            }
        }
    }
}