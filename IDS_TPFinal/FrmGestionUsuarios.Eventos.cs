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
                        "El formulario no recibió el servicio de gestión de usuarios.",
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
                    "Seleccioná una operación válida antes de guardar.",
                    "Operación no disponible"
                );
            }
            catch (Exception ex)
            {
                MostrarAdvertencia(ex.Message, "No se pudo guardar el usuario");
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
                    "Seleccioná un usuario de la grilla antes de editar.",
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
                        "Terminá la edición actual guardando o cancelando antes de cambiar el estado del usuario.",
                        "Edición en curso"
                    );

                    return;
                }

                if (!HayUsuarioSeleccionado())
                {
                    MostrarAdvertencia(
                        "Seleccioná un usuario de la grilla antes de cambiar su estado.",
                        "Usuario no seleccionado"
                    );

                    return;
                }

                CambiarEstadoUsuarioSeleccionado();
            }
            catch (Exception ex)
            {
                MostrarAdvertencia(ex.Message, "No se pudo cambiar el estado del usuario");
            }
        }

        private void btnRestablecerPassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (!HayUsuarioSeleccionado())
                {
                    MostrarAdvertencia(
                        "Seleccioná un usuario de la grilla antes de blanquear su contraseña.",
                        "Usuario no seleccionado"
                    );

                    return;
                }

                BlanquearPasswordUsuarioSeleccionado();
            }
            catch (Exception ex)
            {
                MostrarAdvertencia(ex.Message, "No se pudo blanquear la contraseña");
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
