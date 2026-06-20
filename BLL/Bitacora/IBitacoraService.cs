using BE;
using System;
using System.Collections.Generic;

namespace BLL.Bitacora
{
    public interface IBitacoraService
    {
        void Registrar(Guid? usuarioId, string usuario, string modulo, string accion, string descripcion, string tipo);

        List<BE.Bitacora> Listar(
            string usuario,
            string accion,
            string modulo,
            string tipo,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            int cantidadMaxima
        );

        List<string> ListarUsuarios();

        List<string> ListarAcciones();
    }
}
