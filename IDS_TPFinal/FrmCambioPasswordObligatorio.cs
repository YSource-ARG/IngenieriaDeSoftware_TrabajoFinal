using BLL.Usuarios;
using System;
using System.Drawing;
using System.Windows.Forms;
using UI.Estilos;

namespace UI
{
    public class FrmCambioPasswordObligatorio : Form
    {
        private readonly GestionUsuariosAppService _gestionUsuariosAppService;
        private readonly Guid _usuarioId;
        private readonly string _nombreUsuario;

        private Label lblTitulo;
        private Label lblDescripcion;
        private Label lblNuevaPassword;
        private Label lblConfirmarPassword;
        private TextBox txtNuevaPassword;
        private TextBox txtConfirmarPassword;
        private Button btnConfirmar;
        private Button btnCancelar;

        public FrmCambioPasswordObligatorio(
            GestionUsuariosAppService gestionUsuariosAppService,
            Guid usuarioId,
            string nombreUsuario)
        {
            _gestionUsuariosAppService = gestionUsuariosAppService
                ?? throw new ArgumentNullException(nameof(gestionUsuariosAppService));

            if (usuarioId == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(usuarioId));
            }

            _usuarioId = usuarioId;
            _nombreUsuario = nombreUsuario;

            InicializarControles();
            AplicarEstiloVisual();
        }

        private void InicializarControles()
        {
            lblTitulo = new Label();
            lblDescripcion = new Label();
            lblNuevaPassword = new Label();
            lblConfirmarPassword = new Label();
            txtNuevaPassword = new TextBox();
            txtConfirmarPassword = new TextBox();
            btnConfirmar = new Button();
            btnCancelar = new Button();

            SuspendLayout();

            lblTitulo.Text = "Cambio obligatorio de contraseña";
            lblTitulo.AutoSize = false;
            lblTitulo.Location = new Point(32, 28);
            lblTitulo.Size = new Size(410, 34);

            lblDescripcion.Text = $"Usuario: {_nombreUsuario}\nDebe definir una nueva contraseña antes de continuar.";
            lblDescripcion.AutoSize = false;
            lblDescripcion.Location = new Point(32, 70);
            lblDescripcion.Size = new Size(410, 48);

            lblNuevaPassword.Text = "Nueva contraseña";
            lblNuevaPassword.AutoSize = true;
            lblNuevaPassword.Location = new Point(32, 135);

            txtNuevaPassword.Location = new Point(32, 160);
            txtNuevaPassword.Size = new Size(410, 26);
            txtNuevaPassword.UseSystemPasswordChar = true;

            lblConfirmarPassword.Text = "Confirmar contraseña";
            lblConfirmarPassword.AutoSize = true;
            lblConfirmarPassword.Location = new Point(32, 202);

            txtConfirmarPassword.Location = new Point(32, 227);
            txtConfirmarPassword.Size = new Size(410, 26);
            txtConfirmarPassword.UseSystemPasswordChar = true;

            btnConfirmar.Text = "Confirmar";
            btnConfirmar.Location = new Point(190, 285);
            btnConfirmar.Size = new Size(120, 38);
            btnConfirmar.Click += btnConfirmar_Click;

            btnCancelar.Text = "Cancelar";
            btnCancelar.Location = new Point(322, 285);
            btnCancelar.Size = new Size(120, 38);
            btnCancelar.Click += btnCancelar_Click;

            Controls.Add(lblTitulo);
            Controls.Add(lblDescripcion);
            Controls.Add(lblNuevaPassword);
            Controls.Add(txtNuevaPassword);
            Controls.Add(lblConfirmarPassword);
            Controls.Add(txtConfirmarPassword);
            Controls.Add(btnConfirmar);
            Controls.Add(btnCancelar);

            AcceptButton = btnConfirmar;
            CancelButton = btnCancelar;

            ClientSize = new Size(475, 355);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Cambio obligatorio de contraseña";

            ResumeLayout(false);
            PerformLayout();
        }

        private void AplicarEstiloVisual()
        {
            TemaVisual.AplicarFormularioOscuro(this);

            TemaVisual.AplicarTitulo(lblTitulo);
            TemaVisual.AplicarTextoSecundario(lblDescripcion);
            TemaVisual.AplicarTextoSecundario(lblNuevaPassword);
            TemaVisual.AplicarTextoSecundario(lblConfirmarPassword);

            TemaVisual.AplicarTextBox(txtNuevaPassword);
            TemaVisual.AplicarTextBox(txtConfirmarPassword);

            TemaVisual.AplicarBotonPrincipal(btnConfirmar);
            TemaVisual.AplicarBotonSecundario(btnCancelar);
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                _gestionUsuariosAppService.ConfirmarCambioPasswordObligatorio(
                    _usuarioId,
                    txtNuevaPassword.Text,
                    txtConfirmarPassword.Text
                );

                MessageBox.Show(
                    "Contraseña actualizada correctamente.",
                    "Cambio de contraseña",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "No se pudo cambiar la contraseña",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtNuevaPassword.Clear();
                txtConfirmarPassword.Clear();
                txtNuevaPassword.Focus();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
