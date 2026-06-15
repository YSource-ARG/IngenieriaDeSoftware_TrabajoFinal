using BE;
using DAL.BaseDeDatos;
using DAL.Excepciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL.Integridad
{
    public class DigitoVerificadorRepositorio : IDigitoVerificadorRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;

        public DigitoVerificadorRepositorio(IConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public List<Usuario> ListarUsuariosParaIntegridad()
        {
            const string sql = @"
SELECT
    Id,
    NombreUsuario,
    NombreCompleto,
    PasswordHash,
    Activo,
    DebeCambiarPassword,
    IntentosFallidosLogin,
    BloqueadoHasta,
    BloqueadoPorIntegridad,
    DigitoVerificadorHorizontal,
    FechaCreacion,
    FechaUltimoAcceso
FROM dbo.Usuario
ORDER BY Id";

            List<Usuario> usuarios = new List<Usuario>();

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(MapearUsuario(reader));
                        }
                    }
                }

                return usuarios;
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron listar los usuarios para verificar integridad.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public string ObtenerDigitoVerificadorVertical(string entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad))
            {
                throw new ArgumentException("La entidad no puede estar vacía.", nameof(entidad));
            }

            const string sql = @"
SELECT Valor
FROM dbo.DigitoVerificadorVertical
WHERE Entidad = @Entidad";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Entidad", SqlDbType.NVarChar, 100).Value = entidad.Trim();

                    connection.Open();

                    object resultado = command.ExecuteScalar();

                    return resultado == null || resultado == DBNull.Value
                        ? null
                        : resultado.ToString();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo obtener el dígito verificador vertical.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void ActualizarDigitoVerificadorHorizontal(Guid idUsuario, string valor)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ArgumentException("El valor del DVH no puede estar vacío.", nameof(valor));
            }

            const string sql = @"
UPDATE dbo.Usuario
SET DigitoVerificadorHorizontal = @DigitoVerificadorHorizontal
WHERE Id = @Id";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = idUsuario;
                    command.Parameters.Add("@DigitoVerificadorHorizontal", SqlDbType.NVarChar, 256).Value = valor;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo actualizar el dígito verificador horizontal del usuario.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void GuardarDigitoVerificadorVertical(string entidad, string valor)
        {
            if (string.IsNullOrWhiteSpace(entidad))
            {
                throw new ArgumentException("La entidad no puede estar vacía.", nameof(entidad));
            }

            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ArgumentException("El valor del DVV no puede estar vacío.", nameof(valor));
            }

            const string sql = @"
IF EXISTS
(
    SELECT 1
    FROM dbo.DigitoVerificadorVertical
    WHERE Entidad = @Entidad
)
BEGIN
    UPDATE dbo.DigitoVerificadorVertical
    SET Valor = @Valor,
        FechaCalculo = SYSDATETIME()
    WHERE Entidad = @Entidad
END
ELSE
BEGIN
    INSERT INTO dbo.DigitoVerificadorVertical
    (
        Entidad,
        Valor,
        FechaCalculo
    )
    VALUES
    (
        @Entidad,
        @Valor,
        SYSDATETIME()
    )
END";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Entidad", SqlDbType.NVarChar, 100).Value = entidad.Trim();
                    command.Parameters.Add("@Valor", SqlDbType.NVarChar, 256).Value = valor;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo guardar el dígito verificador vertical.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void BloquearUsuariosPorIntegridadExceptoAdmin()
        {
            const string sql = @"
UPDATE dbo.Usuario
SET BloqueadoPorIntegridad = 1
WHERE NombreUsuario <> 'admin';

UPDATE dbo.Usuario
SET BloqueadoPorIntegridad = 0
WHERE NombreUsuario = 'admin';";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron bloquear los usuarios por integridad.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void DesbloquearUsuariosPorIntegridad()
        {
            const string sql = @"
UPDATE dbo.Usuario
SET BloqueadoPorIntegridad = 0";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron desbloquear los usuarios por integridad.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        private Usuario MapearUsuario(SqlDataReader reader)
        {
            return new Usuario
            {
                Id = Guid.Parse(reader["Id"].ToString()),
                NombreUsuario = reader["NombreUsuario"].ToString(),
                NombreCompleto = reader["NombreCompleto"] == DBNull.Value
                    ? null
                    : reader["NombreCompleto"].ToString(),
                PasswordHash = reader["PasswordHash"].ToString(),
                Activo = Convert.ToBoolean(reader["Activo"]),
                DebeCambiarPassword = Convert.ToBoolean(reader["DebeCambiarPassword"]),
                IntentosFallidosLogin = Convert.ToInt32(reader["IntentosFallidosLogin"]),
                BloqueadoHasta = reader["BloqueadoHasta"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(reader["BloqueadoHasta"]),

                // Bloqueo preventivo por falla de integridad.
                // No reemplaza al campo Activo porque cumple otra finalidad.
                BloqueadoPorIntegridad = Convert.ToBoolean(reader["BloqueadoPorIntegridad"]),

                // DVH persistido en la entidad protegida.
                // Se compara contra el DVH recalculado para detectar cambios externos.
                DigitoVerificadorHorizontal = reader["DigitoVerificadorHorizontal"].ToString(),

                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                FechaUltimoAcceso = reader["FechaUltimoAcceso"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(reader["FechaUltimoAcceso"])
            };
        }
    }
}