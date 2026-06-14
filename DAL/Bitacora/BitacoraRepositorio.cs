using BE;
using DAL.BaseDeDatos;
using DAL.Excepciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL.Bitacora
{
    public class BitacoraRepositorio : IBitacoraRepositorio
    {
        private readonly IConnectionFactory _connectionFactory;

        public BitacoraRepositorio(IConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            _connectionFactory = connectionFactory;
        }

        public void Registrar(BE.Bitacora bitacora)
        {
            if (bitacora == null)
            {
                throw new ArgumentNullException(nameof(bitacora));
            }

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("dbo.Bitacora_Registrar", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = bitacora.Id;
                        command.Parameters.Add("@Fecha", SqlDbType.DateTime2).Value = bitacora.Fecha;

                        command.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value =
                            bitacora.UsuarioId.HasValue
                                ? (object)bitacora.UsuarioId.Value
                                : DBNull.Value;

                        command.Parameters.Add("@Usuario", SqlDbType.NVarChar, 100).Value =
                            string.IsNullOrWhiteSpace(bitacora.Usuario)
                                ? (object)DBNull.Value
                                : bitacora.Usuario;

                        command.Parameters.Add("@Modulo", SqlDbType.NVarChar, 100).Value = bitacora.Modulo;
                        command.Parameters.Add("@Accion", SqlDbType.NVarChar, 100).Value = bitacora.Accion;

                        command.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 500).Value =
                            string.IsNullOrWhiteSpace(bitacora.Descripcion)
                                ? (object)DBNull.Value
                                : bitacora.Descripcion;

                        command.Parameters.Add("@Tipo", SqlDbType.NVarChar, 50).Value = bitacora.Tipo;

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo registrar el evento en la bitácora.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }
        }

        public List<BE.Bitacora> Listar(string modulo, string tipo, DateTime? fechaDesde, DateTime? fechaHasta, int cantidadMaxima)
        {
            const string sql = @"
        SELECT TOP (@CantidadMaxima)
            Id,
            Fecha,
            UsuarioId,
            Usuario,
            Modulo,
            Accion,
            Descripcion,
            Tipo
        FROM dbo.Bitacora
        WHERE (@Modulo IS NULL OR Modulo = @Modulo)
          AND (@Tipo IS NULL OR Tipo = @Tipo)
          AND (@FechaDesde IS NULL OR Fecha >= @FechaDesde)
          AND (@FechaHasta IS NULL OR Fecha <= @FechaHasta)
        ORDER BY Fecha DESC";

            List<BE.Bitacora> registros = new List<BE.Bitacora>();

            try
            {
                using (SqlConnection connection = _connectionFactory.CrearConexion())
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@CantidadMaxima", SqlDbType.Int).Value = cantidadMaxima;

                    command.Parameters.Add("@Tipo", SqlDbType.NVarChar, 20).Value =
                        string.IsNullOrWhiteSpace(tipo)
                            ? (object)DBNull.Value
                            : tipo.Trim();

                    command.Parameters.Add("@Modulo", SqlDbType.NVarChar, 100).Value =
                        string.IsNullOrWhiteSpace(modulo)
                            ? (object)DBNull.Value
                            : modulo.Trim();

                    command.Parameters.Add("@FechaDesde", SqlDbType.DateTime2).Value =
                        fechaDesde.HasValue
                            ? (object)fechaDesde.Value
                            : DBNull.Value;

                    command.Parameters.Add("@FechaHasta", SqlDbType.DateTime2).Value =
                        fechaHasta.HasValue
                            ? (object)fechaHasta.Value
                            : DBNull.Value;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            registros.Add(MapearBitacora(reader));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo consultar la bitácora.", ex);
            }
            catch (InvalidOperationException ex)
            {
                throw new AccesoDatosException("No se pudo abrir o utilizar la conexión con la base de datos.", ex);
            }

            return registros;
        }

        private BE.Bitacora MapearBitacora(SqlDataReader reader)
        {
            return new BE.Bitacora
            {
                Id = Guid.Parse(reader["Id"].ToString()),
                Fecha = Convert.ToDateTime(reader["Fecha"]),
                UsuarioId = reader["UsuarioId"] == DBNull.Value
                    ? (Guid?)null
                    : Guid.Parse(reader["UsuarioId"].ToString()),
                Usuario = reader["Usuario"] == DBNull.Value
                    ? null
                    : reader["Usuario"].ToString(),
                Modulo = reader["Modulo"].ToString(),
                Accion = reader["Accion"].ToString(),
                Descripcion = reader["Descripcion"] == DBNull.Value
                    ? null
                    : reader["Descripcion"].ToString(),
                Tipo = reader["Tipo"].ToString()
            };
        }
    }
}
