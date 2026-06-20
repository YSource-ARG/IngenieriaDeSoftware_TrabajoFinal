namespace IDS_TPFinal
{
    partial class FrmConsultaBitacora
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

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.panelFiltros = new System.Windows.Forms.Panel();
            this.cmbAccion = new System.Windows.Forms.ComboBox();
            this.cmbUsuario = new System.Windows.Forms.ComboBox();
            this.lblAccion = new System.Windows.Forms.Label();
            this.lblTipo = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.cmbTipo = new System.Windows.Forms.ComboBox();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.dtpFechaHasta = new System.Windows.Forms.DateTimePicker();
            this.lblFechaHasta = new System.Windows.Forms.Label();
            this.dtpFechaDesde = new System.Windows.Forms.DateTimePicker();
            this.lblFechaDesde = new System.Windows.Forms.Label();
            this.cmbModulo = new System.Windows.Forms.ComboBox();
            this.lblModulo = new System.Windows.Forms.Label();
            this.dgvBitacora = new System.Windows.Forms.DataGridView();
            this.lblCantidadRegistros = new System.Windows.Forms.Label();
            this.panelFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBitacora)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Location = new System.Drawing.Point(24, 22);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(104, 13);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Tag = "Bitacora.Titulo";
            this.lblTitulo.Text = "Consulta de bitácora";
            // 
            // panelFiltros
            // 
            this.panelFiltros.Controls.Add(this.cmbAccion);
            this.panelFiltros.Controls.Add(this.cmbUsuario);
            this.panelFiltros.Controls.Add(this.lblAccion);
            this.panelFiltros.Controls.Add(this.lblTipo);
            this.panelFiltros.Controls.Add(this.lblUsuario);
            this.panelFiltros.Controls.Add(this.cmbTipo);
            this.panelFiltros.Controls.Add(this.btnCerrar);
            this.panelFiltros.Controls.Add(this.btnLimpiar);
            this.panelFiltros.Controls.Add(this.btnBuscar);
            this.panelFiltros.Controls.Add(this.dtpFechaHasta);
            this.panelFiltros.Controls.Add(this.lblFechaHasta);
            this.panelFiltros.Controls.Add(this.dtpFechaDesde);
            this.panelFiltros.Controls.Add(this.lblFechaDesde);
            this.panelFiltros.Controls.Add(this.cmbModulo);
            this.panelFiltros.Controls.Add(this.lblModulo);
            this.panelFiltros.Location = new System.Drawing.Point(24, 38);
            this.panelFiltros.Name = "panelFiltros";
            this.panelFiltros.Size = new System.Drawing.Size(1018, 140);
            this.panelFiltros.TabIndex = 1;
            // 
            // cmbAccion
            // 
            this.cmbAccion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccion.FormattingEnabled = true;
            this.cmbAccion.Items.AddRange(new object[] {
            "Todos",
            "INFO",
            "WARN",
            "ERROR"});
            this.cmbAccion.Location = new System.Drawing.Point(291, 94);
            this.cmbAccion.Name = "cmbAccion";
            this.cmbAccion.Size = new System.Drawing.Size(264, 21);
            this.cmbAccion.TabIndex = 16;
            // 
            // cmbUsuario
            // 
            this.cmbUsuario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUsuario.FormattingEnabled = true;
            this.cmbUsuario.Location = new System.Drawing.Point(24, 94);
            this.cmbUsuario.Name = "cmbUsuario";
            this.cmbUsuario.Size = new System.Drawing.Size(224, 21);
            this.cmbUsuario.TabIndex = 15;
            // 
            // lblAccion
            // 
            this.lblAccion.AutoSize = true;
            this.lblAccion.Location = new System.Drawing.Point(288, 79);
            this.lblAccion.Name = "lblAccion";
            this.lblAccion.Size = new System.Drawing.Size(40, 13);
            this.lblAccion.TabIndex = 14;
            this.lblAccion.Tag = "Bitacora.Accion";
            this.lblAccion.Text = "Acción";
            // 
            // lblTipo
            // 
            this.lblTipo.AutoSize = true;
            this.lblTipo.Location = new System.Drawing.Point(154, 20);
            this.lblTipo.Name = "lblTipo";
            this.lblTipo.Size = new System.Drawing.Size(28, 13);
            this.lblTipo.TabIndex = 10;
            this.lblTipo.Tag = "Bitacora.Tipo";
            this.lblTipo.Text = "Tipo";
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(19, 78);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(43, 13);
            this.lblUsuario.TabIndex = 11;
            this.lblUsuario.Tag = "Bitacora.Usuario";
            this.lblUsuario.Text = "Usuario";
            // 
            // cmbTipo
            // 
            this.cmbTipo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipo.FormattingEnabled = true;
            this.cmbTipo.Items.AddRange(new object[] {
            "Todos",
            "INFO",
            "WARN",
            "ERROR"});
            this.cmbTipo.Location = new System.Drawing.Point(157, 40);
            this.cmbTipo.Name = "cmbTipo";
            this.cmbTipo.Size = new System.Drawing.Size(91, 21);
            this.cmbTipo.TabIndex = 9;
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(872, 51);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(121, 40);
            this.btnCerrar.TabIndex = 8;
            this.btnCerrar.Tag = "Bitacora.Cerrar";
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(733, 51);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(121, 40);
            this.btnLimpiar.TabIndex = 7;
            this.btnLimpiar.Tag = "Bitacora.Limpiar";
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(594, 51);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(121, 40);
            this.btnBuscar.TabIndex = 6;
            this.btnBuscar.Tag = "Bitacora.Buscar";
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // dtpFechaHasta
            // 
            this.dtpFechaHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaHasta.Location = new System.Drawing.Point(427, 41);
            this.dtpFechaHasta.Name = "dtpFechaHasta";
            this.dtpFechaHasta.Size = new System.Drawing.Size(130, 20);
            this.dtpFechaHasta.TabIndex = 5;
            // 
            // lblFechaHasta
            // 
            this.lblFechaHasta.AutoSize = true;
            this.lblFechaHasta.Location = new System.Drawing.Point(424, 21);
            this.lblFechaHasta.Name = "lblFechaHasta";
            this.lblFechaHasta.Size = new System.Drawing.Size(66, 13);
            this.lblFechaHasta.TabIndex = 4;
            this.lblFechaHasta.Tag = "Bitacora.FechaHasta";
            this.lblFechaHasta.Text = "Fecha hasta";
            // 
            // dtpFechaDesde
            // 
            this.dtpFechaDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaDesde.Location = new System.Drawing.Point(291, 41);
            this.dtpFechaDesde.Name = "dtpFechaDesde";
            this.dtpFechaDesde.Size = new System.Drawing.Size(130, 20);
            this.dtpFechaDesde.TabIndex = 3;
            // 
            // lblFechaDesde
            // 
            this.lblFechaDesde.AutoSize = true;
            this.lblFechaDesde.Location = new System.Drawing.Point(288, 21);
            this.lblFechaDesde.Name = "lblFechaDesde";
            this.lblFechaDesde.Size = new System.Drawing.Size(69, 13);
            this.lblFechaDesde.TabIndex = 2;
            this.lblFechaDesde.Tag = "Bitacora.FechaDesde";
            this.lblFechaDesde.Text = "Fecha desde";
            // 
            // cmbModulo
            // 
            this.cmbModulo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModulo.FormattingEnabled = true;
            this.cmbModulo.Location = new System.Drawing.Point(24, 41);
            this.cmbModulo.Name = "cmbModulo";
            this.cmbModulo.Size = new System.Drawing.Size(127, 21);
            this.cmbModulo.TabIndex = 1;
            // 
            // lblModulo
            // 
            this.lblModulo.AutoSize = true;
            this.lblModulo.Location = new System.Drawing.Point(21, 22);
            this.lblModulo.Name = "lblModulo";
            this.lblModulo.Size = new System.Drawing.Size(42, 13);
            this.lblModulo.TabIndex = 0;
            this.lblModulo.Tag = "Bitacora.Modulo";
            this.lblModulo.Text = "Módulo";
            // 
            // dgvBitacora
            // 
            this.dgvBitacora.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBitacora.Location = new System.Drawing.Point(24, 210);
            this.dgvBitacora.Name = "dgvBitacora";
            this.dgvBitacora.Size = new System.Drawing.Size(1018, 469);
            this.dgvBitacora.TabIndex = 2;
            // 
            // lblCantidadRegistros
            // 
            this.lblCantidadRegistros.AutoSize = true;
            this.lblCantidadRegistros.Location = new System.Drawing.Point(24, 194);
            this.lblCantidadRegistros.Name = "lblCantidadRegistros";
            this.lblCantidadRegistros.Size = new System.Drawing.Size(125, 13);
            this.lblCantidadRegistros.TabIndex = 3;
            this.lblCantidadRegistros.Tag = "Bitacora.RegistrosEncontrados";
            this.lblCantidadRegistros.Text = "Registros encontrados: 0";
            // 
            // FrmConsultaBitacora
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1054, 717);
            this.Controls.Add(this.lblCantidadRegistros);
            this.Controls.Add(this.dgvBitacora);
            this.Controls.Add(this.panelFiltros);
            this.Controls.Add(this.lblTitulo);
            this.Name = "FrmConsultaBitacora";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "Bitacora.TituloVentana";
            this.Text = "Consulta de bitácora";
            this.Load += new System.EventHandler(this.FrmConsultaBitacora_Load);
            this.panelFiltros.ResumeLayout(false);
            this.panelFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBitacora)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel panelFiltros;
        private System.Windows.Forms.ComboBox cmbModulo;
        private System.Windows.Forms.Label lblModulo;
        private System.Windows.Forms.DateTimePicker dtpFechaDesde;
        private System.Windows.Forms.Label lblFechaDesde;
        private System.Windows.Forms.DateTimePicker dtpFechaHasta;
        private System.Windows.Forms.Label lblFechaHasta;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.DataGridView dgvBitacora;
        private System.Windows.Forms.Label lblCantidadRegistros;
        private System.Windows.Forms.Label lblTipo;
        private System.Windows.Forms.ComboBox cmbTipo;
        private System.Windows.Forms.Label lblAccion;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.ComboBox cmbAccion;
        private System.Windows.Forms.ComboBox cmbUsuario;
    }
}
