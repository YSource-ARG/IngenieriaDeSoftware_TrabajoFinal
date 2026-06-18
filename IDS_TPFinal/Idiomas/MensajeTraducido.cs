using BLL.Idiomas;
using System.Windows.Forms;

namespace UI.Idiomas
{
    public static class MensajeTraducido
    {
        public static DialogResult Mostrar(
            IWin32Window owner,
            IIdiomaAppService idiomaAppService,
            string claveMensaje,
            string mensajePorDefecto,
            string claveTitulo,
            string tituloPorDefecto,
            MessageBoxButtons botones,
            MessageBoxIcon icono)
        {
            string mensaje = TraducirConFallback(idiomaAppService, claveMensaje, mensajePorDefecto);
            string titulo = TraducirConFallback(idiomaAppService, claveTitulo, tituloPorDefecto);

            return MessageBox.Show(
                owner,
                mensaje,
                titulo,
                botones,
                icono
            );
        }

        public static string TraducirConFallback(
            IIdiomaAppService idiomaAppService,
            string clave,
            string textoPorDefecto)
        {
            if (idiomaAppService == null)
            {
                return textoPorDefecto;
            }

            string textoTraducido = idiomaAppService.Traducir(clave);

            if (string.IsNullOrWhiteSpace(textoTraducido) || textoTraducido == clave)
            {
                return textoPorDefecto;
            }

            return textoTraducido;
        }
    }
}