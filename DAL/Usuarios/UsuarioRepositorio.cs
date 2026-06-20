using BE;
using DAL.BaseDeDatos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DAL.Excepciones;

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
            Email,
            IdiomaPreferidoId,
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
        WHERE NombreUsuario = @NombreUsuario 
          AND Activo = 1";

            try
            {
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
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo consultar el usuario en la base de datos.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
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
                Email,
                IdiomaPreferidoId,
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
            WHERE Id = @Id";

            try
            {
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
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo consultar el usuario por identificador.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public List<Usuario> Listar(string textoBusqueda, bool? activo)
        {
            const string sql = @"
            SELECT 
                Id, 
                NombreUsuario,
                NombreCompleto,
                Email,
                IdiomaPreferidoId,
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
            WHERE (@TextoBusqueda IS NULL 
                    OR NombreUsuario LIKE '%' + @TextoBusqueda + '%'
                    OR NombreCompleto LIKE '%' + @TextoBusqueda + '%')
                AND (@Activo IS NULL OR Activo = @Activo)
            ORDER BY NombreUsuario";

            List<Usuario> usuarios = new List<Usuario>();

            try
            {
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
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo obtener el listado de usuarios.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
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

            try
            {
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
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo verificar la existencia del nombre de usuario.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
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
            Email,
            IdiomaPreferidoId,
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
            @Email,
            @IdiomaPreferidoId,
            @PasswordHash,
            @Activo,
            @DebeCambiarPassword,
            @FechaCreacion,
            @FechaUltimoAcceso
        )";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    CargarParametrosUsuario(command, usuario);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo crear el usuario en la base de datos.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void ModificarDatos(Guid idUsuario, string nombreCompleto, string email, bool activo)
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
                    Email = @Email,
                    Activo = @Activo
                WHERE Id = @Id";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = idUsuario;
                    command.Parameters.Add("@NombreCompleto", SqlDbType.NVarChar, 150).Value = nombreCompleto.Trim();

                    // El email se permite nulo porque puede haber usuarios existentes
                    // creados antes de incorporar el control de cambios por email.
                    command.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value =
                        string.IsNullOrWhiteSpace(email)
                            ? (object)DBNull.Value
                            : email.Trim();

                    command.Parameters.Add("@Activo", SqlDbType.Bit).Value = activo;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron modificar los datos del usuario.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
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

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = idUsuario;
                    command.Parameters.Add("@Activo", SqlDbType.Bit).Value = activo;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo cambiar el estado del usuario.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
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

            try
            {
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
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo actualizar la contraseña del usuario.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void ActualizarIdiomaPreferido(Guid idUsuario, Guid idiomaPreferidoId)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del usuario no puede estar vacío.",
                    nameof(idUsuario)
                );
            }

            if (idiomaPreferidoId == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del idioma no puede estar vacío.",
                    nameof(idiomaPreferidoId)
                );
            }

            const string sql = @"
            UPDATE dbo.Usuario
            SET IdiomaPreferidoId = @IdiomaPreferidoId
            WHERE Id = @Id";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(
                        "@Id",
                        SqlDbType.UniqueIdentifier
                    ).Value = idUsuario;

                    command.Parameters.Add(
                        "@IdiomaPreferidoId",
                        SqlDbType.UniqueIdentifier
                    ).Value = idiomaPreferidoId;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo actualizar el idioma preferido del usuario.",
                    ex
                );
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo abrir o utilizar la conexión con la base de datos.",
                    ex
                );
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

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = idUsuario;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo actualizar la fecha de último acceso del usuario.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void ActualizarIntentosFallidosLogin(Guid idUsuario, int intentosFallidos, DateTime? bloqueadoHasta)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del usuario no puede estar vacío.",
                    nameof(idUsuario)
                );
            }

            if (intentosFallidos < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(intentosFallidos),
                    "La cantidad de intentos fallidos no puede ser negativa."
                );
            }

            const string sql = @"
UPDATE dbo.Usuario
SET IntentosFallidosLogin = @IntentosFallidosLogin,
    BloqueadoHasta = @BloqueadoHasta
WHERE Id = @Id";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value =
                        idUsuario;

                    command.Parameters.Add("@IntentosFallidosLogin", SqlDbType.Int).Value =
                        intentosFallidos;

                    command.Parameters.Add("@BloqueadoHasta", SqlDbType.DateTime2).Value =
                        bloqueadoHasta.HasValue
                            ? (object)bloqueadoHasta.Value
                            : DBNull.Value;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron actualizar los intentos fallidos de login.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void RestablecerIntentosFallidosLogin(Guid idUsuario)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException(
                    "El identificador del usuario no puede estar vacío.",
                    nameof(idUsuario)
                );
            }

            const string sql = @"
UPDATE dbo.Usuario
SET IntentosFallidosLogin = 0,
    BloqueadoHasta = NULL
WHERE Id = @Id";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value =
                        idUsuario;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron restablecer los intentos fallidos de login.", ex);
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

                // Email del usuario.
                // Se controla para poder registrar historial de cambios sobre esta propiedad.
                Email = reader["Email"] == DBNull.Value
                    ? null
                    : reader["Email"].ToString(),

                IdiomaPreferidoId = reader["IdiomaPreferidoId"] == DBNull.Value
                    ? (Guid?)null
                    : Guid.Parse(reader["IdiomaPreferidoId"].ToString()),

                PasswordHash = reader["PasswordHash"].ToString(),
                Activo = Convert.ToBoolean(reader["Activo"]),
                DebeCambiarPassword = Convert.ToBoolean(reader["DebeCambiarPassword"]),
                IntentosFallidosLogin = Convert.ToInt32(reader["IntentosFallidosLogin"]),
                BloqueadoHasta = reader["BloqueadoHasta"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(reader["BloqueadoHasta"]),

                // Estado de bloqueo por integridad.
                // Se mantiene separado de Activo para no perder la trazabilidad
                // de usuarios inhabilitados administrativamente.
                BloqueadoPorIntegridad = Convert.ToBoolean(reader["BloqueadoPorIntegridad"]),

                // DVH almacenado en la entidad protegida.
                // Se utiliza para comparar el estado guardado contra el estado recalculado.
                DigitoVerificadorHorizontal = reader["DigitoVerificadorHorizontal"].ToString(),

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
            command.Parameters.Add("@NombreCompleto", SqlDbType.NVarChar, 150).Value = string.IsNullOrWhiteSpace(usuario.NombreCompleto)
                    ? (object)DBNull.Value
                    : usuario.NombreCompleto.Trim();
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = string.IsNullOrWhiteSpace(usuario.Email)
                    ? (object)DBNull.Value
                    : usuario.Email.Trim();
            command.Parameters.Add("@IdiomaPreferidoId", SqlDbType.UniqueIdentifier).Value = usuario.IdiomaPreferidoId.HasValue
                    ? (object)usuario.IdiomaPreferidoId.Value
                    : DBNull.Value;
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
