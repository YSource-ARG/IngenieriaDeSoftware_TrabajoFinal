using BE;
using DAL.BaseDeDatos;
using DAL.Excepciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL.Usuarios
{
    public class UsuarioEmailHistorialRepositorio : IUsuarioEmailHistorialRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;

        public UsuarioEmailHistorialRepositorio(IConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public void RegistrarCambio(UsuarioEmailHistorial historial)
        {
            if (historial == null)
            {
                throw new ArgumentNullException(nameof(historial));
            }

            const string sql = @"
INSERT INTO dbo.UsuarioEmailHistorial
(
    Id,
    UsuarioId,
    EmailAnterior,
    EmailNuevo,
    FechaCambio,
    UsuarioCambioId,
    UsuarioCambioNombre
)
VALUES
(
    @Id,
    @UsuarioId,
    @EmailAnterior,
    @EmailNuevo,
    @FechaCambio,
    @UsuarioCambioId,
    @UsuarioCambioNombre
)";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = historial.Id;
                    command.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = historial.UsuarioId;

                    command.Parameters.Add("@EmailAnterior", SqlDbType.NVarChar, 255).Value =
                        string.IsNullOrWhiteSpace(historial.EmailAnterior)
                            ? (object)DBNull.Value
                            : historial.EmailAnterior.Trim();

                    command.Parameters.Add("@EmailNuevo", SqlDbType.NVarChar, 255).Value =
                        string.IsNullOrWhiteSpace(historial.EmailNuevo)
                            ? (object)DBNull.Value
                            : historial.EmailNuevo.Trim();

                    command.Parameters.Add("@FechaCambio", SqlDbType.DateTime2).Value = historial.FechaCambio;

                    command.Parameters.Add("@UsuarioCambioId", SqlDbType.UniqueIdentifier).Value =
                        historial.UsuarioCambioId.HasValue
                            ? (object)historial.UsuarioCambioId.Value
                            : DBNull.Value;

                    command.Parameters.Add("@UsuarioCambioNombre", SqlDbType.NVarChar, 100).Value =
                        string.IsNullOrWhiteSpace(historial.UsuarioCambioNombre)
                            ? (object)DBNull.Value
                            : historial.UsuarioCambioNombre.Trim();

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo registrar el historial de cambio de email.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public List<UsuarioEmailHistorial> ListarPorUsuario(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(usuarioId));
            }

            const string sql = @"
SELECT
    Id,
    UsuarioId,
    EmailAnterior,
    EmailNuevo,
    FechaCambio,
    UsuarioCambioId,
    UsuarioCambioNombre
FROM dbo.UsuarioEmailHistorial
WHERE UsuarioId = @UsuarioId
ORDER BY FechaCambio DESC";

            List<UsuarioEmailHistorial> historial = new List<UsuarioEmailHistorial>();

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = usuarioId;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            historial.Add(MapearHistorial(reader));
                        }
                    }
                }

                return historial;
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo obtener el historial de cambios de email.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        private UsuarioEmailHistorial MapearHistorial(SqlDataReader reader)
        {
            return new UsuarioEmailHistorial
            {
                Id = Guid.Parse(reader["Id"].ToString()),
                UsuarioId = Guid.Parse(reader["UsuarioId"].ToString()),

                EmailAnterior = reader["EmailAnterior"] == DBNull.Value
                    ? null
                    : reader["EmailAnterior"].ToString(),

                EmailNuevo = reader["EmailNuevo"] == DBNull.Value
                    ? null
                    : reader["EmailNuevo"].ToString(),

                FechaCambio = Convert.ToDateTime(reader["FechaCambio"]),

                UsuarioCambioId = reader["UsuarioCambioId"] == DBNull.Value
                    ? (Guid?)null
                    : Guid.Parse(reader["UsuarioCambioId"].ToString()),

                UsuarioCambioNombre = reader["UsuarioCambioNombre"] == DBNull.Value
                    ? null
                    : reader["UsuarioCambioNombre"].ToString()
            };
        }
    }
}