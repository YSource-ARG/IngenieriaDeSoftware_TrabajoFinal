using BE;
using System;
using System.Collections.Generic;

namespace DAL.Idiomas
{
    public interface IIdiomaRepositorio
    {
        List<Idioma> ListarActivos();

        List<Idioma> ListarTodos();

        Idioma ObtenerPorCodigo(string codigo);

        void Guardar(Idioma idioma);
    }
}