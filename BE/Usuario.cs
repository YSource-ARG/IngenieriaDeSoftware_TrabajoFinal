using System;

namespace BE
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordHash { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaUltimoAcceso { get; set; }
    }
}
