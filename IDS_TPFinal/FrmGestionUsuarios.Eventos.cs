using System;
using System.Windows.Forms;

namespace IDS_TPFinal
{
    public partial class FrmGestionUsuarios
    {
        private void RegistrarEventos()
        {
            dgvUsuarios.SelectionChanged += dgvUsuarios_SelectionChanged;
            btnNuevo.Click += btnNuevo_Click;
            btnEditar.Click += btnEditar_Click;
            btnInhabilitarReactivar.Click += btnInhabilitarReactivar_Click;
            btnRestablecerPassword.Click += btnRestablecerPassword_Click;
            btnCancelar.Click += btnCancelar_Click;
            btnCerrar.Click += btnCerrar_Click;
        }

        // Realiza una operación de persistencia en base al modo en que se encuentra el formulario.
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_gestionUsuariosAppService == null)
                {
                    MostrarError(
                        "Mensajes.Usuarios.ServicioGestionNoRecibido",
                        "El formulario no recibió el servicio de gestión de usuarios.",
                        "Mensajes.Titulos.ErrorConfiguracion",
                        "Error de configuración"
                    );

                    return;
                }

                if (_modoActual == ModoFormularioUsuario.Alta)
                {
                    CrearUsuario();
                    return;
                }

                if (_modoActual == ModoFormularioUsuario.Edicion)
                {
                    ModificarUsuario();
                    return;
                }

                MostrarAdvertencia(
                    "Mensajes.Usuarios.OperacionGuardarNoDisponible",
                    "Seleccioná una operación válida antes de guardar.",
                    "Mensajes.Titulos.OperacionNoDisponible",
                    "Operación no disponible"
                );
            }
            catch (Exception ex)
            {
                MostrarAdvertencia(
                    null,
                    ex.Message,
                    "Mensajes.Titulos.NoPudoGuardarUsuario",
                    "No se pudo guardar el usuario"
                );
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            PrepararAltaUsuario();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (!HayUsuarioSeleccionado())
            {
                MostrarAdvertencia(
                    "Mensajes.Usuarios.SeleccionarUsuarioEditar",
                    "Seleccioná un usuario de la grilla antes de editar.",
                    "Mensajes.Titulos.UsuarioNoSeleccionado",
                    "Usuario no seleccionado"
                );

                return;
            }

            AplicarModoEdicion();
            txtNombreCompleto.Focus();
        }

        private void btnInhabilitarReactivar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_modoActual == ModoFormularioUsuario.Edicion)
                {
                    MostrarAdvertencia(
                        "Mensajes.Usuarios.EdicionEnCursoCambioEstado",
                        "Terminá la edición actual guardando o cancelando antes de cambiar el estado del usuario.",
                        "Mensajes.Titulos.EdicionEnCurso",
                        "Edición en curso"
                    );

                    return;
                }

                if (!HayUsuarioSeleccionado())
                {
                    MostrarAdvertencia(
                        "Mensajes.Usuarios.SeleccionarUsuarioCambiarEstado",
                        "Seleccioná un usuario de la grilla antes de cambiar su estado.",
                        "Mensajes.Titulos.UsuarioNoSeleccionado",
                        "Usuario no seleccionado"
                    );

                    return;
                }

                CambiarEstadoUsuarioSeleccionado();
            }
            catch (Exception ex)
            {
                MostrarAdvertencia(
                    null,
                    ex.Message,
                    "Mensajes.Titulos.NoPudoCambiarEstadoUsuario",
                    "No se pudo cambiar el estado del usuario"
                );
            }
        }

        private void btnRestablecerPassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (!HayUsuarioSeleccionado())
                {
                    MostrarAdvertencia(
                        "Mensajes.Usuarios.SeleccionarUsuarioBlanquearPassword",
                        "Seleccioná un usuario de la grilla antes de blanquear su contraseña.",
                        "Mensajes.Titulos.UsuarioNoSeleccionado",
                        "Usuario no seleccionado"
                    );

                    return;
                }

                BlanquearPasswordUsuarioSeleccionado();
            }
            catch (Exception ex)
            {
                MostrarAdvertencia(
                    null,
                    ex.Message,
                    "Mensajes.Titulos.NoPudoBlanquearPassword",
                    "No se pudo blanquear la contraseña"
                );
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            PrepararAltaUsuario();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (_cargandoUsuarios)
            {
                return;
            }

            CargarUsuarioSeleccionadoEnFormulario();
        }
    }
}