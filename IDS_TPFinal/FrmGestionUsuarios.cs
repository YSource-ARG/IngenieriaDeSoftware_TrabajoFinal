using BLL.Idiomas;
using BLL.Usuarios;
using System;
using System.Windows.Forms;
using UI.Idiomas;

namespace IDS_TPFinal
{
    public partial class FrmGestionUsuarios : Form, IObservadorIdioma
    {
        private readonly GestionUsuariosAppService _gestionUsuariosAppService;
        private readonly IIdiomaAppService _idiomaAppService;
        private bool _cargandoUsuarios;
        private ModoFormularioUsuario _modoActual;

        private enum ModoFormularioUsuario
        {
            Alta,
            Consulta,
            Edicion
        }

        public FrmGestionUsuarios()
        {
            InitializeComponent();
        }

        public FrmGestionUsuarios(GestionUsuariosAppService gestionUsuariosAppService)
            : this(gestionUsuariosAppService, null)
        {
        }

        public FrmGestionUsuarios(
            GestionUsuariosAppService gestionUsuariosAppService,
            IIdiomaAppService idiomaAppService)
        {
            _gestionUsuariosAppService = gestionUsuariosAppService
                ?? throw new ArgumentNullException(nameof(gestionUsuariosAppService));

            _idiomaAppService = idiomaAppService;

            InitializeComponent();
            ConfigurarTagsTraduccion();
            RegistrarEventos();

            if (_idiomaAppService != null)
            {
                _idiomaAppService.Suscribir(this);
                ActualizarIdioma();
            }
        }

        private void ConfigurarTagsTraduccion()
        {
            this.Tag = "Usuarios.TituloVentana";

            lblTitulo.Tag = "Usuarios.Titulo";
            lblSubtitulo.Tag = "Usuarios.Subtitulo";
            lblDatosUsuario.Tag = "Usuarios.DatosUsuario";
            lblId.Tag = "Usuarios.Id";
            lblNombreUsuario.Tag = "Usuarios.NombreUsuario";
            lblNombreCompleto.Tag = "Usuarios.NombreCompleto";
            lblEmail.Tag = "Usuarios.Email";
            lblEstado.Tag = "Usuarios.Estado";
            lblPassword.Tag = "Usuarios.NuevaPassword";
            lblFechaCreacion.Tag = "Usuarios.FechaCreacion";
            lblFechaUltimoAcceso.Tag = "Usuarios.UltimoAcceso";
            lblAccionesRapidas.Tag = "Usuarios.AccionesRapidas";

            btnCerrar.Tag = "Usuarios.Cerrar";
            btnGuardar.Tag = "Usuarios.Guardar";
            btnCancelar.Tag = "Usuarios.Cancelar";

            btnNuevo.Tag = "Usuarios.Nuevo";
            btnEditar.Tag = "Usuarios.Editar";
            btnInhabilitarReactivar.Tag = "Usuarios.InhabilitarReactivar";
            btnRestablecerPassword.Tag = "Usuarios.RestablecerPassword";
            btnHistorialEmail.Tag = "Usuarios.HistorialEmail";
        }

        public void ActualizarIdioma()
        {
            if (_idiomaAppService == null)
            {
                return;
            }

            TraductorControles.TraducirFormulario(this, _idiomaAppService);
            ConfigurarEstadoCombo();

            if (_gestionUsuariosAppService != null && dgvUsuarios.DataSource != null)
            {
                CargarUsuarios();
            }

            ActualizarTextosDinamicos();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_idiomaAppService != null)
            {
                _idiomaAppService.Desuscribir(this);
            }

            base.OnFormClosed(e);
        }

        private void btnHistorialEmail_Click(object sender, EventArgs e)
        {
            if (!HayUsuarioSeleccionado())
            {
                MostrarAdvertencia(
                    "Mensajes.Usuarios.SeleccionarUsuarioHistorialEmail",
                    "Debe seleccionar un usuario para ver el historial de email.",
                    "Mensajes.Titulos.HistorialEmail",
                    "Historial de email"
                );

                return;
            }

            Guid idUsuario = ObtenerIdUsuarioSeleccionado();
            string nombreUsuario = txtNombreUsuario.Text;

            FrmHistorialEmailUsuario frmHistorialEmailUsuario =
                new FrmHistorialEmailUsuario(
                    _gestionUsuariosAppService,
                    idUsuario,
                    nombreUsuario,
                    _idiomaAppService
                );

            frmHistorialEmailUsuario.ShowDialog(this);

            CargarUsuarios();
        }
    }
}