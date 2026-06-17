using BE;
using BLL.Idiomas;
using System;
using System.Windows.Forms;
using UI.Idiomas;

namespace UI
{
    public partial class FrmGestionIdiomas : Form, IObservadorIdioma
    {
        private GestionIdiomasAppService _gestionIdiomasAppService;
        private IIdiomaAppService _idiomaAppService;
        private Guid _idiomaSeleccionadoId = Guid.Empty;

        public FrmGestionIdiomas()
        {
            InitializeComponent();

            ConfigurarTagsTraduccion();
            ConfigurarGrilla();
        }

        public FrmGestionIdiomas(
            GestionIdiomasAppService gestionIdiomasAppService,
            IIdiomaAppService idiomaAppService) : this()
        {
            if (gestionIdiomasAppService == null)
            {
                throw new ArgumentNullException(nameof(gestionIdiomasAppService));
            }

            if (idiomaAppService == null)
            {
                throw new ArgumentNullException(nameof(idiomaAppService));
            }

            _gestionIdiomasAppService = gestionIdiomasAppService;
            _idiomaAppService = idiomaAppService;

            SuscribirEventos();

            _idiomaAppService.Suscribir(this);

            TraducirFormularioActual();
            LimpiarFormulario();
            CargarIdiomas();
        }

        private void ConfigurarTagsTraduccion()
        {
            this.Tag = "Idiomas.TituloVentana";

            lblCodigo.Tag = "Idiomas.Codigo";
            lblNombre.Tag = "Idiomas.Nombre";
            chkActivo.Tag = "Idiomas.Activo";

            btnNuevo.Tag = "Idiomas.Nuevo";
            btnGuardar.Tag = "Idiomas.Guardar";
            btnCerrar.Tag = "Idiomas.Cerrar";

            colCodigo.Tag = "Idiomas.Grilla.Codigo";
            colNombre.Tag = "Idiomas.Grilla.Nombre";
            colActivo.Tag = "Idiomas.Grilla.Activo";
        }

        private void ConfigurarGrilla()
        {
            dgvIdiomas.AutoGenerateColumns = false;
            dgvIdiomas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvIdiomas.MultiSelect = false;
            dgvIdiomas.ReadOnly = true;
            dgvIdiomas.AllowUserToAddRows = false;
            dgvIdiomas.AllowUserToDeleteRows = false;
            dgvIdiomas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            colCodigo.DataPropertyName = "Codigo";
            colNombre.DataPropertyName = "Nombre";
            colActivo.DataPropertyName = "Activo";
        }

        private void SuscribirEventos()
        {
            btnNuevo.Click += btnNuevo_Click;
            btnGuardar.Click += btnGuardar_Click;
            btnCerrar.Click += btnCerrar_Click;
            dgvIdiomas.SelectionChanged += dgvIdiomas_SelectionChanged;
        }

        private void CargarIdiomas()
        {
            dgvIdiomas.DataSource = null;
            dgvIdiomas.DataSource = _gestionIdiomasAppService.ListarIdiomas();
        }

        private void LimpiarFormulario()
        {
            _idiomaSeleccionadoId = Guid.Empty;
            txtCodigo.Clear();
            txtNombre.Clear();
            chkActivo.Checked = true;
            txtCodigo.Focus();
        }

        private void TraducirFormularioActual()
        {
            if (_idiomaAppService == null)
            {
                return;
            }

            TraductorControles.TraducirFormulario(this, _idiomaAppService);
        }

        public void ActualizarIdioma()
        {
            TraducirFormularioActual();
        }

        private void dgvIdiomas_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvIdiomas.CurrentRow == null)
            {
                return;
            }

            Idioma idioma = dgvIdiomas.CurrentRow.DataBoundItem as Idioma;

            if (idioma == null)
            {
                return;
            }

            _idiomaSeleccionadoId = idioma.Id;
            txtCodigo.Text = idioma.Codigo;
            txtNombre.Text = idioma.Nombre;
            chkActivo.Checked = idioma.Activo;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            ResultadoGuardadoIdioma resultado = _gestionIdiomasAppService.GuardarIdioma(
                _idiomaSeleccionadoId,
                txtCodigo.Text,
                txtNombre.Text,
                chkActivo.Checked
            );

            MessageBox.Show(
                resultado.Mensaje,
                "Idiomas",
                MessageBoxButtons.OK,
                resultado.Exitoso ? MessageBoxIcon.Information : MessageBoxIcon.Warning
            );

            if (!resultado.Exitoso)
            {
                return;
            }

            CargarIdiomas();
            LimpiarFormulario();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_idiomaAppService != null)
            {
                _idiomaAppService.Desuscribir(this);
            }

            base.OnFormClosed(e);
        }
    }
}