using DAL.Bitacora;
using DAL.Excepciones;
using SSL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Bitacora
{
    // Service encargado de registrar y consultar eventos del sistema.
    public class BitacoraService : IBitacoraService
    {
        private const int CantidadMaximaPorDefecto = 200;
        private const int CantidadMaximaPermitida = 1000;

        private readonly IBitacoraRepositorio _bitacoraRepositorio;
        private readonly IBitacoraContingenciaService _bitacoraContingenciaService;

        public BitacoraService(IBitacoraRepositorio bitacoraRepositorio)
            : this(bitacoraRepositorio, null)
        {
        }

        public BitacoraService(
            IBitacoraRepositorio bitacoraRepositorio,
            IBitacoraContingenciaService bitacoraContingenciaService)
        {
            if (bitacoraRepositorio == null)
            {
                throw new ArgumentNullException(nameof(bitacoraRepositorio));
            }

            _bitacoraRepositorio = bitacoraRepositorio;
            _bitacoraContingenciaService = bitacoraContingenciaService;
        }

        // Crea un registro en la bitácora con los datos mínimos necesarios para auditar la acción.
        public void Registrar(Guid? usuarioId, string usuario, string modulo, string accion, string descripcion, string tipo)
        {
            var bitacora = new BE.Bitacora
            {
                Id = Guid.NewGuid(),
                Fecha = DateTime.Now,
                UsuarioId = usuarioId,
                Usuario = usuario,
                Modulo = modulo,
                Accion = accion,
                Descripcion = descripcion,
                Tipo = tipo
            };

            IntentarSincronizarPendientes();

            try
            {
                _bitacoraRepositorio.Registrar(bitacora);
            }
            catch (AccesoDatosException)
            {
                GuardarEnContingencia(bitacora);
            }
        }

        // Consulta los registros de la bitácora aplicando filtros de fechas y cantidades máx
        public List<BE.Bitacora> Listar(
            string usuario,
            string accion,
            string modulo,
            string tipo,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int cantidadMaxima)
        {
            int cantidadNormalizada = NormalizarCantidadMaxima(cantidadMaxima);

            if (fechaDesde.HasValue &&
                fechaHasta.HasValue &&
                fechaDesde.Value > fechaHasta.Value)
            {
                throw new ArgumentException(
                    "La fecha desde no puede ser mayor que la fecha hasta."
                );
            }

            return _bitacoraRepositorio.Listar(
                usuario,
                accion,
                modulo,
                tipo,
                fechaDesde,
                fechaHasta,
                cantidadNormalizada
            );
        }
        public List<string> ListarUsuarios()
        {
            return _bitacoraRepositorio.ListarUsuarios();
        }

        public List<string> ListarAcciones()
        {
            return _bitacoraRepositorio.ListarAcciones();
        }

        private void IntentarSincronizarPendientes()
        {
            if (_bitacoraContingenciaService == null)
            {
                return;
            }

            List<BE.Bitacora> pendientes;

            try
            {
                pendientes = _bitacoraContingenciaService.LeerPendientes();
            }
            catch
            {
                return;
            }

            foreach (BE.Bitacora pendiente in pendientes.OrderBy(x => x.Fecha))
            {
                try
                {
                    _bitacoraRepositorio.Registrar(pendiente);
                    _bitacoraContingenciaService.EliminarPendiente(pendiente.Id);
                }
                catch (AccesoDatosException)
                {
                    return;
                }
                catch
                {
                    return;
                }
            }
        }

        private void GuardarEnContingencia(BE.Bitacora bitacora)
        {
            if (_bitacoraContingenciaService == null)
            {
                return;
            }

            try
            {
                _bitacoraContingenciaService.Guardar(bitacora);
            }
            catch
            {
                // Si falla también la contingencia, no se rompe la aplicación.
                // No hay otro medio disponible para registrar el evento.
            }
        }

        // Limita cant maxima de registros devueltos para evitar problemas 
        private int NormalizarCantidadMaxima(int cantidadMaxima)
        {
            if (cantidadMaxima <= 0)
            {
                return CantidadMaximaPorDefecto;
            }

            if (cantidadMaxima > CantidadMaximaPermitida)
            {
                return CantidadMaximaPermitida;
            }

            return cantidadMaxima;
        }
    }
}