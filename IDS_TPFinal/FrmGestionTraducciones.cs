using BE;
using BLL.Idiomas;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using UI.Estilos;
using UI.Idiomas;

namespace UI
{
    public partial class FrmGestionTraducciones : Form, IObservadorIdioma
    {
        private readonly GestionTraduccionesAppService _gestionTraduccionesAppService;
        private readonly IIdiomaAppService _idiomaAppService;

        private bool _cargandoIdiomas;
        private Guid _traduccionSeleccionadaId = Guid.Empty;

        public FrmGestionTraducciones(
            GestionTraduccionesAppService gestionTraduccionesAppService,
            IIdiomaAppService idiomaAppService)
        {
            if (gestionTraduccionesAppService == null)
            {
                throw new ArgumentNullException(nameof(gestionTraduccionesAppService));
            }

            if (idiomaAppService == null)
            {
                throw new ArgumentNullException(nameof(idiomaAppService));
            }

            _gestionTraduccionesAppService = gestionTraduccionesAppService;
            _idiomaAppService = idiomaAppService;

            InitializeComponent();

            ConfigurarTagsColumnasGrilla();

            AplicarEstiloVisual();

            cboIdiomas.SelectedIndexChanged += cboIdiomas_SelectedIndexChanged;
            dgvTraducciones.SelectionChanged += dgvTraducciones_SelectionChanged;
            btnNuevo.Click += btnNuevo_Click;
            btnGuardar.Click += btnGuardar_Click;
            btnModificar.Click += btnModificar_Click;
            btnEliminar.Click += btnEliminar_Click;
            btnCerrar.Click += btnCerrar_Click;
            this.FormClosed += FrmGestionTraducciones_FormClosed;

            _idiomaAppService.Suscribir(this);

            CargarIdiomas();
            ActualizarIdioma();
        }

        private void ConfigurarTagsColumnasGrilla()
        {
            colClave.Tag = "Traducciones.Grilla.Clave";
            colTexto.Tag = "Traducciones.Grilla.Texto";
        }

        public void ActualizarIdioma()
        {
            TraductorControles.TraducirFormulario(this, _idiomaAppService);
        }

        private void AplicarEstiloVisual()
        {
            TemaVisual.AplicarFormularioOscuro(this);

            TemaVisual.AplicarTextoSecundario(lblIdioma);
            TemaVisual.AplicarTextoSecundario(lblClave);
            TemaVisual.AplicarTextoSecundario(lblTexto);

            TemaVisual.AplicarTextBox(txtClave);
            TemaVisual.AplicarTextBox(txtTexto);

            TemaVisual.AplicarDataGridView(dgvTraducciones);

            cboIdiomas.BackColor = TemaVisual.FondoInput;
            cboIdiomas.ForeColor = TemaVisual.TextoPrincipal;
            cboIdiomas.FlatStyle = FlatStyle.Flat;
            cboIdiomas.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            TemaVisual.AplicarBotonSecundario(btnNuevo);
            TemaVisual.AplicarBotonPrincipal(btnGuardar);
            TemaVisual.AplicarBotonSecundario(btnModificar);
            TemaVisual.AplicarBotonSecundario(btnEliminar);
            TemaVisual.AplicarBotonSecundario(btnCerrar);
        }

        private void CargarIdiomas()
        {
            _cargandoIdiomas = true;

            List<Idioma> idiomas = _gestionTraduccionesAppService.ListarIdiomasActivos();

            cboIdiomas.DataSource = null;
            cboIdiomas.DisplayMember = "Nombre";
            cboIdiomas.ValueMember = "Id";
            cboIdiomas.DataSource = idiomas;
            cboIdiomas.Enabled = idiomas.Count > 0;

            _cargandoIdiomas = false;

            CargarTraducciones();
        }

        private void CargarTraducciones()
        {
            Idioma idiomaSeleccionado = ObtenerIdiomaSeleccionado();

            if (idiomaSeleccionado == null)
            {
                dgvTraducciones.DataSource = null;
                LimpiarCampos();
                return;
            }

            List<Traduccion> traducciones =
                _gestionTraduccionesAppService.ListarTraduccionesPorIdioma(idiomaSeleccionado.Id);

            dgvTraducciones.AutoGenerateColumns = false;
            dgvTraducciones.DataSource = null;
            dgvTraducciones.DataSource = traducciones;

            LimpiarCampos();
        }

