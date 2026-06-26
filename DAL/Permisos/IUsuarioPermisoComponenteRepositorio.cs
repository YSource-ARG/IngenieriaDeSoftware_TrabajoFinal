using System;
using System.Collections.Generic;
using BE.Permisos;

namespace DAL.Permisos
{
    public interface IUsuarioPermisoComponenteRepositorio
    {
        List<ComponentePermisos> ListarPorUsuario(Guid idUsuario);

        List<Guid> ListarIdsComponentesPorUsuario(Guid idUsuario);

        bool ExisteAsignacion(Guid idUsuario, Guid idComponente);

        void Asignar(Guid idUsuario, Guid idComponente, Guid? asignadoPorUsuarioId);

        void Desasignar(Guid idUsuario, Guid idComponente);

        void DesasignarTodos(Guid idUsuario);

        bool TieneAsignaciones(Guid idUsuario);

        bool ComponenteAsignadoAAlgunUsuario(Guid idComponente);

        void EliminarAsignacionesPorComponente(Guid idComponente);
    }
}
