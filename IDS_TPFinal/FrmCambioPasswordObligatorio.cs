using BLL.Idiomas;
using BLL.Usuarios;
using System;
using System.Windows.Forms;
using UI.Estilos;
using UI.Idiomas;

namespace UI
{
    public partial class FrmCambioPasswordObligatorio : Form
    {
        private readonly GestionUsuariosAppService _gestionUsuariosAppService;
        private readonly IIdiomaAppService _idiomaAppService;
        private readonly Guid _usuarioId;
        private readonly string _nombreUsuario;

        public FrmCambioPasswordObligatorio()
        {
            InitializeComponent();
        }

        public FrmCambioPasswordObligatorio(
            GestionUsuariosAppService gestionUsuariosAppService,
            Guid usuarioId,
            string nombreUsuario,
            IIdiomaAppService idiomaAppService)
        {
            _gestionUsuariosAppService = gestionUsuariosAppService
                ?? throw new ArgumentNullException(nameof(gestionUsuariosAppService));

            if (usuarioId == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(usuarioId));
            }

            _usuarioId = usuarioId;
            _nombreUsuario = nombreUsuario;
            _idiomaAppService = idiomaAppService;

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
                if (!ValidarDatosCambioPassword())
                {
                    return;
                }

                _gestionUsuariosAppService.ConfirmarCambioPasswordObligatorio(
                    _usuarioId,
                    txtNuevaPassword.Text,
                    txtConfirmarPassword.Text
                );

                MostrarInformacion(
                    "Mensajes.CambioPassword.PasswordActualizada",
                    "Contraseña actualizada correctamente.",
                    "Mensajes.Titulos.CambioPassword",
                    "Cambio de contraseña"
                );

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception)
            {
                MostrarAdvertencia(
                    "Mensajes.CambioPassword.ErrorCambio",
                    "No fue posible cambiar la contraseña. Verifique los datos e intente nuevamente.",
                    "Mensajes.Titulos.NoPudoCambiarPassword",
                    "No se pudo cambiar la contraseña"
                );

                txtNuevaPassword.Clear();
                txtConfirmarPassword.Clear();
                txtNuevaPassword.Focus();
            }
        }

        private bool ValidarDatosCambioPassword()
        {
            if (string.IsNullOrWhiteSpace(txtNuevaPassword.Text))
            {
                MostrarAdvertencia(
                    "Mensajes.CambioPassword.NuevaPasswordRequerida",
                    "La nueva contraseña no puede estar vacía.",
                    "Mensajes.Titulos.Validacion",
                    "Validación"
                );

                txtNuevaPassword.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtConfirmarPassword.Text))
            {
                MostrarAdvertencia(
                    "Mensajes.CambioPassword.ConfirmarPasswordRequerida",
                    "Debe confirmar la nueva contraseña.",
                    "Mensajes.Titulos.Validacion",
                    "Validación"
                );

                txtConfirmarPassword.Focus();
                return false;
            }

            if (txtNuevaPassword.Text != txtConfirmarPassword.Text)
            {
                MostrarAdvertencia(
                    "Mensajes.CambioPassword.PasswordsNoCoinciden",
                    "La contraseña y su confirmación no coinciden.",
                    "Mensajes.Titulos.Validacion",
                    "Validación"
                );

                txtNuevaPassword.Clear();
                txtConfirmarPassword.Clear();
                txtNuevaPassword.Focus();
                return false;
            }

            return true;
        }

        private void MostrarInformacion(
            string claveMensaje,
            string mensajePorDefecto,
            string claveTitulo,
            string tituloPorDefecto)
        {
            MensajeTraducido.Mostrar(
                this,
                _idiomaAppService,
                claveMensaje,
                mensajePorDefecto,
                claveTitulo,
                tituloPorDefecto,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void MostrarAdvertencia(
            string claveMensaje,
            string mensajePorDefecto,
            string claveTitulo,
            string tituloPorDefecto)
        {
            MensajeTraducido.Mostrar(
                this,
                _idiomaAppService,
                claveMensaje,
                mensajePorDefecto,
                claveTitulo,
                tituloPorDefecto,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}