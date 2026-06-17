using DAL.BaseDeDatos;
using DAL.Excepciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

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
    }
}