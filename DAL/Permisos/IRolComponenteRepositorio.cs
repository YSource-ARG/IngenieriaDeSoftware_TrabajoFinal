using System;
using System.Collections.Generic;
using BE.Permisos;

namespace DAL.Permisos
{
    public interface IRolComponenteRepositorio
    {
        List<ComponentePermisos> ListarHijos(Guid idRol);

        List<Guid> ListarIdsHijos(Guid idRol);

        List<Guid> ListarRolesPadre(Guid idComponenteHijo);

        bool ExisteRelacion(Guid idRol, Guid idComponenteHijo);

        void Agregar(Guid idRol, Guid idComponenteHijo);

        void Quitar(Guid idRol, Guid idComponenteHijo);

        void QuitarTodosDeRol(Guid idRol);

        bool TieneHijos(Guid idRol);

        bool EstaSiendoUsadoComoHijo(Guid idComponente);

        void EliminarRelacionesPorComponente(Guid idComponente);
    }
}
