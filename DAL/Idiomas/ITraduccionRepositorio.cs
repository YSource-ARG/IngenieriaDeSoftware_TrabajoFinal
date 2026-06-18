using BE;
using System;
using System.Collections.Generic;

namespace DAL.Idiomas
{
    public interface ITraduccionRepositorio
    {
        Dictionary<string, string> ListarPorIdioma(Guid idiomaId);

        List<Traduccion> ListarDetallePorIdioma(Guid idiomaId);

        void Guardar(Guid idiomaId, string clave, string texto);

        void Modificar(Guid id, string clave, string texto);

        void Eliminar(Guid id);
    }
}