        private Idioma ObtenerIdiomaSeleccionado()
        {
            return cboIdiomas.SelectedItem as Idioma;
        }

        private void cboIdiomas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoIdiomas)
            {
                return;
            }

            CargarTraducciones();
        }

        private void dgvTraducciones_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTraducciones.CurrentRow == null)
            {
                return;
            }

            Traduccion traduccion = dgvTraducciones.CurrentRow.DataBoundItem as Traduccion;

            if (traduccion == null)
            {
                return;
            }

            _traduccionSeleccionadaId = traduccion.Id;

            txtClave.Text = traduccion.Clave;
            txtTexto.Text = traduccion.Texto;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            txtClave.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Idioma idiomaSeleccionado = ObtenerIdiomaSeleccionado();

            if (idiomaSeleccionado == null)
            {
                MessageBox.Show(
                    "Debe seleccionar un idioma.",
                    "Traducciones",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            ResultadoGuardadoTraduccion resultado =
                _gestionTraduccionesAppService.GuardarTraduccion(
                    idiomaSeleccionado.Id,
                    txtClave.Text,
                    txtTexto.Text
                );

            MessageBox.Show(
                resultado.Mensaje,
                "Traducciones",
                MessageBoxButtons.OK,
                resultado.Exitoso ? MessageBoxIcon.Information : MessageBoxIcon.Warning
            );

            if (!resultado.Exitoso)
            {
                return;
            }

            CargarTraducciones();

            if (_idiomaAppService.IdiomaActual != null &&
                _idiomaAppService.IdiomaActual.Id == idiomaSeleccionado.Id)
            {
                _idiomaAppService.CambiarIdioma(idiomaSeleccionado.Codigo);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmGestionTraducciones_FormClosed(object sender, FormClosedEventArgs e)
        {
            _idiomaAppService.Desuscribir(this);
        }

        private void LimpiarCampos()
        {
            _traduccionSeleccionadaId = Guid.Empty;
            txtClave.Clear();
            txtTexto.Clear();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Idioma idiomaSeleccionado = ObtenerIdiomaSeleccionado();

            if (idiomaSeleccionado == null)
            {
                MessageBox.Show(
                    "Debe seleccionar un idioma.",
                    "Traducciones",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            ResultadoGuardadoTraduccion resultado =
                _gestionTraduccionesAppService.ModificarTraduccion(
                    _traduccionSeleccionadaId,
                    txtClave.Text,
                    txtTexto.Text
                );

            MessageBox.Show(
                resultado.Mensaje,
                "Traducciones",
                MessageBoxButtons.OK,
                resultado.Exitoso ? MessageBoxIcon.Information : MessageBoxIcon.Warning
            );

            if (!resultado.Exitoso)
            {
                return;
            }

            CargarTraducciones();

            if (_idiomaAppService.IdiomaActual != null &&
                _idiomaAppService.IdiomaActual.Id == idiomaSeleccionado.Id)
            {
                _idiomaAppService.CambiarIdioma(idiomaSeleccionado.Codigo);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Idioma idiomaSeleccionado = ObtenerIdiomaSeleccionado();

            if (idiomaSeleccionado == null)
            {
                MessageBox.Show(
                    "Debe seleccionar un idioma.",
                    "Traducciones",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (_traduccionSeleccionadaId == Guid.Empty)
            {
                MessageBox.Show(
                    "Debe seleccionar una traducción.",
                    "Traducciones",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                "¿Está seguro que desea eliminar la traducción seleccionada?",
                "Eliminar traducción",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            ResultadoGuardadoTraduccion resultado =
                _gestionTraduccionesAppService.EliminarTraduccion(_traduccionSeleccionadaId);

            MessageBox.Show(
                resultado.Mensaje,
                "Traducciones",
                MessageBoxButtons.OK,
                resultado.Exitoso ? MessageBoxIcon.Information : MessageBoxIcon.Warning
            );

            if (!resultado.Exitoso)
            {
                return;
            }

            CargarTraducciones();

            if (_idiomaAppService.IdiomaActual != null &&
                _idiomaAppService.IdiomaActual.Id == idiomaSeleccionado.Id)
            {
                _idiomaAppService.CambiarIdioma(idiomaSeleccionado.Codigo);
            }
        }
    }
}