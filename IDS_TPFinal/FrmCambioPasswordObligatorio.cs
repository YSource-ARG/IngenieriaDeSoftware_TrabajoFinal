using BLL.Usuarios;
using System;
using System.Windows.Forms;
using UI.Estilos;

namespace UI
{
    public partial class FrmCambioPasswordObligatorio : Form
    {
        private readonly GestionUsuariosAppService _gestionUsuariosAppService;
        private readonly Guid _usuarioId;
        private readonly string _nombreUsuario;

        public FrmCambioPasswordObligatorio()
        {
            InitializeComponent();
        }

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

            InitializeComponent();
            ConfigurarDescripcion();
            AplicarEstiloVisual();
        }

        private void ConfigurarDescripcion()
        {
            lblDescripcion.Text = $"Usuario: {_nombreUsuario}{Environment.NewLine}Debe definir una nueva contraseña antes de continuar.";
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
