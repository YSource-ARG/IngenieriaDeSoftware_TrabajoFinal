namespace BLL.Idiomas
{
    public class ResultadoGuardadoTraduccion
    {
        public bool Exitoso { get; }
        public string Mensaje { get; }

        private ResultadoGuardadoTraduccion(bool exitoso, string mensaje)
        {
            Exitoso = exitoso;
            Mensaje = mensaje;
        }

        public static ResultadoGuardadoTraduccion Ok()
        {
            return new ResultadoGuardadoTraduccion(true, "La traducción fue guardada correctamente.");
        }

        public static ResultadoGuardadoTraduccion Ok(string mensaje)
        {
            return new ResultadoGuardadoTraduccion(true, mensaje);
        }

        public static ResultadoGuardadoTraduccion Error(string mensaje)
        {
            return new ResultadoGuardadoTraduccion(false, mensaje);
        }
    }
}