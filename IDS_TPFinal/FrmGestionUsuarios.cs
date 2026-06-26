using BLL.Idiomas;
using BLL.Permisos;
using BLL.Usuarios;
using System;
using System.Windows.Forms;
using UI;
using UI.Idiomas;

namespace IDS_TPFinal
{
    public partial class FrmGestionUsuarios : Form, IObservadorIdioma
    {
        private readonly GestionUsuariosAppService _gestionUsuariosAppService;
        private readonly GestionPermisosAppService _gestionPermisosAppService;
        private readonly AutorizacionService _autorizacionService;
        private readonly IIdiomaAppService _idiomaAppService;
        private bool _cargandoUsuarios;
        private ModoFormularioUsuario _modoActual;
        private Button _btnPermisos;

        private enum ModoFormularioUsuario
        {
            Alta,
            Consulta,
            Edicion
        }

        public FrmGestionUsuarios()
        {
            InitializeComponent();
            InicializarBotonPermisos();
        }

        public FrmGestionUsuarios(GestionUsuariosAppService gestionUsuariosAppService)
            : this(gestionUsuariosAppService, null, null, null)
        {
        }

        public FrmGestionUsuarios(
            GestionUsuariosAppService gestionUsuariosAppService,
            GestionPermisosAppService gestionPermisosAppService,
            AutorizacionService autorizacionService,
            IIdiomaAppService idiomaAppService)
        {
            _gestionUsuariosAppService = gestionUsuariosAppService
                ?? throw new ArgumentNullException(nameof(gestionUsuariosAppService));

            _gestionPermisosAppService = gestionPermisosAppService
                ?? throw new ArgumentNullException(nameof(gestionPermisosAppService));

            _autorizacionService = autorizacionService
                ?? throw new ArgumentNullException(nameof(autorizacionService));

            _idiomaAppService = idiomaAppService;

            InitializeComponent();
            InicializarBotonPermisos();
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
            _btnPermisos.Tag = "Usuarios.Permisos";
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

        private void InicializarBotonPermisos()
        {
            if (_btnPermisos != null)
            {
                return;
            }

            _btnPermisos = new Button
            {
                Name = "btnPermisos",
                Size = new System.Drawing.Size(145, 84),
                Location = new System.Drawing.Point(1179, 367),
                Text = "Permisos",
                UseVisualStyleBackColor = true
            };

            panelContenido.Controls.Add(_btnPermisos);
            _btnPermisos.BringToFront();
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
            if (!ValidarPermiso(
                PermisosSistema.UsuariosVerHistorialEmail,
                "No tenés permisos para consultar el historial de email."))
            {
                return;
            }

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

        private void btnPermisos_Click(object sender, EventArgs e)
        {
            if (!ValidarPermiso(
                PermisosSistema.UsuariosAsignarPermisos,
                "No tenés permisos para asignar componentes de permisos a usuarios."))
            {
                return;
            }

            if (!HayUsuarioSeleccionado())
            {
                MostrarAdvertencia(
                    null,
                    "Debe seleccionar un usuario para asignarle permisos.",
                    "Mensajes.Titulos.UsuarioNoSeleccionado",
                    "Usuario no seleccionado"
                );

                return;
            }

            FrmAsignacionPermisosUsuario frmAsignacion =
                new FrmAsignacionPermisosUsuario(
                    _gestionPermisosAppService,
                    ObtenerIdUsuarioSeleccionado(),
                    txtNombreUsuario.Text,
                    _idiomaAppService
                );

            frmAsignacion.ShowDialog(this);
        }

        private bool ValidarPermiso(string codigoPermiso, string mensaje)
        {
            if (_autorizacionService.UsuarioActualTienePermiso(codigoPermiso))
            {
                return true;
            }

            MostrarAdvertencia(
                null,
                mensaje,
                "Mensajes.Titulos.SinPermiso",
                "Sin permiso"
            );

            return false;
        }
    }
}
