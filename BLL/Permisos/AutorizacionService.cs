using BE.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Permisos
{
    public class AutorizacionService
    {
        /// Se encarga de verificar si un usuario tiene o no un permiso, utilizando la sesión actual
        
        public bool UsuarioActualTienePermiso(string codigoPermiso)
        {
            return true;
        }

        public bool UsuarioTienePermiso(Guid idUsuario, string codigoPermiso)
        {
            return true;
        }

        public List<Permiso> ListarPermisosEfectivosUsuarioActual()
        {
            return null;
        }

        public List<Permiso> ListarPermisosEfectivosUsuario(Guid idUsuario)
        {
            return null;
        }

        #region Integración con sesión

        //public ContextoAutorizacionUsuario ConstruirContextoAutorizacion(Guid idUsuario)     DESCOMENTAR UNA VEZ IMPLEMENTADA LA CLASE CONTEXTO
        //{
        //    return null;
        //}

        public void RefrescarContextoUsuarioActual()
        {

        }

        #endregion

        #region Validaciones

        private List<ComponentePermisos> ObtenerComponentesDirectosUsuario(Guid idUsuario) { return null;  }
        private List<ComponentePermisos> ExpandirJerarquiaComponentes(IEnumerable<ComponentePermisos> componentesBase) { return null; }
        private List<Permiso> FiltrarPermisos(IEnumerable<ComponentePermisos> componentes) { return null; }
        private bool ContieneCodigoPermiso(IEnumerable<Permiso> permisos, string codigoPermiso) { return true; }
        private void ValidarCodigoPermiso(string codigoPermiso) { }

        #endregion
    }
}
