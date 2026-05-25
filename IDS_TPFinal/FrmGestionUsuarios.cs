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

            dgvUsuarios.SelectionChanged += dgvUsuarios_SelectionChanged;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_gestionUsuariosAppService == null)
                {
                    MessageBox.Show("El formulario no recibió el servicio de gestión de usuarios.", "Error de configuración", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                string nombreUsuario = txtNombreUsuario.Text;
                string nombreCompleto = txtNombreCompleto.Text;
                string passwordInicial = txtPassword.Text;
                bool activo = cmbEstado.SelectedItem?.ToString() == "Activo";

                _gestionUsuariosAppService.CrearUsuario(nombreUsuario, nombreCompleto, passwordInicial, activo);

                MessageBox.Show("Usuario creado correctamente.", "Gestión de usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtId.Clear();
                txtNombreUsuario.Clear();
                txtNombreCompleto.Clear();
                txtPassword.Clear();
                txtFechaCreacion.Clear();
                txtFechaUltimoAcceso.Clear();

                if (cmbEstado.Items.Count > 0)
                {
                    cmbEstado.SelectedIndex = 0;
                }

                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo guardar el usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            CargarUsuarioSeleccionadoEnFormulario();
        }

        private void CargarUsuarioSeleccionadoEnFormulario()
        {
            if (dgvUsuarios.CurrentRow == null)
            {
                return;
            }

            txtId.Text = Convert.ToString(dgvUsuarios.CurrentRow.Cells["Id"].Value);
            txtNombreUsuario.Text = Convert.ToString(dgvUsuarios.CurrentRow.Cells["NombreUsuario"].Value);
            txtNombreCompleto.Text = Convert.ToString(dgvUsuarios.CurrentRow.Cells["NombreCompleto"].Value);
            txtFechaCreacion.Text = Convert.ToString(dgvUsuarios.CurrentRow.Cells["FechaCreacion"].Value);
            txtFechaUltimoAcceso.Text = Convert.ToString(dgvUsuarios.CurrentRow.Cells["UltimoAcceso"].Value);

            string estado = Convert.ToString(dgvUsuarios.CurrentRow.Cells["Estado"].Value);

            if (estado == "Activo" || estado == "Inactivo")
            {
                cmbEstado.SelectedItem = estado;
            }

            txtPassword.Clear();
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