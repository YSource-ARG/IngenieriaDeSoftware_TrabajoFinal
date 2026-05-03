using System;

namespace BLL.Bitacora
{
    public interface IBitacoraService
    {
        void Registrar(Guid? usuarioId, string usuario, string modulo, string accion, string descripcion, string tipo);
    }
}