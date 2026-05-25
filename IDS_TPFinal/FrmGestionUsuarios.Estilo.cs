using System;
using System.Drawing;
using System.Windows.Forms;
using UI.Estilos;

namespace IDS_TPFinal
{
    public partial class FrmGestionUsuarios
    {
        private void FrmGestionUsuarios_Load(object sender, EventArgs e)
        {
            AplicarEstiloVisual();
            ConfigurarEstadoInicial();
        }

        private void AplicarEstiloVisual()
        {
            TemaVisual.AplicarFormularioOscuro(this);

            AplicarPanel(panelHeader);
            AplicarPanel(panelContenido);
            AplicarPanel(panelEdicion);

            panelLineaIzquierda.BackColor = TemaVisual.BordeSuave;
            panelLineaDerecha.BackColor = TemaVisual.BordeSuave;

            lblIcono.Font = new Font("Segoe UI Emoji", 28F, FontStyle.Regular);
            lblIcono.ForeColor = TemaVisual.TextoPrincipal;
            lblIcono.BackColor = Color.Transparent;

            lblTitulo.ForeColor = TemaVisual.TextoPrincipal;
            lblTitulo.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblTitulo.BackColor = Color.Transparent;

            lblSubtitulo.ForeColor = TemaVisual.TextoSecundario;
            lblSubtitulo.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            lblSubtitulo.BackColor = Color.Transparent;

            lblDatosUsuario.ForeColor = TemaVisual.TextoPrincipal;
            lblDatosUsuario.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblDatosUsuario.BackColor = Color.Transparent;

            lblAccionesRapidas.ForeColor = TemaVisual.TextoPrincipal;
            lblAccionesRapidas.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblAccionesRapidas.BackColor = Color.Transparent;

            AplicarEtiqueta(lblId);
            AplicarEtiqueta(lblNombreUsuario);
            AplicarEtiqueta(lblNombreCompleto);
            AplicarEtiqueta(lblEstado);
            AplicarEtiqueta(lblPassword);
            AplicarEtiqueta(lblFechaCreacion);
            AplicarEtiqueta(lblFechaUltimoAcceso);

            TemaVisual.AplicarTextBox(txtId);
            TemaVisual.AplicarTextBox(txtNombreUsuario);
            TemaVisual.AplicarTextBox(txtNombreCompleto);
            TemaVisual.AplicarTextBox(txtPassword);

            AplicarComboBox(cmbEstado);
            AplicarDateTimePicker(dtpFechaCreacion);
            AplicarDateTimePicker(dtpFechaUltimoAcceso);

            TemaVisual.AplicarBotonPrincipal(btnGuardar);
            TemaVisual.AplicarBotonSecundario(btnCancelar);
            TemaVisual.AplicarBotonSecundario(btnCerrar);

            AplicarBotonAccion(btnNuevo, "+\nNuevo");
            AplicarBotonAccion(btnEditar, "✎\nEditar");
            AplicarBotonAccion(btnInhabilitarReactivar, "⏻\nInhabilitar/Reactivar");
            AplicarBotonAccion(btnRestablecerPassword, "🔑\nRestablecer contraseña");
        }

        private void ConfigurarEstadoInicial()
        {
            txtId.ReadOnly = true;
            txtPassword.UseSystemPasswordChar = true;

            dtpFechaCreacion.Enabled = false;
            dtpFechaUltimoAcceso.Enabled = false;
            dtpFechaCreacion.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpFechaUltimoAcceso.CustomFormat = "dd/MM/yyyy HH:mm";

            cmbEstado.Items.Clear();
            cmbEstado.Items.Add("Activo");
            cmbEstado.Items.Add("Inactivo");

            if (cmbEstado.Items.Count > 0)
            {
                cmbEstado.SelectedIndex = 0;
            }
        }

        private void AplicarPanel(Panel panel)
        {
            panel.BackColor = TemaVisual.FondoPanel;
        }

        private void AplicarEtiqueta(Label label)
        {
            label.ForeColor = TemaVisual.TextoPrincipal;
            label.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            label.BackColor = Color.Transparent;
        }

        private void AplicarComboBox(ComboBox comboBox)
        {
            comboBox.BackColor = TemaVisual.FondoInput;
            comboBox.ForeColor = TemaVisual.TextoPrincipal;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }

        private void AplicarDateTimePicker(DateTimePicker dateTimePicker)
        {
            dateTimePicker.CalendarForeColor = TemaVisual.TextoPrincipal;
            dateTimePicker.CalendarMonthBackground = TemaVisual.FondoInput;
            dateTimePicker.CalendarTitleBackColor = TemaVisual.FondoPanel;
            dateTimePicker.CalendarTitleForeColor = TemaVisual.TextoPrincipal;
            dateTimePicker.CalendarTrailingForeColor = TemaVisual.TextoMuted;
            dateTimePicker.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }

        private void AplicarBotonAccion(Button boton, string texto)
        {
            boton.Text = texto;
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 1;
            boton.FlatAppearance.BorderColor = TemaVisual.BordeSuave;
            boton.BackColor = TemaVisual.FondoPanel;
            boton.ForeColor = TemaVisual.TextoPrincipal;
            boton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            boton.Cursor = Cursors.Hand;

            boton.MouseEnter += (s, e) =>
            {
                boton.BackColor = Color.FromArgb(42, 47, 56);
            };

            boton.MouseLeave += (s, e) =>
            {
                boton.BackColor = TemaVisual.FondoPanel;
            };
        }
    }
}
