using BE;
using System.Collections.Generic;

namespace DAL.Idiomas
{
    public interface IIdiomaRepositorio
    {
        List<Idioma> ListarActivos();
        Idioma ObtenerPorCodigo(string codigo);
    }
}