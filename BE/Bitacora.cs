using System;

namespace BE
{
    public class Bitacora
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public Guid? UsuarioId { get; set; }
        public string Usuario { get; set; }
        public string Modulo { get; set; }
        public string Accion { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
    }
}
