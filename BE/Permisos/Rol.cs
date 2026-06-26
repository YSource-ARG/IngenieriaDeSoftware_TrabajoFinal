using System.Collections.Generic;

namespace BE.Permisos
{
    public class Rol : ComponentePermisos
    {
        public List<ComponentePermisos> Hijos { get; set; } = new List<ComponentePermisos>();
    }
}
