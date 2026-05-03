using System;
using System.Data.SqlClient;
using BE;
using DAL.BaseDeDatos;

namespace DAL.Usuarios
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;

        public UsuarioRepositorio(IConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public Usuario ObtenerPorNombreUsuario(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                throw new ArgumentException("El nombre de usuario no puede estar vacío.", nameof(nombreUsuario));
            }

            const string sql = "SELECT Id, NombreUsuario, PasswordHash, Activo, FechaCreacion, FechaUltimoAcceso FROM dbo.Usuario WHERE NombreUsuario = @NombreUsuario AND Activo = 1";

            using (SqlConnection connection = _connectionFactory.CrearConexion())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new Usuario
                    {
                        Id = Guid.Parse(reader["Id"].ToString()),
                        NombreUsuario = reader["NombreUsuario"].ToString(),
                        PasswordHash = reader["PasswordHash"].ToString(),
                        Activo = Convert.ToBoolean(reader["Activo"]),
                        FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                        FechaUltimoAcceso = reader["FechaUltimoAcceso"] == DBNull.Value
                            ? (DateTime?)null
                            : Convert.ToDateTime(reader["FechaUltimoAcceso"])
                    };
                }
            }
        }

        public void ActualizarFechaUltimoAcceso(Guid idUsuario)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            const string sql = "UPDATE dbo.Usuario SET FechaUltimoAcceso = SYSDATETIME() WHERE Id = @Id AND Activo = 1";

            using (SqlConnection connection = _connectionFactory.CrearConexion())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Id", idUsuario);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}