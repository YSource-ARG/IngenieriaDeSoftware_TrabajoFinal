using BE;
using System;
using System.Collections.Generic;

namespace SSL.Interfaces
{
    public interface IBitacoraContingenciaService
    {
        void Guardar(Bitacora bitacora);

        List<Bitacora> LeerPendientes();

        void EliminarPendiente(Guid id);
    }
}