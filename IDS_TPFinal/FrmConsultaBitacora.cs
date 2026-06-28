using BLL.Bitacora;
using BLL.Idiomas;
using System;
using System.Linq;
using System.Windows.Forms;
using UI.Estilos;
using UI.Idiomas;

namespace IDS_TPFinal
{
    public partial class FrmConsultaBitacora : Form, IObservadorIdioma
    {
        private readonly IBitacoraService _bitacoraService;
        private readonly IIdiomaAppService _idiomaAppService;

        private int _cantidadRegistrosActual;

        public FrmConsultaBitacora()
        {
            InitializeComponent();
        }

        public FrmConsultaBitacora(IBitacoraService bitacoraService) : this(bitacoraService, null)
        {
        }

        public FrmConsultaBitacora(IBitacoraService bitacoraService, IIdiomaAppService idiomaAppService)
        {
            _bitacoraService = bitacoraService
                ?? throw new ArgumentNullException(nameof(bitacoraService));

            _idiomaAppService = idiomaAppService;

            InitializeComponent();            
            AplicarEstiloVisual();

            if (_idiomaAppService != null)
            {
                _idiomaAppService.Suscribir(this);
                ActualizarIdioma();
            }
        }        

        public void ActualizarIdioma()
        {
            if (_idiomaAppService == null)
            {
                return;
            }

            int indiceModulo = cmbModulo.SelectedIndex;
            int indiceTipo = cmbTipo.SelectedIndex;
            int indiceUsuario = cmbUsuario.SelectedIndex;
            int indiceAccion = cmbAccion.SelectedIndex;

            TraductorControles.TraducirFormulario(this, _idiomaAppService);

            CargarOpcionesModulo();
            CargarOpcionesUsuarioYAccion();

            if (indiceModulo >= 0 && indiceModulo < cmbModulo.Items.Count)
            {
                cmbModulo.SelectedIndex = indiceModulo;
            }

            if (indiceTipo >= 0 && indiceTipo < cmbTipo.Items.Count)
            {
                cmbTipo.SelectedIndex = indiceTipo;
            }

            if (indiceUsuario >= 0 && indiceUsuario < cmbUsuario.Items.Count)
            {
                cmbUsuario.SelectedIndex = indiceUsuario;
            }

            if (indiceAccion >= 0 && indiceAccion < cmbAccion.Items.Count)
            {
                cmbAccion.SelectedIndex = indiceAccion;
            }
            ConfigurarColumnas();
            ActualizarCantidadRegistros();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_idiomaAppService != null)
            {
                _idiomaAppService.Desuscribir(this);
            }

            base.OnFormClosed(e);
        }

