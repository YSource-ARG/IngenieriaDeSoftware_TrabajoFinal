namespace BLL.Integridad
{
    public class ResultadoVerificacionIntegridad
    {
        public bool IntegridadCorrecta { get; private set; }
        public bool PermitirSoloAdministrador { get; private set; }
        public string Mensaje { get; private set; }

        private ResultadoVerificacionIntegridad(
            bool integridadCorrecta,
            bool permitirSoloAdministrador,
            string mensaje)
        {
            IntegridadCorrecta = integridadCorrecta;
            PermitirSoloAdministrador = permitirSoloAdministrador;
            Mensaje = mensaje;
        }

        public static ResultadoVerificacionIntegridad Correcta()
        {
            return new ResultadoVerificacionIntegridad(
                true,
                false,
                "La integridad de la base de datos es correcta."
            );
        }

        public static ResultadoVerificacionIntegridad Vulnerada(string mensaje)
        {
            return new ResultadoVerificacionIntegridad(
                false,
                true,
                mensaje
            );
        }
    }
}