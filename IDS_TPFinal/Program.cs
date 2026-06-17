using BLL.Configuracion;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace UI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


            // Se crea la factory de servicios, responsable de construir e inyectar las dependencias
            // crea repositorios, servicios de negocio, seguridad y bitácora para el caso de uso.
            AplicacionServiceFactory serviceFactory = new AplicacionServiceFactory(connectionString);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new FrmLogin(
                serviceFactory.LoginAppService,
                serviceFactory.CerrarSesionAppService,
                serviceFactory.GestionUsuariosAppService,
                serviceFactory.IntegridadService,
                serviceFactory.BitacoraService,
                serviceFactory.IdiomaAppService,
                serviceFactory.GestionTraduccionesAppService,
                serviceFactory.GestionIdiomasAppService
            ));
        }
    }
}
