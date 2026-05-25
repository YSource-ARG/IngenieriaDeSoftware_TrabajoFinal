using BLL.Usuarios;
using System;
using System.Windows.Forms;
using System.Linq;

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
        private void CargarUsuarios()
        {
            if (_gestionUsuariosAppService == null)
            {
                return;
            }

            var usuarios = _gestionUsuariosAppService.ListarUsuarios(null, null);

            dgvUsuarios.DataSource = usuarios
                .Select(usuario => new
                {
                    usuario.Id,
                    usuario.NombreUsuario,
                    usuario.NombreCompleto,
                    Estado = usuario.Activo ? "Activo" : "Inactivo",
                    FechaCreacion = usuario.FechaCreacion.ToString("dd/MM/yyyy HH:mm"),
                    UltimoAcceso = usuario.FechaUltimoAcceso.HasValue
                        ? usuario.FechaUltimoAcceso.Value.ToString("dd/MM/yyyy HH:mm")
                        : "-"
                })
                .ToList();

            ConfigurarColumnasUsuarios();
        }
        private void ConfigurarColumnasUsuarios()
        {
            if (dgvUsuarios.Columns.Count == 0)
            {
                return;
            }

            dgvUsuarios.Columns["Id"].Visible = false;

            dgvUsuarios.Columns["NombreUsuario"].HeaderText = "Usuario";
            dgvUsuarios.Columns["NombreCompleto"].HeaderText = "Nombre completo";
            dgvUsuarios.Columns["Estado"].HeaderText = "Estado";
            dgvUsuarios.Columns["FechaCreacion"].HeaderText = "Fecha creación";
            dgvUsuarios.Columns["UltimoAcceso"].HeaderText = "Último acceso";

            dgvUsuarios.Columns["NombreUsuario"].FillWeight = 90;
            dgvUsuarios.Columns["NombreCompleto"].FillWeight = 150;
            dgvUsuarios.Columns["Estado"].FillWeight = 70;
            dgvUsuarios.Columns["FechaCreacion"].FillWeight = 110;
            dgvUsuarios.Columns["UltimoAcceso"].FillWeight = 110;
        }
    }
}