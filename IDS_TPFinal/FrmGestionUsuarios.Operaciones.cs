using System;
using System.Windows.Forms;

namespace IDS_TPFinal
{
    public partial class FrmGestionUsuarios
    {
        // Inicia el "Alta de usuario" tomando los datos ingresados
        private void CrearUsuario()
        {
            if (!ValidarDatosAltaUsuario())
            {
                return;
            }

            _gestionUsuariosAppService.CrearUsuario(
                txtNombreUsuario.Text,
                txtNombreCompleto.Text,
                txtEmail.Text,
                txtPassword.Text,
                ObtenerEstadoSeleccionado()
            );

            MostrarInformacion(
                "Mensajes.Usuarios.UsuarioCreado",
                "Usuario creado correctamente.",
                "Mensajes.Titulos.GestionUsuarios",
                "Gestión de usuarios"
            );

            CargarUsuarios();
        }


        private bool ValidarDatosAltaUsuario()
        {
            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text))
            {
                MostrarAdvertencia(
                    "Mensajes.Usuarios.NombreUsuarioRequerido",
                    "El nombre de usuario no puede estar vacío.",
                    "Mensajes.Titulos.Validacion",
                    "Validación"
                );

                txtNombreUsuario.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombreCompleto.Text))
            {
                MostrarAdvertencia(
                    "Mensajes.Usuarios.NombreCompletoRequerido",
                    "El nombre completo no puede estar vacío.",
                    "Mensajes.Titulos.Validacion",
                    "Validación"
                );

                txtNombreCompleto.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MostrarAdvertencia(
                    "Mensajes.Usuarios.EmailRequerido",
                    "El email no puede estar vacío.",
                    "Mensajes.Titulos.Validacion",
                    "Validación"
                );

                txtEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MostrarAdvertencia(
                    "Mensajes.Usuarios.PasswordRequerida",
                    "La contraseña no puede estar vacía.",
                    "Mensajes.Titulos.Validacion",
                    "Validación"
                );

                txtPassword.Focus();
                return false;
            }

            return true;
        }

        // Inicia la "Modificación de Usuario" en base a los datos ingresados para el usuario seleccionado.
        private void ModificarUsuario()
        {
            Guid idUsuario = ObtenerIdUsuarioSeleccionado();

            _gestionUsuariosAppService.ModificarUsuario(
                idUsuario,
                txtNombreCompleto.Text,
                txtEmail.Text,
                ObtenerEstadoSeleccionado()
            );

            MostrarInformacion(
                "Mensajes.Usuarios.UsuarioModificado",
                "Usuario modificado correctamente.",
                "Mensajes.Titulos.GestionUsuarios",
                "Gestión de usuarios"
            );

            CargarUsuarios();
        }

        // Se cambia el estado (Habilitado / Deshabilitado) del usuario seleccionado
        private void CambiarEstadoUsuarioSeleccionado()
        {
            Guid idUsuario = ObtenerIdUsuarioSeleccionado();
            bool usuarioActivo = ObtenerEstadoSeleccionado();
            bool nuevoEstado = !usuarioActivo;

            string nombreUsuario = txtNombreUsuario.Text;

            string claveConfirmacion = nuevoEstado
                ? "Mensajes.Usuarios.ConfirmarReactivar"
                : "Mensajes.Usuarios.ConfirmarInhabilitar";

            string mensajeConfirmacionPorDefecto = nuevoEstado
                ? "¿Confirmás que querés reactivar el usuario '{0}'?"
                : "¿Confirmás que querés inhabilitar el usuario '{0}'?";

            string mensajeConfirmacion = string.Format(
                TraducirMensaje(claveConfirmacion, mensajeConfirmacionPorDefecto),
                nombreUsuario
            );

            DialogResult respuesta = MostrarConfirmacion(
                null,
                mensajeConfirmacion,
                "Mensajes.Titulos.ConfirmarCambioEstado",
                "Confirmar cambio de estado",
                MessageBoxIcon.Question
            );

            if (respuesta != DialogResult.Yes)
            {
                return;
            }

            _gestionUsuariosAppService.CambiarEstadoUsuario(idUsuario, nuevoEstado);

            string claveResultado = nuevoEstado
                ? "Mensajes.Usuarios.UsuarioReactivado"
                : "Mensajes.Usuarios.UsuarioInhabilitado";

            string mensajeResultado = nuevoEstado
                ? "Usuario reactivado correctamente."
                : "Usuario inhabilitado correctamente.";

            MostrarInformacion(
                claveResultado,
                mensajeResultado,
                "Mensajes.Titulos.GestionUsuarios",
                "Gestión de usuarios"
            );

            CargarUsuarios();
        }

        // Se blanquea la contraseña del usuario seleccionado para que deba realizar un cambio de la misma.
        private void BlanquearPasswordUsuarioSeleccionado()
        {
            Guid idUsuario = ObtenerIdUsuarioSeleccionado();
            string nombreUsuario = txtNombreUsuario.Text;

            string mensajeConfirmacion = string.Format(
                TraducirMensaje(
                    "Mensajes.Usuarios.ConfirmarBlanquearPassword",
                    "¿Confirmás que querés blanquear la contraseña del usuario '{0}'?"
                ),
                nombreUsuario
            );

            DialogResult respuesta = MostrarConfirmacion(
                null,
                mensajeConfirmacion,
                "Mensajes.Titulos.ConfirmarBlanqueoPassword",
                "Confirmar blanqueo de contraseña",
                MessageBoxIcon.Question
            );

            if (respuesta != DialogResult.Yes)
            {
                return;
            }

            string passwordTemporal = _gestionUsuariosAppService.BlanquearPassword(idUsuario);

            string mensajeResultado = string.Format(
                TraducirMensaje(
                    "Mensajes.Usuarios.PasswordBlanqueada",
                    "Contraseña blanqueada correctamente.\n\nUsuario: {0}\nContraseña temporal: {1}\n\nAl ingresar, el usuario deberá cambiar su contraseña."
                ),
                nombreUsuario,
                passwordTemporal
            );

            MostrarInformacion(
                null,
                mensajeResultado,
                "Mensajes.Titulos.GestionUsuarios",
                "Gestión de usuarios"
            );

            CargarUsuarios();
        }
    }
}