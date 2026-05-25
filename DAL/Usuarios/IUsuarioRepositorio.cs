using BE;
using System;
using System.Collections.Generic;

namespace DAL.Usuarios
{
    public interface IUsuarioRepositorio
    {
        Usuario ObtenerPorNombreUsuario(string nombreUsuario);

        Usuario ObtenerPorId(Guid idUsuario);

        List<Usuario> Listar(string textoBusqueda, bool? activo);

        bool ExisteNombreUsuario(string nombreUsuario, Guid? idUsuarioExcluido);

        void Crear(Usuario usuario);

        void ModificarDatos(Usuario usuario);

        void CambiarEstado(Guid idUsuario, bool activo);

        void ActualizarPassword(Guid idUsuario, string passwordHash);

        void ActualizarFechaUltimoAcceso(Guid idUsuario);
    }
}