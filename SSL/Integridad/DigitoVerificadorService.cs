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
            // el sistema como reacción ante una falla de integridad. Si se incluyera,
            // bloquear usuarios cambiaría nuevamente el DVH y generaría un ciclo.
            //
            // No se incluyen IntentosFallidosLogin, BloqueadoHasta ni FechaUltimoAcceso
            // porque son datos operativos del proceso de login y pueden cambiar
            // normalmente sin representar una vulneración externa de la base.
            List<string> valores = new List<string>
{
                usuario.Id.ToString("D"),
                NormalizarTexto(usuario.NombreUsuario),
                NormalizarTexto(usuario.NombreCompleto),
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

            // El DVV representa la integridad del conjunto completo.
            // Para que el cálculo sea estable, se ordena siempre por Id.
            // Además, se agrega la posición de fila para detectar agregados,
            // eliminaciones o cambios de orden en el conjunto controlado.
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

            for (int indiceAtributo = 0; indiceAtributo < valores.Count; indiceAtributo++)
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

                for (int indiceCaracter = 0; indiceCaracter < valor.Length; indiceCaracter++)
                {
                    int posicionCaracter = indiceCaracter + 1;
                    int codigoCaracter = valor[indiceCaracter];

                    // La cadena incorpora:
                    // A = posición del atributo dentro de la entidad.
                    // C = posición del carácter dentro del atributo.
                    // Valor numérico del carácter = contenido.
                    //
                    // Con esto se cumple la regla de contenido + posición.
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

                foreach (byte b in bytesHash)
                {
                    resultado.Append(b.ToString("x2"));
                }

                return resultado.ToString();
            }
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
                ? fecha.Value.ToString("O", CultureInfo.InvariantCulture)
                : string.Empty;
        }
    }
}