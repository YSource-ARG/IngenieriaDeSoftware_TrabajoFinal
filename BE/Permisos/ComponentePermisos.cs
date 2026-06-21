using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE.Permisos
{
    public abstract class ComponentePermisos
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; }

        public string Codigo { get; set; }

        public string Descripcion { get; set; }

        public bool Activo { get; set; }

        public enum TipoComponentePermisos { Activo, Desactivado}
        public TipoComponentePermisos Tipo {  get; set; }

    }
}
