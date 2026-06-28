using BE;
using DAL.Excepciones;
using DAL.Idiomas;
using BLL.Bitacora;
using SSL.Interfaces;
using System;
using System.Collections.Generic;

namespace BLL.Idiomas
{
    public class GestionTraduccionesAppService
    {
        private readonly IIdiomaRepositorio _idiomaRepositorio;
        private readonly ITraduccionRepositorio _traduccionRepositorio;
        private readonly ISessionService _sessionService;
        private readonly IBitacoraService _bitacoraService;

        public GestionTraduccionesAppService(
            IIdiomaRepositorio idiomaRepositorio,
            ITraduccionRepositorio traduccionRepositorio,
            ISessionService sessionService,
            IBitacoraService bitacoraService)
        {
            if (idiomaRepositorio == null)
            {
                throw new ArgumentNullException(nameof(idiomaRepositorio));
            }

            if (traduccionRepositorio == null)
            {
                throw new ArgumentNullException(nameof(traduccionRepositorio));
            }

            if (sessionService == null)
            {
                throw new ArgumentNullException(nameof(sessionService));
            }

            if (bitacoraService == null)
            {
                throw new ArgumentNullException(nameof(bitacoraService));
            }

            _idiomaRepositorio = idiomaRepositorio;
            _traduccionRepositorio = traduccionRepositorio;
            _sessionService = sessionService;
            _bitacoraService = bitacoraService;
        }

        public List<Idioma> ListarIdiomasActivos()
        {
            try
            {
                return _idiomaRepositorio.ListarActivos();
            }
            catch (AccesoDatosException)
            {
                return new List<Idioma>();
            }
        }

        public List<Traduccion> ListarTraduccionesPorIdioma(Guid idiomaId)
        {
            if (idiomaId == Guid.Empty)
            {
                return new List<Traduccion>();
            }

            try
            {
                return _traduccionRepositorio.ListarDetallePorIdioma(idiomaId);
            }
            catch (AccesoDatosException)
            {
                return new List<Traduccion>();
            }
        }

        public ResultadoGuardadoTraduccion GuardarTraduccion(Guid idiomaId, string clave, string texto)
        {
            if (idiomaId == Guid.Empty)
            {
                return ResultadoGuardadoTraduccion.Error("Debe seleccionar un idioma.");
            }

            if (string.IsNullOrWhiteSpace(clave))
            {
                return ResultadoGuardadoTraduccion.Error("Debe ingresar una clave de traducción.");
            }

            if (string.IsNullOrWhiteSpace(texto))
            {
                return ResultadoGuardadoTraduccion.Error("Debe ingresar el texto traducido.");
            }

            try
            {
                _traduccionRepositorio.Guardar(idiomaId, clave, texto);

                RegistrarBitacora("TRADUCCION_CREADA",$"Se creó la traducción para la clave '{clave.Trim()}'.");

                return ResultadoGuardadoTraduccion.Ok();
            }
            catch (AccesoDatosException)
            {
                return ResultadoGuardadoTraduccion.Error("No se pudo guardar la traducción. Verifique la conexión con la base de datos.");
            }
        }

        public ResultadoGuardadoTraduccion ModificarTraduccion(Guid id, string clave, string texto)
        {
            if (id == Guid.Empty)
            {
                return ResultadoGuardadoTraduccion.Error("Debe seleccionar una traducción.");
            }

            if (string.IsNullOrWhiteSpace(clave))
            {
                return ResultadoGuardadoTraduccion.Error("Debe ingresar una clave de traducción.");
            }

            if (string.IsNullOrWhiteSpace(texto))
            {
                return ResultadoGuardadoTraduccion.Error("Debe ingresar el texto traducido.");
            }

            try
            {
                _traduccionRepositorio.Modificar(id, clave, texto);

                RegistrarBitacora("TRADUCCION_MODIFICADA",$"Se modificó la traducción para la clave '{clave.Trim()}'.");

                return ResultadoGuardadoTraduccion.Ok("La traducción fue modificada correctamente.");
            }
            catch (AccesoDatosException)
            {
                return ResultadoGuardadoTraduccion.Error("No se pudo modificar la traducción. Verifique la conexión con la base de datos o que la clave no esté duplicada.");
            }
        }

        public ResultadoGuardadoTraduccion EliminarTraduccion(Guid id)
        {
            if (id == Guid.Empty)
            {
                return ResultadoGuardadoTraduccion.Error("Debe seleccionar una traducción.");
            }

            try
            {
                Traduccion traduccion = _traduccionRepositorio.ObtenerPorId(id);

                if (traduccion == null)
                {
                    return ResultadoGuardadoTraduccion.Error("No se encontró la traducción seleccionada.");
                }

                _traduccionRepositorio.Eliminar(id);

                RegistrarBitacora("TRADUCCION_ELIMINADA",$"Se eliminó la traducción para la clave '{traduccion.Clave}'.");

                return ResultadoGuardadoTraduccion.Ok("La traducción fue eliminada correctamente.");
            }
            catch (AccesoDatosException)
            {
                return ResultadoGuardadoTraduccion.Error("No se pudo eliminar la traducción. Verifique la conexión con la base de datos.");
            }
        }

        private void RegistrarBitacora(string accion, string descripcion)
        {
            _bitacoraService.Registrar(
                _sessionService.HaySesionActiva
                    ? (Guid?)_sessionService.UsuarioIdActual
                    : null,
                _sessionService.NombreUsuarioActual,
                "Traducciones",
                accion,
                descripcion,
                "INFO"
            );
        }
    }
}