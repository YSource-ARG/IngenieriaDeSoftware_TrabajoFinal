using BE;
using BLL.Bitacora;
using DAL.Usuarios;
using SSL.Interfaces;
using System;
using System.Collections.Generic;

namespace BLL.Usuarios
{
    // Clase encargada de manejar los casos de uso relacionados con usuarios
    // En estos casos: Validaciones; Repositorios; Hasheo; Sesion actual; Bitácora
    public class GestionUsuariosAppService
    {
        private const string PasswordTemporalBlanqueo = "1234";

        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISessionService _sessionService;
        private readonly IBitacoraService _bitacoraService;

        public GestionUsuariosAppService(
            IUsuarioRepositorio usuarioRepositorio,
            IPasswordHasher passwordHasher,
            ISessionService sessionService,
            IBitacoraService bitacoraService)
        {
            _usuarioRepositorio = usuarioRepositorio ?? throw new ArgumentNullException(nameof(usuarioRepositorio));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _bitacoraService = bitacoraService ?? throw new ArgumentNullException(nameof(bitacoraService));
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

        // Se validan y persisten los datos, se hashea la contraseña y se registra en la bitácora.
        public void CrearUsuario(string nombreUsuario, string nombreCompleto, string passwordInicial, bool activo)
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
                PasswordHash = _passwordHasher.GenerarHash(passwordInicial),
                Activo = activo,
                DebeCambiarPassword = false,
                FechaCreacion = DateTime.Now,
                FechaUltimoAcceso = null
            };

            _usuarioRepositorio.Crear(usuario);

            // Registro para auditoría
            RegistrarBitacora(
                "USUARIO_CREADO",
                $"Se creó el usuario '{usuario.NombreUsuario}'."
            );
        }

        // Se valida y obtiene usuario, y luego se actualiza sus datos editables.
        public void ModificarUsuario(Guid idUsuario, string nombreCompleto, bool activo)
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

            _usuarioRepositorio.ModificarDatos(
                idUsuario,
                nombreCompletoNormalizado,
                activo
            );

            RegistrarBitacora(
                "USUARIO_MODIFICADO",
                $"Se modificaron los datos del usuario '{usuario.NombreUsuario}'."
            );
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

            RegistrarBitacora(
                "PASSWORD_CAMBIADA",
                $"El usuario '{usuario.NombreUsuario}' cambió su contraseña obligatoria."
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
