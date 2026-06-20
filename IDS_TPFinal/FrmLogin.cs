using BLL.Autenticacion;
using BLL.Bitacora;
using BLL.Usuarios;
using System;
using System.Drawing;
using System.Windows.Forms;
using UI.Estilos;
using BLL.Integridad;
using BE;
using BLL.Idiomas;
using System.Collections.Generic;
using UI.Idiomas;


namespace UI
{
    public partial class FrmLogin : Form, IObservadorIdioma
    {
        private readonly LoginAppService _loginAppService;
        private readonly CerrarSesionAppService _cerrarSesionAppService;
        private readonly GestionUsuariosAppService _gestionUsuariosAppService;
        private readonly IBitacoraService _bitacoraService;
        private readonly IIntegridadService _integridadService;
        private readonly IIdiomaAppService _idiomaAppService;
        private readonly GestionTraduccionesAppService _gestionTraduccionesAppService;
        private readonly GestionIdiomasAppService _gestionIdiomasAppService;
        private bool _cargandoIdiomasLogin;

        public FrmLogin(
            LoginAppService loginAppService,
            CerrarSesionAppService cerrarSesionAppService,
            GestionUsuariosAppService gestionUsuariosAppService,
            IIntegridadService integridadService,
            IBitacoraService bitacoraService,
            IIdiomaAppService idiomaAppService,
            GestionTraduccionesAppService gestionTraduccionesAppService,
            GestionIdiomasAppService gestionIdiomasAppService)
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

            if (integridadService == null)
            {
                throw new ArgumentNullException(nameof(integridadService));
            }

            if (bitacoraService == null)
            {
                throw new ArgumentNullException(nameof(bitacoraService));
            }

            if (idiomaAppService == null)
            {
                throw new ArgumentNullException(nameof(idiomaAppService));
            }

            if (gestionTraduccionesAppService == null)
            {
                throw new ArgumentNullException(nameof(gestionTraduccionesAppService));
            }

            if (gestionIdiomasAppService == null)
            {
                throw new ArgumentNullException(nameof(gestionIdiomasAppService));
            }

            _loginAppService = loginAppService;
            _cerrarSesionAppService = cerrarSesionAppService;
            _gestionUsuariosAppService = gestionUsuariosAppService;
            _integridadService = integridadService;
            _bitacoraService = bitacoraService;
            _idiomaAppService = idiomaAppService;
            _gestionTraduccionesAppService = gestionTraduccionesAppService;
            _gestionIdiomasAppService = gestionIdiomasAppService;

            InitializeComponent();

            this.AcceptButton = btnIngresar;

            AplicarEstiloVisual();            
            _idiomaAppService.Suscribir(this);
            CargarIdiomasLogin();
            ActualizarIdioma();

            this.Shown += FrmLogin_Shown;
            this.Resize += FrmLogin_Resize;
            this.FormClosed += FrmLogin_FormClosed;
            cboIdiomaLogin.SelectedIndexChanged += cboIdiomaLogin_SelectedIndexChanged;
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

