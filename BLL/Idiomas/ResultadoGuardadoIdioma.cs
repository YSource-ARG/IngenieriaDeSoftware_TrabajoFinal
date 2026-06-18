namespace BLL.Idiomas
{
    public class ResultadoGuardadoIdioma
    {
        public bool Exitoso { get; }
        public string Mensaje { get; }

        private ResultadoGuardadoIdioma(bool exitoso, string mensaje)
        {
            Exitoso = exitoso;
            Mensaje = mensaje;
        }

        public static ResultadoGuardadoIdioma Ok()
        {
            return new ResultadoGuardadoIdioma(true, "El idioma fue guardado correctamente.");
        }

        public static ResultadoGuardadoIdioma Error(string mensaje)
        {
            return new ResultadoGuardadoIdioma(false, mensaje);
        }
    }
}