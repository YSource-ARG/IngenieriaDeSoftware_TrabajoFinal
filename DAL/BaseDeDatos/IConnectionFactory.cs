using System.Data.SqlClient;

namespace DAL.BaseDeDatos
{
    public interface IConnectionFactory
    {
        SqlConnection CrearConexion();
    }
}
