using BLL.Usuarios;
using System;
using System.Windows.Forms;

namespace IDS_TPFinal
{
    public partial class FrmGestionUsuarios : Form
    {
        private readonly GestionUsuariosAppService _gestionUsuariosAppService;

        public FrmGestionUsuarios()
        {
            InitializeComponent();
        }

        public FrmGestionUsuarios(GestionUsuariosAppService gestionUsuariosAppService)
        {
            _gestionUsuariosAppService = gestionUsuariosAppService
                ?? throw new ArgumentNullException(nameof(gestionUsuariosAppService));

            InitializeComponent();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Formulario conectado. Próximo paso: guardar usuario desde BLL.");
        }
    }
}