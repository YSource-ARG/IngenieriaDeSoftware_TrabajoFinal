using BE;
using BLL.Idiomas;
using System;
using System.Windows.Forms;
using UI.Idiomas;
using System.Drawing;

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
            AplicarEstiloVisual();
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
            btnModificar.Tag = "Idiomas.Modificar";
            btnDesactivar.Tag = "Idiomas.Desactivar";
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

            ActualizarTextoBotonEstado();

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
            ActualizarTextoBotonEstado();
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

            ActualizarTextoBotonEstado();
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

        private void AplicarEstiloVisual()
        {
            Color fondo = Color.FromArgb(13, 17, 23);
            Color panel = Color.FromArgb(22, 27, 34);
            Color borde = Color.FromArgb(48, 54, 61);
            Color texto = Color.FromArgb(240, 246, 252);
            Color azul = Color.FromArgb(59, 113, 221);

            BackColor = fondo;
            ForeColor = texto;

            lblCodigo.ForeColor = texto;
            lblNombre.ForeColor = texto;

            chkActivo.ForeColor = texto;
            chkActivo.BackColor = fondo;
            chkActivo.UseVisualStyleBackColor = false;

            txtCodigo.BackColor = panel;
            txtCodigo.ForeColor = texto;
            txtCodigo.BorderStyle = BorderStyle.FixedSingle;

            txtNombre.BackColor = panel;
            txtNombre.ForeColor = texto;
            txtNombre.BorderStyle = BorderStyle.FixedSingle;

            dgvIdiomas.BackgroundColor = panel;
            dgvIdiomas.BorderStyle = BorderStyle.FixedSingle;
            dgvIdiomas.EnableHeadersVisualStyles = false;
            dgvIdiomas.GridColor = borde;

            dgvIdiomas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(31, 36, 43);
            dgvIdiomas.ColumnHeadersDefaultCellStyle.ForeColor = texto;
            dgvIdiomas.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(31, 36, 43);
            dgvIdiomas.ColumnHeadersDefaultCellStyle.SelectionForeColor = texto;

            dgvIdiomas.DefaultCellStyle.BackColor = panel;
            dgvIdiomas.DefaultCellStyle.ForeColor = texto;
            dgvIdiomas.DefaultCellStyle.SelectionBackColor = azul;
            dgvIdiomas.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvIdiomas.RowHeadersDefaultCellStyle.BackColor = panel;
            dgvIdiomas.RowHeadersDefaultCellStyle.ForeColor = texto;
            dgvIdiomas.RowHeadersDefaultCellStyle.SelectionBackColor = azul;
            dgvIdiomas.RowHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            ConfigurarBotonSecundario(btnNuevo);
            ConfigurarBotonPrincipal(btnGuardar);
            ConfigurarBotonSecundario(btnCerrar);
            ConfigurarBotonSecundario(btnModificar);
            ConfigurarBotonSecundario(btnDesactivar);
        }

        private void ConfigurarBotonPrincipal(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = Color.FromArgb(59, 113, 221);
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            boton.Cursor = Cursors.Hand;
            boton.UseVisualStyleBackColor = false;
        }

        private void ConfigurarBotonSecundario(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 1;
            boton.FlatAppearance.BorderColor = Color.FromArgb(48, 54, 61);
            boton.BackColor = Color.FromArgb(22, 27, 34);
            boton.ForeColor = Color.FromArgb(240, 246, 252);
            boton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            boton.Cursor = Cursors.Hand;
            boton.UseVisualStyleBackColor = false;
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (_idiomaSeleccionadoId == Guid.Empty)
            {
                MessageBox.Show(
                    "Debe seleccionar un idioma.",
                    "Idiomas",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

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

        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            if (_idiomaSeleccionadoId == Guid.Empty)
            {
                MessageBox.Show(
                    "Debe seleccionar un idioma.",
                    "Idiomas",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            bool nuevoEstado = !chkActivo.Checked;

            string accion = nuevoEstado ? "activar" : "desactivar";

            DialogResult confirmacion = MessageBox.Show(
                "¿Está seguro que desea " + accion + " el idioma seleccionado?",
                "Cambiar estado del idioma",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            ResultadoGuardadoIdioma resultado = _gestionIdiomasAppService.GuardarIdioma(
                _idiomaSeleccionadoId,
                txtCodigo.Text,
                txtNombre.Text,
                nuevoEstado
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

        private void ActualizarTextoBotonEstado()
        {
            if (_idiomaSeleccionadoId == Guid.Empty)
            {
                btnDesactivar.Enabled = false;
                btnDesactivar.Text = _idiomaAppService != null
                    ? _idiomaAppService.Traducir("Idiomas.Desactivar")
                    : "Desactivar";

                return;
            }

            btnDesactivar.Enabled = true;

            if (chkActivo.Checked)
            {
                btnDesactivar.Text = _idiomaAppService != null
                    ? _idiomaAppService.Traducir("Idiomas.Desactivar")
                    : "Desactivar";
            }
            else
            {
                btnDesactivar.Text = _idiomaAppService != null
                    ? _idiomaAppService.Traducir("Idiomas.Activar")
                    : "Activar";
            }
        }
    }
}