using BLL.Usuarios;
using System;
using System.Windows.Forms;

namespace IDS_TPFinal
{
    public partial class FrmGestionUsuarios : Form
    {
        private readonly GestionUsuariosAppService _gestionUsuariosAppService;
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

        // Se recibe el servicio de gestión de usuarios (creado por factory) 
        public FrmGestionUsuarios(GestionUsuariosAppService gestionUsuariosAppService)
        {
            _gestionUsuariosAppService = gestionUsuariosAppService
                ?? throw new ArgumentNullException(nameof(gestionUsuariosAppService));

            InitializeComponent();
            RegistrarEventos();
        }

        private void btnHistorialEmail_Click(object sender, EventArgs e)
        {
            if (!HayUsuarioSeleccionado())
            {
                MostrarAdvertencia(
                    "Debe seleccionar un usuario para ver el historial de email.",
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
