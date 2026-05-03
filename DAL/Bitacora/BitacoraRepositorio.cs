using DAL.BaseDeDatos;
using System;
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
    }
}