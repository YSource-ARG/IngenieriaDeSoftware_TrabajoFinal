using System;
using System.Windows.Forms;

namespace IDS_TPFinal
{
    public partial class FrmGestionUsuarios
    {
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

            btnInhabilitarReactivar.Text = "⏻\nInhabilitar/Reactivar";

            lblPassword.Text = "Nueva contraseña";
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

            ActualizarTextoBotonCambioEstado();

            lblPassword.Text = "Nueva contraseña";
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

            txtPassword.Clear();
            lblPassword.Text = "Nueva contraseña";
        }

        private void ActualizarTextoBotonCambioEstado()
        {
            btnInhabilitarReactivar.Text = ObtenerEstadoSeleccionado()
                ? "⏻\nInhabilitar"
                : "⏻\nReactivar";
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

            if (cmbEstado.Items.Count > 0)
            {
                cmbEstado.SelectedIndex = 0;
            }
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
            return cmbEstado.SelectedItem?.ToString() == "Activo";
        }

        private Guid ObtenerIdUsuarioSeleccionado()
        {
            if (!Guid.TryParse(txtId.Text, out Guid idUsuario) || idUsuario == Guid.Empty)
            {
                throw new InvalidOperationException("No se pudo identificar el usuario seleccionado.");
            }

            return idUsuario;
        }

        private void MostrarAdvertencia(string mensaje, string titulo)
        {
            MessageBox.Show(
                mensaje,
                titulo,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        private void MostrarError(string mensaje, string titulo)
        {
            MessageBox.Show(
                mensaje,
                titulo,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
}
