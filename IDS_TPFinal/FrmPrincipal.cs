using BE;
using BLL.Autenticacion;
using BLL.Bitacora;
using IDS_TPFinal;
using System;
using System.Drawing;
using System.Windows.Forms;
using UI.Estilos;
using BLL.Usuarios;
using BLL.Integridad;
using BLL.Idiomas;
using UI.Idiomas;
using BLL.Permisos;

namespace UI
{
    public partial class FrmPrincipal : Form, IObservadorIdioma
    {
        private readonly CerrarSesionAppService _cerrarSesionAppService;
        private readonly GestionUsuariosAppService _gestionUsuariosAppService;
        private readonly IIntegridadService _integridadService;
        private readonly IBitacoraService _bitacoraService;
        private readonly IIdiomaAppService _idiomaAppService;
        private readonly GestionTraduccionesAppService _gestionTraduccionesAppService;
        private readonly GestionIdiomasAppService _gestionIdiomasAppService;
        private readonly GestionPermisosAppService _gestionPermisosAppService;
        private readonly AutorizacionService _autorizacionService;

        private bool _cerrandoSesion;
        private bool _cargandoIdiomasPrincipal;

        public bool CerrandoSesion => _cerrandoSesion;

        public FrmPrincipal(
            CerrarSesionAppService cerrarSesionAppService,
            GestionUsuariosAppService gestionUsuariosAppService,
            IIntegridadService integridadService,
            IBitacoraService bitacoraService,
            IIdiomaAppService idiomaAppService,
            GestionTraduccionesAppService gestionTraduccionesAppService,
            GestionIdiomasAppService gestionIdiomasAppService,
            GestionPermisosAppService gestionPermisosAppService,
            AutorizacionService autorizacionService)
        {
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

            if (gestionPermisosAppService == null)
            {
                throw new ArgumentNullException(nameof(gestionPermisosAppService));
            }

            if (autorizacionService == null)
            {
                throw new ArgumentNullException(nameof(autorizacionService));
            }

            _cerrarSesionAppService = cerrarSesionAppService;
            _gestionUsuariosAppService = gestionUsuariosAppService;
            _integridadService = integridadService;
            _bitacoraService = bitacoraService;
            _idiomaAppService = idiomaAppService;
            _gestionTraduccionesAppService = gestionTraduccionesAppService;
            _gestionIdiomasAppService = gestionIdiomasAppService;
            _gestionPermisosAppService = gestionPermisosAppService;
            _autorizacionService = autorizacionService;

            InitializeComponent();
            AplicarEstiloVisual();

            CargarIdiomasPrincipal();
            cboIdiomaPrincipal.SelectedIndexChanged += cboIdiomaPrincipal_SelectedIndexChanged;

            _idiomaAppService.Suscribir(this);
            ActualizarIdioma();
            ConfigurarAccesosPorPermiso();

            this.FormClosed += FrmPrincipal_FormClosed;
        }

        public void ActualizarIdioma()
        {
            TraductorControles.TraducirFormulario(this, _idiomaAppService);
            SeleccionarIdiomaActualEnCombo();
        }

        private void CargarIdiomasPrincipal()
        {
            _cargandoIdiomasPrincipal = true;

            try
            {
                var idiomas = _idiomaAppService.ListarIdiomasActivos();

                cboIdiomaPrincipal.DataSource = null;
                cboIdiomaPrincipal.DisplayMember = "Nombre";
                cboIdiomaPrincipal.ValueMember = "Codigo";
                cboIdiomaPrincipal.DataSource = idiomas;
                cboIdiomaPrincipal.Enabled = idiomas.Count > 0;

                SeleccionarIdiomaActualEnCombo();
            }
            finally
            {
                _cargandoIdiomasPrincipal = false;
            }
        }

