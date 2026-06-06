using BE;
using System;
using System.Collections.Generic;

namespace DAL.Bitacora
{
    public interface IBitacoraRepositorio
    {
        void Registrar(BE.Bitacora bitacora);

        List<BE.Bitacora> Listar(string modulo, string tipo, DateTime? fechaDesde, DateTime? fechaHasta, int cantidadMaxima);
    }
}
