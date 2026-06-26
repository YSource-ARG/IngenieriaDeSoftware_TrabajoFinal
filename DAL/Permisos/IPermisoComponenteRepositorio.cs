using System;
using System.Collections.Generic;
using BE.Permisos;

namespace DAL.Permisos
{
    public interface IPermisoComponenteRepositorio
    {
        List<ComponentePermisos> Listar();

        List<Rol> ListarRoles();

        List<Permiso> ListarPermisos();

        ComponentePermisos ObtenerPorId(Guid idComponente);

        ComponentePermisos ObtenerPorCodigo(string codigo);

        bool ExisteCodigo(string codigo, Guid? idComponenteExcluido);

        void Crear(ComponentePermisos componente);

        void Modificar(ComponentePermisos componente);

        void CambiarEstado(Guid idComponente, bool activo);

        void Eliminar(Guid idComponente);
    }
}
