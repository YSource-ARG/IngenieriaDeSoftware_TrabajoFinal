using BLL.Idiomas;
using BLL.Usuarios;
using System;
using System.Linq;
using System.Windows.Forms;
using UI.Estilos;
using UI.Idiomas;

namespace IDS_TPFinal
{
    public partial class FrmHistorialEmailUsuario : Form, IObservadorIdioma
    {
        private readonly GestionUsuariosAppService _gestionUsuariosAppService;
        private readonly IIdiomaAppService _idiomaAppService;
        private readonly Guid _usuarioId;
        private readonly string _nombreUsuario;

        public FrmHistorialEmailUsuario()
        {
            InitializeComponent();
        }

        public FrmHistorialEmailUsuario(
            GestionUsuariosAppService gestionUsuariosAppService,
            Guid usuarioId,
            string nombreUsuario)
            : this(gestionUsuariosAppService, usuarioId, nombreUsuario, null)
        {
        }

        public FrmHistorialEmailUsuario(
            GestionUsuariosAppService gestionUsuariosAppService,
            Guid usuarioId,
            string nombreUsuario,
            IIdiomaAppService idiomaAppService)
            : this()
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
            _idiomaAppService = idiomaAppService;

            ConfigurarTagsTraduccion();

            if (_idiomaAppService != null)
            {
                _idiomaAppService.Suscribir(this);
                ActualizarIdioma();
            }

            this.Load += FrmHistorialEmailUsuario_Load;
        }

        private void ConfigurarTagsTraduccion()
        {
            this.Tag = "HistorialEmail.TituloVentana";

            lblTitulo.Tag = "HistorialEmail.Titulo";
            btnRestaurarEmail.Tag = "HistorialEmail.RestaurarEmailSeleccionado";
            btnCerrar.Tag = "HistorialEmail.Cerrar";
        }

        public void ActualizarIdioma()
        {
            if (_idiomaAppService == null)
            {
                return;
            }

            TraductorControles.TraducirFormulario(this, _idiomaAppService);
            ConfigurarColumnas();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_idiomaAppService != null)
            {
                _idiomaAppService.Desuscribir(this);
            }

            base.OnFormClosed(e);
        }

        private void FrmHistorialEmailUsuario_Load(object sender, EventArgs e)
        {
            AplicarEstiloVisual();
            CargarHistorial();
            ActualizarIdioma();
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
            ConfigurarColumnas();
        }

        private void ConfigurarColumnas()
        {
            if (dgvHistorialEmail.Columns.Count == 0)
            {
                return;
            }

            ConfigurarColumna(
                "EmailAnterior",
                "HistorialEmail.Grilla.EmailAnterior",
                "Email anterior"
            );

            ConfigurarColumna(
                "EmailNuevo",
                "HistorialEmail.Grilla.EmailNuevo",
                "Email nuevo"
            );

            ConfigurarColumna(
                "FechaCambio",
                "HistorialEmail.Grilla.FechaCambio",
                "Fecha cambio"
            );

            ConfigurarColumna(
                "UsuarioCambio",
                "HistorialEmail.Grilla.UsuarioCambio",
                "Usuario que cambió"
            );
        }

        private void ConfigurarColumna(
            string nombreColumna,
            string claveTraduccion,
            string textoPorDefecto)
        {
            if (!dgvHistorialEmail.Columns.Contains(nombreColumna))
            {
                return;
            }

            dgvHistorialEmail.Columns[nombreColumna].Tag = claveTraduccion;
            dgvHistorialEmail.Columns[nombreColumna].HeaderText = TraducirMensaje(
                claveTraduccion,
                textoPorDefecto
            );
        }

        private void btnRestaurarEmail_Click(object sender, EventArgs e)
        {
            if (dgvHistorialEmail.CurrentRow == null)
            {
                MostrarAdvertencia(
                    "Mensajes.HistorialEmail.SeleccionarRegistro",
                    "Debe seleccionar un registro del historial.",
                    "Mensajes.Titulos.HistorialEmail",
                    "Historial de email"
                );

                return;
            }

            string emailAnterior = Convert.ToString(
                dgvHistorialEmail.CurrentRow.Cells["EmailAnterior"].Value
            );

            string plantillaConfirmacion = TraducirMensaje(
                "Mensajes.HistorialEmail.ConfirmarRestaurar",
                "¿Desea restaurar el email anterior del usuario '{0}'?\n\nEmail a restaurar: {1}"
            );

            string mensajeConfirmacion = string.Format(
                plantillaConfirmacion,
                _nombreUsuario,
                emailAnterior
            );

            DialogResult confirmacion = MostrarConfirmacion(
                mensajeConfirmacion,
                "Mensajes.Titulos.RestaurarEmail",
                "Restaurar email",
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

                MostrarInformacion(
                    "Mensajes.HistorialEmail.EmailRestaurado",
                    "El email fue restaurado correctamente.",
                    "Mensajes.Titulos.HistorialEmail",
                    "Historial de email"
                );

                CargarHistorial();
            }
            catch (Exception)
            {
                MostrarError(
                    "Mensajes.HistorialEmail.ErrorRestaurar",
                    "No fue posible restaurar el email. Verifique los datos e intente nuevamente.",
                    "Mensajes.Titulos.Error",
                    "Error"
                );
            }
        }

        private string TraducirMensaje(string clave, string textoPorDefecto)
        {
            return MensajeTraducido.TraducirConFallback(
                _idiomaAppService,
                clave,
                textoPorDefecto
            );
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

        private void MostrarError(
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
                MessageBoxIcon.Error
            );
        }

        private DialogResult MostrarConfirmacion(
            string mensaje,
            string claveTitulo,
            string tituloPorDefecto,
            MessageBoxIcon icono)
        {
            return MessageBox.Show(
                this,
                mensaje,
                TraducirMensaje(claveTitulo, tituloPorDefecto),
                MessageBoxButtons.YesNo,
                icono
            );
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