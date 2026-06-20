using BE;
using BLL.Bitacora;
using DAL.Usuarios;
using SSL.Interfaces;
using System;
using System.Collections.Generic;
using BLL.Integridad;

namespace BLL.Usuarios
{
    // Clase encargada de manejar los casos de uso relacionados con usuarios
    // En estos casos: Validaciones; Repositorios; Hasheo; Sesion actual; Bitácora
    public class GestionUsuariosAppService
    {
        private const string PasswordTemporalBlanqueo = "1234";

        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IUsuarioEmailHistorialRepositorio _usuarioEmailHistorialRepositorio;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISessionService _sessionService;
        private readonly IBitacoraService _bitacoraService;
        private readonly IIntegridadService _integridadService;

        public GestionUsuariosAppService(
            IUsuarioRepositorio usuarioRepositorio,
            IUsuarioEmailHistorialRepositorio usuarioEmailHistorialRepositorio,
            IPasswordHasher passwordHasher,
            ISessionService sessionService,
            IBitacoraService bitacoraService,
            IIntegridadService integridadService)
        {
            _usuarioRepositorio = usuarioRepositorio ?? throw new ArgumentNullException(nameof(usuarioRepositorio));
            _usuarioEmailHistorialRepositorio = usuarioEmailHistorialRepositorio ?? throw new ArgumentNullException(nameof(usuarioEmailHistorialRepositorio));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _bitacoraService = bitacoraService ?? throw new ArgumentNullException(nameof(bitacoraService));
            _integridadService = integridadService ?? throw new ArgumentNullException(nameof(integridadService));
        }

        // Se lista usuarios a través de la consulta ubicada en el repositorio de usuarios (DAL) 
        public List<Usuario> ListarUsuarios(string textoBusqueda, bool? activo)
        {
            return _usuarioRepositorio.Listar(textoBusqueda, activo);
        }

        public Usuario ObtenerUsuarioPorId(Guid idUsuario)
        {
            return _usuarioRepositorio.ObtenerPorId(idUsuario);
        }

        public void GuardarIdiomaPreferidoUsuarioActual(Guid idiomaPreferidoId)
        {
            if (idiomaPreferidoId == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del idioma no puede estar vacío.",
                    nameof(idiomaPreferidoId)
                );
            }

            if (!_sessionService.HaySesionActiva)
            {
                throw new InvalidOperationException(
                    "No hay una sesión activa para guardar la preferencia de idioma."
                );
            }

            _usuarioRepositorio.ActualizarIdiomaPreferido(
                _sessionService.UsuarioIdActual,
                idiomaPreferidoId
            );

            RecalcularIntegridadUsuarios();

            RegistrarBitacora(
                "IDIOMA_PREFERIDO_MODIFICADO",
                $"El usuario '{_sessionService.NombreUsuarioActual}' modificó su idioma preferido."
            );
        }

        // Se validan y persisten los datos, se hashea la contraseña y se registra en la bitácora.
        public void CrearUsuario(string nombreUsuario, string nombreCompleto, string email, string passwordInicial, bool activo)
        {
            ValidarDatosAltaUsuario(nombreUsuario, nombreCompleto);

            if (string.IsNullOrWhiteSpace(passwordInicial))
            {
                throw new ArgumentException("La contraseña inicial no puede estar vacía.", nameof(passwordInicial));
            }

            string nombreUsuarioNormalizado = nombreUsuario.Trim();

            if (_usuarioRepositorio.ExisteNombreUsuario(nombreUsuarioNormalizado, null))
            {
                throw new InvalidOperationException("Ya existe un usuario con ese nombre.");
            }

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                NombreUsuario = nombreUsuarioNormalizado,
                NombreCompleto = nombreCompleto.Trim(),
                Email = NormalizarEmail(email),
                PasswordHash = _passwordHasher.GenerarHash(passwordInicial),
                Activo = activo,
                DebeCambiarPassword = false,
                FechaCreacion = DateTime.Now,
                FechaUltimoAcceso = null
            };

            _usuarioRepositorio.Crear(usuario);

            RecalcularIntegridadUsuarios();

