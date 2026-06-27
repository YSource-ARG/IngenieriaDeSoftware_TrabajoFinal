using BE.Permisos;
using System;
using System.Collections.Generic;

namespace DAL.Permisos
{
    /// <summary>
    /// Define las operaciones de permisos que deben persistirse como una sola unidad.
    /// La implementación confirma todos los cambios o revierte la operación completa.
    /// </summary>
    public interface IUnidadDeTrabajoPermisos
    {
        void CrearRolConComponentes(
            Rol rol,
            IReadOnlyCollection<Guid> idsComponentesHijos);

        void ReemplazarComponentesDeRol(
            Guid idRol,
            IReadOnlyCollection<Guid> idsComponentesHijos);

        void ReemplazarAsignacionesDeUsuario(
            Guid idUsuario,
            IReadOnlyCollection<Guid> idsComponentes,
            Guid? asignadoPorUsuarioId);

        void EliminarComponente(Guid idComponente);
    }
}
