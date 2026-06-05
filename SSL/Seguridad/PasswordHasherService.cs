using System;
using System.Security.Cryptography;
using System.Text;
using SSL.Interfaces;

namespace SSL.Seguridad
{
    public class PasswordHasherService : IPasswordHasher
    {
        public string GenerarHash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("La contraseña no puede estar vacía.", nameof(password));
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public bool VerificarPassword(string passwordIngresada, string passwordHashAlmacenado)
        {
            if (string.IsNullOrWhiteSpace(passwordIngresada) || string.IsNullOrWhiteSpace(passwordHashAlmacenado))
            {
                return false;
            }

            string hashGenerado = GenerarHash(passwordIngresada);
            return SonIgualesTiempoConstante(hashGenerado, passwordHashAlmacenado);
        }
        /// <summary>
        /// Compara los hashes recorriendo todos los bytes para reducir el riesgo
        /// de ataques por medición de tiempo.
        /// </summary>
        /// <param name="valor1"></param>
        /// <param name="valor2"></param>
        /// <returns></returns>
        private bool SonIgualesTiempoConstante(string valor1, string valor2)
        {
            byte[] valor1Bytes = Convert.FromBase64String(valor1);
            byte[] valor2Bytes = Convert.FromBase64String(valor2);

            if (valor1Bytes.Length != valor2Bytes.Length)
            {
                return false;
            }

            int resultado = 0;

            for (int i = 0; i < valor1Bytes.Length; i++)
            {
                resultado |= valor1Bytes[i] ^ valor2Bytes[i];
            }

            return resultado == 0;
        }
    }
}
