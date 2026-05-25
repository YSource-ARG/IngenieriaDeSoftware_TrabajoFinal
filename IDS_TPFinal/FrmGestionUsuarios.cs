using BLL.Usuarios;
using System;
using System.Linq;
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

        private void RegistrarEventos()
        {
            dgvUsuarios.SelectionChanged += dgvUsuarios_SelectionChanged;
            btnNuevo.Click += btnNuevo_Click;
            btnEditar.Click += btnEditar_Click;
            btnCancelar.Click += btnCancelar_Click;
            btnCerrar.Click += btnCerrar_Click;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_gestionUsuariosAppService == null)
                {
                    MostrarError(
                        "El formulario no recibió el servicio de gestión de usuarios.",
                        "Error de configuración"
                    );

                    return;
                }

                if (_modoActual == ModoFormularioUsuario.Alta)
                {
                    CrearUsuario();
                    return;
                }

                if (_modoActual == ModoFormularioUsuario.Edicion)
                {
                    ModificarUsuario();
                    return;
                }

                MostrarAdvertencia(
                    "Para modificar un usuario, primero seleccioná un registro y presioná Editar.",
                    "Operación no disponible"
                );
            }
            catch (Exception ex)
            {
                MostrarAdvertencia(ex.Message, "No se pudo guardar el usuario");
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            PrepararAltaUsuario();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (!HayUsuarioSeleccionado())
            {
                MostrarAdvertencia(
                    "Seleccioná un usuario de la grilla antes de editar.",
                    "Usuario no seleccionado"
                );

                return;
            }

            AplicarModoEdicion();
            txtNombreUsuario.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            PrepararAltaUsuario();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (_cargandoUsuarios)
            {
                return;
            }

            CargarUsuarioSeleccionadoEnFormulario();
        }

        private void CrearUsuario()
        {
            _gestionUsuariosAppService.CrearUsuario(
                txtNombreUsuario.Text,
                txtNombreCompleto.Text,
                txtPassword.Text,
                ObtenerEstadoSeleccionado()
            );

            MessageBox.Show(
                "Usuario creado correctamente.",
                "Gestión de usuarios",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            CargarUsuarios();
        }

        private void ModificarUsuario()
        {
            Guid idUsuario = ObtenerIdUsuarioSeleccionado();

            _gestionUsuariosAppService.ModificarUsuario(
                idUsuario,
                txtNombreUsuario.Text,
                txtNombreCompleto.Text,
                ObtenerEstadoSeleccionado()
            );

            MessageBox.Show(
                "Usuario modificado correctamente.",
                "Gestión de usuarios",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            if (_gestionUsuariosAppService == null)
            {
                return;
            }

            _cargandoUsuarios = true;

            try
            {
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
                LimpiarSeleccionGrilla();
                LimpiarCampos();
                AplicarModoAlta();
            }
            finally
            {
                _cargandoUsuarios = false;
            }
        }

        private void CargarUsuarioSeleccionadoEnFormulario()
        {
            if (dgvUsuarios.CurrentRow == null)
            {
                return;
            }

            txtId.Text = ObtenerValorCeldaActual("Id");
            txtNombreUsuario.Text = ObtenerValorCeldaActual("NombreUsuario");
            txtNombreCompleto.Text = ObtenerValorCeldaActual("NombreCompleto");
            txtFechaCreacion.Text = ObtenerValorCeldaActual("FechaCreacion");
            txtFechaUltimoAcceso.Text = ObtenerValorCeldaActual("UltimoAcceso");

            string estado = ObtenerValorCeldaActual("Estado");

            if (estado == "Activo" || estado == "Inactivo")
            {
                cmbEstado.SelectedItem = estado;
            }

            txtPassword.Clear();
            AplicarModoConsulta();
        }

        private void ConfigurarColumnasUsuarios()
        {
            if (dgvUsuarios.Columns.Count == 0 || !dgvUsuarios.Columns.Contains("Id"))
            {
                return;
            }

            dgvUsuarios.Columns["Id"].Visible = false;

            ConfigurarColumna("NombreUsuario", "Usuario", 90);
            ConfigurarColumna("NombreCompleto", "Nombre completo", 150);
            ConfigurarColumna("Estado", "Estado", 70);
            ConfigurarColumna("FechaCreacion", "Fecha creación", 110);
            ConfigurarColumna("UltimoAcceso", "Último acceso", 110);
        }

        private void ConfigurarColumna(string nombreColumna, string textoCabecera, float anchoRelativo)
        {
            if (!dgvUsuarios.Columns.Contains(nombreColumna))
            {
                return;
            }

            dgvUsuarios.Columns[nombreColumna].HeaderText = textoCabecera;
            dgvUsuarios.Columns[nombreColumna].FillWeight = anchoRelativo;
        }

        private void PrepararAltaUsuario()
        {
            _cargandoUsuarios = true;

            try
            {
                LimpiarSeleccionGrilla();
                LimpiarCampos();
                AplicarModoAlta();
            }
            finally
            {
                _cargandoUsuarios = false;
            }

            txtNombreUsuario.Focus();
        }

        private void AplicarModoAlta()
        {
            _modoActual = ModoFormularioUsuario.Alta;

            txtNombreUsuario.ReadOnly = false;
            txtNombreCompleto.ReadOnly = false;
            txtPassword.ReadOnly = false;
            cmbEstado.Enabled = true;

            btnGuardar.Enabled = true;
            btnEditar.Enabled = false;

            lblPassword.Text = "Nueva contraseña";
        }

        private void AplicarModoConsulta()
        {
            _modoActual = ModoFormularioUsuario.Consulta;

            txtNombreUsuario.ReadOnly = true;
            txtNombreCompleto.ReadOnly = true;
            txtPassword.ReadOnly = true;
            cmbEstado.Enabled = false;

            btnGuardar.Enabled = false;
            btnEditar.Enabled = true;

            lblPassword.Text = "Nueva contraseña";
        }

        private void AplicarModoEdicion()
        {
            _modoActual = ModoFormularioUsuario.Edicion;

            txtNombreUsuario.ReadOnly = false;
            txtNombreCompleto.ReadOnly = false;
            txtPassword.ReadOnly = true;
            cmbEstado.Enabled = true;

            btnGuardar.Enabled = true;
            btnEditar.Enabled = false;

            txtPassword.Clear();
            lblPassword.Text = "Nueva contraseña";
        }

        private void LimpiarCampos()
        {
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
        }

        private void LimpiarSeleccionGrilla()
        {
            dgvUsuarios.ClearSelection();

            if (dgvUsuarios.Rows.Count > 0)
            {
                dgvUsuarios.CurrentCell = null;
            }
        }

        private bool HayUsuarioSeleccionado()
        {
            return Guid.TryParse(txtId.Text, out Guid idUsuario) && idUsuario != Guid.Empty;
        }

        private bool ObtenerEstadoSeleccionado()
        {
            return cmbEstado.SelectedItem?.ToString() == "Activo";
        }

        private Guid ObtenerIdUsuarioSeleccionado()
        {
            if (!Guid.TryParse(txtId.Text, out Guid idUsuario) || idUsuario == Guid.Empty)
            {
                throw new InvalidOperationException("No se pudo identificar el usuario seleccionado.");
            }

            return idUsuario;
        }

        private string ObtenerValorCeldaActual(string nombreColumna)
        {
            if (dgvUsuarios.CurrentRow == null || !dgvUsuarios.Columns.Contains(nombreColumna))
            {
                return string.Empty;
            }

            object valor = dgvUsuarios.CurrentRow.Cells[nombreColumna].Value;

            return Convert.ToString(valor);
        }

        private void MostrarAdvertencia(string mensaje, string titulo)
        {
            MessageBox.Show(
                mensaje,
                titulo,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        private void MostrarError(string mensaje, string titulo)
        {
            MessageBox.Show(
                mensaje,
                titulo,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
}
