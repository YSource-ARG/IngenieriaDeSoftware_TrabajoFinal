using System;

namespace BE.Permisos
{
    public class UsuarioPermisoComponente
    {
        public Guid UsuarioId { get; set; }

        public Guid ComponenteId { get; set; }

        public DateTime FechaAsignacion { get; set; }

        public Guid? AsignadoPorUsuarioId { get; set; }

        public string DigitoVerificadorHorizontal { get; set; }
    }
}