        private void SeleccionarIdiomaActualEnCombo()
        {
            if (_idiomaAppService == null || _idiomaAppService.IdiomaActual == null)
            {
                return;
            }

            if (cboIdiomaPrincipal == null)
            {
                return;
            }

            bool estadoAnterior = _cargandoIdiomasPrincipal;
            _cargandoIdiomasPrincipal = true;

            try
            {
                foreach (object item in cboIdiomaPrincipal.Items)
                {
                    Idioma idioma = item as Idioma;

                    if (idioma != null && idioma.Id == _idiomaAppService.IdiomaActual.Id)
                    {
                        cboIdiomaPrincipal.SelectedItem = idioma;
                        return;
                    }
                }
            }
            finally
            {
                _cargandoIdiomasPrincipal = estadoAnterior;
            }
        }

        private void cboIdiomaPrincipal_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoIdiomasPrincipal)
            {
                return;
            }

            Idioma idiomaSeleccionado = cboIdiomaPrincipal.SelectedItem as Idioma;

            if (idiomaSeleccionado == null)
            {
                return;
            }

            if (_idiomaAppService.IdiomaActual != null && _idiomaAppService.IdiomaActual.Id == idiomaSeleccionado.Id)
            {
                return;
            }

            ResultadoCambioIdioma resultado = _idiomaAppService.CambiarIdioma(idiomaSeleccionado.Codigo);

            if (!resultado.Exitoso)
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

