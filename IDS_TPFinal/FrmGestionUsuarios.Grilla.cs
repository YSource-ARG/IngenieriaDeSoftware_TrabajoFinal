using System;
using System.Linq;

namespace IDS_TPFinal
{
    public partial class FrmGestionUsuarios
    {
        private void CargarUsuarios()
        {
            if (_gestionUsuariosAppService == null)
            {
                return;
            }

            _cargandoUsuarios = true;

            try
            {
                var usuarios = _gestionUsuariosAppService.ListarUsuarios(null, null);

                dgvUsuarios.DataSource = usuarios
                    .Select(usuario => new
                    {
                        usuario.Id,
                        usuario.NombreUsuario,
                        usuario.NombreCompleto,
                        usuario.Email,
                        usuario.Activo,
                        Estado = usuario.Activo
                            ? TraducirMensaje("Usuarios.Estado.Activo", "Activo")
                            : TraducirMensaje("Usuarios.Estado.Inactivo", "Inactivo"),
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

        private void CargarUsuarioSeleccionadoEnFormulario()
        {
            if (dgvUsuarios.CurrentRow == null)
            {
                return;
            }

            txtId.Text = ObtenerValorCeldaActual("Id");
            txtNombreUsuario.Text = ObtenerValorCeldaActual("NombreUsuario");
            txtNombreCompleto.Text = ObtenerValorCeldaActual("NombreCompleto");
            txtEmail.Text = ObtenerValorCeldaActual("Email");
            txtFechaCreacion.Text = ObtenerValorCeldaActual("FechaCreacion");
            txtFechaUltimoAcceso.Text = ObtenerValorCeldaActual("UltimoAcceso");

            bool activo = true;

            if (dgvUsuarios.Columns.Contains("Activo"))
            {
                object valorActivo = dgvUsuarios.CurrentRow.Cells["Activo"].Value;

                if (valorActivo is bool)
                {
                    activo = (bool)valorActivo;
                }
            }

            SeleccionarEstadoCombo(activo);

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

            if (dgvUsuarios.Columns.Contains("Activo"))
            {
                dgvUsuarios.Columns["Activo"].Visible = false;
            }

            ConfigurarColumna(
                "NombreUsuario",
                TraducirMensaje("Usuarios.Grilla.Usuario", "Usuario"),
                90
            );

            ConfigurarColumna(
                "NombreCompleto",
                TraducirMensaje("Usuarios.Grilla.NombreCompleto", "Nombre completo"),
                150
            );

            ConfigurarColumna(
                "Email",
                TraducirMensaje("Usuarios.Grilla.Email", "Email"),
                150
            );

            ConfigurarColumna(
                "Estado",
                TraducirMensaje("Usuarios.Grilla.Estado", "Estado"),
                70
            );

            ConfigurarColumna(
                "FechaCreacion",
                TraducirMensaje("Usuarios.Grilla.FechaCreacion", "Fecha creación"),
                110
            );

            ConfigurarColumna(
                "UltimoAcceso",
                TraducirMensaje("Usuarios.Grilla.UltimoAcceso", "Último acceso"),
                110
            );
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