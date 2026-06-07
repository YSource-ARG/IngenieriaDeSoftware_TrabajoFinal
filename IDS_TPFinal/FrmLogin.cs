using BLL.Autenticacion;
using BLL.Bitacora;
using BLL.Usuarios;
using System;
using System.Drawing;
using System.Windows.Forms;
using UI.Estilos;

namespace UI
{
    public partial class FrmLogin : Form
    {
        private readonly LoginAppService _loginAppService;
        private readonly CerrarSesionAppService _cerrarSesionAppService;
        private readonly GestionUsuariosAppService _gestionUsuariosAppService;
        private readonly IBitacoraService _bitacoraService;

        public FrmLogin(
            LoginAppService loginAppService,
            CerrarSesionAppService cerrarSesionAppService,
            GestionUsuariosAppService gestionUsuariosAppService,
            IBitacoraService bitacoraService)
        {
            if (loginAppService == null)
            {
                throw new ArgumentNullException(nameof(loginAppService));
            }

            if (cerrarSesionAppService == null)
            {
                throw new ArgumentNullException(nameof(cerrarSesionAppService));
            }

            if (gestionUsuariosAppService == null)
            {
                throw new ArgumentNullException(nameof(gestionUsuariosAppService));
            }

            if (bitacoraService == null)
            {
                throw new ArgumentNullException(nameof(bitacoraService));
            }

            _loginAppService = loginAppService;
            _cerrarSesionAppService = cerrarSesionAppService;
            _gestionUsuariosAppService = gestionUsuariosAppService;
            _bitacoraService = bitacoraService;

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
                MessageBox.Show(
                    "Por favor, ingrese su nombre de usuario.",
                    "Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtNombreUsuario.Clear();
                txtNombreUsuario.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show(
                    "Por favor, ingrese su contraseña.",
                    "Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            ResultadoLogin resultadoLogin = _loginAppService.IniciarSesion(
                txtNombreUsuario.Text.Trim(),
                txtPassword.Text
            );

            if (resultadoLogin.UsuarioBloqueado)
            {
                string mensaje =
                    "El acceso fue bloqueado temporalmente por varios intentos fallidos. " +
                    "Intente nuevamente en unos minutos.";

                if (resultadoLogin.BloqueadoHasta.HasValue)
                {
                    TimeSpan tiempoRestante =
                        resultadoLogin.BloqueadoHasta.Value - DateTime.Now;

                    int minutosRestantes = Math.Max(
                        1,
                        (int)Math.Ceiling(tiempoRestante.TotalMinutes)
                    );

                    mensaje =
                        $"El acceso fue bloqueado temporalmente. " +
                        $"Intente nuevamente en aproximadamente {minutosRestantes} minuto(s).";
                }

                MessageBox.Show(
                    mensaje,
                    "Acceso bloqueado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            if (!resultadoLogin.LoginExitoso)
            {
                MessageBox.Show(
                    "Usuario o contraseña incorrectos.",
                    "Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            if (resultadoLogin.DebeCambiarPassword &&
                !CambiarPasswordObligatorio(resultadoLogin))
            {
                _cerrarSesionAppService.CerrarSesion();
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            AbrirFormularioPrincipal();
        }

        private bool CambiarPasswordObligatorio(ResultadoLogin resultadoLogin)
        {
            using (FrmCambioPasswordObligatorio frmCambioPassword = new FrmCambioPasswordObligatorio(
                _gestionUsuariosAppService,
                resultadoLogin.UsuarioId,
                resultadoLogin.NombreUsuario))
            {
                DialogResult resultado = frmCambioPassword.ShowDialog(this);

                return resultado == DialogResult.OK;
            }
        }

        private void AbrirFormularioPrincipal()
        {
            FrmPrincipal frmPrincipal = new FrmPrincipal(
                _cerrarSesionAppService,
                _gestionUsuariosAppService,
                _bitacoraService
            );

            this.Hide();
            //aca el evento de cerrar con la x el formulario invoca el cierre de sesion del usuario logueado
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
