namespace UI
{
    partial class FrmPrincipal
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados deben desecharse; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblMarca = new System.Windows.Forms.Label();
            this.panelLateral = new System.Windows.Forms.Panel();
            this.btnSalir = new System.Windows.Forms.Button();
            this.btnCerrarSesion = new System.Windows.Forms.Button();
            this.lblSeccion = new System.Windows.Forms.Label();
            this.panelContenido = new System.Windows.Forms.Panel();
            this.panelTarjetaResumen = new System.Windows.Forms.Panel();
            this.lblResumenTexto = new System.Windows.Forms.Label();
            this.lblResumenTitulo = new System.Windows.Forms.Label();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.lblBienvenida = new System.Windows.Forms.Label();
            this.panelHeader.SuspendLayout();
            this.panelLateral.SuspendLayout();
            this.panelContenido.SuspendLayout();
            this.panelTarjetaResumen.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.Controls.Add(this.lblSubtitulo);
            this.panelHeader.Controls.Add(this.lblTitulo);
            this.panelHeader.Controls.Add(this.lblMarca);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(165, 0);
            this.panelHeader.Margin = new System.Windows.Forms.Padding(2);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(735, 77);
            this.panelHeader.TabIndex = 0;
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Location = new System.Drawing.Point(64, 46);
            this.lblSubtitulo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(140, 13);
            this.lblSubtitulo.TabIndex = 2;
            this.lblSubtitulo.Text = "Gestión principal del sistema";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Location = new System.Drawing.Point(63, 20);
            this.lblTitulo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(76, 13);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "Panel principal";
            // 
            // lblMarca
            // 
            this.lblMarca.AutoSize = true;
            this.lblMarca.Location = new System.Drawing.Point(18, 20);
            this.lblMarca.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMarca.Name = "lblMarca";
            this.lblMarca.Size = new System.Drawing.Size(22, 13);
            this.lblMarca.TabIndex = 0;
            this.lblMarca.Text = "BG";
            // 
            // panelLateral
            // 
            this.panelLateral.Controls.Add(this.btnSalir);
            this.panelLateral.Controls.Add(this.btnCerrarSesion);
            this.panelLateral.Controls.Add(this.lblSeccion);
            this.panelLateral.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLateral.Location = new System.Drawing.Point(0, 0);
            this.panelLateral.Margin = new System.Windows.Forms.Padding(2);
            this.panelLateral.Name = "panelLateral";
            this.panelLateral.Size = new System.Drawing.Size(165, 585);
            this.panelLateral.TabIndex = 1;
            // 
            // btnSalir
            // 
            this.btnSalir.Location = new System.Drawing.Point(15, 138);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(2);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(135, 34);
            this.btnSalir.TabIndex = 2;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnCerrarSesion
            // 
            this.btnCerrarSesion.Location = new System.Drawing.Point(15, 96);
            this.btnCerrarSesion.Margin = new System.Windows.Forms.Padding(2);
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Size = new System.Drawing.Size(135, 34);
            this.btnCerrarSesion.TabIndex = 1;
            this.btnCerrarSesion.Text = "Cerrar sesión";
            this.btnCerrarSesion.UseVisualStyleBackColor = true;
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);
            // 
            // lblSeccion
            // 
            this.lblSeccion.AutoSize = true;
            this.lblSeccion.Location = new System.Drawing.Point(15, 26);
            this.lblSeccion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSeccion.Name = "lblSeccion";
            this.lblSeccion.Size = new System.Drawing.Size(65, 13);
            this.lblSeccion.TabIndex = 0;
            this.lblSeccion.Text = "Navegación";
            // 
            // panelContenido
            // 
            this.panelContenido.Controls.Add(this.panelTarjetaResumen);
            this.panelContenido.Controls.Add(this.lblDescripcion);
            this.panelContenido.Controls.Add(this.lblBienvenida);
            this.panelContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenido.Location = new System.Drawing.Point(165, 77);
            this.panelContenido.Margin = new System.Windows.Forms.Padding(2);
            this.panelContenido.Name = "panelContenido";
            this.panelContenido.Padding = new System.Windows.Forms.Padding(26, 26, 26, 26);
            this.panelContenido.Size = new System.Drawing.Size(735, 508);
            this.panelContenido.TabIndex = 2;
            // 
            // panelTarjetaResumen
            // 
            this.panelTarjetaResumen.Controls.Add(this.lblResumenTexto);
            this.panelTarjetaResumen.Controls.Add(this.lblResumenTitulo);
            this.panelTarjetaResumen.Location = new System.Drawing.Point(29, 102);
            this.panelTarjetaResumen.Margin = new System.Windows.Forms.Padding(2);
            this.panelTarjetaResumen.Name = "panelTarjetaResumen";
            this.panelTarjetaResumen.Size = new System.Drawing.Size(322, 122);
            this.panelTarjetaResumen.TabIndex = 2;
            // 
            // lblResumenTexto
            // 
            this.lblResumenTexto.Location = new System.Drawing.Point(16, 44);
            this.lblResumenTexto.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblResumenTexto.Name = "lblResumenTexto";
            this.lblResumenTexto.Size = new System.Drawing.Size(291, 60);
            this.lblResumenTexto.TabIndex = 1;
            this.lblResumenTexto.Text = "Panel para acceder a los diferentes modulos del sistema a medida que los vayamos " +
    "implementando.";
            this.lblResumenTexto.Click += new System.EventHandler(this.lblResumenTexto_Click);
            // 
            // lblResumenTitulo
            // 
            this.lblResumenTitulo.AutoSize = true;
            this.lblResumenTitulo.Location = new System.Drawing.Point(16, 16);
            this.lblResumenTitulo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblResumenTitulo.Name = "lblResumenTitulo";
            this.lblResumenTitulo.Size = new System.Drawing.Size(95, 13);
            this.lblResumenTitulo.TabIndex = 0;
            this.lblResumenTitulo.Text = "Estado del sistema";
            // 
            // lblDescripcion
            // 
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Location = new System.Drawing.Point(30, 62);
            this.lblDescripcion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(306, 13);
            this.lblDescripcion.TabIndex = 1;
            this.lblDescripcion.Text = "La sesión fue iniciada correctamente. Este es el panel principal.";
            // 
            // lblBienvenida
            // 
            this.lblBienvenida.AutoSize = true;
            this.lblBienvenida.Location = new System.Drawing.Point(28, 26);
            this.lblBienvenida.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBienvenida.Name = "lblBienvenida";
            this.lblBienvenida.Size = new System.Drawing.Size(109, 13);
            this.lblBienvenida.TabIndex = 0;
            this.lblBienvenida.Text = "Bienvenido al sistema";
            // 
            // FrmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 585);
            this.Controls.Add(this.panelContenido);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelLateral);
            this.IsMdiContainer = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FrmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmPrincipal";
            this.Load += new System.EventHandler(this.FrmPrincipal_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelLateral.ResumeLayout(false);
            this.panelLateral.PerformLayout();
            this.panelContenido.ResumeLayout(false);
            this.panelContenido.PerformLayout();
            this.panelTarjetaResumen.ResumeLayout(false);
            this.panelTarjetaResumen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblMarca;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Panel panelLateral;
        private System.Windows.Forms.Label lblSeccion;
        private System.Windows.Forms.Button btnCerrarSesion;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Panel panelContenido;
        private System.Windows.Forms.Label lblBienvenida;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.Panel panelTarjetaResumen;
        private System.Windows.Forms.Label lblResumenTitulo;
        private System.Windows.Forms.Label lblResumenTexto;
    }
}