            // Registro para auditoría
            RegistrarBitacora(
                "USUARIO_CREADO",
                $"Se creó el usuario '{usuario.NombreUsuario}'."
            );
        }

        // Se valida y obtiene usuario, y luego se actualiza sus datos editables.
        public void ModificarUsuario(Guid idUsuario, string nombreCompleto, string email, bool activo)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            ValidarNombreCompleto(nombreCompleto);

            Usuario usuario = _usuarioRepositorio.ObtenerPorId(idUsuario);

            if (usuario == null)
            {
                throw new InvalidOperationException("No se encontró el usuario indicado.");
            }

            string nombreCompletoNormalizado = nombreCompleto.Trim();
            string emailNormalizado = NormalizarEmail(email);

            // Se compara el email anterior contra el nuevo antes de modificar.
            // Si cambió, se registra el historial para cumplir el control de cambios.
            string emailAnteriorNormalizado = NormalizarEmail(usuario.Email);

            bool cambioEmail =
                !string.Equals(
                    emailAnteriorNormalizado,
                    emailNormalizado,
                    StringComparison.OrdinalIgnoreCase
                );
            bool cambioDatosGenerales =
                !string.Equals(
                    usuario.NombreCompleto,
                    nombreCompletoNormalizado,
                    StringComparison.OrdinalIgnoreCase
                )
                    || usuario.Activo != activo;
                    _usuarioRepositorio.ModificarDatos(
                        idUsuario,
                        nombreCompletoNormalizado,
                        emailNormalizado,
                        activo
                );
            if (cambioDatosGenerales || cambioEmail)
            {
                RecalcularIntegridadUsuarios();
            }
            if (cambioEmail)
            {
                UsuarioEmailHistorial historial = new UsuarioEmailHistorial
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuario.Id,
                    EmailAnterior = emailAnteriorNormalizado,
                    EmailNuevo = emailNormalizado,
                    FechaCambio = DateTime.Now,
                    UsuarioCambioId = _sessionService.HaySesionActiva
                        ? (Guid?)_sessionService.UsuarioIdActual
                        : null,
                    UsuarioCambioNombre = _sessionService.NombreUsuarioActual
                };

                _usuarioEmailHistorialRepositorio.RegistrarCambio(historial);

                RegistrarBitacora(
                    "EMAIL_USUARIO_MODIFICADO",
                    $"Se modificó el email del usuario '{usuario.NombreUsuario}'."
                );
            }

            if (cambioDatosGenerales)
            {
                RegistrarBitacora(
                    "USUARIO_MODIFICADO",
                    $"Se modificaron los datos del usuario '{usuario.NombreUsuario}'."
                );
            }
        }

        public void CambiarEstadoUsuario(Guid idUsuario, bool activo)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            Usuario usuario = _usuarioRepositorio.ObtenerPorId(idUsuario);

            if (usuario == null)
            {
                throw new InvalidOperationException("No se encontró el usuario indicado.");
            }

            _usuarioRepositorio.CambiarEstado(idUsuario, activo);

            RecalcularIntegridadUsuarios();

            RegistrarBitacora(
                activo ? "USUARIO_REACTIVADO" : "USUARIO_INHABILITADO",
                activo
                    ? $"Se reactivó el usuario '{usuario.NombreUsuario}'."
                    : $"Se inhabilitó el usuario '{usuario.NombreUsuario}'."
            );
        }

        public string BlanquearPassword(Guid idUsuario)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            Usuario usuario = _usuarioRepositorio.ObtenerPorId(idUsuario);

            if (usuario == null)
            {
                throw new InvalidOperationException("No se encontró el usuario indicado.");
            }

            string passwordTemporalHash = _passwordHasher.GenerarHash(PasswordTemporalBlanqueo);

            _usuarioRepositorio.ActualizarPasswordYEstadoCambioObligatorio(
                idUsuario,
                passwordTemporalHash,
                true
            );

            RecalcularIntegridadUsuarios();

            RegistrarBitacora(
                "PASSWORD_BLANQUEADA",
                $"Se blanqueó la contraseña del usuario '{usuario.NombreUsuario}' y se marcó cambio obligatorio."
            );

            return PasswordTemporalBlanqueo;
        }

        public void ConfirmarCambioPasswordObligatorio(Guid idUsuario, string nuevaPassword, string confirmarPassword)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            if (string.IsNullOrWhiteSpace(nuevaPassword))
            {
                throw new ArgumentException("La nueva contraseña no puede estar vacía.", nameof(nuevaPassword));
            }

            if (nuevaPassword != confirmarPassword)
            {
                throw new InvalidOperationException("La nueva contraseña y su confirmación no coinciden.");
            }

            Usuario usuario = _usuarioRepositorio.ObtenerPorId(idUsuario);

            if (usuario == null)
            {
                throw new InvalidOperationException("No se encontró el usuario indicado.");
            }

            string nuevoPasswordHash = _passwordHasher.GenerarHash(nuevaPassword);

            _usuarioRepositorio.ActualizarPasswordYEstadoCambioObligatorio(
                idUsuario,
                nuevoPasswordHash,
                false
            );

            RecalcularIntegridadUsuarios();

            RegistrarBitacora(
                "PASSWORD_CAMBIADA",
                $"El usuario '{usuario.NombreUsuario}' cambió su contraseña obligatoria."
            );
        }

        public List<UsuarioEmailHistorial> ListarHistorialEmail(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(usuarioId));
            }

            Usuario usuario = _usuarioRepositorio.ObtenerPorId(usuarioId);

            if (usuario == null)
            {
                throw new InvalidOperationException("No se encontró el usuario indicado.");
            }

            return _usuarioEmailHistorialRepositorio.ListarPorUsuario(usuarioId);
        }

        public void RestaurarEmailAnterior(Guid usuarioId, string emailARestaurar)
        {
            if (usuarioId == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(usuarioId));
            }

            Usuario usuario = _usuarioRepositorio.ObtenerPorId(usuarioId);

            if (usuario == null)
            {
                throw new InvalidOperationException("No se encontró el usuario indicado.");
            }

            string emailActualNormalizado = NormalizarEmail(usuario.Email);
            string emailRestauradoNormalizado = NormalizarEmail(emailARestaurar);

            if (string.Equals(
                    emailActualNormalizado,
                    emailRestauradoNormalizado,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Se restaura solo el email.
            // El nombre completo y el estado se conservan como están actualmente.
            _usuarioRepositorio.ModificarDatos(
                usuario.Id,
                usuario.NombreCompleto,
                emailRestauradoNormalizado,
                usuario.Activo
            );

            RecalcularIntegridadUsuarios();

            UsuarioEmailHistorial historial = new UsuarioEmailHistorial
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                EmailAnterior = emailActualNormalizado,
                EmailNuevo = emailRestauradoNormalizado,
                FechaCambio = DateTime.Now,
                UsuarioCambioId = _sessionService.HaySesionActiva
                    ? (Guid?)_sessionService.UsuarioIdActual
                    : null,
                UsuarioCambioNombre = _sessionService.NombreUsuarioActual
            };

            _usuarioEmailHistorialRepositorio.RegistrarCambio(historial);

            RegistrarBitacora(
                "EMAIL_RESTAURADO",
                $"Se restauró el email anterior del usuario '{usuario.NombreUsuario}'."
            );
        }

        private void RecalcularIntegridadUsuarios()
        {
            _integridadService.RecalcularDigitosUsuarios(
                _sessionService.HaySesionActiva ? (Guid?)_sessionService.UsuarioIdActual : null,
                _sessionService.NombreUsuarioActual
            );
        }

        private void ValidarDatosAltaUsuario(string nombreUsuario, string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                throw new ArgumentException("El nombre de usuario no puede estar vacío.", nameof(nombreUsuario));
            }

            ValidarNombreCompleto(nombreCompleto);
        }

        private void ValidarNombreCompleto(string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto))
            {
                throw new ArgumentException("El nombre completo no puede estar vacío.", nameof(nombreCompleto));
            }
        }

        private string NormalizarEmail(string email)
        {
            // El email se permite vacío para no romper usuarios existentes
            // y porque el objetivo del módulo es controlar sus cambios cuando se informe.
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            return email.Trim();
        }

        // Centralización de registro de auditoria de gestión de usuario.
        private void RegistrarBitacora(string accion, string descripcion)
        {
            _bitacoraService.Registrar(
                _sessionService.UsuarioIdActual,
                _sessionService.NombreUsuarioActual,
                "Usuarios",
                accion,
                descripcion,
                "INFO"
            );
        }
    }
}
