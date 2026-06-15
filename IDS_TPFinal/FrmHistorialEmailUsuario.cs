using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using BLL.Usuarios;
using System;
using System.Linq;
using System.Windows.Forms;
using UI.Estilos;

namespace IDS_TPFinal
{
    public partial class FrmHistorialEmailUsuario : Form
    {
        private readonly GestionUsuariosAppService _gestionUsuariosAppService;
        private readonly Guid _usuarioId;
        private readonly string _nombreUsuario;

        public FrmHistorialEmailUsuario()
        {
            InitializeComponent();
        }

        public FrmHistorialEmailUsuario(
            GestionUsuariosAppService gestionUsuariosAppService,
            Guid usuarioId,
            string nombreUsuario) : this()
        {
            if (gestionUsuariosAppService == null)
            {
                throw new ArgumentNullException(nameof(gestionUsuariosAppService));
            }

            if (usuarioId == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(usuarioId));
            }

            _gestionUsuariosAppService = gestionUsuariosAppService;
            _usuarioId = usuarioId;
            _nombreUsuario = nombreUsuario;

            this.Load += FrmHistorialEmailUsuario_Load;
        }

        private void FrmHistorialEmailUsuario_Load(object sender, EventArgs e)
        {
            AplicarEstiloVisual();
            CargarHistorial();
        }

        private void CargarHistorial()
        {
            var historial = _gestionUsuariosAppService
                .ListarHistorialEmail(_usuarioId)
                .Select(cambio => new
                {
                    cambio.EmailAnterior,
                    cambio.EmailNuevo,
                    FechaCambio = cambio.FechaCambio.ToString("dd/MM/yyyy HH:mm"),
                    UsuarioCambio = cambio.UsuarioCambioNombre
                })
                .ToList();

            dgvHistorialEmail.DataSource = historial;

            if (dgvHistorialEmail.Columns.Contains("EmailAnterior"))
            {
                dgvHistorialEmail.Columns["EmailAnterior"].HeaderText = "Email anterior";
            }

            if (dgvHistorialEmail.Columns.Contains("EmailNuevo"))
            {
                dgvHistorialEmail.Columns["EmailNuevo"].HeaderText = "Email nuevo";
            }

            if (dgvHistorialEmail.Columns.Contains("FechaCambio"))
            {
                dgvHistorialEmail.Columns["FechaCambio"].HeaderText = "Fecha cambio";
            }

            if (dgvHistorialEmail.Columns.Contains("UsuarioCambio"))
            {
                dgvHistorialEmail.Columns["UsuarioCambio"].HeaderText = "Usuario que cambió";
            }
        }

        private void btnRestaurarEmail_Click(object sender, EventArgs e)
        {
            if (dgvHistorialEmail.CurrentRow == null)
            {
                MessageBox.Show(
                    "Debe seleccionar un registro del historial.",
                    "Historial de email",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            string emailAnterior = Convert.ToString(
                dgvHistorialEmail.CurrentRow.Cells["EmailAnterior"].Value
            );

            DialogResult confirmacion = MessageBox.Show(
                $"¿Desea restaurar el email anterior del usuario '{_nombreUsuario}'?\n\n" +
                $"Email a restaurar: {emailAnterior}",
                "Restaurar email",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            try
            {
                // La restauración no borra el historial anterior.
                // Genera un nuevo cambio, dejando trazabilidad completa.
                _gestionUsuariosAppService.RestaurarEmailAnterior(
                    _usuarioId,
                    emailAnterior
                );

                MessageBox.Show(
                    "El email fue restaurado correctamente.",
                    "Historial de email",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                CargarHistorial();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible restaurar el email.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AplicarEstiloVisual()
        {
            TemaVisual.AplicarFormularioOscuro(this);
            TemaVisual.AplicarDataGridView(dgvHistorialEmail);
            TemaVisual.AplicarBotonPrincipal(btnRestaurarEmail);
            TemaVisual.AplicarBotonSecundario(btnCerrar);

            lblTitulo.ForeColor = TemaVisual.TextoPrincipal;
        }
    }
}