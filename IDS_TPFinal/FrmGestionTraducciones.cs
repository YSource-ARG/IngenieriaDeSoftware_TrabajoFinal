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

            if (!ValidarIdiomaSeleccionado(idiomaSeleccionado))
            {
                return;
            }

            if (!ValidarDatosTraduccion())
            {
                return;
            }

            ResultadoGuardadoTraduccion resultado =
                _gestionTraduccionesAppService.GuardarTraduccion(
                    idiomaSeleccionado.Id,
                    txtClave.Text,
                    txtTexto.Text
                );

            if (!resultado.Exitoso)
            {
                MostrarAdvertencia(
                    "Mensajes.Traducciones.ErrorGuardar",
                    "No fue posible guardar la traducción. Verifique los datos e intente nuevamente.",
                    "Mensajes.Titulos.Traducciones",
                    "Traducciones"
                );

                return;
            }

            MostrarInformacion(
                "Mensajes.Traducciones.TraduccionGuardada",
                "Traducción guardada correctamente.",
                "Mensajes.Titulos.Traducciones",
                "Traducciones"
            );

            CargarTraducciones();

            ActualizarIdiomaActualSiCorresponde(idiomaSeleccionado);
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

            if (!ValidarIdiomaSeleccionado(idiomaSeleccionado))
            {
                return;
            }

            if (!ValidarTraduccionSeleccionada())
            {
                return;
            }

            if (!ValidarDatosTraduccion())
            {
                return;
            }

            ResultadoGuardadoTraduccion resultado =
                _gestionTraduccionesAppService.ModificarTraduccion(
                    _traduccionSeleccionadaId,
                    txtClave.Text,
                    txtTexto.Text
                );

            if (!resultado.Exitoso)
            {
                MostrarAdvertencia(
                    "Mensajes.Traducciones.ErrorModificar",
                    "No fue posible modificar la traducción. Verifique los datos e intente nuevamente.",
                    "Mensajes.Titulos.Traducciones",
                    "Traducciones"
                );

                return;
            }

            MostrarInformacion(
                "Mensajes.Traducciones.TraduccionModificada",
                "Traducción modificada correctamente.",
                "Mensajes.Titulos.Traducciones",
                "Traducciones"
            );

            CargarTraducciones();

            ActualizarIdiomaActualSiCorresponde(idiomaSeleccionado);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Idioma idiomaSeleccionado = ObtenerIdiomaSeleccionado();

            if (!ValidarIdiomaSeleccionado(idiomaSeleccionado))
            {
                return;
            }

            if (!ValidarTraduccionSeleccionada())
            {
                return;
            }

            DialogResult confirmacion = MostrarConfirmacion(
                "Mensajes.Traducciones.ConfirmarEliminar",
                "¿Está seguro que desea eliminar la traducción seleccionada?",
                "Mensajes.Titulos.EliminarTraduccion",
                "Eliminar traducción"
            );

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            ResultadoGuardadoTraduccion resultado =
                _gestionTraduccionesAppService.EliminarTraduccion(_traduccionSeleccionadaId);

            if (!resultado.Exitoso)
            {
                MostrarAdvertencia(
                    "Mensajes.Traducciones.ErrorEliminar",
                    "No fue posible eliminar la traducción. Verifique los datos e intente nuevamente.",
                    "Mensajes.Titulos.Traducciones",
                    "Traducciones"
                );

                return;
            }

            MostrarInformacion(
                "Mensajes.Traducciones.TraduccionEliminada",
                "Traducción eliminada correctamente.",
                "Mensajes.Titulos.Traducciones",
                "Traducciones"
            );

            CargarTraducciones();

            ActualizarIdiomaActualSiCorresponde(idiomaSeleccionado);
        }

        private bool ValidarIdiomaSeleccionado(Idioma idiomaSeleccionado)
        {
            if (idiomaSeleccionado != null)
            {
                return true;
            }

            MostrarAdvertencia(
                "Mensajes.Traducciones.DebeSeleccionarIdioma",
                "Debe seleccionar un idioma.",
                "Mensajes.Titulos.Traducciones",
                "Traducciones"
            );

            return false;
        }

        private bool ValidarTraduccionSeleccionada()
        {
            if (_traduccionSeleccionadaId != Guid.Empty)
            {
                return true;
            }

            MostrarAdvertencia(
                "Mensajes.Traducciones.DebeSeleccionarTraduccion",
                "Debe seleccionar una traducción.",
                "Mensajes.Titulos.Traducciones",
                "Traducciones"
            );

            return false;
        }

        private bool ValidarDatosTraduccion()
        {
            if (string.IsNullOrWhiteSpace(txtClave.Text))
            {
                MostrarAdvertencia(
                    "Mensajes.Traducciones.ClaveRequerida",
                    "Debe ingresar la clave de traducción.",
                    "Mensajes.Titulos.Validacion",
                    "Validación"
                );

                txtClave.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTexto.Text))
            {
                MostrarAdvertencia(
                    "Mensajes.Traducciones.TextoRequerido",
                    "Debe ingresar el texto de la traducción.",
                    "Mensajes.Titulos.Validacion",
                    "Validación"
                );

                txtTexto.Focus();
                return false;
            }

            return true;
        }

        private void ActualizarIdiomaActualSiCorresponde(Idioma idiomaSeleccionado)
        {
            if (_idiomaAppService.IdiomaActual != null &&
                _idiomaAppService.IdiomaActual.Id == idiomaSeleccionado.Id)
            {
                _idiomaAppService.CambiarIdioma(idiomaSeleccionado.Codigo);
            }
        }

        private void MostrarInformacion(
            string claveMensaje,
            string mensajePorDefecto,
            string claveTitulo,
            string tituloPorDefecto)
        {
            MensajeTraducido.Mostrar(
                this,
                _idiomaAppService,
                claveMensaje,
                mensajePorDefecto,
                claveTitulo,
                tituloPorDefecto,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void MostrarAdvertencia(
            string claveMensaje,
            string mensajePorDefecto,
            string claveTitulo,
            string tituloPorDefecto)
        {
            MensajeTraducido.Mostrar(
                this,
                _idiomaAppService,
                claveMensaje,
                mensajePorDefecto,
                claveTitulo,
                tituloPorDefecto,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        private DialogResult MostrarConfirmacion(
            string claveMensaje,
            string mensajePorDefecto,
            string claveTitulo,
            string tituloPorDefecto)
        {
            return MensajeTraducido.Mostrar(
                this,
                _idiomaAppService,
                claveMensaje,
                mensajePorDefecto,
                claveTitulo,
                tituloPorDefecto,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
        }
    }
}