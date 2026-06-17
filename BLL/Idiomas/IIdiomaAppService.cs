using BE;
using System.Collections.Generic;

namespace BLL.Idiomas
{
    public interface IIdiomaAppService
    {
        Idioma IdiomaActual { get; }

        List<Idioma> ListarIdiomasActivos();

        ResultadoCambioIdioma CambiarIdioma(string codigo);

        string Traducir(string clave);

        void Suscribir(IObservadorIdioma observador);

        void Desuscribir(IObservadorIdioma observador);
    }
}