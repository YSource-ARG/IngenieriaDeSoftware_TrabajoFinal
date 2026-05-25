using BLL.Autenticacion;
using IDS_TPFinal;
using System;
using System.Drawing;
using System.Windows.Forms;
using UI.Estilos;
using BLL.Usuarios;

namespace UI
{
    public partial class FrmPrincipal : Form
    {
        private readonly CerrarSesionAppService _cerrarSesionAppService;
        private readonly GestionUsuariosAppService _gestionUsuariosAppService;
        private bool _cerrandoSesion;

        public bool CerrandoSesion => _cerrandoSesion;

        public FrmPrincipal(CerrarSesionAppService cerrarSesionAppService, GestionUsuariosAppService gestionUsuariosAppService)
        {
            if (cerrarSesionAppService == null)
            {
                throw new ArgumentNullException(nameof(cerrarSesionAppService));
            }

            if (gestionUsuariosAppService == null)
            {
                throw new ArgumentNullException(nameof(gestionUsuariosAppService));
            }

            _cerrarSesionAppService = cerrarSesionAppService;
            _gestionUsuariosAppService = gestionUsuariosAppService;

            InitializeComponent();
            AplicarEstiloVisual();
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
            FrmGestionUsuarios frmGestionUsuarios = new FrmGestionUsuarios(_gestionUsuariosAppService);

            frmGestionUsuarios.StartPosition = FormStartPosition.Manual;

            frmGestionUsuarios.Shown += (s, args) =>
            {
                Screen pantalla = Screen.FromControl(this);
                Rectangle areaTrabajo = pantalla.WorkingArea;

                int x = areaTrabajo.Left + (areaTrabajo.Width - frmGestionUsuarios.Width) / 2;
                int y = areaTrabajo.Top + (areaTrabajo.Height - frmGestionUsuarios.Height) / 2;

                frmGestionUsuarios.Location = new Point(x, y);
            };

            frmGestionUsuarios.ShowDialog(this);
        }
    }
}