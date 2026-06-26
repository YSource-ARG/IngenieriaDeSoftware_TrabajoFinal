using System;
using System.Windows.Forms;
using UI.Idiomas;
using UI;

namespace IDS_TPFinal
{
    public partial class FrmGestionUsuarios
    {
        private class EstadoUsuarioCombo
        {
            public EstadoUsuarioCombo(bool activo, string texto)
            {
                Activo = activo;
                Texto = texto;
            }

            public bool Activo { get; private set; }
            public string Texto { get; private set; }

            public override string ToString()
            {
                return Texto;
            }
        }

        private void PrepararAltaUsuario()
        {
            _cargandoUsuarios = true;

            try
            {
                LimpiarSeleccionGrilla();
                LimpiarCampos();
                AplicarModoAlta();
            }
            finally
            {
                _cargandoUsuarios = false;
            }

            txtNombreUsuario.Focus();
        }

        private void AplicarModoAlta()
        {
            _modoActual = ModoFormularioUsuario.Alta;

            txtNombreUsuario.ReadOnly = false;
            txtNombreCompleto.ReadOnly = false;
            txtEmail.ReadOnly = false;
            txtPassword.ReadOnly = false;
            cmbEstado.Enabled = true;

            btnGuardar.Enabled = true;
            btnEditar.Enabled = false;
            btnInhabilitarReactivar.Enabled = false;
            btnRestablecerPassword.Enabled = false;
            _btnPermisos.Enabled = _autorizacionService.UsuarioActualTienePermiso(PermisosSistema.UsuariosAsignarPermisos);

            lblPassword.Text = TraducirMensaje("Usuarios.NuevaPassword", "Nueva contraseña");

            ActualizarTextosDinamicos();
        }

        private void AplicarModoConsulta()
        {
            _modoActual = ModoFormularioUsuario.Consulta;

            txtNombreUsuario.ReadOnly = true;
            txtNombreCompleto.ReadOnly = true;
            txtEmail.ReadOnly = true;
            txtPassword.ReadOnly = true;
            cmbEstado.Enabled = false;

            btnGuardar.Enabled = false;
            btnEditar.Enabled = true;
            btnInhabilitarReactivar.Enabled = true;
            btnRestablecerPassword.Enabled = true;
            _btnPermisos.Enabled = _autorizacionService.UsuarioActualTienePermiso(PermisosSistema.UsuariosAsignarPermisos);

            lblPassword.Text = TraducirMensaje("Usuarios.NuevaPassword", "Nueva contraseña");

            ActualizarTextosDinamicos();
        }

        private void AplicarModoEdicion()
        {
            _modoActual = ModoFormularioUsuario.Edicion;

            txtNombreUsuario.ReadOnly = true;
            txtNombreCompleto.ReadOnly = false;
            txtEmail.ReadOnly = false;
            txtPassword.ReadOnly = true;
            cmbEstado.Enabled = true;

            btnGuardar.Enabled = true;
            btnEditar.Enabled = false;
            btnInhabilitarReactivar.Enabled = false;
            btnRestablecerPassword.Enabled = false;
            _btnPermisos.Enabled = false;

            txtPassword.Clear();
            lblPassword.Text = TraducirMensaje("Usuarios.NuevaPassword", "Nueva contraseña");

            ActualizarTextosDinamicos();
        }

        private void ActualizarTextosDinamicos()
        {
            btnNuevo.Text = "+\n" + TraducirMensaje("Usuarios.Nuevo", "Nuevo");
            btnEditar.Text = "✎\n" + TraducirMensaje("Usuarios.Editar", "Editar");
            btnRestablecerPassword.Text = "🔑\n" + TraducirMensaje("Usuarios.RestablecerPassword", "Restablecer contraseña");
            btnHistorialEmail.Text = "✉\n" + TraducirMensaje("Usuarios.HistorialEmail", "Historial email");
            _btnPermisos.Text = "🛡\n" + TraducirMensaje("Usuarios.Permisos", "Permisos");

            ActualizarTextoBotonCambioEstado();
        }

