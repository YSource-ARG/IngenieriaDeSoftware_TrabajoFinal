using BE.Permisos;
using DAL.Permisos;
using DAL.Usuarios;
using SSL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Permisos
{
    public class AutorizacionService
    {
        private readonly IPermisoComponenteRepositorio _permisoComponenteRepositorio;
        private readonly IRolComponenteRepositorio _rolComponenteRepositorio;
        private readonly IUsuarioPermisoComponenteRepositorio _usuarioPermisoComponenteRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ISessionService _sessionService;

        public AutorizacionService(
            IPermisoComponenteRepositorio permisoComponenteRepositorio,
            IRolComponenteRepositorio rolComponenteRepositorio,
            IUsuarioPermisoComponenteRepositorio usuarioPermisoComponenteRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            ISessionService sessionService)
        {
            _permisoComponenteRepositorio = permisoComponenteRepositorio ?? throw new ArgumentNullException(nameof(permisoComponenteRepositorio));
            _rolComponenteRepositorio = rolComponenteRepositorio ?? throw new ArgumentNullException(nameof(rolComponenteRepositorio));
            _usuarioPermisoComponenteRepositorio = usuarioPermisoComponenteRepositorio ?? throw new ArgumentNullException(nameof(usuarioPermisoComponenteRepositorio));
            _usuarioRepositorio = usuarioRepositorio ?? throw new ArgumentNullException(nameof(usuarioRepositorio));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        public bool UsuarioActualTienePermiso(string codigoPermiso)
        {
            if (!_sessionService.HaySesionActiva)
            {
                return false;
            }

            return UsuarioTienePermiso(_sessionService.UsuarioIdActual, codigoPermiso);
        }

        public bool UsuarioTienePermiso(Guid idUsuario, string codigoPermiso)
        {
            ValidarCodigoPermiso(codigoPermiso);
            ValidarUsuarioExiste(idUsuario);

            List<Permiso> permisos = ListarPermisosEfectivosUsuario(idUsuario);

            return ContieneCodigoPermiso(permisos, codigoPermiso);
        }

        public List<Permiso> ListarPermisosEfectivosUsuarioActual()
        {
            if (!_sessionService.HaySesionActiva)
            {
                return new List<Permiso>();
            }

            return ListarPermisosEfectivosUsuario(_sessionService.UsuarioIdActual);
        }

        public List<Permiso> ListarPermisosEfectivosUsuario(Guid idUsuario)
        {
            ValidarUsuarioExiste(idUsuario);

            List<ComponentePermisos> componentesDirectos = ObtenerComponentesDirectosUsuario(idUsuario);
            List<ComponentePermisos> componentesExpandidos = ExpandirJerarquiaComponentes(componentesDirectos);

            return FiltrarPermisos(componentesExpandidos);
        }

        #region Integración con sesión

        public void RefrescarContextoUsuarioActual()
        {
            if (!_sessionService.HaySesionActiva)
            {
                throw new InvalidOperationException("No hay una sesión activa para refrescar la autorización.");
            }

            ListarPermisosEfectivosUsuarioActual();
        }

        #endregion

        #region Validaciones

        private List<ComponentePermisos> ObtenerComponentesDirectosUsuario(Guid idUsuario)
        {
            return _usuarioPermisoComponenteRepositorio.ListarPorUsuario(idUsuario);
        }

        private List<ComponentePermisos> ExpandirJerarquiaComponentes(IEnumerable<ComponentePermisos> componentesBase)
        {
            List<ComponentePermisos> resultado = new List<ComponentePermisos>();
            HashSet<Guid> componentesVisitados = new HashSet<Guid>();
            HashSet<Guid> rolesExpandidos = new HashSet<Guid>();

            if (componentesBase == null)
            {
                return resultado;
            }

            foreach (ComponentePermisos componente in componentesBase)
            {
                ExpandirComponente(
                    componente,
                    resultado,
                    componentesVisitados,
                    rolesExpandidos
                );
            }

            return resultado;
        }

        private List<Permiso> FiltrarPermisos(IEnumerable<ComponentePermisos> componentes)
        {
            Dictionary<string, Permiso> permisos = new Dictionary<string, Permiso>(StringComparer.OrdinalIgnoreCase);

            if (componentes == null)
            {
                return new List<Permiso>();
            }

            foreach (ComponentePermisos componente in componentes)
            {
                if (componente == null || componente.Tipo != TipoComponentePermisos.Permiso)
                {
                    continue;
                }

                if (!permisos.ContainsKey(componente.Codigo))
                {
                    permisos.Add(
                        componente.Codigo,
                        new Permiso
                        {
                            Id = componente.Id,
                            Nombre = componente.Nombre,
                            Codigo = componente.Codigo,
                            Descripcion = componente.Descripcion,
                            Activo = componente.Activo,
                            Tipo = TipoComponentePermisos.Permiso
                        }
                    );
                }
            }

            return permisos.Values
                .OrderBy(x => x.Nombre)
                .ToList();
        }

        private bool ContieneCodigoPermiso(IEnumerable<Permiso> permisos, string codigoPermiso)
        {
            return permisos.Any(
                x => string.Equals(
                    x.Codigo,
                    codigoPermiso.Trim(),
                    StringComparison.OrdinalIgnoreCase
                )
            );
        }

        private void ValidarCodigoPermiso(string codigoPermiso)
        {
            if (string.IsNullOrWhiteSpace(codigoPermiso))
            {
                throw new ArgumentException("El código del permiso no puede estar vacío.", nameof(codigoPermiso));
            }
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

        private void ExpandirComponente(
            ComponentePermisos componente,
            List<ComponentePermisos> acumulador,
            HashSet<Guid> componentesVisitados,
            HashSet<Guid> rolesExpandidos)
        {
            if (componente == null)
            {
                return;
            }

            ComponentePermisos componenteActual = _permisoComponenteRepositorio.ObtenerPorId(componente.Id) ?? componente;

            if (!componenteActual.Activo)
            {
                return;
            }

            if (componentesVisitados.Add(componenteActual.Id))
            {
                acumulador.Add(componenteActual);
            }

            if (componenteActual.Tipo != TipoComponentePermisos.Rol)
            {
                return;
            }

            if (!rolesExpandidos.Add(componenteActual.Id))
            {
                return;
            }

            List<ComponentePermisos> hijos = _rolComponenteRepositorio.ListarHijos(componenteActual.Id);

            Rol rol = componenteActual as Rol;
            if (rol != null)
            {
                rol.Hijos = hijos;
            }

            foreach (ComponentePermisos hijo in hijos)
            {
                ExpandirComponente(hijo, acumulador, componentesVisitados, rolesExpandidos);
            }
        }
    }
}
