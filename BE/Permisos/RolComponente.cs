using System;

namespace BE.Permisos
{
    public class RolComponente
    {
        public Guid RolId { get; set; }

        public Guid ComponenteHijoId { get; set; }

        public string DigitoVerificadorHorizontal { get; set; }
    }
}