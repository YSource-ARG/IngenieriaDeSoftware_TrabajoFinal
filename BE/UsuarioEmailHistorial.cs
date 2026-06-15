using System;

namespace BE
{
    public class UsuarioEmailHistorial
    {
        public Guid Id { get; set; }

        public Guid UsuarioId { get; set; }

        public string EmailAnterior { get; set; }

        public string EmailNuevo { get; set; }

        public DateTime FechaCambio { get; set; }

        public Guid? UsuarioCambioId { get; set; }

        public string UsuarioCambioNombre { get; set; }
    }
}