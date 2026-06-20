using BE;
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

            // Se incluyen los datos propios del usuario que forman parte
            // del estado protegido por integridad.
            //
            // No se incluye DigitoVerificadorHorizontal porque sería circular:
            // el DVH no puede calcularse usando su propio valor.
            //
            // No se incluye BloqueadoPorIntegridad porque ese campo lo modifica
            // el sistema como reacción ante una falla de integridad.
            //
            // No se incluyen IntentosFallidosLogin, BloqueadoHasta,
            // FechaUltimoAcceso porque son datos operativos que pueden cambiar
            // normalmente desde la aplicación.

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

            List<string> valoresVerticales = usuarios
                .OrderBy(x => x.Id)
                .Select((usuario, indice) =>
                    "F" + (indice + 1).ToString(CultureInfo.InvariantCulture)
                    + "="
                    + NormalizarTexto(usuario.DigitoVerificadorHorizontal))
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
    }
}