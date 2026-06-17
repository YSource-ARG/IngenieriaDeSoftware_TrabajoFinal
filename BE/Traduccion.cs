using System;

namespace BE
{
    public class Traduccion
    {
        public Guid Id { get; set; }
        public string Clave { get; set; }
        public Guid IdiomaId { get; set; }
        public string Texto { get; set; }
    }
}