using BLL.Idiomas;
using BLL.Usuarios;
using System;
using System.Windows.Forms;

namespace IDS_TPFinal
{
    public partial class FrmGestionUsuarios : Form
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

        // Se recibe el servicio de gestión de usuarios y, opcionalmente, el servicio de idioma.
        public FrmGestionUsuarios(
            GestionUsuariosAppService gestionUsuariosAppService,
            IIdiomaAppService idiomaAppService)
        {
            _gestionUsuariosAppService = gestionUsuariosAppService
                ?? throw new ArgumentNullException(nameof(gestionUsuariosAppService));

            _idiomaAppService = idiomaAppService;

            InitializeComponent();
            RegistrarEventos();
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
                    nombreUsuario
                );

            frmHistorialEmailUsuario.ShowDialog(this);

            // Se vuelve a cargar la grilla porque el formulario de historial
            // puede restaurar un email anterior y modificar el dato actual del usuario.
            CargarUsuarios();
        }
    }
}