        private void FrmConsultaBitacora_Load(object sender, EventArgs e)
        {
            CargarOpcionesModulo();
            CargarOpcionesUsuarioYAccion();

            dtpFechaDesde.Value = DateTime.Today.AddDays(-7);
            dtpFechaHasta.Value = DateTime.Today;

            CargarBitacora();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarBitacora();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            CargarOpcionesModulo();
            CargarOpcionesUsuarioYAccion();

            dtpFechaDesde.Value = DateTime.Today.AddDays(-7);
            dtpFechaHasta.Value = DateTime.Today;

            CargarBitacora();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CargarOpcionesModulo()
        {
            cmbModulo.Items.Clear();
            cmbModulo.Items.Add(TraducirTexto("Bitacora.Opciones.Todos", "Todos"));
            cmbModulo.Items.Add(TraducirTexto("Bitacora.Opciones.Seguridad", "Seguridad"));
            cmbModulo.Items.Add(TraducirTexto("Bitacora.Opciones.Usuarios", "Usuarios"));
            cmbModulo.Items.Add(TraducirTexto("Bitacora.Opciones.Permisos", "Permisos"));
            cmbModulo.Items.Add(TraducirTexto("Bitacora.Opciones.Integridad", "Integridad"));
            cmbModulo.Items.Add(TraducirTexto("Bitacora.Opciones.Idiomas", "Idiomas"));
            cmbModulo.Items.Add(TraducirTexto("Bitacora.Opciones.Traducciones", "Traducciones"));
            cmbModulo.SelectedIndex = 0;

            cmbTipo.Items.Clear();
            cmbTipo.Items.Add(TraducirTexto("Bitacora.Opciones.Todos", "Todos"));
            cmbTipo.Items.Add("INFO");
            cmbTipo.Items.Add("WARN");
            cmbTipo.Items.Add("ERROR");
            cmbTipo.SelectedIndex = 0;
        }

        private void CargarOpcionesUsuarioYAccion()
        {
            if (_bitacoraService == null)
            {
                return;
            }

            string opcionTodos = TraducirTexto("Bitacora.Opciones.Todos", "Todos");

            cmbUsuario.Items.Clear();
            cmbUsuario.Items.Add(opcionTodos);

            foreach (string usuario in _bitacoraService.ListarUsuarios())
            {
                cmbUsuario.Items.Add(usuario);
            }

            cmbUsuario.SelectedIndex = 0;

            cmbAccion.Items.Clear();
            cmbAccion.Items.Add(opcionTodos);

            foreach (string accion in _bitacoraService.ListarAcciones())
            {
                cmbAccion.Items.Add(accion);
            }

            cmbAccion.SelectedIndex = 0;
        }

        private string ObtenerUsuarioSeleccionado()
        {
            if (cmbUsuario.SelectedIndex <= 0)
            {
                return null;
            }

            return cmbUsuario.SelectedItem.ToString();
        }

        private string ObtenerAccionSeleccionada()
        {
            if (cmbAccion.SelectedIndex <= 0)
            {
                return null;
            }

            return cmbAccion.SelectedItem.ToString();
        }

        // Consulta a la bitácora según los filtros seleccionados
        private void CargarBitacora()
        {
            if (_bitacoraService == null)
            {
                return;
            }

            try
            {
                string modulo = ObtenerModuloSeleccionado();
                string tipo = ObtenerTipoSeleccionado();
                string usuario = ObtenerUsuarioSeleccionado();
                string accion = ObtenerAccionSeleccionada();

                DateTime fechaDesde = dtpFechaDesde.Value.Date;
                DateTime fechaHasta = dtpFechaHasta.Value.Date.AddDays(1).AddTicks(-1);

                var registros = _bitacoraService
                    .Listar(
                        usuario,
                        accion,
                        modulo,
                        tipo,
                        fechaDesde,
                        fechaHasta,
                        500
                    )
                    .Select(registro => new
                    {
                        Fecha = registro.Fecha.ToString("dd/MM/yyyy HH:mm:ss"),
                        Usuario = string.IsNullOrWhiteSpace(registro.Usuario) ? "-" : registro.Usuario,
                        registro.Modulo,
                        registro.Accion,
                        Descripcion = string.IsNullOrWhiteSpace(registro.Descripcion) ? "-" : registro.Descripcion,
                        registro.Tipo
                    })
                    .ToList();

                dgvBitacora.DataSource = registros;
                ConfigurarColumnas();

                _cantidadRegistrosActual = registros.Count;
                ActualizarCantidadRegistros();
            }
            catch (Exception)
            {
                MensajeTraducido.Mostrar(
                    this,
                    _idiomaAppService,
                    "Mensajes.Bitacora.ErrorConsulta",
                    "No se pudo consultar la bitácora. Verifique los filtros e intente nuevamente.",
                    "Mensajes.Titulos.NoPudoConsultarBitacora",
                    "No se pudo consultar la bitácora",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private string ObtenerModuloSeleccionado()
        {
            if (cmbModulo.SelectedIndex == 1)
            {
                return "Seguridad";
            }

            if (cmbModulo.SelectedIndex == 2)
            {
                return "Usuarios";
            }

            if (cmbModulo.SelectedIndex == 3)
            {
                return "Permisos";
            }

            if (cmbModulo.SelectedIndex == 4)
            {
                return "Integridad";
            }

            if (cmbModulo.SelectedIndex == 5)
            {
                return "Idiomas";
            }

            if (cmbModulo.SelectedIndex == 6)
            {
                return "Traducciones";
            }

            return null;
        }

        private string ObtenerTipoSeleccionado()
        {
            if (cmbTipo.SelectedIndex == 1)
            {
                return "INFO";
            }

            if (cmbTipo.SelectedIndex == 2)
            {
                return "WARN";
            }

            if (cmbTipo.SelectedIndex == 3)
            {
                return "ERROR";
            }

            return null;
        }

        private void ConfigurarColumnas()
        {
            if (dgvBitacora.Columns.Count == 0)
            {
                return;
            }

            ConfigurarColumna("Fecha", 105, "Bitacora.Grilla.Fecha", "Fecha");
            ConfigurarColumna("Usuario", 80, "Bitacora.Grilla.Usuario", "Usuario");
            ConfigurarColumna("Modulo", 80, "Bitacora.Grilla.Modulo", "Módulo");
            ConfigurarColumna("Accion", 120, "Bitacora.Grilla.Accion", "Acción");
            ConfigurarColumna("Descripcion", 240, "Bitacora.Grilla.Descripcion", "Descripción");
            ConfigurarColumna("Tipo", 60, "Bitacora.Grilla.Tipo", "Tipo");
        }

        private void ConfigurarColumna(
            string nombreColumna,
            int fillWeight,
            string claveTraduccion,
            string textoPorDefecto)
        {
            if (!dgvBitacora.Columns.Contains(nombreColumna))
            {
                return;
            }

            dgvBitacora.Columns[nombreColumna].FillWeight = fillWeight;
            dgvBitacora.Columns[nombreColumna].Tag = claveTraduccion;
            dgvBitacora.Columns[nombreColumna].HeaderText = TraducirTexto(
                claveTraduccion,
                textoPorDefecto
            );
        }

        private void ActualizarCantidadRegistros()
        {
            lblCantidadRegistros.Text = FormatearTexto(
                "Bitacora.RegistrosEncontrados",
                "Registros encontrados: {0}",
                _cantidadRegistrosActual
            );
        }

        private string TraducirTexto(string clave, string textoPorDefecto)
        {
            return MensajeTraducido.TraducirConFallback(
                _idiomaAppService,
                clave,
                textoPorDefecto
            );
        }

        private string FormatearTexto(
            string clave,
            string textoPorDefecto,
            params object[] valores)
        {
            string plantilla = TraducirTexto(clave, textoPorDefecto);

            try
            {
                return string.Format(plantilla, valores);
            }
            catch (FormatException)
            {
                return string.Format(textoPorDefecto, valores);
            }
        }

        private void AplicarEstiloVisual()
        {
            TemaVisual.AplicarFormularioOscuro(this);
            TemaVisual.AplicarPanelPrincipal(panelFiltros);
            TemaVisual.AplicarDataGridView(dgvBitacora);
            TemaVisual.AplicarBotonPrincipal(btnBuscar);
            TemaVisual.AplicarBotonSecundario(btnLimpiar);
            TemaVisual.AplicarBotonSecundario(btnCerrar);

            TemaVisual.AplicarTitulo(lblTitulo);
            TemaVisual.AplicarTextoSecundario(lblModulo);
            TemaVisual.AplicarTextoSecundario(lblFechaDesde);
            TemaVisual.AplicarTextoSecundario(lblFechaHasta);
            TemaVisual.AplicarTextoSecundario(lblTipo);
            TemaVisual.AplicarTextoSecundario(lblCantidadRegistros);
            TemaVisual.AplicarTextoSecundario(lblUsuario);
            TemaVisual.AplicarTextoSecundario(lblAccion);            
        }
    }
}