                return;
            }

            try
            {
                _gestionUsuariosAppService.GuardarIdiomaPreferidoUsuarioActual(idiomaSeleccionado.Id);
            }
            catch (Exception)
            {
                MensajeTraducido.Mostrar(
                    this,
                    _idiomaAppService,
                    "Mensajes.Idioma.ErrorGuardarPreferencia",
                    "El idioma fue cambiado, pero no se pudo guardar como preferencia del usuario.",
                    "Mensajes.Titulos.Idioma",
                    "Idioma",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void FrmPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
            _idiomaAppService.Desuscribir(this);
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            ConfigurarAreaMdi();
        }

        private void AplicarEstiloVisual()
        {
            TemaVisual.AplicarFormularioOscuro(this);

            this.Text = "Panel principal";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1100, 700);

            panelHeader.BackColor = Color.FromArgb(18, 22, 28);
            panelLateral.BackColor = Color.FromArgb(22, 27, 34);
            panelContenido.BackColor = Color.FromArgb(13, 17, 23);
            panelTarjetaResumen.BackColor = Color.FromArgb(22, 27, 34);

            lblMarca.ForeColor = TemaVisual.AzulPrincipal;
            lblMarca.Font = new Font("Segoe UI", 24F, FontStyle.Bold);

            lblTitulo.ForeColor = Color.FromArgb(240, 246, 252);
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);

            lblSubtitulo.ForeColor = TemaVisual.TextoSecundario;
            lblSubtitulo.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            lblSeccion.ForeColor = Color.FromArgb(240, 246, 252);
            lblSeccion.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            lblBienvenida.ForeColor = Color.FromArgb(240, 246, 252);
            lblBienvenida.Font = new Font("Segoe UI", 24F, FontStyle.Bold);

            lblDescripcion.ForeColor = TemaVisual.TextoSecundario;
            lblDescripcion.Font = new Font("Segoe UI", 11F, FontStyle.Regular);

            lblResumenTitulo.ForeColor = Color.FromArgb(240, 246, 252);
            lblResumenTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);

            lblResumenTexto.ForeColor = TemaVisual.TextoSecundario;
            lblResumenTexto.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            lblIdiomaPrincipal.ForeColor = TemaVisual.TextoSecundario;
            lblIdiomaPrincipal.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            cboIdiomaPrincipal.DropDownStyle = ComboBoxStyle.DropDownList;
            cboIdiomaPrincipal.BackColor = TemaVisual.FondoInput;
            cboIdiomaPrincipal.ForeColor = TemaVisual.TextoPrincipal;
            cboIdiomaPrincipal.FlatStyle = FlatStyle.Flat;
            cboIdiomaPrincipal.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            ConfigurarBotonPrincipal(btnCerrarSesion);
            ConfigurarBotonSecundario(btnSalir);
            ConfigurarBotonSecundario(btnGestionUsuarios);
            ConfigurarBotonSecundario(btnGestionRoles);
            ConfigurarBotonSecundario(btnConsultaBitacora);
            ConfigurarBotonSecundario(btnGestionTraducciones);
            ConfigurarBotonSecundario(btnRecalcularDigitos);
            ConfigurarBotonSecundario(btnDesbloquearIntegridad);
            ConfigurarBotonSecundario(btnGestionIdiomas);
        }

        private void ConfigurarAreaMdi()
        {
            foreach (Control control in this.Controls)
            {
                if (control is MdiClient mdiClient)
                {
                    mdiClient.BackColor = Color.FromArgb(13, 17, 23);
                }
            }
        }

        private void ConfigurarBotonPrincipal(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = TemaVisual.AzulPrincipal;
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            boton.Cursor = Cursors.Hand;
        }

        private void ConfigurarBotonSecundario(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 1;
            boton.FlatAppearance.BorderColor = Color.FromArgb(48, 54, 61);
            boton.BackColor = Color.FromArgb(22, 27, 34);
            boton.ForeColor = Color.FromArgb(240, 246, 252);
            boton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            boton.Cursor = Cursors.Hand;
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            _cerrarSesionAppService.CerrarSesion();
            _cerrandoSesion = true;
            this.Close();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGestionUsuarios_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisosSistema.UsuariosGestionar, "No tenés permisos para gestionar usuarios."))
            {
                return;
            }

            FrmGestionUsuarios frmGestionUsuarios = new FrmGestionUsuarios(
                _gestionUsuariosAppService,
                _gestionPermisosAppService,
                _autorizacionService,
                _idiomaAppService
            );

            MostrarFormularioDialogoCentrado(frmGestionUsuarios);
        }

        private void btnGestionRoles_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisosSistema.UsuariosAsignarPermisos, "No tenés permisos para gestionar roles."))
            {
                return;
            }

            FrmGestionRoles frmGestionRoles = new FrmGestionRoles(
                _gestionPermisosAppService,
                _idiomaAppService
            );

            MostrarFormularioDialogoCentrado(frmGestionRoles);
        }

        private void btnConsultaBitacora_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisosSistema.BitacoraConsultar, "No tenés permisos para consultar la bitácora."))
            {
                return;
            }

            FrmConsultaBitacora frmConsultaBitacora = new FrmConsultaBitacora(
                _bitacoraService,
                _idiomaAppService
            );

            MostrarFormularioDialogoCentrado(frmConsultaBitacora);
        }

        private void MostrarFormularioDialogoCentrado(Form formulario)
        {
            formulario.StartPosition = FormStartPosition.Manual;

            formulario.Shown += (s, args) =>
            {
                Screen pantalla = Screen.FromControl(this);
                Rectangle areaTrabajo = pantalla.WorkingArea;

                int x = areaTrabajo.Left + (areaTrabajo.Width - formulario.Width) / 2;
                int y = areaTrabajo.Top + (areaTrabajo.Height - formulario.Height) / 2;

                formulario.Location = new Point(x, y);
            };

            formulario.ShowDialog(this);
        }

        private void btnRecalcularDigitos_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisosSistema.IntegridadGestionar, "No tenés permisos para gestionar integridad."))
            {
                return;
            }

            DialogResult confirmacion = MensajeTraducido.Mostrar(
                this,
                _idiomaAppService,
                "Mensajes.Principal.ConfirmarRecalcularDigitos",
                "Esta acción recalculará los dígitos verificadores de usuarios.\n\nImportante: el recálculo no restaura datos modificados por fuera del sistema, sino que toma el estado actual de la base como nuevo estado válido.\n\n¿Desea continuar?",
                "Mensajes.Titulos.RecalcularDigitos",
                "Recalcular dígitos verificadores",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _integridadService.RecalcularDigitosUsuarios(
                    null,
                    "admin"
                );

                MensajeTraducido.Mostrar(
                    this,
                    _idiomaAppService,
                    "Mensajes.Principal.RecalculoDigitosCorrecto",
                    "Los dígitos verificadores fueron recalculados correctamente.",
                    "Mensajes.Titulos.Integridad",
                    "Integridad",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception)
            {
                MensajeTraducido.Mostrar(
                    this,
                    _idiomaAppService,
                    "Mensajes.Principal.ErrorRecalcularDigitos",
                    "No fue posible recalcular los dígitos verificadores. Verifique el estado de la base e intente nuevamente.",
                    "Mensajes.Titulos.ErrorIntegridad",
                    "Error de integridad",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnDesbloquearIntegridad_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisosSistema.IntegridadGestionar, "No tenés permisos para gestionar integridad."))
            {
                return;
            }

            DialogResult confirmacion = MensajeTraducido.Mostrar(
                this,
                _idiomaAppService,
                "Mensajes.Principal.ConfirmarDesbloquearIntegridad",
                "Esta acción desbloqueará a los usuarios que fueron bloqueados por falla de integridad.\n\nSe recomienda hacerlo después de recalcular los dígitos verificadores.\n\n¿Desea continuar?",
                "Mensajes.Titulos.DesbloquearIntegridad",
                "Desbloquear usuarios por integridad",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _integridadService.DesbloquearUsuariosPorIntegridad(
                    null,
                    "admin"
                );

                MensajeTraducido.Mostrar(
                    this,
                    _idiomaAppService,
                    "Mensajes.Principal.DesbloqueoIntegridadCorrecto",
                    "Los usuarios bloqueados por integridad fueron desbloqueados correctamente.",
                    "Mensajes.Titulos.Integridad",
                    "Integridad",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception)
            {
                MensajeTraducido.Mostrar(
                    this,
                    _idiomaAppService,
                    "Mensajes.Principal.ErrorDesbloquearIntegridad",
                    "No fue posible desbloquear los usuarios por integridad. Verifique el estado de la base e intente nuevamente.",
                    "Mensajes.Titulos.ErrorIntegridad",
                    "Error de integridad",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnGestionTraducciones_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisosSistema.TraduccionesGestionar, "No tenés permisos para gestionar traducciones."))
            {
                return;
            }

            FrmGestionTraducciones frmGestionTraducciones = new FrmGestionTraducciones(
                _gestionTraduccionesAppService,
                _idiomaAppService
            );

            MostrarFormularioDialogoCentrado(frmGestionTraducciones);
        }

        private void btnGestionIdiomas_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(PermisosSistema.IdiomasGestionar, "No tenés permisos para gestionar idiomas."))
            {
                return;
            }

            FrmGestionIdiomas frmGestionIdiomas = new FrmGestionIdiomas(
                _gestionIdiomasAppService,
                _idiomaAppService
            );

            MostrarFormularioDialogoCentrado(frmGestionIdiomas);
        }

        private void ConfigurarAccesosPorPermiso()
        {
            btnGestionUsuarios.Enabled = _autorizacionService.UsuarioActualTienePermiso(PermisosSistema.UsuariosGestionar);
            btnGestionRoles.Enabled = _autorizacionService.UsuarioActualTienePermiso(PermisosSistema.UsuariosAsignarPermisos);
            btnConsultaBitacora.Enabled = _autorizacionService.UsuarioActualTienePermiso(PermisosSistema.BitacoraConsultar);
            btnRecalcularDigitos.Enabled = _autorizacionService.UsuarioActualTienePermiso(PermisosSistema.IntegridadGestionar);
            btnDesbloquearIntegridad.Enabled = _autorizacionService.UsuarioActualTienePermiso(PermisosSistema.IntegridadGestionar);
            btnGestionTraducciones.Enabled = _autorizacionService.UsuarioActualTienePermiso(PermisosSistema.TraduccionesGestionar);
            btnGestionIdiomas.Enabled = _autorizacionService.UsuarioActualTienePermiso(PermisosSistema.IdiomasGestionar);
        }

        private bool ValidarPermiso(string codigoPermiso, string mensaje)
        {
            if (_autorizacionService.UsuarioActualTienePermiso(codigoPermiso))
            {
                return true;
            }

            MessageBox.Show(
                this,
                mensaje,
                "Permisos",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return false;
        }
    }
}
