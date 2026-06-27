using BE;
using BE.Permisos;
using BLL.Bitacora;
using DAL.Integridad;
using SSL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Integridad
{
    public class IntegridadService : IIntegridadService
    {
        private const string EntidadUsuario = "Usuario";
        private const string EntidadPermisoComponente = "PermisoComponente";
        private const string EntidadRolComponente = "RolComponente";
        private const string EntidadUsuarioPermisoComponente =
            "UsuarioPermisoComponente";

        private readonly IDigitoVerificadorRepositorio
            _digitoVerificadorRepositorio;

        private readonly IDigitoVerificadorService _digitoVerificadorService;
        private readonly IBitacoraService _bitacoraService;

        public IntegridadService(
            IDigitoVerificadorRepositorio digitoVerificadorRepositorio,
            IDigitoVerificadorService digitoVerificadorService,
            IBitacoraService bitacoraService)
        {
            _digitoVerificadorRepositorio =
                digitoVerificadorRepositorio
                ?? throw new ArgumentNullException(
                    nameof(digitoVerificadorRepositorio)
                );

            _digitoVerificadorService =
                digitoVerificadorService
                ?? throw new ArgumentNullException(
                    nameof(digitoVerificadorService)
                );

            _bitacoraService =
                bitacoraService
                ?? throw new ArgumentNullException(nameof(bitacoraService));
        }

        public ResultadoVerificacionIntegridad VerificarIntegridadUsuarios()
        {
            string mensajeFalla = VerificarUsuarios();

            if (mensajeFalla == null)
            {
                mensajeFalla = VerificarComponentesPermisos();
            }

            if (mensajeFalla == null)
            {
                mensajeFalla = VerificarRelacionesDeRoles();
            }

            if (mensajeFalla == null)
            {
                mensajeFalla = VerificarAsignacionesDePermisos();
            }

            return mensajeFalla == null
                ? ResultadoVerificacionIntegridad.Correcta()
                : MarcarIntegridadVulnerada(mensajeFalla);
        }

        public void RecalcularDigitosUsuarios(
            Guid? usuarioId,
            string usuario)
        {
            RecalcularUsuarios();
            RecalcularComponentesPermisos();
            RecalcularRelacionesDeRoles();
            RecalcularAsignacionesDePermisos();

            // El recálculo no restaura información alterada externamente.
            // El administrador acepta el estado actual como nuevo estado íntegro.
            _bitacoraService.Registrar(
                usuarioId,
                usuario,
                "Integridad",
                "DV_RECALCULADO",
                "El administrador recalculó los dígitos verificadores de usuarios, componentes, relaciones de roles y asignaciones de permisos.",
                "INFO"
            );
        }

        public void RecalcularDigitosPermisos()
        {
            // Las operaciones legítimas de permisos actualizan sus DV sin
            // generar un evento adicional: cada caso de uso ya registra
            // su propia acción en la bitácora.
            RecalcularComponentesPermisos();
            RecalcularRelacionesDeRoles();
            RecalcularAsignacionesDePermisos();
        }

        public void DesbloquearUsuariosPorIntegridad(
            Guid? usuarioId,
            string usuario)
        {
            _digitoVerificadorRepositorio
                .DesbloquearUsuariosPorIntegridad();

            _bitacoraService.Registrar(
                usuarioId,
                usuario,
                "Integridad",
                "USUARIOS_DESBLOQUEADOS_INTEGRIDAD",
                "El administrador desbloqueó los usuarios bloqueados por falla de integridad.",
                "INFO"
            );
        }

        private string VerificarUsuarios()
        {
            List<Usuario> usuarios =
                _digitoVerificadorRepositorio
                    .ListarUsuariosParaIntegridad();

            return VerificarConjunto(
                EntidadUsuario,
                usuarios,
                x => x.DigitoVerificadorHorizontal,
                x => _digitoVerificadorService
                    .CalcularDigitoHorizontalUsuario(x),
                x => _digitoVerificadorService
                    .CalcularDigitoVerticalUsuarios(x),
                "Se detectó una modificación externa en los datos de usuarios.",
                "Se detectó una alteración en el conjunto de usuarios."
            );
        }

        private string VerificarComponentesPermisos()
        {
            List<ComponentePermisos> componentes =
                _digitoVerificadorRepositorio
                    .ListarComponentesPermisosParaIntegridad();

            return VerificarConjunto(
                EntidadPermisoComponente,
                componentes,
                x => x.DigitoVerificadorHorizontal,
                x => _digitoVerificadorService
                    .CalcularDigitoHorizontalComponentePermisos(x),
                x => _digitoVerificadorService
                    .CalcularDigitoVerticalComponentesPermisos(x),
                "Se detectó una modificación externa en roles o permisos.",
                "Se detectó una alteración en el conjunto de roles y permisos."
            );
        }

        private string VerificarRelacionesDeRoles()
        {
            List<RolComponente> relaciones =
                _digitoVerificadorRepositorio
                    .ListarRolComponentesParaIntegridad();

            return VerificarConjunto(
                EntidadRolComponente,
                relaciones,
                x => x.DigitoVerificadorHorizontal,
                x => _digitoVerificadorService
                    .CalcularDigitoHorizontalRolComponente(x),
                x => _digitoVerificadorService
                    .CalcularDigitoVerticalRolComponentes(x),
                "Se detectó una modificación externa en la composición de los roles.",
                "Se detectó una alteración en el conjunto de relaciones entre roles y componentes."
            );
        }

        private string VerificarAsignacionesDePermisos()
        {
            List<UsuarioPermisoComponente> asignaciones =
                _digitoVerificadorRepositorio
                    .ListarUsuarioPermisoComponentesParaIntegridad();

            return VerificarConjunto(
                EntidadUsuarioPermisoComponente,
                asignaciones,
                x => x.DigitoVerificadorHorizontal,
                x => _digitoVerificadorService
                    .CalcularDigitoHorizontalUsuarioPermisoComponente(x),
                x => _digitoVerificadorService
                    .CalcularDigitoVerticalUsuarioPermisoComponentes(x),
                "Se detectó una modificación externa en las asignaciones de permisos.",
                "Se detectó una alteración en el conjunto de asignaciones de permisos."
            );
        }

        private string VerificarConjunto<T>(
            string entidad,
            List<T> elementos,
            Func<T, string> obtenerDigitoHorizontal,
            Func<T, string> calcularDigitoHorizontal,
            Func<List<T>, string> calcularDigitoVertical,
            string mensajeModificacion,
            string mensajeAlteracionConjunto)
        {
            string digitoVerticalGuardado =
                _digitoVerificadorRepositorio
                    .ObtenerDigitoVerificadorVertical(entidad);

            bool integridadInicializada =
                !string.IsNullOrWhiteSpace(digitoVerticalGuardado)
                && elementos.All(
                    x => !string.IsNullOrWhiteSpace(
                        obtenerDigitoHorizontal(x)
                    )
                );

            if (!integridadInicializada)
            {
                return
                    $"Los dígitos verificadores de '{entidad}' no se encuentran inicializados.";
            }

            foreach (T elemento in elementos)
            {
                string digitoHorizontalGuardado =
                    obtenerDigitoHorizontal(elemento);

                string digitoHorizontalCalculado =
                    calcularDigitoHorizontal(elemento);

                if (!string.Equals(
                        digitoHorizontalGuardado,
                        digitoHorizontalCalculado,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return mensajeModificacion;
                }
            }

            string digitoVerticalCalculado =
                calcularDigitoVertical(elementos);

            if (!string.Equals(
                    digitoVerticalGuardado,
                    digitoVerticalCalculado,
                    StringComparison.OrdinalIgnoreCase))
            {
                return mensajeAlteracionConjunto;
            }

            return null;
        }

        private void RecalcularUsuarios()
        {
            List<Usuario> usuarios =
                _digitoVerificadorRepositorio
                    .ListarUsuariosParaIntegridad();

            foreach (Usuario usuario in usuarios)
            {
                string digitoHorizontal =
                    _digitoVerificadorService
                        .CalcularDigitoHorizontalUsuario(usuario);

                _digitoVerificadorRepositorio
                    .ActualizarDigitoVerificadorHorizontal(
                        usuario.Id,
                        digitoHorizontal
                    );

                usuario.DigitoVerificadorHorizontal =
                    digitoHorizontal;
            }

            string digitoVertical =
                _digitoVerificadorService
                    .CalcularDigitoVerticalUsuarios(usuarios);

            _digitoVerificadorRepositorio
                .GuardarDigitoVerificadorVertical(
                    EntidadUsuario,
                    digitoVertical
                );
        }

        private void RecalcularComponentesPermisos()
        {
            List<ComponentePermisos> componentes =
                _digitoVerificadorRepositorio
                    .ListarComponentesPermisosParaIntegridad();

            foreach (ComponentePermisos componente in componentes)
            {
                string digitoHorizontal =
                    _digitoVerificadorService
                        .CalcularDigitoHorizontalComponentePermisos(
                            componente
                        );

                _digitoVerificadorRepositorio
                    .ActualizarDigitoVerificadorHorizontalComponentePermisos(
                        componente.Id,
                        digitoHorizontal
                    );

                componente.DigitoVerificadorHorizontal =
                    digitoHorizontal;
            }

            string digitoVertical =
                _digitoVerificadorService
                    .CalcularDigitoVerticalComponentesPermisos(
                        componentes
                    );

            _digitoVerificadorRepositorio
                .GuardarDigitoVerificadorVertical(
                    EntidadPermisoComponente,
                    digitoVertical
                );
        }

        private void RecalcularRelacionesDeRoles()
        {
            List<RolComponente> relaciones =
                _digitoVerificadorRepositorio
                    .ListarRolComponentesParaIntegridad();

            foreach (RolComponente relacion in relaciones)
            {
                string digitoHorizontal =
                    _digitoVerificadorService
                        .CalcularDigitoHorizontalRolComponente(
                            relacion
                        );

                _digitoVerificadorRepositorio
                    .ActualizarDigitoVerificadorHorizontalRolComponente(
                        relacion.RolId,
                        relacion.ComponenteHijoId,
                        digitoHorizontal
                    );

                relacion.DigitoVerificadorHorizontal =
                    digitoHorizontal;
            }

            string digitoVertical =
                _digitoVerificadorService
                    .CalcularDigitoVerticalRolComponentes(relaciones);

            _digitoVerificadorRepositorio
                .GuardarDigitoVerificadorVertical(
                    EntidadRolComponente,
                    digitoVertical
                );
        }

        private void RecalcularAsignacionesDePermisos()
        {
            List<UsuarioPermisoComponente> asignaciones =
                _digitoVerificadorRepositorio
                    .ListarUsuarioPermisoComponentesParaIntegridad();

            foreach (
                UsuarioPermisoComponente asignacion
                in asignaciones)
            {
                string digitoHorizontal =
                    _digitoVerificadorService
                        .CalcularDigitoHorizontalUsuarioPermisoComponente(
                            asignacion
                        );

                _digitoVerificadorRepositorio
                    .ActualizarDigitoVerificadorHorizontalUsuarioPermisoComponente(
                        asignacion.UsuarioId,
                        asignacion.ComponenteId,
                        digitoHorizontal
                    );

                asignacion.DigitoVerificadorHorizontal =
                    digitoHorizontal;
            }

            string digitoVertical =
                _digitoVerificadorService
                    .CalcularDigitoVerticalUsuarioPermisoComponentes(
                        asignaciones
                    );

            _digitoVerificadorRepositorio
                .GuardarDigitoVerificadorVertical(
                    EntidadUsuarioPermisoComponente,
                    digitoVertical
                );
        }

        private ResultadoVerificacionIntegridad
            MarcarIntegridadVulnerada(string mensaje)
        {
            // Se bloquean los usuarios comunes y se conserva el acceso
            // administrativo para poder revisar y recalcular la integridad.
            _digitoVerificadorRepositorio
                .BloquearUsuariosPorIntegridadExceptoAdmin();

            _bitacoraService.Registrar(
                null,
                "Sistema",
                "Integridad",
                "INTEGRIDAD_VULNERADA",
                mensaje,
                "ERROR"
            );

            return ResultadoVerificacionIntegridad
                .Vulnerada(mensaje);
        }
    }
}
