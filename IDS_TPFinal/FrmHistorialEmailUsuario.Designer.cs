namespace IDS_TPFinal
{
    partial class FrmHistorialEmailUsuario
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.dgvHistorialEmail = new System.Windows.Forms.DataGridView();
            this.btnRestaurarEmail = new System.Windows.Forms.Button();
            this.btnCerrar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialEmail)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Location = new System.Drawing.Point(55, 22);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(86, 13);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Historial de email";
            // 
            // dgvHistorialEmail
            // 
            this.dgvHistorialEmail.AllowUserToAddRows = false;
            this.dgvHistorialEmail.AllowUserToDeleteRows = false;
            this.dgvHistorialEmail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistorialEmail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorialEmail.Location = new System.Drawing.Point(58, 38);
            this.dgvHistorialEmail.MultiSelect = false;
            this.dgvHistorialEmail.Name = "dgvHistorialEmail";
            this.dgvHistorialEmail.ReadOnly = true;
            this.dgvHistorialEmail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistorialEmail.Size = new System.Drawing.Size(479, 324);
            this.dgvHistorialEmail.TabIndex = 1;
            // 
            // btnRestaurarEmail
            // 
            this.btnRestaurarEmail.Location = new System.Drawing.Point(88, 425);
            this.btnRestaurarEmail.Name = "btnRestaurarEmail";
            this.btnRestaurarEmail.Size = new System.Drawing.Size(171, 73);
            this.btnRestaurarEmail.TabIndex = 2;
            this.btnRestaurarEmail.Text = "Restaurar email seleccionado";
            this.btnRestaurarEmail.UseVisualStyleBackColor = true;
            this.btnRestaurarEmail.Click += new System.EventHandler(this.btnRestaurarEmail_Click);
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(324, 429);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(171, 69);
            this.btnCerrar.TabIndex = 3;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FrmHistorialEmailUsuario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 569);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.btnRestaurarEmail);
            this.Controls.Add(this.dgvHistorialEmail);
            this.Controls.Add(this.lblTitulo);
            this.Name = "FrmHistorialEmailUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmHistorialEmailUsuario";
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorialEmail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.DataGridView dgvHistorialEmail;
        private System.Windows.Forms.Button btnRestaurarEmail;
        private System.Windows.Forms.Button btnCerrar;
    }
}