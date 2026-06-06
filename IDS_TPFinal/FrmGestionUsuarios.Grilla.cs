using System;
using System.Linq;

namespace IDS_TPFinal
{
    public partial class FrmGestionUsuarios
    {
        // Se consulta los usuarios mediante GestionUsuariosAppService y los muestra en la grilla
        private void CargarUsuarios()
        {
            if (_gestionUsuariosAppService == null)
            {
                return;
            }

            _cargandoUsuarios = true;

            try
            {
                // Se pide los usuarios a GestionUsuariosAppServices en la BLL
                var usuarios = _gestionUsuariosAppService.ListarUsuarios(null, null);

                dgvUsuarios.DataSource = usuarios
                    .Select(usuario => new
                    {
                        usuario.Id,
                        usuario.NombreUsuario,
                        usuario.NombreCompleto,
                        Estado = usuario.Activo ? "Activo" : "Inactivo",
                        FechaCreacion = usuario.FechaCreacion.ToString("dd/MM/yyyy HH:mm"),
                        UltimoAcceso = usuario.FechaUltimoAcceso.HasValue
                            ? usuario.FechaUltimoAcceso.Value.ToString("dd/MM/yyyy HH:mm")
                            : "-"
                    })
                    .ToList();

                ConfigurarColumnasUsuarios();
                LimpiarSeleccionGrilla();
                LimpiarCampos();
                AplicarModoAlta();
            }
            finally
            {
                _cargandoUsuarios = false;
            }
        }

        // Lógica para extrar y plasmar datos de un usuario seleccionado de la grilla
        private void CargarUsuarioSeleccionadoEnFormulario()
        {
            if (dgvUsuarios.CurrentRow == null)
            {
                return;
            }

            txtId.Text = ObtenerValorCeldaActual("Id");
            txtNombreUsuario.Text = ObtenerValorCeldaActual("NombreUsuario");
            txtNombreCompleto.Text = ObtenerValorCeldaActual("NombreCompleto");
            txtFechaCreacion.Text = ObtenerValorCeldaActual("FechaCreacion");
            txtFechaUltimoAcceso.Text = ObtenerValorCeldaActual("UltimoAcceso");

            string estado = ObtenerValorCeldaActual("Estado");

            if (estado == "Activo" || estado == "Inactivo")
            {
                cmbEstado.SelectedItem = estado;
            }

            txtPassword.Clear();
            AplicarModoConsulta();
        }

        private void ConfigurarColumnasUsuarios()
        {
            if (dgvUsuarios.Columns.Count == 0 || !dgvUsuarios.Columns.Contains("Id"))
            {
                return;
            }

            dgvUsuarios.Columns["Id"].Visible = false;

            ConfigurarColumna("NombreUsuario", "Usuario", 90);
            ConfigurarColumna("NombreCompleto", "Nombre completo", 150);
            ConfigurarColumna("Estado", "Estado", 70);
            ConfigurarColumna("FechaCreacion", "Fecha creación", 110);
            ConfigurarColumna("UltimoAcceso", "Último acceso", 110);
        }

        private void ConfigurarColumna(string nombreColumna, string textoCabecera, float anchoRelativo)
        {
            if (!dgvUsuarios.Columns.Contains(nombreColumna))
            {
                return;
            }

            dgvUsuarios.Columns[nombreColumna].HeaderText = textoCabecera;
            dgvUsuarios.Columns[nombreColumna].FillWeight = anchoRelativo;
        }

        private string ObtenerValorCeldaActual(string nombreColumna)
        {
            if (dgvUsuarios.CurrentRow == null || !dgvUsuarios.Columns.Contains(nombreColumna))
            {
                return string.Empty;
            }

            object valor = dgvUsuarios.CurrentRow.Cells[nombreColumna].Value;

            return Convert.ToString(valor);
        }
    }
}
