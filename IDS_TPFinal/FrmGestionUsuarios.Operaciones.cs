using System;
using System.Windows.Forms;

namespace IDS_TPFinal
{
    public partial class FrmGestionUsuarios
    {
        // Inicia el "Alta de usuario" tomando los datos ingresados
        private void CrearUsuario()
        {
            _gestionUsuariosAppService.CrearUsuario(
                txtNombreUsuario.Text,
                txtNombreCompleto.Text,
                txtPassword.Text,
                ObtenerEstadoSeleccionado()
            );

            MessageBox.Show(
                "Usuario creado correctamente.",
                "Gestión de usuarios",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            CargarUsuarios();
        }

        // Inicia la "Modificación de Usuario" en base a los datos ingresados para el usuario seleccionado.
        private void ModificarUsuario()
        {
            Guid idUsuario = ObtenerIdUsuarioSeleccionado();

            _gestionUsuariosAppService.ModificarUsuario(
                idUsuario,
                txtNombreCompleto.Text,
                ObtenerEstadoSeleccionado()
            );

            MessageBox.Show(
                "Usuario modificado correctamente.",
                "Gestión de usuarios",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            CargarUsuarios();
        }

        // Se cambia el estado (Habilitado / Deshabilitado) del usuario seleccionado
        private void CambiarEstadoUsuarioSeleccionado()
        {
            Guid idUsuario = ObtenerIdUsuarioSeleccionado();
            bool usuarioActivo = ObtenerEstadoSeleccionado();
            bool nuevoEstado = !usuarioActivo;

            string accion = nuevoEstado ? "reactivar" : "inhabilitar";
            string nombreUsuario = txtNombreUsuario.Text;

            DialogResult respuesta = MessageBox.Show(
                $"¿Confirmás que querés {accion} el usuario '{nombreUsuario}'?",
                "Confirmar cambio de estado",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta != DialogResult.Yes)
            {
                return;
            }

            _gestionUsuariosAppService.CambiarEstadoUsuario(idUsuario, nuevoEstado);

            MessageBox.Show(
                nuevoEstado
                    ? "Usuario reactivado correctamente."
                    : "Usuario inhabilitado correctamente.",
                "Gestión de usuarios",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            CargarUsuarios();
        }

        // Se blanquea la contraseña del usuario seleccionado para que deba realizar un cambio de la misma.
        private void BlanquearPasswordUsuarioSeleccionado()
        {
            Guid idUsuario = ObtenerIdUsuarioSeleccionado();
            string nombreUsuario = txtNombreUsuario.Text;

            DialogResult respuesta = MessageBox.Show(
                $"¿Confirmás que querés blanquear la contraseña del usuario '{nombreUsuario}'?",
                "Confirmar blanqueo de contraseña",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta != DialogResult.Yes)
            {
                return;
            }

            string passwordTemporal = _gestionUsuariosAppService.BlanquearPassword(idUsuario);

            MessageBox.Show(
                $"Contraseña blanqueada correctamente.\n\nUsuario: {nombreUsuario}\nContraseña temporal: {passwordTemporal}\n\nAl ingresar, el usuario deberá cambiar su contraseña.",
                "Gestión de usuarios",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            CargarUsuarios();
        }
    }
}
