using BE;
using DAL.BaseDeDatos;
using DAL.Excepciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL.Idiomas
{
    public class IdiomaRepositorio : IIdiomaRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;

        public IdiomaRepositorio(IConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public List<Idioma> ListarActivos()
        {
            const string sql = @"
                SELECT
                    Id,
                    Codigo,
                    Nombre,
                    Activo
                FROM dbo.Idioma
                WHERE Activo = 1
                ORDER BY Nombre";

            List<Idioma> idiomas = new List<Idioma>();

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
                            idiomas.Add(MapearIdioma(reader));
                        }
                    }
                }

                return idiomas;
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron listar los idiomas activos.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public Idioma ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new ArgumentException("El código de idioma no puede estar vacío.", nameof(codigo));
            }

            const string sql = @"
                SELECT
                    Id,
                    Codigo,
                    Nombre,
                    Activo
                FROM dbo.Idioma
                WHERE Codigo = @Codigo";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Codigo", SqlDbType.NVarChar, 10).Value = codigo.Trim();

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapearIdioma(reader);
                        }
                    }
                }

                return null;
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo obtener el idioma por código.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }
        public List<Idioma> ListarTodos()
        {
            const string sql = @"
                SELECT
                    Id,
                    Codigo,
                    Nombre,
                    Activo
                FROM dbo.Idioma
                ORDER BY Nombre";

            List<Idioma> idiomas = new List<Idioma>();

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
                            idiomas.Add(MapearIdioma(reader));
                        }
                    }
                }

                return idiomas;
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron listar los idiomas.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void Guardar(Idioma idioma)
        {
            if (idioma == null)
            {
                throw new ArgumentNullException(nameof(idioma));
            }

            const string sql = @"
                IF EXISTS (SELECT 1 FROM dbo.Idioma WHERE Id = @Id)
                BEGIN
                    UPDATE dbo.Idioma
                    SET
                        Codigo = @Codigo,
                        Nombre = @Nombre,
                        Activo = @Activo
                    WHERE Id = @Id
                END
                ELSE
                BEGIN
                    INSERT INTO dbo.Idioma
                    (
                        Id,
                        Codigo,
                        Nombre,
                        Activo
                    )
                    VALUES
                    (
                        @Id,
                        @Codigo,
                        @Nombre,
                        @Activo
                    )
                END";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = idioma.Id;
                    command.Parameters.Add("@Codigo", SqlDbType.NVarChar, 10).Value = idioma.Codigo.Trim();
                    command.Parameters.Add("@Nombre", SqlDbType.NVarChar, 100).Value = idioma.Nombre.Trim();
                    command.Parameters.Add("@Activo", SqlDbType.Bit).Value = idioma.Activo;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo guardar el idioma.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }
        private Idioma MapearIdioma(SqlDataReader reader)
        {
            return new Idioma
            {
                Id = Guid.Parse(reader["Id"].ToString()),
                Codigo = reader["Codigo"].ToString(),
                Nombre = reader["Nombre"].ToString(),
                Activo = Convert.ToBoolean(reader["Activo"])
            };
        }
    }
}