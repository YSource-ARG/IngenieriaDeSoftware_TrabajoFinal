using System;

namespace BE
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCompleto { get; set; }        
        public string Email { get; set; }
        public Guid? IdiomaPreferidoId { get; set; }
        public string PasswordHash { get; set; }
        public bool Activo { get; set; }
        public bool DebeCambiarPassword { get; set; }

        public int IntentosFallidosLogin { get; set; }
        public DateTime? BloqueadoHasta { get; set; }

        // Bloqueo independiente del estado Activo.
        // Activo representa una decisión administrativa normal,
        // mientras que BloqueadoPorIntegridad representa un bloqueo preventivo
        // por una posible modificación externa de la base de datos.
        public bool BloqueadoPorIntegridad { get; set; }

        // DVH del usuario.
        // Se guarda en la entidad protegida para detectar modificaciones externas
        // sobre los datos del registro.
        public string DigitoVerificadorHorizontal { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaUltimoAcceso { get; set; }
    }
}