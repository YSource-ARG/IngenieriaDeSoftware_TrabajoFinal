using BE.Permisos;
using BLL.Bitacora;
using DAL.Permisos;
using DAL.Usuarios;
using SSL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Permisos
{
    public class GestionPermisosAppService
    {
        private readonly IPermisoComponenteRepositorio _permisoComponenteRepositorio;
        private readonly IRolComponenteRepositorio _rolComponenteRepositorio;
        private readonly IUsuarioPermisoComponenteRepositorio _usuarioPermisoComponenteRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ISessionService _sessionService;
        private readonly IBitacoraService _bitacoraService;

        public GestionPermisosAppService(
            IPermisoComponenteRepositorio permisoComponenteRepositorio,
            IRolComponenteRepositorio rolComponenteRepositorio,
            IUsuarioPermisoComponenteRepositorio usuarioPermisoComponenteRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            ISessionService sessionService,
            IBitacoraService bitacoraService)
        {
            _permisoComponenteRepositorio = permisoComponenteRepositorio ?? throw new ArgumentNullException(nameof(permisoComponenteRepositorio));
            _rolComponenteRepositorio = rolComponenteRepositorio ?? throw new ArgumentNullException(nameof(rolComponenteRepositorio));
            _usuarioPermisoComponenteRepositorio = usuarioPermisoComponenteRepositorio ?? throw new ArgumentNullException(nameof(usuarioPermisoComponenteRepositorio));
            _usuarioRepositorio = usuarioRepositorio ?? throw new ArgumentNullException(nameof(usuarioRepositorio));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _bitacoraService = bitacoraService ?? throw new ArgumentNullException(nameof(bitacoraService));
        }

        #region Listados y busquedas
        public List<ComponentePermisos> ListarComponentesPermisos()
        {
            return _permisoComponenteRepositorio.Listar();
        }

        public List<Rol> ListarRoles()
        {
            return _permisoComponenteRepositorio.ListarRoles();
        }

        public List<Permiso> ListarPermisos()
        {
            return _permisoComponenteRepositorio.ListarPermisos();
        }

        public ComponentePermisos ObtenerComponentePorId(Guid idComponente)
        {
            if (idComponente == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente no puede estar vacío.", nameof(idComponente));
            }

            return _permisoComponenteRepositorio.ObtenerPorId(idComponente);
        }

        public Rol ObtenerRolPorId(Guid idRol)
        {
            ValidarRolExiste(idRol);

            Rol rol = (Rol)_permisoComponenteRepositorio.ObtenerPorId(idRol);
            rol.Hijos = _rolComponenteRepositorio.ListarHijos(idRol);

            return rol;
        }

        public Permiso ObtenerPermisoPorId(Guid idPermiso)
        {
            ValidarPermisoExiste(idPermiso);

            return (Permiso)_permisoComponenteRepositorio.ObtenerPorId(idPermiso);
        }
        #endregion

        #region Alta, Modificación y Baja
        public void CrearRol(string pNombre, string pCodigo, string pDescripcion, bool pEstado)
        {
            ValidarDatosComponente(Guid.Empty, pNombre, pCodigo, pDescripcion, pEstado);
            ValidarCodigoUnico(pCodigo, null);

            Rol rol = new Rol
            {
                Id = Guid.NewGuid(),
                Nombre = pNombre.Trim(),
                Codigo = pCodigo.Trim(),
                Descripcion = NormalizarDescripcion(pDescripcion),
                Activo = pEstado,
                Tipo = TipoComponentePermisos.Rol
            };

            _permisoComponenteRepositorio.Crear(rol);

            RegistrarBitacora(
                "ROL_CREADO",
                $"Se creó el rol '{rol.Nombre}' con código '{rol.Codigo}'."
            );
        }

        public void ModificarRol(Guid pId, string pNombre, string pCodigo, string pDescripcion, bool pEstado)
        {
            ValidarRolExiste(pId);
            ValidarDatosComponente(pId, pNombre, pCodigo, pDescripcion, pEstado);
            ValidarCodigoUnico(pCodigo, pId);

            Rol rol = new Rol
            {
                Id = pId,
                Nombre = pNombre.Trim(),
                Codigo = pCodigo.Trim(),
                Descripcion = NormalizarDescripcion(pDescripcion),
                Activo = pEstado,
                Tipo = TipoComponentePermisos.Rol
            };

            _permisoComponenteRepositorio.Modificar(rol);

            RegistrarBitacora(
                "ROL_MODIFICADO",
                $"Se modificó el rol '{rol.Nombre}' con código '{rol.Codigo}'."
            );
        }

        public void CrearPermiso(string pNombre, string pCodigo, string pDescripcion, bool pEstado)
        {
            ValidarDatosComponente(Guid.Empty, pNombre, pCodigo, pDescripcion, pEstado);
            ValidarCodigoUnico(pCodigo, null);

            Permiso permiso = new Permiso
            {
                Id = Guid.NewGuid(),
                Nombre = pNombre.Trim(),
                Codigo = pCodigo.Trim(),
                Descripcion = NormalizarDescripcion(pDescripcion),
                Activo = pEstado,
                Tipo = TipoComponentePermisos.Permiso
            };

            _permisoComponenteRepositorio.Crear(permiso);

            RegistrarBitacora(
                "PERMISO_CREADO",
                $"Se creó el permiso '{permiso.Nombre}' con código '{permiso.Codigo}'."
            );
        }

        public void ModificarPermiso(Guid pId, string pNombre, string pCodigo, string pDescripcion, bool pEstado)
        {
            ValidarPermisoExiste(pId);
            ValidarDatosComponente(pId, pNombre, pCodigo, pDescripcion, pEstado);
            ValidarCodigoUnico(pCodigo, pId);

            Permiso permiso = new Permiso
            {
                Id = pId,
                Nombre = pNombre.Trim(),
                Codigo = pCodigo.Trim(),
                Descripcion = NormalizarDescripcion(pDescripcion),
                Activo = pEstado,
                Tipo = TipoComponentePermisos.Permiso
            };

            _permisoComponenteRepositorio.Modificar(permiso);

            RegistrarBitacora(
                "PERMISO_MODIFICADO",
                $"Se modificó el permiso '{permiso.Nombre}' con código '{permiso.Codigo}'."
            );
        }

        public void CambiarEstadoComponente(Guid pId, bool pEstado)
        {
            if (pId == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente no puede estar vacío.", nameof(pId));
            }

            ComponentePermisos componente = _permisoComponenteRepositorio.ObtenerPorId(pId);

            if (componente == null)
            {
                throw new InvalidOperationException("No se encontró el componente indicado.");
            }

            _permisoComponenteRepositorio.CambiarEstado(pId, pEstado);

            RegistrarBitacora(
                pEstado ? "COMPONENTE_ACTIVADO" : "COMPONENTE_DESACTIVADO",
                pEstado
                    ? $"Se activó el componente '{componente.Nombre}'."
                    : $"Se desactivó el componente '{componente.Nombre}'."
            );
        }

        public void EliminarRol(Guid pId)
        {
            ValidarRolExiste(pId);

            Rol rol = (Rol)_permisoComponenteRepositorio.ObtenerPorId(pId);

            _rolComponenteRepositorio.EliminarRelacionesPorComponente(pId);
            _usuarioPermisoComponenteRepositorio.EliminarAsignacionesPorComponente(pId);
            _permisoComponenteRepositorio.Eliminar(pId);

            RegistrarBitacora(
                "ROL_ELIMINADO",
                $"Se eliminó el rol '{rol.Nombre}' con código '{rol.Codigo}'."
            );
        }

        public void EliminarPermiso(Guid pId)
        {
            ValidarPermisoExiste(pId);

            Permiso permiso = (Permiso)_permisoComponenteRepositorio.ObtenerPorId(pId);

            _rolComponenteRepositorio.EliminarRelacionesPorComponente(pId);
            _usuarioPermisoComponenteRepositorio.EliminarAsignacionesPorComponente(pId);
            _permisoComponenteRepositorio.Eliminar(pId);

            RegistrarBitacora(
                "PERMISO_ELIMINADO",
                $"Se eliminó el permiso '{permiso.Nombre}' con código '{permiso.Codigo}'."
            );
        }
        #endregion

        #region Composicion de roles

        public List<ComponentePermisos> ListarHijosDeRol(Guid pId)
        {
            ValidarRolExiste(pId);

            return _rolComponenteRepositorio.ListarHijos(pId);
        }

        public void AgregarComponenteARol(Guid pIdRol, Guid pIdComponente)
        {
            ValidarRolExiste(pIdRol);

            ComponentePermisos componente = ObtenerComponenteRequerido(pIdComponente);

            ValidarNoAutoreferencia(pIdRol, pIdComponente);
            ValidarNoCiclo(pIdRol, pIdComponente);
            ValidarNoDuplicadoEnRol(pIdRol, pIdComponente);

            _rolComponenteRepositorio.Agregar(pIdRol, pIdComponente);

            RegistrarBitacora(
                "COMPONENTE_AGREGADO_A_ROL",
                $"Se agregó el componente '{componente.Nombre}' al rol '{ObtenerNombreComponente(pIdRol)}'."
            );
        }

        public void QuitarComponenteARol(Guid pIdRol, Guid pIdComponente)
        {
            ValidarRolExiste(pIdRol);
            ComponentePermisos componente = ObtenerComponenteRequerido(pIdComponente);

            if (!_rolComponenteRepositorio.ExisteRelacion(pIdRol, pIdComponente))
            {
                throw new InvalidOperationException("El componente indicado no está asociado al rol.");
            }

            _rolComponenteRepositorio.Quitar(pIdRol, pIdComponente);

            RegistrarBitacora(
                "COMPONENTE_QUITADO_DE_ROL",
                $"Se quitó el componente '{componente.Nombre}' del rol '{ObtenerNombreComponente(pIdRol)}'."
            );
        }

        public void ReemplazarComponentesDeRol(Guid pIdRol, List<Guid> pHijosId)
        {
            ValidarRolExiste(pIdRol);

            List<Guid> hijosNormalizados = NormalizarListaIds(pHijosId);
            ValidarConjuntoDeComponentesDeRol(pIdRol, hijosNormalizados);

            _rolComponenteRepositorio.QuitarTodosDeRol(pIdRol);

            foreach (Guid idHijo in hijosNormalizados)
            {
                _rolComponenteRepositorio.Agregar(pIdRol, idHijo);
            }

            RegistrarBitacora(
                "ROL_COMPONENTES_REEMPLAZADOS",
                $"Se reemplazó la composición del rol '{ObtenerNombreComponente(pIdRol)}'."
            );
        }

        #endregion

        #region Asignación a usuarios
        public List<ComponentePermisos> ListarComponentesAsignadosAUsuario(Guid pIdUsuario)
        {
            ValidarUsuarioExiste(pIdUsuario);

            return _usuarioPermisoComponenteRepositorio.ListarPorUsuario(pIdUsuario);
        }

        public void AsignarComponenteAUsuario(Guid pIdUsuario, Guid pIdComponente)
        {
            ValidarUsuarioExiste(pIdUsuario);
            ObtenerComponenteRequerido(pIdComponente);
            ValidarNoDuplicadoAsignacionUsuario(pIdUsuario, pIdComponente);

            _usuarioPermisoComponenteRepositorio.Asignar(
                pIdUsuario,
                pIdComponente,
                _sessionService.HaySesionActiva ? (Guid?)_sessionService.UsuarioIdActual : null
            );

            RegistrarBitacora(
                "COMPONENTE_ASIGNADO_USUARIO",
                $"Se asignó el componente '{ObtenerNombreComponente(pIdComponente)}' al usuario '{ObtenerNombreUsuario(pIdUsuario)}'."
            );
        }

        public void DesasignarComponenteAUsuario(Guid pIdUsuario, Guid pIdComponente)
        {
            ValidarUsuarioExiste(pIdUsuario);
            ObtenerComponenteRequerido(pIdComponente);

            if (!_usuarioPermisoComponenteRepositorio.ExisteAsignacion(pIdUsuario, pIdComponente))
            {
                throw new InvalidOperationException("El componente indicado no está asignado al usuario.");
            }

            _usuarioPermisoComponenteRepositorio.Desasignar(pIdUsuario, pIdComponente);

            RegistrarBitacora(
                "COMPONENTE_DESASIGNADO_USUARIO",
                $"Se desasignó el componente '{ObtenerNombreComponente(pIdComponente)}' del usuario '{ObtenerNombreUsuario(pIdUsuario)}'."
            );
        }

        public void ReemplazarAsignacionesUsuario(Guid pIdUsuario, List<Guid> pHijosId)
        {
            ValidarUsuarioExiste(pIdUsuario);

            List<Guid> componentesNormalizados = NormalizarListaIds(pHijosId);

            foreach (Guid idComponente in componentesNormalizados)
            {
                ObtenerComponenteRequerido(idComponente);
            }

            _usuarioPermisoComponenteRepositorio.DesasignarTodos(pIdUsuario);

            foreach (Guid idComponente in componentesNormalizados)
            {
                _usuarioPermisoComponenteRepositorio.Asignar(
                    pIdUsuario,
                    idComponente,
                    _sessionService.HaySesionActiva ? (Guid?)_sessionService.UsuarioIdActual : null
                );
            }

            RegistrarBitacora(
                "USUARIO_COMPONENTES_REEMPLAZADOS",
                $"Se reemplazaron las asignaciones del usuario '{ObtenerNombreUsuario(pIdUsuario)}'."
            );
        }
        #endregion

        #region Validaciones

        private void ValidarDatosComponente(Guid pId, string pNombre, string pCodigo, string pDescripcion, bool pEstado)
        {
            if (string.IsNullOrWhiteSpace(pNombre))
            {
                throw new ArgumentException("El nombre del componente no puede estar vacío.", nameof(pNombre));
            }

            if (string.IsNullOrWhiteSpace(pCodigo))
            {
                throw new ArgumentException("El código del componente no puede estar vacío.", nameof(pCodigo));
            }

            if (pNombre.Trim().Length > 150)
            {
                throw new ArgumentException("El nombre del componente no puede superar los 150 caracteres.", nameof(pNombre));
            }

            if (pCodigo.Trim().Length > 100)
            {
                throw new ArgumentException("El código del componente no puede superar los 100 caracteres.", nameof(pCodigo));
            }

            if (!string.IsNullOrWhiteSpace(pDescripcion) && pDescripcion.Trim().Length > 300)
            {
                throw new ArgumentException("La descripción del componente no puede superar los 300 caracteres.", nameof(pDescripcion));
            }
        }

        private void ValidarCodigoUnico(string codigo, Guid? idExcluido)
        {
            if (_permisoComponenteRepositorio.ExisteCodigo(codigo.Trim(), idExcluido))
            {
                throw new InvalidOperationException("Ya existe un componente con el código indicado.");
            }
        }

        private void ValidarRolExiste(Guid idRol)
        {
            if (idRol == Guid.Empty)
            {
                throw new ArgumentException("El identificador del rol no puede estar vacío.", nameof(idRol));
            }

            ComponentePermisos componente = _permisoComponenteRepositorio.ObtenerPorId(idRol);

            if (componente == null || componente.Tipo != TipoComponentePermisos.Rol)
            {
                throw new InvalidOperationException("No se encontró el rol indicado.");
            }
        }

        private void ValidarPermisoExiste(Guid idPermiso)
        {
            if (idPermiso == Guid.Empty)
            {
                throw new ArgumentException("El identificador del permiso no puede estar vacío.", nameof(idPermiso));
            }

            ComponentePermisos componente = _permisoComponenteRepositorio.ObtenerPorId(idPermiso);

            if (componente == null || componente.Tipo != TipoComponentePermisos.Permiso)
            {
                throw new InvalidOperationException("No se encontró el permiso indicado.");
            }
        }

        private void ValidarNoAutoreferencia(Guid idRol, Guid idComponenteHijo)
        {
            if (idRol == idComponenteHijo)
            {
                throw new InvalidOperationException("Un rol no puede contenerse a sí mismo.");
            }
        }

        private void ValidarNoCiclo(Guid idRol, Guid idComponenteHijo)
        {
            ComponentePermisos componente = ObtenerComponenteRequerido(idComponenteHijo);

            if (componente.Tipo != TipoComponentePermisos.Rol)
            {
                return;
            }

            if (ExisteCaminoEntreRoles(idComponenteHijo, idRol, new HashSet<Guid>()))
            {
                throw new InvalidOperationException("La operación genera un ciclo en la jerarquía de roles.");
            }
        }

        private void ValidarNoDuplicadoEnRol(Guid idRol, Guid idComponenteHijo)
        {
            if (_rolComponenteRepositorio.ExisteRelacion(idRol, idComponenteHijo))
            {
                throw new InvalidOperationException("El componente ya está asociado directamente al rol.");
            }

            HashSet<string> permisosActuales = ObtenerCodigosPermisosDeRol(idRol);
            HashSet<string> permisosNuevoComponente = ObtenerCodigosPermisosDeComponente(idComponenteHijo, new HashSet<Guid>());

            if (permisosActuales.Overlaps(permisosNuevoComponente))
            {
                throw new InvalidOperationException("El componente agrega permisos que el rol ya posee, directa o indirectamente.");
            }
        }

        private void ValidarNoDuplicadoAsignacionUsuario(Guid idUsuario, Guid idComponente)
        {
            if (_usuarioPermisoComponenteRepositorio.ExisteAsignacion(idUsuario, idComponente))
            {
                throw new InvalidOperationException("El componente ya está asignado al usuario.");
            }
        }

        private void RegistrarBitacora(string accion, string descripcion)
        {
            _bitacoraService.Registrar(
                _sessionService.HaySesionActiva ? (Guid?)_sessionService.UsuarioIdActual : null,
                _sessionService.NombreUsuarioActual,
                "Permisos",
                accion,
                descripcion,
                "INFO"
            );
        }

        #endregion

        private void ValidarUsuarioExiste(Guid idUsuario)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            if (_usuarioRepositorio.ObtenerPorId(idUsuario) == null)
            {
                throw new InvalidOperationException("No se encontró el usuario indicado.");
            }
        }

        private ComponentePermisos ObtenerComponenteRequerido(Guid idComponente)
        {
            if (idComponente == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente no puede estar vacío.", nameof(idComponente));
            }

            ComponentePermisos componente = _permisoComponenteRepositorio.ObtenerPorId(idComponente);

            if (componente == null)
            {
                throw new InvalidOperationException("No se encontró el componente indicado.");
            }

            return componente;
        }

        private string ObtenerNombreComponente(Guid idComponente)
        {
            return ObtenerComponenteRequerido(idComponente).Nombre;
        }

        private string ObtenerNombreUsuario(Guid idUsuario)
        {
            return _usuarioRepositorio.ObtenerPorId(idUsuario).NombreUsuario;
        }

        private string NormalizarDescripcion(string descripcion)
        {
            return string.IsNullOrWhiteSpace(descripcion)
                ? null
                : descripcion.Trim();
        }

        private List<Guid> NormalizarListaIds(List<Guid> ids)
        {
            if (ids == null)
            {
                return new List<Guid>();
            }

            List<Guid> idsNormalizados = ids
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            return idsNormalizados;
        }

        private void ValidarConjuntoDeComponentesDeRol(Guid idRol, List<Guid> idsHijos)
        {
            HashSet<string> permisosAcumulados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (Guid idHijo in idsHijos)
            {
                ObtenerComponenteRequerido(idHijo);
                ValidarNoAutoreferencia(idRol, idHijo);
                ValidarNoCiclo(idRol, idHijo);

                HashSet<string> permisosDelHijo = ObtenerCodigosPermisosDeComponente(idHijo, new HashSet<Guid>());

                if (permisosAcumulados.Overlaps(permisosDelHijo))
                {
                    throw new InvalidOperationException("La composición del rol contiene permisos duplicados, directos o indirectos.");
                }

                permisosAcumulados.UnionWith(permisosDelHijo);
            }
        }

        private bool ExisteCaminoEntreRoles(Guid idRolOrigen, Guid idRolBuscado, HashSet<Guid> rolesVisitados)
        {
            if (!rolesVisitados.Add(idRolOrigen))
            {
                return false;
            }

            List<ComponentePermisos> hijos = _rolComponenteRepositorio.ListarHijos(idRolOrigen);

            foreach (ComponentePermisos hijo in hijos.Where(x => x.Tipo == TipoComponentePermisos.Rol))
            {
                if (hijo.Id == idRolBuscado)
                {
                    return true;
                }

                if (ExisteCaminoEntreRoles(hijo.Id, idRolBuscado, rolesVisitados))
                {
                    return true;
                }
            }

            return false;
        }

        private HashSet<string> ObtenerCodigosPermisosDeRol(Guid idRol)
        {
            HashSet<string> permisos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (ComponentePermisos hijo in _rolComponenteRepositorio.ListarHijos(idRol))
            {
                permisos.UnionWith(ObtenerCodigosPermisosDeComponente(hijo.Id, new HashSet<Guid>()));
            }

            return permisos;
        }

        private HashSet<string> ObtenerCodigosPermisosDeComponente(Guid idComponente, HashSet<Guid> rolesVisitados)
        {
            ComponentePermisos componente = ObtenerComponenteRequerido(idComponente);

            if (componente.Tipo == TipoComponentePermisos.Permiso)
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    componente.Codigo
                };
            }

            if (!rolesVisitados.Add(componente.Id))
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            HashSet<string> permisos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (ComponentePermisos hijo in _rolComponenteRepositorio.ListarHijos(componente.Id))
            {
                permisos.UnionWith(ObtenerCodigosPermisosDeComponente(hijo.Id, rolesVisitados));
            }

            return permisos;
        }
    }
}
