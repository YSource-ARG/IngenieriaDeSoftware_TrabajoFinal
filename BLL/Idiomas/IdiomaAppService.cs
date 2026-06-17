using BE;
using DAL.Excepciones;
using DAL.Idiomas;
using System;
using System.Collections.Generic;

namespace BLL.Idiomas
{
    public class IdiomaAppService : IIdiomaAppService
    {
        private const string CodigoIdiomaDefault = "es-AR";

        private readonly IIdiomaRepositorio _idiomaRepositorio;
        private readonly ITraduccionRepositorio _traduccionRepositorio;
        private readonly List<IObservadorIdioma> _observadores;
        private Dictionary<string, string> _traducciones;

        public Idioma IdiomaActual { get; private set; }

        public IdiomaAppService(
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
            _observadores = new List<IObservadorIdioma>();
            _traducciones = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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

        public ResultadoCambioIdioma CambiarIdioma(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return ResultadoCambioIdioma.Error("Debe seleccionar un idioma.");
            }

            try
            {
                Idioma idioma = _idiomaRepositorio.ObtenerPorCodigo(codigo);

                if (idioma == null || !idioma.Activo)
                {
                    return ResultadoCambioIdioma.Error("El idioma seleccionado no está disponible.");
                }

                IdiomaActual = idioma;
                _traducciones = _traduccionRepositorio.ListarPorIdioma(idioma.Id);

                NotificarObservadores();

                return ResultadoCambioIdioma.Ok();
            }
            catch (AccesoDatosException)
            {
                return ResultadoCambioIdioma.Error("No se pudieron cargar las traducciones. Se conservarán los textos actuales.");
            }
        }

        public string Traducir(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
            {
                return string.Empty;
            }

            string claveNormalizada = clave.Trim();

            if (_traducciones.ContainsKey(claveNormalizada))
            {
                return _traducciones[claveNormalizada];
            }

            return claveNormalizada;
        }

        public void Suscribir(IObservadorIdioma observador)
        {
            if (observador == null)
            {
                return;
            }

            if (!_observadores.Contains(observador))
            {
                _observadores.Add(observador);
            }
        }

        public void Desuscribir(IObservadorIdioma observador)
        {
            if (observador == null)
            {
                return;
            }

            if (_observadores.Contains(observador))
            {
                _observadores.Remove(observador);
            }
        }

        private void NotificarObservadores()
        {
            List<IObservadorIdioma> copiaObservadores =
                new List<IObservadorIdioma>(_observadores);

            foreach (IObservadorIdioma observador in copiaObservadores)
            {
                observador.ActualizarIdioma();
            }
        }
    }
}