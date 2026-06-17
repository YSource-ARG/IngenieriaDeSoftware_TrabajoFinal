using BE;
using DAL.Excepciones;
using DAL.Idiomas;
using System;
using System.Collections.Generic;

namespace BLL.Idiomas
{
    public class GestionTraduccionesAppService
    {
        private readonly IIdiomaRepositorio _idiomaRepositorio;
        private readonly ITraduccionRepositorio _traduccionRepositorio;

        public GestionTraduccionesAppService(
            IIdiomaRepositorio idiomaRepositorio,
            ITraduccionRepositorio traduccionRepositorio)
        {
            if (idiomaRepositorio == null)
            {
                throw new ArgumentNullException(nameof(idiomaRepositorio));
            }

            if (traduccionRepositorio == null)
            {
                throw new ArgumentNullException(nameof(traduccionRepositorio));
            }

            _idiomaRepositorio = idiomaRepositorio;
            _traduccionRepositorio = traduccionRepositorio;
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

                return ResultadoGuardadoTraduccion.Ok();
            }
            catch (AccesoDatosException)
            {
                return ResultadoGuardadoTraduccion.Error("No se pudo guardar la traducción. Verifique la conexión con la base de datos.");
            }
        }
    }
}