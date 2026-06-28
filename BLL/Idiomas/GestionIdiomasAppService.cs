using BE;
using DAL.Excepciones;
using DAL.Idiomas;
using BLL.Bitacora;
using SSL.Interfaces;
using System;
using System.Collections.Generic;

namespace BLL.Idiomas
{
    public class GestionIdiomasAppService
    {
        private readonly IIdiomaRepositorio _idiomaRepositorio;
        private readonly ISessionService _sessionService;
        private readonly IBitacoraService _bitacoraService;

        public GestionIdiomasAppService(
            IIdiomaRepositorio idiomaRepositorio,
            ISessionService sessionService,
            IBitacoraService bitacoraService)
        {
            _idiomaRepositorio = idiomaRepositorio
                ?? throw new ArgumentNullException(nameof(idiomaRepositorio));

            _sessionService = sessionService
                ?? throw new ArgumentNullException(nameof(sessionService));

            _bitacoraService = bitacoraService
                ?? throw new ArgumentNullException(nameof(bitacoraService));
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

            bool esNuevo = id == Guid.Empty;

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

                RegistrarBitacora(
                    esNuevo
                        ? "IDIOMA_CREADO"
                        : "IDIOMA_MODIFICADO",
                    esNuevo
                        ? $"Se creó el idioma '{idioma.Nombre}' con código '{idioma.Codigo}'."
                        : $"Se modificó el idioma '{idioma.Nombre}' con código '{idioma.Codigo}'."
                );

                return ResultadoGuardadoIdioma.Ok();
            }
            catch (AccesoDatosException)
            {
                return ResultadoGuardadoIdioma.Error("No se pudo guardar el idioma. Verifique la conexión con la base de datos.");
            }
        }

        private void RegistrarBitacora(string accion, string descripcion)
        {
            _bitacoraService.Registrar(
                _sessionService.HaySesionActiva
                    ? (Guid?)_sessionService.UsuarioIdActual
                    : null,
                _sessionService.NombreUsuarioActual,
                "Idiomas",
                accion,
                descripcion,
                "INFO"
            );
        }
    }
}