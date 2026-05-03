using System;
using System.Drawing;
using System.Windows.Forms;
using BLL.Autenticacion;
using UI.Estilos;

namespace UI
{
    public partial class FrmLogin : Form
    {
        private readonly LoginAppService _loginAppService;
        private readonly CerrarSesionAppService _cerrarSesionAppService;

        public FrmLogin(LoginAppService loginAppService, CerrarSesionAppService cerrarSesionAppService)
        {
            if (loginAppService == null)
            {
                throw new ArgumentNullException(nameof(loginAppService));
            }

            if (cerrarSesionAppService == null)
            {
                throw new ArgumentNullException(nameof(cerrarSesionAppService));
            }

            _loginAppService = loginAppService;
            _cerrarSesionAppService = cerrarSesionAppService;

            InitializeComponent();

            this.AcceptButton = btnIngresar;

            AplicarEstiloVisual();

            this.Shown += FrmLogin_Shown;
            this.Resize += FrmLogin_Resize;
        }

        private void FrmLogin_Shown(object sender, EventArgs e)
        {
            CentrarPanelLogin();
            txtNombreUsuario.Focus();
        }

        private void FrmLogin_Resize(object sender, EventArgs e)
        {
            CentrarPanelLogin();
        }

        private void CentrarPanelLogin()
        {
            panelLogin.Left = (this.ClientSize.Width - panelLogin.Width) / 2;
            panelLogin.Top = (this.ClientSize.Height - panelLogin.Height) / 2;
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text))
            {
                MessageBox.Show("Por favor, ingrese su nombre de usuario.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombreUsuario.Clear();
                txtNombreUsuario.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Por favor, ingrese su contraseña.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            bool loginExitoso = _loginAppService.IniciarSesion(txtNombreUsuario.Text.Trim(), txtPassword.Text);

            if (loginExitoso)
            {
                FrmPrincipal frmPrincipal = new FrmPrincipal(_cerrarSesionAppService);

                this.Hide();

                frmPrincipal.FormClosed += (s, args) =>
                {
                    if (frmPrincipal.CerrandoSesion)
                    {
                        txtPassword.Clear();
                        this.Show();
                        txtPassword.Focus();
                    }
                    else
                    {
                        this.Close();
                    }
                };

                frmPrincipal.Show();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private void AplicarEstiloVisual()
        {
            TemaVisual.AplicarFormularioOscuro(this);

            this.Text = "Acceso al sistema";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            TemaVisual.AplicarPanelPrincipal(panelLogin);
            TemaVisual.AplicarTextBox(txtNombreUsuario);
            TemaVisual.AplicarTextBox(txtPassword);
            TemaVisual.AplicarBotonPrincipal(btnIngresar);

            TemaVisual.AplicarTitulo(lblTitulo);
            TemaVisual.AplicarTextoSecundario(lblSubtitulo);
            TemaVisual.AplicarTextoSecundario(lblUsuario);
            TemaVisual.AplicarTextoSecundario(lblPassword);
            TemaVisual.AplicarTextoSecundario(lblMensajeSeguridad);

            panelLogin.Size = new Size(380, 380);

            AlinearControlesLogin();
            CentrarPanelLogin();
        }

        private void AlinearControlesLogin()
        {
            int anchoControl = 285;
            int margenIzquierdo = (panelLogin.Width - anchoControl) / 2;

            lblTitulo.Left = margenIzquierdo;
            lblSubtitulo.Left = margenIzquierdo;

            lblUsuario.Left = margenIzquierdo;
            txtNombreUsuario.Left = margenIzquierdo;

            lblPassword.Left = margenIzquierdo;
            txtPassword.Left = margenIzquierdo;

            btnIngresar.Left = margenIzquierdo;
            lblMensajeSeguridad.Left = margenIzquierdo;

            txtNombreUsuario.Width = anchoControl;
            txtPassword.Width = anchoControl;
            btnIngresar.Width = anchoControl;
            lblSubtitulo.Width = anchoControl;
            lblMensajeSeguridad.Width = anchoControl;
        }
    }
}