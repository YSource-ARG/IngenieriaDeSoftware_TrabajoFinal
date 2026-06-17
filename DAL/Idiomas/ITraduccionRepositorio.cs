using System;
using System.Collections.Generic;

namespace DAL.Idiomas
{
    public interface ITraduccionRepositorio
    {
        Dictionary<string, string> ListarPorIdioma(Guid idiomaId);
    }
}