        private DialogResult MostrarMensajeTraducido(
            string claveMensaje,
            string mensajePorDefecto,
            string claveTitulo,
            string tituloPorDefecto,
            MessageBoxIcon icono)
        {
            return MensajeTraducido.Mostrar(
                this,
                _idiomaAppService,
                claveMensaje,
                mensajePorDefecto,
                claveTitulo,
                tituloPorDefecto,
                MessageBoxButtons.OK,
                icono
            );
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text))
            {
                MostrarMensajeTraducido(
                    "Mensajes.Login.UsuarioRequerido",
                    "Por favor, ingrese su nombre de usuario.",
                    "Mensajes.Titulos.Login",
                    "Login",
                    MessageBoxIcon.Warning
                );

                txtNombreUsuario.Clear();
                txtNombreUsuario.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MostrarMensajeTraducido(
                    "Mensajes.Login.PasswordRequerido",
                    "Por favor, ingrese su contraseña.",
                    "Mensajes.Titulos.Login",
                    "Login",
                    MessageBoxIcon.Warning
                );

                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            ResultadoVerificacionIntegridad resultadoIntegridad;

            try
            {
                // La integridad se verifica antes del login porque el usuario todavía
                // no debe acceder al sistema si la base fue modificada por fuera.
                // Si se detecta una falla, el servicio bloquea a los usuarios comunes
                // y deja habilitado al administrador para recalcular los DV.
                resultadoIntegridad = _integridadService.VerificarIntegridadUsuarios();
            }
            catch (Exception)
            {
                MostrarMensajeTraducido(
                    "Mensajes.Login.ErrorVerificarIntegridad",
                    "No fue posible verificar la integridad de la base de datos. Verifique la conexión e intente nuevamente.",
                    "Mensajes.Titulos.ErrorIntegridad",
                    "Error de integridad",
                    MessageBoxIcon.Error
                );

                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            bool usuarioEsAdmin = string.Equals(txtNombreUsuario.Text.Trim(), "admin", StringComparison.OrdinalIgnoreCase);

            if (!resultadoIntegridad.IntegridadCorrecta)
            {
                if (!usuarioEsAdmin)
                {
                    MostrarMensajeTraducido(
                        "Mensajes.Login.IntegridadVulneradaUsuario",
                        "Se detectó una falla de integridad en la base de datos. Por seguridad, solo el administrador puede ingresar para recalcular los dígitos verificadores.",
                        "Mensajes.Titulos.IntegridadVulnerada",
                        "Integridad vulnerada",
                        MessageBoxIcon.Error
                    );

                    txtPassword.Clear();
                    txtPassword.Focus();
                    return;
                }

                MostrarMensajeTraducido(
                    "Mensajes.Login.IntegridadVulneradaAdmin",
                    "Se detectó una falla de integridad en la base de datos.\n\nIngresará como administrador para recalcular los dígitos verificadores y desbloquear usuarios.",
                    "Mensajes.Titulos.IntegridadVulnerada",
                    "Integridad vulnerada",
                    MessageBoxIcon.Warning
                );
            }

            ResultadoLogin resultadoLogin = _loginAppService.IniciarSesion(
                txtNombreUsuario.Text.Trim(),
                txtPassword.Text
            );

            if (resultadoLogin.ErrorAccesoDatos)
            {
                MostrarMensajeTraducido(
                    "Mensajes.Login.ErrorConexion",
                    "No fue posible conectarse con la base de datos. Verifique la conexión e intente nuevamente.",
                    "Mensajes.Titulos.ErrorConexion",
                    "Error de conexión",
                    MessageBoxIcon.Error
                );

                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            if (resultadoLogin.BloqueadoPorIntegridad)
            {
                MostrarMensajeTraducido(
                    "Mensajes.Login.UsuarioBloqueadoPorIntegridad",
                    "El usuario se encuentra bloqueado por una falla de integridad. Debe ingresar el administrador para recalcular los dígitos verificadores.",
                    "Mensajes.Titulos.AccesoBloqueadoIntegridad",
                    "Acceso bloqueado por integridad",
                    MessageBoxIcon.Error
                );

                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            if (resultadoLogin.UsuarioBloqueado)
            {
                string mensaje = MensajeTraducido.TraducirConFallback(
                    _idiomaAppService,
                    "Mensajes.Login.UsuarioBloqueado",
                    "El acceso fue bloqueado temporalmente por varios intentos fallidos. Intente nuevamente en unos minutos."
                );

                if (resultadoLogin.BloqueadoHasta.HasValue)
                {
                    TimeSpan tiempoRestante =
                        resultadoLogin.BloqueadoHasta.Value - DateTime.Now;

                    int minutosRestantes = Math.Max(
                        1,
                        (int)Math.Ceiling(tiempoRestante.TotalMinutes)
                    );

                    string plantillaMensaje = MensajeTraducido.TraducirConFallback(
                        _idiomaAppService,
                        "Mensajes.Login.UsuarioBloqueadoConMinutos",
                        "El acceso fue bloqueado temporalmente. Intente nuevamente en aproximadamente {0} minuto(s)."
                    );

                    mensaje = string.Format(plantillaMensaje, minutosRestantes);
                }

                string titulo = MensajeTraducido.TraducirConFallback(
                    _idiomaAppService,
                    "Mensajes.Titulos.AccesoBloqueado",
                    "Acceso bloqueado"
                );

                MessageBox.Show(
                    this,
                    mensaje,
                    titulo,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            if (!resultadoLogin.LoginExitoso)
            {
                MostrarMensajeTraducido(
                    "Mensajes.Login.CredencialesInvalidas",
                    "Usuario o contraseña incorrectos.",
                    "Mensajes.Titulos.Login",
                    "Login",
                    MessageBoxIcon.Warning
                );

                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            if (resultadoLogin.IdiomaPreferidoId.HasValue)
            {
                AplicarIdiomaPreferido(resultadoLogin.IdiomaPreferidoId);
            }
            else
            {
                Idioma idiomaSeleccionado = _idiomaAppService.IdiomaActual;

                if (idiomaSeleccionado != null)
                {
                    _gestionUsuariosAppService.GuardarIdiomaPreferidoUsuarioActual(idiomaSeleccionado.Id);
                }
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
                resultadoLogin.NombreUsuario,
                _idiomaAppService))
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
                _integridadService,
                _bitacoraService,
                _idiomaAppService,
                _gestionTraduccionesAppService,
                _gestionIdiomasAppService
            );

            this.Hide();

            // Acá el evento de cerrar con la X el formulario invoca el cierre de sesión
            // del usuario logueado o cierra toda la aplicación según corresponda.
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

        public void ActualizarIdioma()
        {
            TraductorControles.TraducirFormulario(this, _idiomaAppService);
        }

        private void FrmLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            _idiomaAppService.Desuscribir(this);
        }

        private void AplicarIdiomaPreferido(Guid? idiomaPreferidoId)
        {
            if (!idiomaPreferidoId.HasValue)
            {
                return;
            }

            List<Idioma> idiomas = _idiomaAppService.ListarIdiomasActivos();

            Idioma idiomaPreferido = idiomas.Find(
                idioma => idioma.Id == idiomaPreferidoId.Value
            );

            if (idiomaPreferido == null)
            {
                return;
            }

            _cargandoIdiomasLogin = true;
            cboIdiomaLogin.SelectedValue = idiomaPreferido.Codigo;
            _cargandoIdiomasLogin = false;

            _idiomaAppService.CambiarIdioma(idiomaPreferido.Codigo);
        }

        private void CargarIdiomasLogin()
        {
            _cargandoIdiomasLogin = true;

            List<Idioma> idiomas = _idiomaAppService.ListarIdiomasActivos();

            cboIdiomaLogin.DataSource = null;
            cboIdiomaLogin.DisplayMember = "Nombre";
            cboIdiomaLogin.ValueMember = "Codigo";
            cboIdiomaLogin.DataSource = idiomas;
            cboIdiomaLogin.Enabled = idiomas.Count > 0;

            if (idiomas.Count > 0)
            {
                if (ExisteIdioma(idiomas, "es-AR"))
                {
                    cboIdiomaLogin.SelectedValue = "es-AR";
                }
                else
                {
                    cboIdiomaLogin.SelectedIndex = 0;
                }
            }

            _cargandoIdiomasLogin = false;

            CambiarIdiomaDesdeLogin(false);
        }

        private bool ExisteIdioma(List<Idioma> idiomas, string codigo)
        {
            foreach (Idioma idioma in idiomas)
            {
                if (string.Equals(idioma.Codigo, codigo, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void cboIdiomaLogin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoIdiomasLogin)
            {
                return;
            }

            CambiarIdiomaDesdeLogin(true);
        }

        private void CambiarIdiomaDesdeLogin(bool mostrarMensajeError)
        {
            if (cboIdiomaLogin.SelectedValue == null)
            {
                return;
            }

            string codigo = cboIdiomaLogin.SelectedValue.ToString();

            ResultadoCambioIdioma resultado = _idiomaAppService.CambiarIdioma(codigo);

            if (!resultado.Exitoso && mostrarMensajeError)
            {
                MensajeTraducido.Mostrar(
                    this,
                    _idiomaAppService,
                    "Mensajes.Idioma.ErrorCambio",
                    resultado.Mensaje,
                    "Mensajes.Titulos.Idioma",
                    "Idioma",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
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

            cboIdiomaLogin.BackColor = TemaVisual.FondoInput;
            cboIdiomaLogin.ForeColor = TemaVisual.TextoPrincipal;
            cboIdiomaLogin.FlatStyle = FlatStyle.Flat;
            cboIdiomaLogin.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            TemaVisual.AplicarBotonPrincipal(btnIngresar);

            TemaVisual.AplicarTitulo(lblTitulo);
            TemaVisual.AplicarTextoSecundario(lblSubtitulo);
            TemaVisual.AplicarTextoSecundario(lblIdiomaLogin);
            TemaVisual.AplicarTextoSecundario(lblUsuario);
            TemaVisual.AplicarTextoSecundario(lblPassword);
            TemaVisual.AplicarTextoSecundario(lblMensajeSeguridad);

            CentrarPanelLogin();
        }        
    }
}
