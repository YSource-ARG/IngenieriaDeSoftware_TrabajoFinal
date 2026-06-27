using BE;
using BE.Permisos;
using SSL.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SSL.Integridad
{
    public class DigitoVerificadorService : IDigitoVerificadorService
    {
        public string CalcularDigitoHorizontalUsuario(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario));
            }

            // No se incluye el propio DVH para evitar una dependencia circular.
            // Tampoco se incluyen campos operativos que cambian normalmente
            // durante el uso de la aplicación.
            List<string> valores = new List<string>
            {
                usuario.Id.ToString("D"),
                NormalizarTexto(usuario.NombreUsuario),
                NormalizarTexto(usuario.NombreCompleto),
                NormalizarTexto(usuario.Email),
                NormalizarGuidNullable(usuario.IdiomaPreferidoId),
                NormalizarTexto(usuario.PasswordHash),
                NormalizarBool(usuario.Activo),
                NormalizarBool(usuario.DebeCambiarPassword),
                NormalizarFecha(usuario.FechaCreacion)
            };

            return CalcularHashConPosiciones(valores);
        }

        public string CalcularDigitoVerticalUsuarios(List<Usuario> usuarios)
        {
            if (usuarios == null)
            {
                throw new ArgumentNullException(nameof(usuarios));
            }

            return CalcularDigitoVertical(
                usuarios
                    .OrderBy(x => x.Id)
                    .Select(x => x.DigitoVerificadorHorizontal)
            );
        }

        public string CalcularDigitoHorizontalComponentePermisos(
            ComponentePermisos componente)
        {
            if (componente == null)
            {
                throw new ArgumentNullException(nameof(componente));
            }

            List<string> valores = new List<string>
            {
                componente.Id.ToString("D"),
                NormalizarTexto(componente.Nombre),
                NormalizarTexto(componente.Codigo),
                NormalizarTexto(componente.Descripcion),
                NormalizarBool(componente.Activo),
                ((int)componente.Tipo).ToString(CultureInfo.InvariantCulture),
                NormalizarFecha(componente.FechaCreacion),
                NormalizarFechaNullable(componente.FechaModificacion)
            };

            return CalcularHashConPosiciones(valores);
        }

        public string CalcularDigitoVerticalComponentesPermisos(
            List<ComponentePermisos> componentes)
        {
            if (componentes == null)
            {
                throw new ArgumentNullException(nameof(componentes));
            }

            return CalcularDigitoVertical(
                componentes
                    .OrderBy(x => x.Id)
                    .Select(x => x.DigitoVerificadorHorizontal)
            );
        }

        public string CalcularDigitoHorizontalRolComponente(
            RolComponente relacion)
        {
            if (relacion == null)
            {
                throw new ArgumentNullException(nameof(relacion));
            }

            List<string> valores = new List<string>
            {
                relacion.RolId.ToString("D"),
                relacion.ComponenteHijoId.ToString("D")
            };

            return CalcularHashConPosiciones(valores);
        }

        public string CalcularDigitoVerticalRolComponentes(
            List<RolComponente> relaciones)
        {
            if (relaciones == null)
            {
                throw new ArgumentNullException(nameof(relaciones));
            }

            return CalcularDigitoVertical(
                relaciones
                    .OrderBy(x => x.RolId)
                    .ThenBy(x => x.ComponenteHijoId)
                    .Select(x => x.DigitoVerificadorHorizontal)
            );
        }

        public string CalcularDigitoHorizontalUsuarioPermisoComponente(
            UsuarioPermisoComponente asignacion)
        {
            if (asignacion == null)
            {
                throw new ArgumentNullException(nameof(asignacion));
            }

            List<string> valores = new List<string>
            {
                asignacion.UsuarioId.ToString("D"),
                asignacion.ComponenteId.ToString("D"),
                NormalizarFecha(asignacion.FechaAsignacion),
                NormalizarGuidNullable(asignacion.AsignadoPorUsuarioId)
            };

            return CalcularHashConPosiciones(valores);
        }

        public string CalcularDigitoVerticalUsuarioPermisoComponentes(
            List<UsuarioPermisoComponente> asignaciones)
        {
            if (asignaciones == null)
            {
                throw new ArgumentNullException(nameof(asignaciones));
            }

            return CalcularDigitoVertical(
                asignaciones
                    .OrderBy(x => x.UsuarioId)
                    .ThenBy(x => x.ComponenteId)
                    .Select(x => x.DigitoVerificadorHorizontal)
            );
        }

        private string CalcularDigitoVertical(IEnumerable<string> digitosHorizontales)
        {
            List<string> valoresVerticales = digitosHorizontales
                .Select((digitoHorizontal, indice) =>
                    "F"
                    + (indice + 1).ToString(CultureInfo.InvariantCulture)
                    + "="
                    + NormalizarTexto(digitoHorizontal))
                .ToList();

            return CalcularHashConPosiciones(valoresVerticales);
        }

        private string CalcularHashConPosiciones(List<string> valores)
        {
            StringBuilder cadena = new StringBuilder();

            for (int indiceAtributo = 0;
                 indiceAtributo < valores.Count;
                 indiceAtributo++)
            {
                string valor = valores[indiceAtributo] ?? string.Empty;
                int posicionAtributo = indiceAtributo + 1;

                if (valor.Length == 0)
                {
                    cadena.Append("A")
                        .Append(posicionAtributo)
                        .Append(":C0:0|");

                    continue;
                }

                for (int indiceCaracter = 0;
                     indiceCaracter < valor.Length;
                     indiceCaracter++)
                {
                    int posicionCaracter = indiceCaracter + 1;
                    int codigoCaracter = valor[indiceCaracter];

                    cadena.Append("A")
                        .Append(posicionAtributo)
                        .Append(":C")
                        .Append(posicionCaracter)
                        .Append(":")
                        .Append(codigoCaracter)
                        .Append("|");
                }
            }

            return CalcularSha256(cadena.ToString());
        }

        private string CalcularSha256(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytesTexto = Encoding.UTF8.GetBytes(texto);
                byte[] bytesHash = sha256.ComputeHash(bytesTexto);

                StringBuilder resultado = new StringBuilder();

                foreach (byte byteHash in bytesHash)
                {
                    resultado.Append(byteHash.ToString("x2"));
                }

                return resultado.ToString();
            }
        }

        private string NormalizarGuidNullable(Guid? valor)
        {
            return valor.HasValue
                ? valor.Value.ToString("D")
                : string.Empty;
        }

        private string NormalizarTexto(string texto)
        {
            return string.IsNullOrWhiteSpace(texto)
                ? string.Empty
                : texto.Trim();
        }

        private string NormalizarBool(bool valor)
        {
            return valor ? "1" : "0";
        }

        private string NormalizarFecha(DateTime fecha)
        {
            return fecha.ToString("O", CultureInfo.InvariantCulture);
        }

        private string NormalizarFechaNullable(DateTime? fecha)
        {
            return fecha.HasValue
                ? NormalizarFecha(fecha.Value)
                : string.Empty;
        }
    }
}
