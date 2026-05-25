using BE;
using DAL.BaseDeDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

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

            const string sql = @"
                SELECT 
                    Id, 
                    NombreUsuario, 
                    NombreCompleto,
                    PasswordHash, 
                    Activo, 
                    DebeCambiarPassword,
                    FechaCreacion, 
                    FechaUltimoAcceso 
                FROM dbo.Usuario 
                WHERE NombreUsuario = @NombreUsuario 
                  AND Activo = 1";

            using (SqlConnection connection = _connectionFactory.CrearConexion())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar, 100).Value = nombreUsuario;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return MapearUsuario(reader);
                }
            }
        }

        public Usuario ObtenerPorId(Guid idUsuario)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            const string sql = @"
                SELECT 
                    Id, 
                    NombreUsuario, 
                    NombreCompleto,
                    PasswordHash, 
                    Activo, 
                    DebeCambiarPassword,
                    FechaCreacion, 
                    FechaUltimoAcceso 
                FROM dbo.Usuario 
                WHERE Id = @Id";

            using (SqlConnection connection = _connectionFactory.CrearConexion())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = idUsuario;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return MapearUsuario(reader);
                }
            }
        }

        public List<Usuario> Listar(string textoBusqueda, bool? activo)
        {
            const string sql = @"
                SELECT 
                    Id, 
                    NombreUsuario, 
                    NombreCompleto,
                    PasswordHash, 
                    Activo, 
                    DebeCambiarPassword,
                    FechaCreacion, 
                    FechaUltimoAcceso 
                FROM dbo.Usuario
                WHERE (@TextoBusqueda IS NULL 
                       OR NombreUsuario LIKE '%' + @TextoBusqueda + '%'
                       OR NombreCompleto LIKE '%' + @TextoBusqueda + '%')
                  AND (@Activo IS NULL OR Activo = @Activo)
                ORDER BY NombreUsuario";

            List<Usuario> usuarios = new List<Usuario>();

            using (SqlConnection connection = _connectionFactory.CrearConexion())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@TextoBusqueda", SqlDbType.NVarChar, 150).Value =
                    string.IsNullOrWhiteSpace(textoBusqueda)
                        ? (object)DBNull.Value
                        : textoBusqueda.Trim();

                command.Parameters.Add("@Activo", SqlDbType.Bit).Value =
                    activo.HasValue
                        ? (object)activo.Value
                        : DBNull.Value;

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

        public bool ExisteNombreUsuario(string nombreUsuario, Guid? idUsuarioExcluido)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                throw new ArgumentException("El nombre de usuario no puede estar vacío.", nameof(nombreUsuario));
            }

            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.Usuario
                WHERE NombreUsuario = @NombreUsuario
                  AND (@IdUsuarioExcluido IS NULL OR Id <> @IdUsuarioExcluido)";

            using (SqlConnection connection = _connectionFactory.CrearConexion())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar, 100).Value = nombreUsuario.Trim();

                command.Parameters.Add("@IdUsuarioExcluido", SqlDbType.UniqueIdentifier).Value =
                    idUsuarioExcluido.HasValue
                        ? (object)idUsuarioExcluido.Value
                        : DBNull.Value;

                connection.Open();

                int cantidad = Convert.ToInt32(command.ExecuteScalar());

                return cantidad > 0;
            }
        }

        public void Crear(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario));
            }

            const string sql = @"
                INSERT INTO dbo.Usuario
                (
                    Id,
                    NombreUsuario,
                    NombreCompleto,
                    PasswordHash,
                    Activo,
                    DebeCambiarPassword,
                    FechaCreacion,
                    FechaUltimoAcceso
                )
                VALUES
                (
                    @Id,
                    @NombreUsuario,
                    @NombreCompleto,
                    @PasswordHash,
                    @Activo,
                    @DebeCambiarPassword,
                    @FechaCreacion,
                    @FechaUltimoAcceso
                )";

            using (SqlConnection connection = _connectionFactory.CrearConexion())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                CargarParametrosUsuario(command, usuario);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void ModificarDatos(Guid idUsuario, string nombreCompleto, bool activo)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            if (string.IsNullOrWhiteSpace(nombreCompleto))
            {
                throw new ArgumentException("El nombre completo no puede estar vacío.", nameof(nombreCompleto));
            }

            const string sql = @"
                UPDATE dbo.Usuario
                SET NombreCompleto = @NombreCompleto,
                    Activo = @Activo
                WHERE Id = @Id";

            using (SqlConnection connection = _connectionFactory.CrearConexion())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = idUsuario;
                command.Parameters.Add("@NombreCompleto", SqlDbType.NVarChar, 150).Value = nombreCompleto.Trim();
                command.Parameters.Add("@Activo", SqlDbType.Bit).Value = activo;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void CambiarEstado(Guid idUsuario, bool activo)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            const string sql = @"
                UPDATE dbo.Usuario
                SET Activo = @Activo
                WHERE Id = @Id";

            using (SqlConnection connection = _connectionFactory.CrearConexion())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = idUsuario;
                command.Parameters.Add("@Activo", SqlDbType.Bit).Value = activo;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void ActualizarPassword(Guid idUsuario, string passwordHash)
        {
            ActualizarPasswordYEstadoCambioObligatorio(idUsuario, passwordHash, false);
        }

        public void ActualizarPasswordYEstadoCambioObligatorio(Guid idUsuario, string passwordHash, bool debeCambiarPassword)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new ArgumentException("El hash de contraseña no puede estar vacío.", nameof(passwordHash));
            }

            const string sql = @"
                UPDATE dbo.Usuario
                SET PasswordHash = @PasswordHash,
                    DebeCambiarPassword = @DebeCambiarPassword
                WHERE Id = @Id";

            using (SqlConnection connection = _connectionFactory.CrearConexion())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = idUsuario;
                command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = passwordHash;
                command.Parameters.Add("@DebeCambiarPassword", SqlDbType.Bit).Value = debeCambiarPassword;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void ActualizarFechaUltimoAcceso(Guid idUsuario)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            const string sql = @"
                UPDATE dbo.Usuario 
                SET FechaUltimoAcceso = SYSDATETIME() 
                WHERE Id = @Id 
                  AND Activo = 1";

            using (SqlConnection connection = _connectionFactory.CrearConexion())
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = idUsuario;

                connection.Open();
                command.ExecuteNonQuery();
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
                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                FechaUltimoAcceso = reader["FechaUltimoAcceso"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(reader["FechaUltimoAcceso"])
            };
        }

        private void CargarParametrosUsuario(SqlCommand command, Usuario usuario)
        {
            command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = usuario.Id;
            command.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar, 100).Value = usuario.NombreUsuario;
            command.Parameters.Add("@NombreCompleto", SqlDbType.NVarChar, 150).Value =
                string.IsNullOrWhiteSpace(usuario.NombreCompleto)
                    ? (object)DBNull.Value
                    : usuario.NombreCompleto.Trim();
            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = usuario.PasswordHash;
            command.Parameters.Add("@Activo", SqlDbType.Bit).Value = usuario.Activo;
            command.Parameters.Add("@DebeCambiarPassword", SqlDbType.Bit).Value = usuario.DebeCambiarPassword;
            command.Parameters.Add("@FechaCreacion", SqlDbType.DateTime2).Value = usuario.FechaCreacion;
            command.Parameters.Add("@FechaUltimoAcceso", SqlDbType.DateTime2).Value =
                usuario.FechaUltimoAcceso.HasValue
                    ? (object)usuario.FechaUltimoAcceso.Value
                    : DBNull.Value;
        }
    }
}
