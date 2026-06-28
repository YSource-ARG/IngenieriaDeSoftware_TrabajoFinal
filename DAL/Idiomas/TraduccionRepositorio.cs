using DAL.BaseDeDatos;
using DAL.Excepciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BE;

namespace DAL.Idiomas
{
    public class TraduccionRepositorio : ITraduccionRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;

        public TraduccionRepositorio(IConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public Dictionary<string, string> ListarPorIdioma(Guid idiomaId)
        {
            if (idiomaId == Guid.Empty)
            {
                throw new ArgumentException("El identificador del idioma no puede estar vacío.", nameof(idiomaId));
            }

            const string sql = @"
                SELECT
                    Clave,
                    Texto
                FROM dbo.Traduccion
                WHERE IdiomaId = @IdiomaId
                ORDER BY Clave";

            Dictionary<string, string> traducciones =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@IdiomaId", SqlDbType.UniqueIdentifier).Value = idiomaId;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string clave = reader["Clave"].ToString();
                            string texto = reader["Texto"].ToString();

                            if (!traducciones.ContainsKey(clave))
                            {
                                traducciones.Add(clave, texto);
                            }
                        }
                    }
                }

                return traducciones;
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron listar las traducciones del idioma.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }
        public List<Traduccion> ListarDetallePorIdioma(Guid idiomaId)
        {
            if (idiomaId == Guid.Empty)
            {
                throw new ArgumentException("El identificador del idioma no puede estar vacío.", nameof(idiomaId));
            }

            const string sql = @"
                SELECT
                    Id,
                    Clave,
                    IdiomaId,
                    Texto
                FROM dbo.Traduccion
                WHERE IdiomaId = @IdiomaId
                ORDER BY Clave";

            List<Traduccion> traducciones = new List<Traduccion>();

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@IdiomaId", SqlDbType.UniqueIdentifier).Value = idiomaId;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            traducciones.Add(MapearTraduccion(reader));
                        }
                    }
                }

                return traducciones;
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron listar las traducciones para gestión.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public Traduccion ObtenerPorId(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(
                    "El id de la traducción no puede estar vacío.",
                    nameof(id)
                );
            }

            const string sql = @"
                SELECT
                    Id,
                    Clave,
                    IdiomaId,
                    Texto
                FROM dbo.Traduccion
                WHERE Id = @Id";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters
                        .Add("@Id", SqlDbType.UniqueIdentifier)
                        .Value = id;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        return reader.Read()
                            ? MapearTraduccion(reader)
                            : null;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException(
                    "No se pudo obtener la traducción.",
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

        public void Guardar(Guid idiomaId, string clave, string texto)
        {
            if (idiomaId == Guid.Empty)
            {
                throw new ArgumentException("El identificador del idioma no puede estar vacío.", nameof(idiomaId));
            }

            if (string.IsNullOrWhiteSpace(clave))
            {
                throw new ArgumentException("La clave de traducción no puede estar vacía.", nameof(clave));
            }

            if (texto == null)
            {
                throw new ArgumentNullException(nameof(texto));
            }

            const string sql = @"
                IF EXISTS
                (
                    SELECT 1
                    FROM dbo.Traduccion
                    WHERE IdiomaId = @IdiomaId
                      AND Clave = @Clave
                )
                BEGIN
                    UPDATE dbo.Traduccion
                    SET Texto = @Texto
                    WHERE IdiomaId = @IdiomaId
                      AND Clave = @Clave
                END
                ELSE
                BEGIN
                    INSERT INTO dbo.Traduccion
                    (
                        Id,
                        Clave,
                        IdiomaId,
                        Texto
                    )
                    VALUES
                    (
                        NEWID(),
                        @Clave,
                        @IdiomaId,
                        @Texto
                    )
                END";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@IdiomaId", SqlDbType.UniqueIdentifier).Value = idiomaId;
                    command.Parameters.Add("@Clave", SqlDbType.NVarChar, 200).Value = clave.Trim();
                    command.Parameters.Add("@Texto", SqlDbType.NVarChar, 500).Value = texto.Trim();

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo guardar la traducción.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void Modificar(Guid id, string clave, string texto)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("El id de la traducción no puede estar vacío.", nameof(id));
            }

            const string sql = @"
                UPDATE dbo.Traduccion
                SET
                    Clave = @Clave,
                    Texto = @Texto
                WHERE Id = @Id";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
                    command.Parameters.Add("@Clave", SqlDbType.NVarChar, 200).Value = clave.Trim();
                    command.Parameters.Add("@Texto", SqlDbType.NVarChar, 500).Value = texto.Trim();

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo modificar la traducción.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public void Eliminar(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("El id de la traducción no puede estar vacío.", nameof(id));
            }

            const string sql = @"
                DELETE FROM dbo.Traduccion
                WHERE Id = @Id";

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo eliminar la traducción.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        private Traduccion MapearTraduccion(SqlDataReader reader)
        {
            return new Traduccion
            {
                Id = Guid.Parse(reader["Id"].ToString()),
                Clave = reader["Clave"].ToString(),
                IdiomaId = Guid.Parse(reader["IdiomaId"].ToString()),
                Texto = reader["Texto"].ToString()
            };
        }
    }
}