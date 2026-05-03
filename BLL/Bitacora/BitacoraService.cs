using DAL.Bitacora;
using System;

namespace BLL.Bitacora
{
    public class BitacoraService : IBitacoraService
    {
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
    }
}