using DAL.Bitacora;
using System;
using System.Collections.Generic;

namespace BLL.Bitacora
{
    public class BitacoraService : IBitacoraService
    {
        private const int CantidadMaximaPorDefecto = 200;
        private const int CantidadMaximaPermitida = 1000;

        private readonly IBitacoraRepositorio _bitacoraRepositorio;

        public BitacoraService(IBitacoraRepositorio bitacoraRepositorio)
        {
            if (bitacoraRepositorio == null)
            {
                throw new ArgumentNullException(nameof(bitacoraRepositorio));
            }

            _bitacoraRepositorio = bitacoraRepositorio;
        }

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

            _bitacoraRepositorio.Registrar(bitacora);
        }

        public List<BE.Bitacora> Listar(string modulo, DateTime? fechaDesde, DateTime? fechaHasta, int cantidadMaxima)
        {
            int cantidadNormalizada = NormalizarCantidadMaxima(cantidadMaxima);

            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value > fechaHasta.Value)
            {
                throw new ArgumentException("La fecha desde no puede ser mayor que la fecha hasta.");
            }

            return _bitacoraRepositorio.Listar(
                modulo,
                fechaDesde,
                fechaHasta,
                cantidadNormalizada
            );
        }

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
