using BE.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Permisos
{
    public class GestionPermisosAppService
    {
        /// Maneja el ABM de roles y permisos, composición de roles y asignación de roles
        /// Realiza las validaciones requeridas por estado o por existencia
        /// Registra cambios en la bitácora
        
        
        #region Listados y busquedas
        public List<ComponentePermisos> ListarComponentesPermisos()
        {
            return null; // IMPLEMENTAR
        }

        public List<Rol> ListarRoles()
        {
            return null;
        }

        public List<Permiso> ListarPermisos()
        {
            return null;
        }

        public ComponentePermisos ObtenerComponentePorId(Guid idComponente)
        {
            return null;
        }

        public Rol ObtenerRolPorId(Guid idRol)
        {
            return null;
        }

        public Permiso ObtenerPermisoPorId(Guid idPermiso)
        {
            return null;
        }
        #endregion

        #region Alta, Modificación y Baja
        public void CrearRol(string pNombre, string pCodigo, string pDescripcion, bool pEstado)
        {

        }

        public void ModificarRol(Guid pId, string pNombre, string pCodigo, string pDescripcion, bool pEstado)
        {

        }

        public void CrearPermiso(string pNombre, string pCodigo, string pDescripcion, bool pEstado)
        {

        }

        public void ModificarPermiso(Guid pId, string pNombre, string pCodigo, string pDescripcion, bool pEstado)
        {

        }
        public void CambiarEstadoComponente(Guid pId, bool pEstado)
        {

        }

        public void EliminarRol(Guid pId)
        {

        }
        public void EliminarPermiso(Guid pId)
        {

        }

        #endregion

        #region Composicion de roles

        public List<ComponentePermisos> ListarHijosDeRol(Guid pId)
        {
            return null;
        }

        public void AgregarComponenteARol(Guid pIdRol, Guid pIdComponente)
        {

        }

        public void QuitarComponenteARol(Guid pIdRol, Guid pIdComponente)
        {

        }

        public void ReemplazarComponentesDeRol(Guid pIdRol, List<Guid> pHijosId)
        {

        }

        #endregion

        #region Asignación a usuarios
        public List<ComponentePermisos> ListarComponentesAsignadosAUsuario(Guid pIdUsuario)
        {
            return null;
        }

        public void AsignarComponenteAUsuario(Guid pIdUsuario, Guid pIdComponente)
        {

        }

        public void DesasignarComponenteAUsuario(Guid pIdUsuario, Guid pIdComponente)
        {

        }

        public void ReemplazarAsignacionesUsuario(Guid pIdUsuario, List<Guid> pHijosId)
        {

        }
        #endregion

        #region Validaciones

        private void ValidarDatosComponente(Guid pId, string pNombre, string pCodigo, string pDescripcion, bool pEstado) { }
        private void ValidarCodigoUnico(string codigo, Guid? idExcluido) { }
        private void ValidarRolExiste(Guid idRol) { }
        private void ValidarPermisoExiste(Guid idPermiso) { }
        private void ValidarNoAutoreferencia(Guid idRol, Guid idComponenteHijo) { }
        private void ValidarNoCiclo(Guid idRol, Guid idComponenteHijo) { }
        private void ValidarNoDuplicadoEnRol(Guid idRol, Guid idComponenteHijo) { }
        private void ValidarNoDuplicadoAsignacionUsuario(Guid idUsuario, Guid idComponente) { }
        private void RegistrarBitacora(string accion, string descripcion) { }

        #endregion
    }
}
