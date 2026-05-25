using BLL.Autenticacion;
using BLL.Bitacora;
using BLL.Usuarios;
using DAL.BaseDeDatos;
using DAL.Bitacora;
using DAL.Usuarios;
using SSL.Interfaces;
using SSL.Seguridad;
using SSL.Sesion;
using System;

namespace BLL.Configuracion
{
    public class AplicacionServiceFactory
    {
        public LoginAppService LoginAppService { get; }

        public CerrarSesionAppService CerrarSesionAppService { get; }

        public GestionUsuariosAppService GestionUsuariosAppService { get; }

        public IBitacoraService BitacoraService { get; }

        public AplicacionServiceFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("La cadena de conexión no puede estar vacía.", nameof(connectionString));
            }

            IConnectionFactory connectionFactory = new SqlConnectionFactory(connectionString);

            IUsuarioRepositorio usuarioRepositorio = new UsuarioRepositorio(connectionFactory);
            IBitacoraRepositorio bitacoraRepositorio = new BitacoraRepositorio(connectionFactory);

            IPasswordHasher passwordHasher = new PasswordHasherService();
            ISessionService sessionService = SessionService.ObtenerSesion();

            BitacoraService = new BitacoraService(bitacoraRepositorio);

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
                passwordHasher,
                sessionService,
                BitacoraService
            );
        }
    }
}
