using BE;
using System;
using System.Collections.Generic;

namespace BLL.Bitacora
{
    public interface IBitacoraService
    {
        void Registrar(Guid? usuarioId, string usuario, string modulo, string accion, string descripcion, string tipo);

        List<BE.Bitacora> Listar(string modulo, DateTime? fechaDesde, DateTime? fechaHasta, int cantidadMaxima);
    }
}
