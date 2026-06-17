using BLL.Idiomas;
using System;
using System.Windows.Forms;

namespace UI.Idiomas
{
    public static class TraductorControles
    {
        public static void TraducirFormulario(Form formulario, IIdiomaAppService idiomaAppService)
        {
            if (formulario == null || idiomaAppService == null)
            {
                return;
            }

            TraducirTextoPorTag(formulario, idiomaAppService);

            foreach (Control control in formulario.Controls)
            {
                TraducirControl(control, idiomaAppService);
            }
        }

        private static void TraducirControl(Control control, IIdiomaAppService idiomaAppService)
        {
            if (control == null)
            {
                return;
            }

            TraducirTextoPorTag(control, idiomaAppService);

            if (control is DataGridView dataGridView)
            {
                TraducirColumnasGrilla(dataGridView, idiomaAppService);
            }

            if (control is MenuStrip menuStrip)
            {
                TraducirItemsMenu(menuStrip.Items, idiomaAppService);
            }

            if (control is ToolStrip toolStrip)
            {
                TraducirItemsMenu(toolStrip.Items, idiomaAppService);
            }

            if (control is TabControl tabControl)
            {
                TraducirTabPages(tabControl, idiomaAppService);
            }

            foreach (Control hijo in control.Controls)
            {
                TraducirControl(hijo, idiomaAppService);
            }
        }

        private static void TraducirTextoPorTag(Control control, IIdiomaAppService idiomaAppService)
        {
            string clave = control.Tag as string;

            if (string.IsNullOrWhiteSpace(clave))
            {
                return;
            }

            string textoTraducido = idiomaAppService.Traducir(clave);

            if (TieneTraduccionValida(clave, textoTraducido))
            {
                control.Text = textoTraducido;
            }
        }

        private static void TraducirColumnasGrilla(DataGridView grilla, IIdiomaAppService idiomaAppService)
        {
            foreach (DataGridViewColumn columna in grilla.Columns)
            {
                string clave = columna.Tag as string;

                if (string.IsNullOrWhiteSpace(clave))
                {
                    continue;
                }

                string textoTraducido = idiomaAppService.Traducir(clave);

                if (TieneTraduccionValida(clave, textoTraducido))
                {
                    columna.HeaderText = textoTraducido;
                }
            }
        }

        private static void TraducirItemsMenu(ToolStripItemCollection items, IIdiomaAppService idiomaAppService)
        {
            foreach (ToolStripItem item in items)
            {
                string clave = item.Tag as string;

                if (!string.IsNullOrWhiteSpace(clave))
                {
                    string textoTraducido = idiomaAppService.Traducir(clave);

                    if (TieneTraduccionValida(clave, textoTraducido))
                    {
                        item.Text = textoTraducido;
                    }
                }

                if (item is ToolStripMenuItem menuItem)
                {
                    TraducirItemsMenu(menuItem.DropDownItems, idiomaAppService);
                }
            }
        }

        private static void TraducirTabPages(TabControl tabControl, IIdiomaAppService idiomaAppService)
        {
            foreach (TabPage tabPage in tabControl.TabPages)
            {
                string clave = tabPage.Tag as string;

                if (!string.IsNullOrWhiteSpace(clave))
                {
                    string textoTraducido = idiomaAppService.Traducir(clave);

                    if (TieneTraduccionValida(clave, textoTraducido))
                    {
                        tabPage.Text = textoTraducido;
                    }
                }

                foreach (Control control in tabPage.Controls)
                {
                    TraducirControl(control, idiomaAppService);
                }
            }
        }

        private static bool TieneTraduccionValida(string clave, string textoTraducido)
        {
            return !string.IsNullOrWhiteSpace(textoTraducido) &&
                   !string.Equals(textoTraducido, clave, StringComparison.OrdinalIgnoreCase);
        }
    }
}