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

        public FrmGestionUsuarios(GestionUsuariosAppService gestionUsuariosAppService)
        {
            _gestionUsuariosAppService = gestionUsuariosAppService
                ?? throw new ArgumentNullException(nameof(gestionUsuariosAppService));

            InitializeComponent();
            RegistrarEventos();
        }
    }
}
