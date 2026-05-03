using BE;
using System;

namespace DAL.Usuarios
{
    public interface IUsuarioRepositorio
    {
        Usuario ObtenerPorNombreUsuario(string nombreUsuario);
        void ActualizarFechaUltimoAcceso(Guid idUsuario);
    }
}