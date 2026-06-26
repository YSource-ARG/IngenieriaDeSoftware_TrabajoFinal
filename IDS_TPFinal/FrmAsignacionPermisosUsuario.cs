using BE.Permisos;
using BLL.Idiomas;
using BLL.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UI.Estilos;
using UI.Idiomas;

namespace IDS_TPFinal
{
    public partial class FrmAsignacionPermisosUsuario : Form, IObservadorIdioma
    {
        private readonly GestionPermisosAppService _gestionPermisosAppService;
        private readonly Guid _idUsuario;
        private readonly string _nombreUsuario;
        private readonly IIdiomaAppService _idiomaAppService;
        private bool _actualizandoChecks;

        public FrmAsignacionPermisosUsuario()
        {
            InitializeComponent();
        }

        public FrmAsignacionPermisosUsuario(
            GestionPermisosAppService gestionPermisosAppService,
            Guid idUsuario,
            string nombreUsuario,
            IIdiomaAppService idiomaAppService)
            : this()
        {
            _gestionPermisosAppService = gestionPermisosAppService
                ?? throw new ArgumentNullException(nameof(gestionPermisosAppService));

            _idUsuario = idUsuario;
            _nombreUsuario = nombreUsuario ?? string.Empty;
            _idiomaAppService = idiomaAppService;

            AplicarEstiloVisual();

            if (_idiomaAppService != null)
            {
                _idiomaAppService.Suscribir(this);
                ActualizarIdioma();
            }
            else
            {
                ActualizarTextoUsuario();
                CargarArbolComponentes();
            }
        }

        private void AplicarEstiloVisual()
        {
            TemaVisual.AplicarFormularioOscuro(this);
            TemaVisual.AplicarTitulo(_lblTitulo);
            TemaVisual.AplicarTextoSecundario(_lblUsuario);
            TemaVisual.AplicarTextoSecundario(_lblAyuda);

            _treeComponentes.BackColor = TemaVisual.FondoInput;
            _treeComponentes.ForeColor = TemaVisual.TextoPrincipal;
            _treeComponentes.LineColor = TemaVisual.BordeSuave;

            TemaVisual.AplicarBotonPrincipal(_btnGuardar);
            TemaVisual.AplicarBotonSecundario(_btnCerrar);
        }

        public void ActualizarIdioma()
        {
            if (_idiomaAppService == null)
            {
                return;
            }

            TraductorControles.TraducirFormulario(this, _idiomaAppService);
            ActualizarTextoUsuario();
            CargarArbolComponentes();
        }

        private void ActualizarTextoUsuario()
        {
            _lblUsuario.Text = string.Format(
                Traducir("Permisos.Asignacion.Usuario", "Usuario: {0}"),
                _nombreUsuario
            );
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_idiomaAppService != null)
            {
                _idiomaAppService.Desuscribir(this);
            }

            base.OnFormClosed(e);
        }

        private void CargarArbolComponentes()
        {
            _treeComponentes.Nodes.Clear();

            List<ComponentePermisos> asignados = _gestionPermisosAppService.ListarComponentesAsignadosAUsuario(_idUsuario);
            HashSet<Guid> idsAsignados = new HashSet<Guid>(asignados.Select(x => x.Id));

            List<Rol> roles = _gestionPermisosAppService.ListarRoles();
            List<Permiso> permisos = _gestionPermisosAppService.ListarPermisos();
            HashSet<Guid> idsComponentesHijos = ObtenerIdsComponentesHijos();

            TreeNode nodoRoles = new TreeNode(Traducir("Permisos.Asignacion.Roles", "Roles"));
            TreeNode nodoPermisos = new TreeNode(Traducir("Permisos.Asignacion.PermisosDirectos", "Permisos directos"));

            foreach (Rol rolRaiz in roles.Where(x => !idsComponentesHijos.Contains(x.Id)).OrderBy(x => x.Nombre))
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
            TreeNode nodo = new TreeNode(ConstruirTextoNodo(componente))
            {
                Tag = componente,
                Checked = idsAsignados.Contains(componente.Id)
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

        private string ConstruirTextoNodo(ComponentePermisos componente)
        {
            string prefijo = componente.Tipo == TipoComponentePermisos.Rol
                ? "[Rol]"
                : "[Permiso]";

            return $"{prefijo} {componente.Nombre} ({componente.Codigo})";
        }

        private void treeComponentes_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (_actualizandoChecks)
            {
                return;
            }

            ComponentePermisos componente = e.Node.Tag as ComponentePermisos;

            if (componente == null || componente.Tipo != TipoComponentePermisos.Rol)
            {
                return;
            }

            try
            {
                _actualizandoChecks = true;
                PropagarCheckAHijos(e.Node, e.Node.Checked);
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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                List<Guid> componentesSeleccionados = new List<Guid>();

                foreach (TreeNode nodoRaiz in _treeComponentes.Nodes)
                {
                    ObtenerComponentesDirectosSeleccionadosRecursivo(nodoRaiz, false, componentesSeleccionados);
                }

                _gestionPermisosAppService.ReemplazarAsignacionesUsuario(
                    _idUsuario,
                    componentesSeleccionados.Distinct().ToList()
                );

                MessageBox.Show(
                    this,
                    Traducir("Permisos.Asignacion.GuardadoOk", "Los permisos del usuario se guardaron correctamente."),
                    Traducir("Permisos.Asignacion.Titulo", "Asignación de permisos"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                DialogResult = DialogResult.OK;
                Close();
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

        private void ObtenerComponentesDirectosSeleccionadosRecursivo(
            TreeNode nodo,
            bool ancestroSeleccionado,
            List<Guid> componentesSeleccionados)
        {
            ComponentePermisos componente = nodo.Tag as ComponentePermisos;
            bool componenteSeleccionado = componente != null && nodo.Checked;

            if (componenteSeleccionado && !ancestroSeleccionado)
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

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private string Traducir(string clave, string fallback)
        {
            return MensajeTraducido.TraducirConFallback(_idiomaAppService, clave, fallback);
        }
    }
}
