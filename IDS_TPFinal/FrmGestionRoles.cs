using BE.Permisos;
using BLL.Idiomas;
using BLL.Permisos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using UI.Estilos;
using UI.Idiomas;

namespace IDS_TPFinal
{
    public partial class FrmGestionRoles : Form, IObservadorIdioma
    {
        private readonly GestionPermisosAppService _gestionPermisosAppService;
        private readonly IIdiomaAppService _idiomaAppService;
        
        private bool _actualizandoChecks;
        private bool _cargandoRoles;
        private bool _modoCreacion;
        private bool _modoEdicion;
        private Guid _idRolActual;

        public FrmGestionRoles()
        {
            InitializeComponent();
        }

        public FrmGestionRoles(
            GestionPermisosAppService gestionPermisosAppService,
            IIdiomaAppService idiomaAppService)
            : this()
        {
            _gestionPermisosAppService = gestionPermisosAppService
                ?? throw new ArgumentNullException(nameof(gestionPermisosAppService));

            _idiomaAppService = idiomaAppService;

            AplicarEstiloVisual();

            if (_idiomaAppService != null)
            {
                _idiomaAppService.Suscribir(this);
                ActualizarIdioma();
            }

            CargarRoles();
            AplicarModoVisual();
        }

        private void AplicarEstiloVisual()
        {
            TemaVisual.AplicarFormularioOscuro(this);
            TemaVisual.AplicarTitulo(_lblTitulo);
            TemaVisual.AplicarTextoSecundario(_lblAyuda);

            Label[] etiquetas =
            {
                _lblRolExistente,
                _lblNombre,
                _lblCodigo,
                _lblDescripcion
            };

            foreach (Label etiqueta in etiquetas)
            {
                etiqueta.ForeColor = TemaVisual.TextoPrincipal;
                etiqueta.BackColor = Color.Transparent;
                etiqueta.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            }

            _cboRoles.BackColor = TemaVisual.FondoInput;
            _cboRoles.ForeColor = TemaVisual.TextoPrincipal;
            _cboRoles.FlatStyle = FlatStyle.Flat;
            _cboRoles.Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            TemaVisual.AplicarTextBox(_txtNombre);
            TemaVisual.AplicarTextBox(_txtCodigo);
            TemaVisual.AplicarTextBox(_txtDescripcion);

            _chkActivo.ForeColor = TemaVisual.TextoPrincipal;
            _chkActivo.BackColor = Color.Transparent;

            _treeComponentes.BackColor = TemaVisual.FondoInput;
            _treeComponentes.ForeColor = TemaVisual.TextoPrincipal;
            _treeComponentes.LineColor = TemaVisual.BordeSuave;

            TemaVisual.AplicarBotonPrincipal(_btnGuardar);
            TemaVisual.AplicarBotonSecundario(_btnNuevoRol);
            TemaVisual.AplicarBotonSecundario(_btnEditarRol);
            TemaVisual.AplicarBotonSecundario(_btnEliminarRol);
            TemaVisual.AplicarBotonSecundario(_btnCancelarNuevo);
            TemaVisual.AplicarBotonSecundario(_btnCerrar);
        }

        public void ActualizarIdioma()
        {
            if (_idiomaAppService == null)
            {
                return;
            }

            TraductorControles.TraducirFormulario(this, _idiomaAppService);
            AplicarModoVisual();

            if (_gestionPermisosAppService == null)
            {
                return;
            }

            if (_modoCreacion)
            {
                CargarArbolModoCreacion();
                return;
            }

            if (_cboRoles.DataSource != null)
            {
                CargarRolSeleccionado();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_idiomaAppService != null)
            {
                _idiomaAppService.Desuscribir(this);
            }

            base.OnFormClosed(e);
        }

