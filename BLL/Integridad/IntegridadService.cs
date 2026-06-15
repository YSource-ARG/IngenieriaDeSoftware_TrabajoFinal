using BE;
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

        private readonly IDigitoVerificadorRepositorio _digitoVerificadorRepositorio;
        private readonly IDigitoVerificadorService _digitoVerificadorService;
        private readonly IBitacoraService _bitacoraService;

        public IntegridadService(
            IDigitoVerificadorRepositorio digitoVerificadorRepositorio,
            IDigitoVerificadorService digitoVerificadorService,
            IBitacoraService bitacoraService)
        {
            if (digitoVerificadorRepositorio == null)
            {
                throw new ArgumentNullException(nameof(digitoVerificadorRepositorio));
            }

            if (digitoVerificadorService == null)
            {
                throw new ArgumentNullException(nameof(digitoVerificadorService));
            }

            if (bitacoraService == null)
            {
                throw new ArgumentNullException(nameof(bitacoraService));
            }

            _digitoVerificadorRepositorio = digitoVerificadorRepositorio;
            _digitoVerificadorService = digitoVerificadorService;
            _bitacoraService = bitacoraService;
        }

        public ResultadoVerificacionIntegridad VerificarIntegridadUsuarios()
        {
            List<Usuario> usuarios =
                _digitoVerificadorRepositorio.ListarUsuariosParaIntegridad();

            string digitoVerticalGuardado =
                _digitoVerificadorRepositorio.ObtenerDigitoVerificadorVertical(EntidadUsuario);

            // Si no existe DVV o hay usuarios sin DVH, significa que la integridad
            // todavía no fue inicializada. Para la defensa, se trata igual que una
            // integridad no válida: se bloquean los usuarios comunes y solo el admin
            // puede ingresar para recalcular los dígitos.
            if (string.IsNullOrWhiteSpace(digitoVerticalGuardado)
                || usuarios.Any(x => string.IsNullOrWhiteSpace(x.DigitoVerificadorHorizontal)))
            {
                return MarcarIntegridadVulnerada(
                    "Los dígitos verificadores no se encuentran inicializados."
                );
            }

            foreach (Usuario usuario in usuarios)
            {
                string digitoHorizontalCalculado =
                    _digitoVerificadorService.CalcularDigitoHorizontalUsuario(usuario);

                if (!string.Equals(
                        usuario.DigitoVerificadorHorizontal,
                        digitoHorizontalCalculado,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return MarcarIntegridadVulnerada(
                        "Se detectó una modificación externa en los datos de usuarios."
                    );
                }
            }

            string digitoVerticalCalculado =
                _digitoVerificadorService.CalcularDigitoVerticalUsuarios(usuarios);

            if (!string.Equals(
                    digitoVerticalGuardado,
                    digitoVerticalCalculado,
                    StringComparison.OrdinalIgnoreCase))
            {
                return MarcarIntegridadVulnerada(
                    "Se detectó una alteración en el conjunto de usuarios."
                );
            }

            return ResultadoVerificacionIntegridad.Correcta();
        }

        public void RecalcularDigitosUsuarios(Guid? usuarioId, string usuario)
        {
            List<Usuario> usuarios =
                _digitoVerificadorRepositorio.ListarUsuariosParaIntegridad();

            foreach (Usuario usuarioEntidad in usuarios)
            {
                string digitoHorizontal =
                    _digitoVerificadorService.CalcularDigitoHorizontalUsuario(usuarioEntidad);

                _digitoVerificadorRepositorio.ActualizarDigitoVerificadorHorizontal(
                    usuarioEntidad.Id,
                    digitoHorizontal
                );

                // Se actualiza también el objeto en memoria para calcular luego el DVV
                // con los mismos DVH que acaban de persistirse en la base.
                usuarioEntidad.DigitoVerificadorHorizontal = digitoHorizontal;
            }

            string digitoVertical =
                _digitoVerificadorService.CalcularDigitoVerticalUsuarios(usuarios);

            _digitoVerificadorRepositorio.GuardarDigitoVerificadorVertical(
                EntidadUsuario,
                digitoVertical
            );

            // El recálculo no restaura datos modificados por fuera del sistema.
            // Toma el estado actual validado por el administrador como nuevo estado íntegro.
            _bitacoraService.Registrar(
                usuarioId,
                usuario,
                "Integridad",
                "DV_RECALCULADO",
                "El administrador recalculó los dígitos verificadores de usuarios.",
                "INFO"
            );
        }

        public void DesbloquearUsuariosPorIntegridad(Guid? usuarioId, string usuario)
        {
            _digitoVerificadorRepositorio.DesbloquearUsuariosPorIntegridad();

            _bitacoraService.Registrar(
                usuarioId,
                usuario,
                "Integridad",
                "USUARIOS_DESBLOQUEADOS_INTEGRIDAD",
                "El administrador desbloqueó los usuarios bloqueados por falla de integridad.",
                "INFO"
            );
        }

        private ResultadoVerificacionIntegridad MarcarIntegridadVulnerada(string mensaje)
        {
            // Ante una falla de integridad se bloquean los usuarios comunes,
            // pero se mantiene habilitado el admin para poder recalcular los DV
            // y recuperar el acceso operativo al sistema.
            _digitoVerificadorRepositorio.BloquearUsuariosPorIntegridadExceptoAdmin();

            _bitacoraService.Registrar(
                null,
                "Sistema",
                "Integridad",
                "INTEGRIDAD_VULNERADA",
                mensaje,
                "ERROR"
            );

            return ResultadoVerificacionIntegridad.Vulnerada(mensaje);
        }
    }
}