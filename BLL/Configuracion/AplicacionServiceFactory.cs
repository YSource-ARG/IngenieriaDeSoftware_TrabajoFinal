using BLL.Autenticacion;
using BLL.Bitacora;
using BLL.Usuarios;
using DAL.BaseDeDatos;
using DAL.Bitacora;
using DAL.Usuarios;
using SSL.Interfaces;
using SSL.Seguridad;
using SSL.Sesion;
using SSL.Logging;
using System;
using BLL.Integridad;
using DAL.Integridad;
using SSL.Integridad;
using BLL.Idiomas;
using DAL.Idiomas;
using BLL.Permisos;
using DAL.Permisos;

namespace BLL.Configuracion
{
    public class AplicacionServiceFactory
    {
        public LoginAppService LoginAppService { get; }

        public CerrarSesionAppService CerrarSesionAppService { get; }

        public GestionUsuariosAppService GestionUsuariosAppService { get; }

        public IIdiomaAppService IdiomaAppService { get; }

        public IBitacoraService BitacoraService { get; }

        public IIntegridadService IntegridadService { get; }

        public GestionTraduccionesAppService GestionTraduccionesAppService { get; }

        public GestionIdiomasAppService GestionIdiomasAppService { get; }

        public GestionPermisosAppService GestionPermisosAppService { get; }

        public AutorizacionService AutorizacionService { get; }

        public AplicacionServiceFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("La cadena de conexión no puede estar vacía.", nameof(connectionString));
            }

            IConnectionFactory connectionFactory = new SqlConnectionFactory(connectionString);

            IUsuarioRepositorio usuarioRepositorio = new UsuarioRepositorio(connectionFactory);
            IUsuarioEmailHistorialRepositorio usuarioEmailHistorialRepositorio = new UsuarioEmailHistorialRepositorio(connectionFactory);
            IBitacoraRepositorio bitacoraRepositorio = new BitacoraRepositorio(connectionFactory);
            IDigitoVerificadorRepositorio digitoVerificadorRepositorio = new DigitoVerificadorRepositorio(connectionFactory);
            IPermisoComponenteRepositorio permisoComponenteRepositorio = new PermisoComponenteRepositorio(connectionFactory);
            IRolComponenteRepositorio rolComponenteRepositorio = new RolComponenteRepositorio(connectionFactory);
            IUsuarioPermisoComponenteRepositorio usuarioPermisoComponenteRepositorio = new UsuarioPermisoComponenteRepositorio(connectionFactory);
            IUnidadDeTrabajoPermisos unidadDeTrabajoPermisos = new UnidadDeTrabajoPermisos(connectionFactory);

            IIdiomaRepositorio idiomaRepositorio = new IdiomaRepositorio(connectionFactory);
            ITraduccionRepositorio traduccionRepositorio = new TraduccionRepositorio(connectionFactory);

            IPasswordHasher passwordHasher = new PasswordHasherService();
            ISessionService sessionService = SessionService.ObtenerSesion();
            IDigitoVerificadorService digitoVerificadorService = new DigitoVerificadorService();

            IBitacoraContingenciaService bitacoraContingenciaService = new BitacoraContingenciaService();

            IdiomaAppService = new IdiomaAppService(idiomaRepositorio, traduccionRepositorio);
            GestionIdiomasAppService = new GestionIdiomasAppService(idiomaRepositorio);
            GestionTraduccionesAppService = new GestionTraduccionesAppService(idiomaRepositorio, traduccionRepositorio);

            BitacoraService = new BitacoraService(bitacoraRepositorio, bitacoraContingenciaService);

            IntegridadService = new IntegridadService(
            digitoVerificadorRepositorio,
            digitoVerificadorService,
            BitacoraService
            );

            LoginAppService = new LoginAppService(
                usuarioRepositorio,
                passwordHasher,
                sessionService,
                BitacoraService
            );

            CerrarSesionAppService = new CerrarSesionAppService(
                sessionService,
                BitacoraService
            );

            GestionUsuariosAppService = new GestionUsuariosAppService(
                usuarioRepositorio,
                usuarioEmailHistorialRepositorio,
                passwordHasher,
                sessionService,
                BitacoraService,
                IntegridadService
            );

            GestionPermisosAppService = new GestionPermisosAppService(
                permisoComponenteRepositorio,
                rolComponenteRepositorio,
                usuarioPermisoComponenteRepositorio,
                usuarioRepositorio,
                unidadDeTrabajoPermisos,
                sessionService,
                BitacoraService
            );

            AutorizacionService = new AutorizacionService(
                permisoComponenteRepositorio,
                rolComponenteRepositorio,
                usuarioPermisoComponenteRepositorio,
                usuarioRepositorio,
                sessionService
            );
        }
    }
}
