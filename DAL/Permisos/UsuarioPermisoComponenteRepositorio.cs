using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE.Permisos;
using DAL.BaseDeDatos;
using DAL.Excepciones;

namespace DAL.Permisos
{
    public class UsuarioPermisoComponenteRepositorio : IUsuarioPermisoComponenteRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;

        public UsuarioPermisoComponenteRepositorio(IConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public List<ComponentePermisos> ListarPorUsuario(Guid idUsuario)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            const string sql = @"
                SELECT
                    pc.Id,
                    pc.Nombre,
                    pc.Codigo,
                    pc.Descripcion,
                    pc.Activo,
                    pc.Tipo,
                    pc.FechaCreacion,
                    pc.FechaModificacion
                FROM dbo.UsuarioPermisoComponente upc
                INNER JOIN dbo.PermisoComponente pc
                    ON pc.Id = upc.ComponenteId
                WHERE upc.UsuarioId = @UsuarioId
                ORDER BY pc.Tipo, pc.Nombre";

            List<ComponentePermisos> componentes = new List<ComponentePermisos>();

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = idUsuario;

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
                throw new AccesoDatosException("No se pudieron listar los componentes asignados al usuario.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }

            return componentes;
        }

        public List<Guid> ListarIdsComponentesPorUsuario(Guid idUsuario)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            const string sql = @"
                SELECT ComponenteId
                FROM dbo.UsuarioPermisoComponente
                WHERE UsuarioId = @UsuarioId";

            List<Guid> ids = new List<Guid>();

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = idUsuario;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ids.Add(Guid.Parse(reader["ComponenteId"].ToString()));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron listar los identificadores de componentes del usuario.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }

            return ids;
        }

        public bool ExisteAsignacion(Guid idUsuario, Guid idComponente)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            if (idComponente == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente no puede estar vacío.", nameof(idComponente));
            }

            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.UsuarioPermisoComponente
                WHERE UsuarioId = @UsuarioId
                  AND ComponenteId = @ComponenteId";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = idUsuario;
                    command.Parameters.Add("@ComponenteId", SqlDbType.UniqueIdentifier).Value = idComponente;

                    connection.Open();

                    int cantidad = Convert.ToInt32(command.ExecuteScalar());

                    return cantidad > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo verificar la asignación del componente al usuario.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void Asignar(Guid idUsuario, Guid idComponente, Guid? asignadoPorUsuarioId)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            if (idComponente == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente no puede estar vacío.", nameof(idComponente));
            }

            const string sql = @"
                INSERT INTO dbo.UsuarioPermisoComponente
                (
                    UsuarioId,
                    ComponenteId,
                    FechaAsignacion,
                    AsignadoPorUsuarioId
                )
                VALUES
                (
                    @UsuarioId,
                    @ComponenteId,
                    @FechaAsignacion,
                    @AsignadoPorUsuarioId
                )";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = idUsuario;
                    command.Parameters.Add("@ComponenteId", SqlDbType.UniqueIdentifier).Value = idComponente;
                    command.Parameters.Add("@FechaAsignacion", SqlDbType.DateTime2).Value = DateTime.Now;
                    command.Parameters.Add("@AsignadoPorUsuarioId", SqlDbType.UniqueIdentifier).Value =
                        asignadoPorUsuarioId.HasValue
                            ? (object)asignadoPorUsuarioId.Value
                            : DBNull.Value;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo asignar el componente al usuario.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void Desasignar(Guid idUsuario, Guid idComponente)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            if (idComponente == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente no puede estar vacío.", nameof(idComponente));
            }

            const string sql = @"
                DELETE FROM dbo.UsuarioPermisoComponente
                WHERE UsuarioId = @UsuarioId
                  AND ComponenteId = @ComponenteId";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = idUsuario;
                    command.Parameters.Add("@ComponenteId", SqlDbType.UniqueIdentifier).Value = idComponente;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo desasignar el componente del usuario.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void DesasignarTodos(Guid idUsuario)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            const string sql = @"
                DELETE FROM dbo.UsuarioPermisoComponente
                WHERE UsuarioId = @UsuarioId";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = idUsuario;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron desasignar los componentes del usuario.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public bool TieneAsignaciones(Guid idUsuario)
        {
            if (idUsuario == Guid.Empty)
            {
                throw new ArgumentException("El identificador del usuario no puede estar vacío.", nameof(idUsuario));
            }

            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.UsuarioPermisoComponente
                WHERE UsuarioId = @UsuarioId";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = idUsuario;

                    connection.Open();

                    int cantidad = Convert.ToInt32(command.ExecuteScalar());

                    return cantidad > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo verificar si el usuario tiene asignaciones.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public bool ComponenteAsignadoAAlgunUsuario(Guid idComponente)
        {
            if (idComponente == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente no puede estar vacío.", nameof(idComponente));
            }

            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.UsuarioPermisoComponente
                WHERE ComponenteId = @ComponenteId";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@ComponenteId", SqlDbType.UniqueIdentifier).Value = idComponente;

                    connection.Open();

                    int cantidad = Convert.ToInt32(command.ExecuteScalar());

                    return cantidad > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo verificar si el componente está asignado a algún usuario.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void EliminarAsignacionesPorComponente(Guid idComponente)
        {
            if (idComponente == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente no puede estar vacío.", nameof(idComponente));
            }

            const string sql = @"
                DELETE FROM dbo.UsuarioPermisoComponente
                WHERE ComponenteId = @ComponenteId";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@ComponenteId", SqlDbType.UniqueIdentifier).Value = idComponente;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron eliminar las asignaciones del componente.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
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
