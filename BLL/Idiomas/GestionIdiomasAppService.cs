using BE;
using DAL.Excepciones;
using DAL.Idiomas;
using System;
using System.Collections.Generic;

namespace BLL.Idiomas
{
    public class GestionIdiomasAppService
    {
        private readonly IIdiomaRepositorio _idiomaRepositorio;

        public GestionIdiomasAppService(IIdiomaRepositorio idiomaRepositorio)
        {
            if (idiomaRepositorio == null)
            {
                throw new ArgumentNullException(nameof(idiomaRepositorio));
            }

            _idiomaRepositorio = idiomaRepositorio;
        }

        public List<Idioma> ListarIdiomas()
        {
            try
            {
                return _idiomaRepositorio.ListarTodos();
            }
            catch (AccesoDatosException)
            {
                return new List<Idioma>();
            }
        }

        public ResultadoGuardadoIdioma GuardarIdioma(Guid id, string codigo, string nombre, bool activo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return ResultadoGuardadoIdioma.Error("Debe ingresar el código del idioma.");
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                return ResultadoGuardadoIdioma.Error("Debe ingresar el nombre del idioma.");
            }

            string codigoNormalizado = codigo.Trim();
            string nombreNormalizado = nombre.Trim();

            if (codigoNormalizado.Length > 10)
            {
                return ResultadoGuardadoIdioma.Error("El código del idioma no puede superar los 10 caracteres.");
            }

            if (nombreNormalizado.Length > 100)
            {
                return ResultadoGuardadoIdioma.Error("El nombre del idioma no puede superar los 100 caracteres.");
            }

            try
            {
                Idioma idiomaExistente = _idiomaRepositorio.ObtenerPorCodigo(codigoNormalizado);

                if (idiomaExistente != null && idiomaExistente.Id != id)
                {
                    return ResultadoGuardadoIdioma.Error("Ya existe un idioma con ese código.");
                }

                Idioma idioma = new Idioma
                {
                    Id = id == Guid.Empty ? Guid.NewGuid() : id,
                    Codigo = codigoNormalizado,
                    Nombre = nombreNormalizado,
                    Activo = activo
                };

                _idiomaRepositorio.Guardar(idioma);

                return ResultadoGuardadoIdioma.Ok();
            }
            catch (AccesoDatosException)
            {
                return ResultadoGuardadoIdioma.Error("No se pudo guardar el idioma. Verifique la conexión con la base de datos.");
            }
        }
    }
}