        private void CargarRoles(Guid? idRolSeleccionado = null)
        {
            _cargandoRoles = true;

            try
            {
                List<Rol> roles = _gestionPermisosAppService
                    .ListarRoles()
                    .OrderBy(x => x.Nombre)
                    .ToList();

                _cboRoles.DataSource = null;
                _cboRoles.DisplayMember = "Nombre";
                _cboRoles.ValueMember = "Id";
                _cboRoles.DataSource = roles;

                if (roles.Count == 0)
                {
                    _idRolActual = Guid.Empty;
                    LimpiarAtributosRol();
                    _treeComponentes.Nodes.Clear();
                    _btnGuardar.Enabled = _modoCreacion;
                    return;
                }

                Rol rolSeleccionado = roles.FirstOrDefault(x => idRolSeleccionado.HasValue && x.Id == idRolSeleccionado.Value)
                    ?? roles.First();

                _cboRoles.SelectedItem = rolSeleccionado;
            }
            finally
            {
                _cargandoRoles = false;
            }

            if (!_modoCreacion)
            {
                CargarRolSeleccionado();
            }
        }

        private void cboRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cargandoRoles || _modoCreacion)
            {
                return;
            }

            CargarRolSeleccionado();
        }

        private void CargarRolSeleccionado()
        {
            Rol rolSeleccionadoCombo = _cboRoles.SelectedItem as Rol;

            if (rolSeleccionadoCombo == null)
            {
                _idRolActual = Guid.Empty;
                LimpiarAtributosRol();
                _treeComponentes.Nodes.Clear();
                return;
            }

            Rol rolSeleccionado = _gestionPermisosAppService.ObtenerRolPorId(rolSeleccionadoCombo.Id);
            _idRolActual = rolSeleccionado.Id;

            CargarAtributosRol(rolSeleccionado);
            CargarArbolComponentes(new HashSet<Guid>(rolSeleccionado.Hijos.Select(x => x.Id)));

            AplicarModoVisual();
        }

        private void CargarArbolModoCreacion()
        {
            _idRolActual = Guid.Empty;
            CargarArbolComponentes(new HashSet<Guid>());
        }

        private void CargarArbolComponentes(HashSet<Guid> idsAsignados)
        {
            _treeComponentes.Nodes.Clear();

            List<Rol> roles = _gestionPermisosAppService.ListarRoles();
            List<Permiso> permisos = _gestionPermisosAppService.ListarPermisos();
            HashSet<Guid> idsComponentesHijos = ObtenerIdsComponentesHijos();

            TreeNode nodoRoles = new TreeNode(Traducir("Roles.Gestion.Roles", "Roles y subroles"));
            TreeNode nodoPermisos = new TreeNode(Traducir("Roles.Gestion.Permisos", "Permisos directos"));

            foreach (Rol rolRaiz in ObtenerRolesVisibles(roles, idsComponentesHijos))
            {
                nodoRoles.Nodes.Add(CrearNodoComponenteRecursivo(rolRaiz, idsAsignados, new HashSet<Guid>()));
            }

            foreach (Permiso permisoRaiz in permisos.Where(x => !idsComponentesHijos.Contains(x.Id)).OrderBy(x => x.Nombre))
            {
                nodoPermisos.Nodes.Add(CrearNodoComponenteRecursivo(permisoRaiz, idsAsignados, new HashSet<Guid>()));
            }

            if (nodoRoles.Nodes.Count > 0)
            {
                nodoRoles.Expand();
                _treeComponentes.Nodes.Add(nodoRoles);
            }

            if (nodoPermisos.Nodes.Count > 0)
            {
                nodoPermisos.Expand();
                _treeComponentes.Nodes.Add(nodoPermisos);
            }

            AplicarChecksVisualesDesdeRoles();
            _treeComponentes.ExpandAll();
        }

        private IEnumerable<Rol> ObtenerRolesVisibles(List<Rol> roles, HashSet<Guid> idsComponentesHijos)
        {
            List<Rol> rolesVisibles = roles
                .Where(x => !idsComponentesHijos.Contains(x.Id))
                .OrderBy(x => x.Nombre)
                .ToList();

            if (_idRolActual != Guid.Empty)
            {
                Rol rolActual = roles.FirstOrDefault(x => x.Id == _idRolActual);

                if (rolActual != null && !rolesVisibles.Any(x => x.Id == rolActual.Id))
                {
                    rolesVisibles.Add(rolActual);
                    rolesVisibles = rolesVisibles
                        .OrderBy(x => x.Nombre)
                        .ToList();
                }
            }

            if (rolesVisibles.Count > 0)
            {
                return rolesVisibles;
            }

            return roles.OrderBy(x => x.Nombre).ToList();
        }

        private HashSet<Guid> ObtenerIdsComponentesHijos()
        {
            HashSet<Guid> ids = new HashSet<Guid>();

            foreach (Rol rol in _gestionPermisosAppService.ListarRoles())
            {
                foreach (ComponentePermisos hijo in _gestionPermisosAppService.ListarHijosDeRol(rol.Id))
                {
                    ids.Add(hijo.Id);
                }
            }

            return ids;
        }

        private TreeNode CrearNodoComponenteRecursivo(
            ComponentePermisos componente,
            HashSet<Guid> idsAsignados,
            HashSet<Guid> rolesVisitados)
        {
            bool esRolActual = componente.Id == _idRolActual;

            TreeNode nodo = new TreeNode(ConstruirTextoNodo(componente, esRolActual))
            {
                Tag = componente,
                Checked = idsAsignados.Contains(componente.Id),
                ForeColor = esRolActual ? Color.FromArgb(255, 196, 0) : TemaVisual.TextoPrincipal
            };

            if (componente.Tipo != TipoComponentePermisos.Rol)
            {
                return nodo;
            }

            if (!rolesVisitados.Add(componente.Id))
            {
                return nodo;
            }

            foreach (ComponentePermisos hijo in _gestionPermisosAppService
                .ListarHijosDeRol(componente.Id)
                .OrderBy(x => x.Tipo)
                .ThenBy(x => x.Nombre))
            {
                nodo.Nodes.Add(CrearNodoComponenteRecursivo(hijo, idsAsignados, new HashSet<Guid>(rolesVisitados)));
            }

            return nodo;
        }

        private string ConstruirTextoNodo(ComponentePermisos componente, bool esRolActual)
        {
            string prefijo = componente.Tipo == TipoComponentePermisos.Rol
                ? "[Rol]"
                : "[Permiso]";

            string sufijo = esRolActual
                ? Traducir("Roles.Gestion.RolActual", " - rol actual")
                : string.Empty;

            return $"{prefijo} {componente.Nombre} ({componente.Codigo}){sufijo}";
        }

        private void treeComponentes_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (_actualizandoChecks)
            {
                return;
            }

            ComponentePermisos componente = e.Node.Tag as ComponentePermisos;

            if (componente == null)
            {
                return;
            }

            try
            {
                _actualizandoChecks = true;

                if (_idRolActual != Guid.Empty && componente.Id == _idRolActual)
                {
                    e.Node.Checked = false;
                    return;
                }

                if (componente.Tipo == TipoComponentePermisos.Rol)
                {
                    PropagarCheckAHijos(e.Node, e.Node.Checked);
                }
            }
            finally
            {
                _actualizandoChecks = false;
            }
        }

        private void PropagarCheckAHijos(TreeNode nodoPadre, bool checkeado)
        {
            foreach (TreeNode nodoHijo in nodoPadre.Nodes)
            {
                nodoHijo.Checked = checkeado;
                PropagarCheckAHijos(nodoHijo, checkeado);
            }
        }

        private void AplicarChecksVisualesDesdeRoles()
        {
            try
            {
                _actualizandoChecks = true;

                foreach (TreeNode nodoRaiz in _treeComponentes.Nodes)
                {
                    AplicarChecksVisualesDesdeRolesRecursivo(nodoRaiz);
                }
            }
            finally
            {
                _actualizandoChecks = false;
            }
        }

        private void AplicarChecksVisualesDesdeRolesRecursivo(TreeNode nodo)
        {
            ComponentePermisos componente = nodo.Tag as ComponentePermisos;

            if (componente != null && componente.Tipo == TipoComponentePermisos.Rol && nodo.Checked)
            {
                PropagarCheckAHijos(nodo, true);
            }

            foreach (TreeNode hijo in nodo.Nodes)
            {
                AplicarChecksVisualesDesdeRolesRecursivo(hijo);
            }
        }

        private void btnNuevoRol_Click(object sender, EventArgs e)
        {
            _modoCreacion = true;
            LimpiarAtributosRol();
            _chkActivo.Checked = true;
            AplicarModoVisual();
            CargarArbolModoCreacion();
        }

        private void btnCancelarNuevo_Click(object sender, EventArgs e)
        {
            _modoCreacion = false;
            _modoEdicion = false;

            AplicarModoVisual();
            CargarRoles(_idRolActual);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_modoCreacion)
                {
                    CrearNuevoRol();
                    return;
                }

                if (_modoEdicion)
                {
                    GuardarCambiosRol();
                    return;
                }

                GuardarComposicionRol();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    Traducir("Mensajes.Titulos.Error", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void CrearNuevoRol()
        {
            List<Guid> componentesSeleccionados = ObtenerComponentesDirectosSeleccionados();

            Guid idNuevoRol = _gestionPermisosAppService.CrearRolConComponentes(
                _txtNombre.Text,
                _txtCodigo.Text,
                _txtDescripcion.Text,
                _chkActivo.Checked,
                componentesSeleccionados
            );

            MessageBox.Show(
                this,
                Traducir("Roles.Gestion.CreadoOk", "El rol se creo correctamente."),
                Traducir("Roles.Gestion.Titulo", "Gestion de roles"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            _modoCreacion = false;
            AplicarModoVisual();
            CargarRoles(idNuevoRol);
        }

        private void GuardarCambiosRol()
        {
            if (_idRolActual == Guid.Empty)
            {
                return;
            }

            _gestionPermisosAppService.ModificarRol(
                _idRolActual,
                _txtNombre.Text,
                _txtCodigo.Text,
                _txtDescripcion.Text,
                _chkActivo.Checked
            );

            MessageBox.Show(
                this,
                Traducir(
                    "Roles.Gestion.ModificadoOk",
                    "Los datos del rol se modificaron correctamente."
                ),
                Traducir("Roles.Gestion.Titulo", "Gestión de roles"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            _modoEdicion = false;

            AplicarModoVisual();
            CargarRoles(_idRolActual);
        }

        private void GuardarComposicionRol()
        {
            if (_idRolActual == Guid.Empty)
            {
                return;
            }

            _gestionPermisosAppService.ReemplazarComponentesDeRol(
                _idRolActual,
                ObtenerComponentesDirectosSeleccionados()
            );

            MessageBox.Show(
                this,
                Traducir("Roles.Gestion.GuardadoOk", "La composicion del rol se guardo correctamente."),
                Traducir("Roles.Gestion.Titulo", "Gestion de roles"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            CargarRolSeleccionado();
        }

        private List<Guid> ObtenerComponentesDirectosSeleccionados()
        {
            List<Guid> componentesSeleccionados = new List<Guid>();

            foreach (TreeNode nodoRaiz in _treeComponentes.Nodes)
            {
                ObtenerComponentesDirectosSeleccionadosRecursivo(nodoRaiz, false, componentesSeleccionados);
            }

            return componentesSeleccionados
                .Where(id => id != Guid.Empty && id != _idRolActual)
                .Distinct()
                .ToList();
        }

        private void ObtenerComponentesDirectosSeleccionadosRecursivo(
            TreeNode nodo,
            bool ancestroSeleccionado,
            List<Guid> componentesSeleccionados)
        {
            ComponentePermisos componente = nodo.Tag as ComponentePermisos;
            bool componenteSeleccionado = componente != null && nodo.Checked;

            if (componenteSeleccionado && !ancestroSeleccionado && componente.Id != _idRolActual)
            {
                componentesSeleccionados.Add(componente.Id);
            }

            foreach (TreeNode hijo in nodo.Nodes)
            {
                ObtenerComponentesDirectosSeleccionadosRecursivo(
                    hijo,
                    ancestroSeleccionado || componenteSeleccionado,
                    componentesSeleccionados
                );
            }
        }

        private void AplicarModoVisual()
        {
            bool camposEditables = _modoCreacion || _modoEdicion;

            _cboRoles.Enabled = !camposEditables;
            _btnNuevoRol.Enabled = !camposEditables;
            _btnCancelarNuevo.Visible = camposEditables;
            _btnEditarRol.Visible = !camposEditables;
            _btnEditarRol.Enabled = !camposEditables && _cboRoles.Items.Count > 0;

            _txtNombre.ReadOnly = !camposEditables;
            _txtDescripcion.ReadOnly = !camposEditables;

            _txtNombre.BackColor = TemaVisual.FondoInput;
            _txtCodigo.BackColor = TemaVisual.FondoInput;
            _txtDescripcion.BackColor = TemaVisual.FondoInput;

            _btnGuardar.Text = _modoCreacion
            ? Traducir("Roles.Gestion.CrearRol", "Crear rol")
            : _modoEdicion
                ? Traducir("Roles.Gestion.GuardarCambios", "Guardar cambios")
                : Traducir(
                    "Roles.Gestion.GuardarComposicion",
                    "Guardar composición"
                );
            _btnGuardar.Enabled = camposEditables || _cboRoles.Items.Count > 0;

            Rol rolSeleccionado = _cboRoles.SelectedItem as Rol;

            bool esRolAdministrador =
                rolSeleccionado != null &&
                string.Equals(
                    rolSeleccionado.Codigo,
                    "ROL_ADMINISTRADOR",
                    StringComparison.OrdinalIgnoreCase
                );

            bool protegerAdministrador = _modoEdicion && esRolAdministrador;

            _txtCodigo.ReadOnly = !camposEditables || protegerAdministrador;

            _chkActivo.Enabled = camposEditables && !protegerAdministrador;

            _btnEliminarRol.Visible = !camposEditables;

            _btnEliminarRol.Enabled = !camposEditables && rolSeleccionado != null && !esRolAdministrador;
        }

        private void CargarAtributosRol(Rol rol)
        {
            _txtNombre.Text = rol.Nombre;
            _txtCodigo.Text = rol.Codigo;
            _txtDescripcion.Text = rol.Descripcion ?? string.Empty;
            _chkActivo.Checked = rol.Activo;
        }

        private void LimpiarAtributosRol()
        {
            _txtNombre.Text = string.Empty;
            _txtCodigo.Text = string.Empty;
            _txtDescripcion.Text = string.Empty;
            _chkActivo.Checked = false;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private string Traducir(string clave, string fallback)
        {
            return MensajeTraducido.TraducirConFallback(_idiomaAppService, clave, fallback);
        }

        private void _btnEliminarRol_Click(object sender, EventArgs e)
        {
            try
            {
                if (_modoCreacion || _idRolActual == Guid.Empty)
                {
                    return;
                }

                Rol rolSeleccionado = _cboRoles.SelectedItem as Rol;

                if (rolSeleccionado == null)
                {
                    return;
                }

                DialogResult respuesta = MessageBox.Show(
                    this,
                    $"¿Está seguro de eliminar el rol '{rolSeleccionado.Nombre}'?\n\n" +
                    "También se eliminarán sus relaciones y asignaciones.",
                    Traducir("Roles.Gestion.ConfirmarEliminarTitulo", "Confirmar eliminación"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (respuesta != DialogResult.Yes)
                {
                    return;
                }

                _gestionPermisosAppService.EliminarRol(_idRolActual);

                MessageBox.Show(
                    this,
                    Traducir(
                        "Roles.Gestion.EliminadoOk",
                        "El rol se eliminó correctamente."
                    ),
                    Traducir("Roles.Gestion.Titulo", "Gestión de roles"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                CargarRoles();
                AplicarModoVisual();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    Traducir("Mensajes.Titulos.Error", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void _btnEditarRol_Click(object sender, EventArgs e)
        {
            if (_modoCreacion || _idRolActual == Guid.Empty)
            {
                return;
            }

            _modoEdicion = true;
            AplicarModoVisual();
        }
    }
}
