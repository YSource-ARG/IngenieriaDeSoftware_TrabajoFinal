using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE.Permisos;
using DAL.BaseDeDatos;
using DAL.Excepciones;

namespace DAL.Permisos
{
    public class RolComponenteRepositorio : IRolComponenteRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;

        public RolComponenteRepositorio(IConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public List<ComponentePermisos> ListarHijos(Guid idRol)
        {
            if (idRol == Guid.Empty)
            {
                throw new ArgumentException("El identificador del rol no puede estar vacío.", nameof(idRol));
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
                FROM dbo.RolComponente rc
                INNER JOIN dbo.PermisoComponente pc
                    ON pc.Id = rc.ComponenteHijoId
                WHERE rc.RolId = @RolId
                ORDER BY pc.Tipo, pc.Nombre";

            List<ComponentePermisos> hijos = new List<ComponentePermisos>();

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@RolId", SqlDbType.UniqueIdentifier).Value = idRol;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            hijos.Add(MapearComponente(reader));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron listar los hijos del rol.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }

            return hijos;
        }

        public List<Guid> ListarIdsHijos(Guid idRol)
        {
            if (idRol == Guid.Empty)
            {
                throw new ArgumentException("El identificador del rol no puede estar vacío.", nameof(idRol));
            }

            const string sql = @"
                SELECT ComponenteHijoId
                FROM dbo.RolComponente
                WHERE RolId = @RolId";

            List<Guid> ids = new List<Guid>();

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@RolId", SqlDbType.UniqueIdentifier).Value = idRol;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ids.Add(Guid.Parse(reader["ComponenteHijoId"].ToString()));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron listar los identificadores hijos del rol.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }

            return ids;
        }

        public List<Guid> ListarRolesPadre(Guid idComponenteHijo)
        {
            if (idComponenteHijo == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente hijo no puede estar vacío.", nameof(idComponenteHijo));
            }

            const string sql = @"
                SELECT RolId
                FROM dbo.RolComponente
                WHERE ComponenteHijoId = @ComponenteHijoId";

            List<Guid> ids = new List<Guid>();

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@ComponenteHijoId", SqlDbType.UniqueIdentifier).Value = idComponenteHijo;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ids.Add(Guid.Parse(reader["RolId"].ToString()));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron listar los roles padre del componente.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }

            return ids;
        }

        public bool ExisteRelacion(Guid idRol, Guid idComponenteHijo)
        {
            if (idRol == Guid.Empty)
            {
                throw new ArgumentException("El identificador del rol no puede estar vacío.", nameof(idRol));
            }

            if (idComponenteHijo == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente hijo no puede estar vacío.", nameof(idComponenteHijo));
            }

            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.RolComponente
                WHERE RolId = @RolId
                  AND ComponenteHijoId = @ComponenteHijoId";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@RolId", SqlDbType.UniqueIdentifier).Value = idRol;
                    command.Parameters.Add("@ComponenteHijoId", SqlDbType.UniqueIdentifier).Value = idComponenteHijo;

                    connection.Open();

                    int cantidad = Convert.ToInt32(command.ExecuteScalar());

                    return cantidad > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo verificar la relación entre rol y componente.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void Agregar(Guid idRol, Guid idComponenteHijo)
        {
            if (idRol == Guid.Empty)
            {
                throw new ArgumentException("El identificador del rol no puede estar vacío.", nameof(idRol));
            }

            if (idComponenteHijo == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente hijo no puede estar vacío.", nameof(idComponenteHijo));
            }

            const string sql = @"
                INSERT INTO dbo.RolComponente
                (
                    RolId,
                    ComponenteHijoId
                )
                VALUES
                (
                    @RolId,
                    @ComponenteHijoId
                )";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@RolId", SqlDbType.UniqueIdentifier).Value = idRol;
                    command.Parameters.Add("@ComponenteHijoId", SqlDbType.UniqueIdentifier).Value = idComponenteHijo;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo agregar el componente al rol.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void Quitar(Guid idRol, Guid idComponenteHijo)
        {
            if (idRol == Guid.Empty)
            {
                throw new ArgumentException("El identificador del rol no puede estar vacío.", nameof(idRol));
            }

            if (idComponenteHijo == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente hijo no puede estar vacío.", nameof(idComponenteHijo));
            }

            const string sql = @"
                DELETE FROM dbo.RolComponente
                WHERE RolId = @RolId
                  AND ComponenteHijoId = @ComponenteHijoId";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@RolId", SqlDbType.UniqueIdentifier).Value = idRol;
                    command.Parameters.Add("@ComponenteHijoId", SqlDbType.UniqueIdentifier).Value = idComponenteHijo;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo quitar el componente del rol.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void QuitarTodosDeRol(Guid idRol)
        {
            if (idRol == Guid.Empty)
            {
                throw new ArgumentException("El identificador del rol no puede estar vacío.", nameof(idRol));
            }

            const string sql = @"
                DELETE FROM dbo.RolComponente
                WHERE RolId = @RolId";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@RolId", SqlDbType.UniqueIdentifier).Value = idRol;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron quitar los componentes del rol.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public bool TieneHijos(Guid idRol)
        {
            if (idRol == Guid.Empty)
            {
                throw new ArgumentException("El identificador del rol no puede estar vacío.", nameof(idRol));
            }

            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.RolComponente
                WHERE RolId = @RolId";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@RolId", SqlDbType.UniqueIdentifier).Value = idRol;

                    connection.Open();

                    int cantidad = Convert.ToInt32(command.ExecuteScalar());

                    return cantidad > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo verificar si el rol tiene hijos.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public bool EstaSiendoUsadoComoHijo(Guid idComponente)
        {
            if (idComponente == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente no puede estar vacío.", nameof(idComponente));
            }

            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.RolComponente
                WHERE ComponenteHijoId = @ComponenteHijoId";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@ComponenteHijoId", SqlDbType.UniqueIdentifier).Value = idComponente;

                    connection.Open();

                    int cantidad = Convert.ToInt32(command.ExecuteScalar());

                    return cantidad > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo verificar si el componente está siendo usado como hijo.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void EliminarRelacionesPorComponente(Guid idComponente)
        {
            if (idComponente == Guid.Empty)
            {
                throw new ArgumentException("El identificador del componente no puede estar vacío.", nameof(idComponente));
            }

            const string sql = @"
                DELETE FROM dbo.RolComponente
                WHERE RolId = @IdComponente
                   OR ComponenteHijoId = @IdComponente";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@IdComponente", SqlDbType.UniqueIdentifier).Value = idComponente;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron eliminar las relaciones del componente.", ex);
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
