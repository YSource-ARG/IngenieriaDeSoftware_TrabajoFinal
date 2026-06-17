namespace BLL.Idiomas
{
    public class ResultadoCambioIdioma
    {
        public bool Exitoso { get; }
        public string Mensaje { get; }

        private ResultadoCambioIdioma(bool exitoso, string mensaje)
        {
            Exitoso = exitoso;
            Mensaje = mensaje;
        }

        public static ResultadoCambioIdioma Ok()
        {
            return new ResultadoCambioIdioma(true, string.Empty);
        }

        public static ResultadoCambioIdioma Error(string mensaje)
        {
            return new ResultadoCambioIdioma(false, mensaje);
        }
    }
}