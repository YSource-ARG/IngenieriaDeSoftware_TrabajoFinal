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
    public class FrmAsignacionPermisosUsuario : Form
    {
        private readonly GestionPermisosAppService _gestionPermisosAppService;
        private readonly Guid _idUsuario;
        private readonly string _nombreUsuario;
        private readonly IIdiomaAppService _idiomaAppService;

        private Label _lblTitulo;
        private Label _lblUsuario;
        private Label _lblAyuda;
        private TreeView _treeComponentes;
        private Button _btnGuardar;
        private Button _btnCerrar;

        public FrmAsignacionPermisosUsuario(
            GestionPermisosAppService gestionPermisosAppService,
            Guid idUsuario,
            string nombreUsuario,
            IIdiomaAppService idiomaAppService)
        {
            _gestionPermisosAppService = gestionPermisosAppService ?? throw new ArgumentNullException(nameof(gestionPermisosAppService));
            _idUsuario = idUsuario;
            _nombreUsuario = nombreUsuario ?? string.Empty;
            _idiomaAppService = idiomaAppService;

            InicializarComponentes();
            CargarArbolComponentes();
        }

        private void InicializarComponentes()
        {
            Text = "Asignación de permisos";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(760, 560);

            TemaVisual.AplicarFormularioOscuro(this);

            _lblTitulo = new Label
            {
                AutoSize = true,
                Location = new Point(25, 20),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = TemaVisual.TextoPrincipal,
                Text = Traducir("Permisos.Asignacion.Titulo", "Asignación de permisos")
            };

            _lblUsuario = new Label
            {
                AutoSize = true,
                Location = new Point(28, 62),
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = TemaVisual.TextoSecundario,
                Text = string.Format(
                    Traducir("Permisos.Asignacion.Usuario", "Usuario: {0}"),
                    _nombreUsuario
                )
            };

            _lblAyuda = new Label
            {
                AutoSize = true,
                Location = new Point(28, 86),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = TemaVisual.TextoSecundario,
                Text = Traducir(
                    "Permisos.Asignacion.Ayuda",
                    "Marque roles o permisos directos. El árbol refleja la jerarquía Composite en forma recursiva."
                )
            };

            _treeComponentes = new TreeView
            {
                Location = new Point(30, 120),
                Size = new Size(690, 360),
                CheckBoxes = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = TemaVisual.FondoInput,
                ForeColor = TemaVisual.TextoPrincipal,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                HideSelection = false
            };

            _btnGuardar = new Button
            {
                Location = new Point(420, 500),
                Size = new Size(140, 40),
                Text = Traducir("General.Guardar", "Guardar")
            };
            TemaVisual.AplicarBotonPrincipal(_btnGuardar);
            _btnGuardar.Click += btnGuardar_Click;

            _btnCerrar = new Button
            {
                Location = new Point(580, 500),
                Size = new Size(140, 40),
                Text = Traducir("General.Cerrar", "Cerrar")
            };
            TemaVisual.AplicarBotonSecundario(_btnCerrar);
            _btnCerrar.Click += (s, e) => Close();

            Controls.Add(_lblTitulo);
            Controls.Add(_lblUsuario);
            Controls.Add(_lblAyuda);
            Controls.Add(_treeComponentes);
            Controls.Add(_btnGuardar);
            Controls.Add(_btnCerrar);
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

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                List<Guid> componentesSeleccionados = new List<Guid>();

                foreach (TreeNode nodoRaiz in _treeComponentes.Nodes)
                {
                    ObtenerComponentesSeleccionadosRecursivo(nodoRaiz, componentesSeleccionados);
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

        private void ObtenerComponentesSeleccionadosRecursivo(TreeNode nodo, List<Guid> componentesSeleccionados)
        {
            ComponentePermisos componente = nodo.Tag as ComponentePermisos;

            if (componente != null && nodo.Checked)
            {
                componentesSeleccionados.Add(componente.Id);
            }

            foreach (TreeNode hijo in nodo.Nodes)
            {
                ObtenerComponentesSeleccionadosRecursivo(hijo, componentesSeleccionados);
            }
        }

        private string Traducir(string clave, string fallback)
        {
            return MensajeTraducido.TraducirConFallback(_idiomaAppService, clave, fallback);
        }
    }
}