        private void ActualizarTextoBotonCambioEstado()
        {
            if (!HayUsuarioSeleccionado())
            {
                btnInhabilitarReactivar.Text = "⏻\n" + TraducirMensaje(
                    "Usuarios.InhabilitarReactivar",
                    "Inhabilitar/Reactivar"
                );

                return;
            }

            btnInhabilitarReactivar.Text = ObtenerEstadoSeleccionado()
                ? "⏻\n" + TraducirMensaje("Usuarios.Inhabilitar", "Inhabilitar")
                : "⏻\n" + TraducirMensaje("Usuarios.Reactivar", "Reactivar");
        }

        private void LimpiarCampos()
        {
            txtId.Clear();
            txtNombreUsuario.Clear();
            txtNombreCompleto.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            txtFechaCreacion.Clear();
            txtFechaUltimoAcceso.Clear();

            SeleccionarEstadoCombo(true);
        }

        private void LimpiarSeleccionGrilla()
        {
            dgvUsuarios.ClearSelection();

            if (dgvUsuarios.Rows.Count > 0)
            {
                dgvUsuarios.CurrentCell = null;
            }
        }

        private bool HayUsuarioSeleccionado()
        {
            return Guid.TryParse(txtId.Text, out Guid idUsuario) && idUsuario != Guid.Empty;
        }

        private bool ObtenerEstadoSeleccionado()
        {
            EstadoUsuarioCombo estado = cmbEstado.SelectedItem as EstadoUsuarioCombo;

            if (estado != null)
            {
                return estado.Activo;
            }

            string texto = cmbEstado.SelectedItem?.ToString();

            return texto == "Activo" || texto == "Active";
        }

        private void SeleccionarEstadoCombo(bool activo)
        {
            foreach (object item in cmbEstado.Items)
            {
                EstadoUsuarioCombo estado = item as EstadoUsuarioCombo;

                if (estado != null && estado.Activo == activo)
                {
                    cmbEstado.SelectedItem = estado;
                    return;
                }
            }

            if (cmbEstado.Items.Count > 0)
            {
                cmbEstado.SelectedIndex = activo ? 0 : Math.Min(1, cmbEstado.Items.Count - 1);
            }
        }

        private Guid ObtenerIdUsuarioSeleccionado()
        {
            if (!Guid.TryParse(txtId.Text, out Guid idUsuario) || idUsuario == Guid.Empty)
            {
                throw new InvalidOperationException("No se pudo identificar el usuario seleccionado.");
            }

            return idUsuario;
        }

        private string TraducirMensaje(string clave, string textoPorDefecto)
        {
            if (string.IsNullOrWhiteSpace(clave))
            {
                return textoPorDefecto;
            }

            return MensajeTraducido.TraducirConFallback(
                _idiomaAppService,
                clave,
                textoPorDefecto
            );
        }

        private void MostrarInformacion(
            string claveMensaje,
            string mensaje,
            string claveTitulo,
            string titulo)
        {
            MessageBox.Show(
                this,
                TraducirMensaje(claveMensaje, mensaje),
                TraducirMensaje(claveTitulo, titulo),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void MostrarAdvertencia(
            string claveMensaje,
            string mensaje,
            string claveTitulo,
            string titulo)
        {
            MessageBox.Show(
                this,
                TraducirMensaje(claveMensaje, mensaje),
                TraducirMensaje(claveTitulo, titulo),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        private void MostrarError(
            string claveMensaje,
            string mensaje,
            string claveTitulo,
            string titulo)
        {
            MessageBox.Show(
                this,
                TraducirMensaje(claveMensaje, mensaje),
                TraducirMensaje(claveTitulo, titulo),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        private DialogResult MostrarConfirmacion(
            string claveMensaje,
            string mensaje,
            string claveTitulo,
            string titulo,
            MessageBoxIcon icono)
        {
            return MessageBox.Show(
                this,
                TraducirMensaje(claveMensaje, mensaje),
                TraducirMensaje(claveTitulo, titulo),
                MessageBoxButtons.YesNo,
                icono
            );
        }
    }
}
