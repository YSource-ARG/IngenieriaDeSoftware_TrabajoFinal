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
        private bool _cerrandoSesion;

        public bool CerrandoSesion => _cerrandoSesion;

        public FrmPrincipal(
            CerrarSesionAppService cerrarSesionAppService,
            GestionUsuariosAppService gestionUsuariosAppService,
            IIntegridadService integridadService,
            IBitacoraService bitacoraService,
            IIdiomaAppService idiomaAppService,
            GestionTraduccionesAppService gestionTraduccionesAppService,
            GestionIdiomasAppService gestionIdiomasAppService)
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

            _cerrarSesionAppService = cerrarSesionAppService;
            _gestionUsuariosAppService = gestionUsuariosAppService;
            _integridadService = integridadService;
            _bitacoraService = bitacoraService;
            _idiomaAppService = idiomaAppService;
            _gestionTraduccionesAppService = gestionTraduccionesAppService;
            _gestionIdiomasAppService = gestionIdiomasAppService;

            InitializeComponent();
            AplicarEstiloVisual();
            _idiomaAppService.Suscribir(this);
            ActualizarIdioma();

            this.FormClosed += FrmPrincipal_FormClosed;
        }

        public void ActualizarIdioma()
        {
            TraductorControles.TraducirFormulario(this, _idiomaAppService);
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

            ConfigurarBotonPrincipal(btnCerrarSesion);
            ConfigurarBotonSecundario(btnSalir);
            ConfigurarBotonSecundario(btnGestionUsuarios);
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
        // Al cerrarse el formulario principal, se decide si volver al login
        // porque el usuario cerró sesión, o cerrar toda la aplicación.
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
            FrmGestionUsuarios frmGestionUsuarios = new FrmGestionUsuarios(_gestionUsuariosAppService);
            MostrarFormularioDialogoCentrado(frmGestionUsuarios);
        }

        private void btnConsultaBitacora_Click(object sender, EventArgs e)
        {
            FrmConsultaBitacora frmConsultaBitacora = new FrmConsultaBitacora(_bitacoraService);
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
            DialogResult confirmacion = MessageBox.Show(
        "Esta acción recalculará los dígitos verificadores de usuarios.\n\n" +
        "Importante: el recálculo no restaura datos modificados por fuera del sistema, " +
        "sino que toma el estado actual de la base como nuevo estado válido.\n\n" +
        "¿Desea continuar?",
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
                // El administrador acepta el estado actual de la base
                // y genera nuevamente los DVH de cada usuario y el DVV general.
                _integridadService.RecalcularDigitosUsuarios(
                    null,
                    "admin"
                );

                MessageBox.Show(
                    "Los dígitos verificadores fueron recalculados correctamente.",
                    "Integridad",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible recalcular los dígitos verificadores.\n\n" + ex.Message,
                    "Error de integridad",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnDesbloquearIntegridad_Click(object sender, EventArgs e)
        {
            DialogResult confirmacion = MessageBox.Show(
        "Esta acción desbloqueará a los usuarios que fueron bloqueados por falla de integridad.\n\n" +
        "Se recomienda hacerlo después de recalcular los dígitos verificadores.\n\n" +
        "¿Desea continuar?",
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
                // El desbloqueo solo limpia el bloqueo preventivo por integridad.
                // No modifica el campo Activo, porque ese estado pertenece
                // a la administración normal de usuarios.
                _integridadService.DesbloquearUsuariosPorIntegridad(
                    null,
                    "admin"
                );

                MessageBox.Show(
                    "Los usuarios bloqueados por integridad fueron desbloqueados correctamente.",
                    "Integridad",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible desbloquear los usuarios por integridad.\n\n" + ex.Message,
                    "Error de integridad",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnGestionTraducciones_Click(object sender, EventArgs e)
        {
            FrmGestionTraducciones frmGestionTraducciones = new FrmGestionTraducciones(
                _gestionTraduccionesAppService,
                _idiomaAppService
            );

            MostrarFormularioDialogoCentrado(frmGestionTraducciones);
        }

        private void btnGestionIdiomas_Click(object sender, EventArgs e)
        {
            FrmGestionIdiomas frmGestionIdiomas = new FrmGestionIdiomas(
                _gestionIdiomasAppService,
                _idiomaAppService
            );

            MostrarFormularioDialogoCentrado(frmGestionIdiomas);
        }
    }
}
