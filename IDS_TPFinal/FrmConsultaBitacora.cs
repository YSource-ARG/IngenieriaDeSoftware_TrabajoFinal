using BLL.Bitacora;
using System;
using System.Linq;
using System.Windows.Forms;
using UI.Estilos;

namespace IDS_TPFinal
{
    public partial class FrmConsultaBitacora : Form
    {
        private readonly IBitacoraService _bitacoraService;

        public FrmConsultaBitacora()
        {
            InitializeComponent();
        }

        public FrmConsultaBitacora(IBitacoraService bitacoraService)
        {
            _bitacoraService = bitacoraService
                ?? throw new ArgumentNullException(nameof(bitacoraService));

            InitializeComponent();
            AplicarEstiloVisual();
        }

        private void FrmConsultaBitacora_Load(object sender, EventArgs e)
        {
            CargarOpcionesModulo();
            CargarBitacora();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarBitacora();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            cmbModulo.SelectedIndex = 0;
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
            cmbModulo.Items.Add("Todos");
            cmbModulo.Items.Add("Seguridad");
            cmbModulo.Items.Add("Usuarios");
            cmbModulo.SelectedIndex = 0;

            dtpFechaDesde.Value = DateTime.Today.AddDays(-7);
            dtpFechaHasta.Value = DateTime.Today;
        }

        private void CargarBitacora()
        {
            if (_bitacoraService == null)
            {
                return;
            }

            try
            {
                string modulo = ObtenerModuloSeleccionado();
                DateTime fechaDesde = dtpFechaDesde.Value.Date;
                DateTime fechaHasta = dtpFechaHasta.Value.Date.AddDays(1).AddTicks(-1);

                var registros = _bitacoraService
                    .Listar(modulo, fechaDesde, fechaHasta, 500)
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
                lblCantidadRegistros.Text = $"Registros encontrados: {registros.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "No se pudo consultar la bitácora",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private string ObtenerModuloSeleccionado()
        {
            string modulo = cmbModulo.SelectedItem?.ToString();

            return modulo == "Todos"
                ? null
                : modulo;
        }

        private void ConfigurarColumnas()
        {
            if (dgvBitacora.Columns.Count == 0)
            {
                return;
            }

            dgvBitacora.Columns["Fecha"].FillWeight = 105;
            dgvBitacora.Columns["Usuario"].FillWeight = 80;
            dgvBitacora.Columns["Modulo"].FillWeight = 80;
            dgvBitacora.Columns["Accion"].FillWeight = 120;
            dgvBitacora.Columns["Descripcion"].FillWeight = 240;
            dgvBitacora.Columns["Tipo"].FillWeight = 60;
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
            TemaVisual.AplicarTextoSecundario(lblCantidadRegistros);

            cmbModulo.DropDownStyle = ComboBoxStyle.DropDownList;
        }
    }
}
