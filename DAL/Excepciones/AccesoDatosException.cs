using System;

namespace DAL.Excepciones
{
    public class AccesoDatosException : Exception
    {
        public AccesoDatosException(string mensaje) : base(mensaje)
        {
        }

        public AccesoDatosException(string mensaje, Exception innerException) : base(mensaje, innerException)
        {
        }
    }
}