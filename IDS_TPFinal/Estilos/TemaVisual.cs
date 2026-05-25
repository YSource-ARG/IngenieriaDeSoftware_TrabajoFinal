using System.Drawing;
using System.Windows.Forms;

namespace UI.Estilos
{
    public static class TemaVisual
    {
        public static readonly Color FondoPrincipal = Color.FromArgb(22, 24, 29);
        public static readonly Color FondoPanel = Color.FromArgb(34, 37, 44);
        public static readonly Color FondoInput = Color.FromArgb(26, 29, 35);
        public static readonly Color AzulPrincipal = Color.FromArgb(58, 110, 220);
        public static readonly Color AzulHover = Color.FromArgb(47, 96, 204);
        public static readonly Color TextoPrincipal = Color.FromArgb(245, 247, 250);
        public static readonly Color TextoSecundario = Color.FromArgb(186, 191, 199);
        public static readonly Color TextoMuted = Color.FromArgb(130, 136, 145);
        public static readonly Color BordeSuave = Color.FromArgb(68, 73, 82);

        public static void AplicarFormularioOscuro(Form formulario)
        {
            formulario.BackColor = FondoPrincipal;
            formulario.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        }

        public static void AplicarPanelPrincipal(Panel panel)
        {
            panel.BackColor = FondoPanel;
        }

        public static void AplicarTextBox(TextBox textBox)
        {
            textBox.BackColor = FondoInput;
            textBox.ForeColor = TextoPrincipal;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }

        public static void AplicarBotonPrincipal(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.BackColor = AzulPrincipal;
            boton.ForeColor = Color.White;
            boton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            boton.Cursor = Cursors.Hand;

            boton.MouseEnter += (s, e) =>
            {
                boton.BackColor = AzulHover;
            };

            boton.MouseLeave += (s, e) =>
            {
                boton.BackColor = AzulPrincipal;
            };
        }

        public static void AplicarBotonSecundario(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 1;
            boton.FlatAppearance.BorderColor = BordeSuave;
            boton.BackColor = FondoPanel;
            boton.ForeColor = TextoPrincipal;
            boton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            boton.Cursor = Cursors.Hand;

            boton.MouseEnter += (s, e) =>
            {
                boton.BackColor = Color.FromArgb(42, 47, 56);
            };

            boton.MouseLeave += (s, e) =>
            {
                boton.BackColor = FondoPanel;
            };
        }

        public static void AplicarTitulo(Label label)
        {
            label.ForeColor = TextoPrincipal;
            label.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            label.BackColor = Color.Transparent;
        }

        public static void AplicarTextoSecundario(Label label)
        {
            label.ForeColor = TextoSecundario;
            label.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            label.BackColor = Color.Transparent;
        }

        public static void AplicarDataGridView(DataGridView grilla)
        {
            grilla.BackgroundColor = FondoInput;
            grilla.BorderStyle = BorderStyle.FixedSingle;
            grilla.GridColor = BordeSuave;

            grilla.EnableHeadersVisualStyles = false;

            grilla.ColumnHeadersDefaultCellStyle.BackColor = FondoPanel;
            grilla.ColumnHeadersDefaultCellStyle.ForeColor = TextoPrincipal;
            grilla.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grilla.ColumnHeadersDefaultCellStyle.SelectionBackColor = FondoPanel;
            grilla.ColumnHeadersDefaultCellStyle.SelectionForeColor = TextoPrincipal;

            grilla.DefaultCellStyle.BackColor = FondoInput;
            grilla.DefaultCellStyle.ForeColor = TextoPrincipal;
            grilla.DefaultCellStyle.SelectionBackColor = AzulPrincipal;
            grilla.DefaultCellStyle.SelectionForeColor = Color.White;
            grilla.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            grilla.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(30, 34, 40);
            grilla.AlternatingRowsDefaultCellStyle.ForeColor = TextoPrincipal;
            grilla.AlternatingRowsDefaultCellStyle.SelectionBackColor = AzulPrincipal;
            grilla.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            grilla.RowHeadersVisible = false;
            grilla.AllowUserToAddRows = false;
            grilla.AllowUserToDeleteRows = false;
            grilla.ReadOnly = true;
            grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grilla.MultiSelect = false;
            grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
}