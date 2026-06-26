using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE.Permisos;
using DAL.BaseDeDatos;
using DAL.Excepciones;

namespace DAL.Permisos
{
    public class PermisoComponenteRepositorio : IPermisoComponenteRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;

        public PermisoComponenteRepositorio(IConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public List<ComponentePermisos> Listar()
        {
            const string sql = @"
                SELECT
                    Id,
                    Nombre,
                    Codigo,
                    Descripcion,
                    Activo,
                    Tipo,
                    FechaCreacion,
                    FechaModificacion
                FROM dbo.PermisoComponente
                ORDER BY Tipo, Nombre";

            List<ComponentePermisos> componentes = new List<ComponentePermisos>();

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
                            componentes.Add(MapearComponente(reader));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron listar los componentes de permisos.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }

            return componentes;
        }

        public List<Rol> ListarRoles()
        {
            const string sql = @"
                SELECT
                    Id,
                    Nombre,
                    Codigo,
                    Descripcion,
                    Activo,
                    Tipo,
                    FechaCreacion,
                    FechaModificacion
                FROM dbo.PermisoComponente
                WHERE Tipo = @Tipo
                ORDER BY Nombre";

            List<Rol> roles = new List<Rol>();

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Tipo", SqlDbType.Int).Value = (int)TipoComponentePermisos.Rol;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add((Rol)MapearComponente(reader));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron listar los roles.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }

            return roles;
        }

        public List<Permiso> ListarPermisos()
        {
            const string sql = @"
                SELECT
                    Id,
                    Nombre,
                    Codigo,
                    Descripcion,
                    Activo,
                    Tipo,
                    FechaCreacion,
                    FechaModificacion
                FROM dbo.PermisoComponente
                WHERE Tipo = @Tipo
                ORDER BY Nombre";

            List<Permiso> permisos = new List<Permiso>();

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Tipo", SqlDbType.Int).Value = (int)TipoComponentePermisos.Permiso;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            permisos.Add((Permiso)MapearComponente(reader));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron listar los permisos.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }

            return permisos;
        }

        public ComponentePermisos ObtenerPorId(Guid idComponente)
        {
            if (idComponente == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente no puede estar vacío.", nameof(idComponente));
            }

            const string sql = @"
                SELECT
                    Id,
                    Nombre,
                    Codigo,
                    Descripcion,
                    Activo,
                    Tipo,
                    FechaCreacion,
                    FechaModificacion
                FROM dbo.PermisoComponente
                WHERE Id = @Id";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = idComponente;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        return MapearComponente(reader);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo obtener el componente por identificador.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public ComponentePermisos ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new ArgumentException("El código no puede estar vacío.", nameof(codigo));
            }

            const string sql = @"
                SELECT
                    Id,
                    Nombre,
                    Codigo,
                    Descripcion,
                    Activo,
                    Tipo,
                    FechaCreacion,
                    FechaModificacion
                FROM dbo.PermisoComponente
                WHERE Codigo = @Codigo";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Codigo", SqlDbType.NVarChar, 100).Value = codigo.Trim();

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        return MapearComponente(reader);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo obtener el componente por código.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public bool ExisteCodigo(string codigo, Guid? idComponenteExcluido)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new ArgumentException("El código no puede estar vacío.", nameof(codigo));
            }

            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.PermisoComponente
                WHERE Codigo = @Codigo
                  AND (@IdComponenteExcluido IS NULL OR Id <> @IdComponenteExcluido)";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Codigo", SqlDbType.NVarChar, 100).Value = codigo.Trim();
                    command.Parameters.Add("@IdComponenteExcluido", SqlDbType.UniqueIdentifier).Value =
                        idComponenteExcluido.HasValue
                            ? (object)idComponenteExcluido.Value
                            : DBNull.Value;

                    connection.Open();

                    int cantidad = Convert.ToInt32(command.ExecuteScalar());

                    return cantidad > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo verificar la existencia del código de componente.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void Crear(ComponentePermisos componente)
        {
            if (componente == null)
            {
                throw new ArgumentNullException(nameof(componente));
            }

            const string sql = @"
                INSERT INTO dbo.PermisoComponente
                (
                    Id,
                    Nombre,
                    Codigo,
                    Descripcion,
                    Activo,
                    Tipo,
                    FechaCreacion,
                    FechaModificacion
                )
                VALUES
                (
                    @Id,
                    @Nombre,
                    @Codigo,
                    @Descripcion,
                    @Activo,
                    @Tipo,
                    @FechaCreacion,
                    @FechaModificacion
                )";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    CargarParametrosComponente(command, componente);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo crear el componente de permisos.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void Modificar(ComponentePermisos componente)
        {
            if (componente == null)
            {
                throw new ArgumentNullException(nameof(componente));
            }

            const string sql = @"
                UPDATE dbo.PermisoComponente
                SET Nombre = @Nombre,
                    Codigo = @Codigo,
                    Descripcion = @Descripcion,
                    Activo = @Activo,
                    Tipo = @Tipo,
                    FechaModificacion = @FechaModificacion
                WHERE Id = @Id";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    CargarParametrosComponente(command, componente);
                    command.Parameters.RemoveAt("@FechaCreacion");

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo modificar el componente de permisos.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void CambiarEstado(Guid idComponente, bool activo)
        {
            if (idComponente == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente no puede estar vacío.", nameof(idComponente));
            }

            const string sql = @"
                UPDATE dbo.PermisoComponente
                SET Activo = @Activo,
                    FechaModificacion = SYSDATETIME()
                WHERE Id = @Id";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = idComponente;
                    command.Parameters.Add("@Activo", SqlDbType.Bit).Value = activo;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo cambiar el estado del componente.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void Eliminar(Guid idComponente)
        {
            if (idComponente == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente no puede estar vacío.", nameof(idComponente));
            }

            const string sql = @"
                DELETE FROM dbo.PermisoComponente
                WHERE Id = @Id";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = idComponente;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo eliminar el componente de permisos.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        private void CargarParametrosComponente(SqlCommand command, ComponentePermisos componente)
        {
            command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = componente.Id;
            command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 150).Value = componente.Nombre.Trim();
            command.Parameters.Add("@Codigo", SqlDbType.NVarChar, 100).Value = componente.Codigo.Trim();
            command.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 300).Value =
                string.IsNullOrWhiteSpace(componente.Descripcion)
                    ? (object)DBNull.Value
                    : componente.Descripcion.Trim();
            command.Parameters.Add("@Activo", SqlDbType.Bit).Value = componente.Activo;
            command.Parameters.Add("@Tipo", SqlDbType.Int).Value = (int)componente.Tipo;
            command.Parameters.Add("@FechaCreacion", SqlDbType.DateTime2).Value = DateTime.Now;
            command.Parameters.Add("@FechaModificacion", SqlDbType.DateTime2).Value = DateTime.Now;
        }

        private ComponentePermisos MapearComponente(SqlDataReader reader)
        {
            TipoComponentePermisos tipo = (TipoComponentePermisos)Convert.ToInt32(reader["Tipo"]);

            ComponentePermisos componente =
                tipo == TipoComponentePermisos.Rol
                    ? (ComponentePermisos)new Rol()
                    : new Permiso();

            componente.Id = Guid.Parse(reader["Id"].ToString());
            componente.Nombre = reader["Nombre"].ToString();
            componente.Codigo = reader["Codigo"].ToString();
            componente.Descripcion = reader["Descripcion"] == DBNull.Value
                ? null
                : reader["Descripcion"].ToString();
            componente.Activo = Convert.ToBoolean(reader["Activo"]);
            componente.Tipo = tipo;

            return componente;
        }
    }
}
