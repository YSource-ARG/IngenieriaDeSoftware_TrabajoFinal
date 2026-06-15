using BE;
using System;
using System.Collections.Generic;

namespace DAL.Usuarios
{
    public interface IUsuarioEmailHistorialRepositorio
    {
        void RegistrarCambio(UsuarioEmailHistorial historial);

        List<UsuarioEmailHistorial> ListarPorUsuario(Guid usuarioId);
    }
}