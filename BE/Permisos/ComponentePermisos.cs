using System;

namespace BE.Permisos
{
    public abstract class ComponentePermisos
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; }

        public string Codigo { get; set; }

        public string Descripcion { get; set; }

        public bool Activo { get; set; }

        public TipoComponentePermisos Tipo { get; set; }
    }
}
