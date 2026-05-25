namespace IDS_TPFinal
{
    partial class FrmGestionUsuarios
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
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
        /// Método necesario para admitir el Diseñador. No modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblIcono = new System.Windows.Forms.Label();
            this.panelContenido = new System.Windows.Forms.Panel();
            this.dgvUsuarios = new System.Windows.Forms.DataGridView();
            this.panelLineaDerecha = new System.Windows.Forms.Panel();
            this.panelLineaIzquierda = new System.Windows.Forms.Panel();
            this.lblAccionesRapidas = new System.Windows.Forms.Label();
            this.panelEdicion = new System.Windows.Forms.Panel();
            this.txtFechaUltimoAcceso = new System.Windows.Forms.TextBox();
            this.txtFechaCreacion = new System.Windows.Forms.TextBox();
            this.lblFechaUltimoAcceso = new System.Windows.Forms.Label();
            this.lblFechaCreacion = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.cmbEstado = new System.Windows.Forms.ComboBox();
            this.lblEstado = new System.Windows.Forms.Label();
            this.lblNombreCompleto = new System.Windows.Forms.Label();
            this.lblNombreUsuario = new System.Windows.Forms.Label();
            this.lblId = new System.Windows.Forms.Label();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtNombreCompleto = new System.Windows.Forms.TextBox();
            this.txtNombreUsuario = new System.Windows.Forms.TextBox();
            this.txtId = new System.Windows.Forms.TextBox();
            this.lblDatosUsuario = new System.Windows.Forms.Label();
            this.btnRestablecerPassword = new System.Windows.Forms.Button();
            this.btnInhabilitarReactivar = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.panelHeader.SuspendLayout();
            this.panelContenido.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).BeginInit();
            this.panelEdicion.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.Controls.Add(this.btnCerrar);
            this.panelHeader.Controls.Add(this.lblSubtitulo);
            this.panelHeader.Controls.Add(this.lblTitulo);
            this.panelHeader.Controls.Add(this.lblIcono);
            this.panelHeader.Location = new System.Drawing.Point(18, 18);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1373, 122);
            this.panelHeader.TabIndex = 0;
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(1234, 39);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(112, 44);
            this.btnCerrar.TabIndex = 3;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Location = new System.Drawing.Point(143, 70);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(187, 13);
            this.lblSubtitulo.TabIndex = 2;
            this.lblSubtitulo.Text = "Administración de usuarios del sistema";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Location = new System.Drawing.Point(140, 32);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(102, 13);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "Gestión de Usuarios";
            // 
            // lblIcono
            // 
            this.lblIcono.Location = new System.Drawing.Point(35, 28);
            this.lblIcono.Name = "lblIcono";
            this.lblIcono.Size = new System.Drawing.Size(74, 65);
            this.lblIcono.TabIndex = 0;
            this.lblIcono.Text = "👥";
            this.lblIcono.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelContenido
            // 
            this.panelContenido.Controls.Add(this.dgvUsuarios);
            this.panelContenido.Controls.Add(this.panelLineaDerecha);
            this.panelContenido.Controls.Add(this.panelLineaIzquierda);
            this.panelContenido.Controls.Add(this.lblAccionesRapidas);
            this.panelContenido.Controls.Add(this.panelEdicion);
            this.panelContenido.Controls.Add(this.btnRestablecerPassword);
            this.panelContenido.Controls.Add(this.btnInhabilitarReactivar);
            this.panelContenido.Controls.Add(this.btnEditar);
            this.panelContenido.Controls.Add(this.btnNuevo);
            this.panelContenido.Location = new System.Drawing.Point(18, 154);
            this.panelContenido.Name = "panelContenido";
            this.panelContenido.Size = new System.Drawing.Size(1373, 486);
            this.panelContenido.TabIndex = 1;
            // 
            // dgvUsuarios
            // 
            this.dgvUsuarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsuarios.Location = new System.Drawing.Point(38, 65);
            this.dgvUsuarios.Name = "dgvUsuarios";
            this.dgvUsuarios.Size = new System.Drawing.Size(585, 202);
            this.dgvUsuarios.TabIndex = 8;
            // 
            // panelLineaDerecha
            // 
            this.panelLineaDerecha.Location = new System.Drawing.Point(636, 311);
            this.panelLineaDerecha.Name = "panelLineaDerecha";
            this.panelLineaDerecha.Size = new System.Drawing.Size(360, 1);
            this.panelLineaDerecha.TabIndex = 7;
            // 
            // panelLineaIzquierda
            // 
            this.panelLineaIzquierda.Location = new System.Drawing.Point(88, 311);
            this.panelLineaIzquierda.Name = "panelLineaIzquierda";
            this.panelLineaIzquierda.Size = new System.Drawing.Size(360, 1);
            this.panelLineaIzquierda.TabIndex = 6;
            // 
            // lblAccionesRapidas
            // 
            this.lblAccionesRapidas.AutoSize = true;
            this.lblAccionesRapidas.Location = new System.Drawing.Point(584, 334);
            this.lblAccionesRapidas.Name = "lblAccionesRapidas";
            this.lblAccionesRapidas.Size = new System.Drawing.Size(88, 13);
            this.lblAccionesRapidas.TabIndex = 5;
            this.lblAccionesRapidas.Text = "Acciones rápidas";
            // 
            // panelEdicion
            // 
            this.panelEdicion.Controls.Add(this.txtFechaUltimoAcceso);
            this.panelEdicion.Controls.Add(this.txtFechaCreacion);
            this.panelEdicion.Controls.Add(this.lblFechaUltimoAcceso);
            this.panelEdicion.Controls.Add(this.lblFechaCreacion);
            this.panelEdicion.Controls.Add(this.lblPassword);
            this.panelEdicion.Controls.Add(this.cmbEstado);
            this.panelEdicion.Controls.Add(this.lblEstado);
            this.panelEdicion.Controls.Add(this.lblNombreCompleto);
            this.panelEdicion.Controls.Add(this.lblNombreUsuario);
            this.panelEdicion.Controls.Add(this.lblId);
            this.panelEdicion.Controls.Add(this.btnCancelar);
            this.panelEdicion.Controls.Add(this.btnGuardar);
            this.panelEdicion.Controls.Add(this.txtPassword);
            this.panelEdicion.Controls.Add(this.txtNombreCompleto);
            this.panelEdicion.Controls.Add(this.txtNombreUsuario);
            this.panelEdicion.Controls.Add(this.txtId);
            this.panelEdicion.Controls.Add(this.lblDatosUsuario);
            this.panelEdicion.Location = new System.Drawing.Point(677, 17);
            this.panelEdicion.Name = "panelEdicion";
            this.panelEdicion.Size = new System.Drawing.Size(669, 275);
            this.panelEdicion.TabIndex = 0;
            // 
            // txtFechaUltimoAcceso
            // 
            this.txtFechaUltimoAcceso.Location = new System.Drawing.Point(453, 146);
            this.txtFechaUltimoAcceso.Name = "txtFechaUltimoAcceso";
            this.txtFechaUltimoAcceso.Size = new System.Drawing.Size(180, 20);
            this.txtFechaUltimoAcceso.TabIndex = 16;
            // 
            // txtFechaCreacion
            // 
            this.txtFechaCreacion.Location = new System.Drawing.Point(453, 106);
            this.txtFechaCreacion.Name = "txtFechaCreacion";
            this.txtFechaCreacion.Size = new System.Drawing.Size(180, 20);
            this.txtFechaCreacion.TabIndex = 15;
            // 
            // lblFechaUltimoAcceso
            // 
            this.lblFechaUltimoAcceso.AutoSize = true;
            this.lblFechaUltimoAcceso.Location = new System.Drawing.Point(334, 146);
            this.lblFechaUltimoAcceso.Name = "lblFechaUltimoAcceso";
            this.lblFechaUltimoAcceso.Size = new System.Drawing.Size(74, 13);
            this.lblFechaUltimoAcceso.TabIndex = 14;
            this.lblFechaUltimoAcceso.Text = "Último acceso";
            // 
            // lblFechaCreacion
            // 
            this.lblFechaCreacion.AutoSize = true;
            this.lblFechaCreacion.Location = new System.Drawing.Point(334, 106);
            this.lblFechaCreacion.Name = "lblFechaCreacion";
            this.lblFechaCreacion.Size = new System.Drawing.Size(81, 13);
            this.lblFechaCreacion.TabIndex = 13;
            this.lblFechaCreacion.Text = "Fecha creación";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(334, 66);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(95, 13);
            this.lblPassword.TabIndex = 12;
            this.lblPassword.Text = "Nueva contraseña";
            // 
            // cmbEstado
            // 
            this.cmbEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEstado.FormattingEnabled = true;
            this.cmbEstado.Location = new System.Drawing.Point(136, 170);
            this.cmbEstado.Name = "cmbEstado";
            this.cmbEstado.Size = new System.Drawing.Size(180, 21);
            this.cmbEstado.TabIndex = 8;
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(27, 173);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(40, 13);
            this.lblEstado.TabIndex = 7;
            this.lblEstado.Text = "Estado";
            // 
            // lblNombreCompleto
            // 
            this.lblNombreCompleto.AutoSize = true;
            this.lblNombreCompleto.Location = new System.Drawing.Point(27, 136);
            this.lblNombreCompleto.Name = "lblNombreCompleto";
            this.lblNombreCompleto.Size = new System.Drawing.Size(90, 13);
            this.lblNombreCompleto.TabIndex = 5;
            this.lblNombreCompleto.Text = "Nombre completo";
            // 
            // lblNombreUsuario
            // 
            this.lblNombreUsuario.AutoSize = true;
            this.lblNombreUsuario.Location = new System.Drawing.Point(27, 99);
            this.lblNombreUsuario.Name = "lblNombreUsuario";
            this.lblNombreUsuario.Size = new System.Drawing.Size(96, 13);
            this.lblNombreUsuario.TabIndex = 3;
            this.lblNombreUsuario.Text = "Nombre de usuario";
            // 
            // lblId
            // 
            this.lblId.AutoSize = true;
            this.lblId.Location = new System.Drawing.Point(27, 62);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(16, 13);
            this.lblId.TabIndex = 1;
            this.lblId.Text = "Id";
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(337, 212);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(130, 38);
            this.btnCancelar.TabIndex = 11;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(163, 212);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(130, 38);
            this.btnGuardar.TabIndex = 10;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(453, 63);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(180, 20);
            this.txtPassword.TabIndex = 9;
            // 
            // txtNombreCompleto
            // 
            this.txtNombreCompleto.Location = new System.Drawing.Point(136, 133);
            this.txtNombreCompleto.Name = "txtNombreCompleto";
            this.txtNombreCompleto.Size = new System.Drawing.Size(180, 20);
            this.txtNombreCompleto.TabIndex = 6;
            // 
            // txtNombreUsuario
            // 
            this.txtNombreUsuario.Location = new System.Drawing.Point(136, 96);
            this.txtNombreUsuario.Name = "txtNombreUsuario";
            this.txtNombreUsuario.Size = new System.Drawing.Size(180, 20);
            this.txtNombreUsuario.TabIndex = 4;
            // 
            // txtId
            // 
            this.txtId.Location = new System.Drawing.Point(136, 59);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(180, 20);
            this.txtId.TabIndex = 2;
            // 
            // lblDatosUsuario
            // 
            this.lblDatosUsuario.AutoSize = true;
            this.lblDatosUsuario.Location = new System.Drawing.Point(27, 10);
            this.lblDatosUsuario.Name = "lblDatosUsuario";
            this.lblDatosUsuario.Size = new System.Drawing.Size(94, 13);
            this.lblDatosUsuario.TabIndex = 0;
            this.lblDatosUsuario.Text = "Edición de usuario";
            // 
            // btnRestablecerPassword
            // 
            this.btnRestablecerPassword.Location = new System.Drawing.Point(891, 376);
            this.btnRestablecerPassword.Name = "btnRestablecerPassword";
            this.btnRestablecerPassword.Size = new System.Drawing.Size(180, 84);
            this.btnRestablecerPassword.TabIndex = 4;
            this.btnRestablecerPassword.Text = "Restablecer contraseña";
            this.btnRestablecerPassword.UseVisualStyleBackColor = true;
            // 
            // btnInhabilitarReactivar
            // 
            this.btnInhabilitarReactivar.Location = new System.Drawing.Point(655, 376);
            this.btnInhabilitarReactivar.Name = "btnInhabilitarReactivar";
            this.btnInhabilitarReactivar.Size = new System.Drawing.Size(180, 84);
            this.btnInhabilitarReactivar.TabIndex = 3;
            this.btnInhabilitarReactivar.Text = "Inhabilitar/Reactivar";
            this.btnInhabilitarReactivar.UseVisualStyleBackColor = true;
            // 
            // btnEditar
            // 
            this.btnEditar.Location = new System.Drawing.Point(467, 376);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(130, 84);
            this.btnEditar.TabIndex = 2;
            this.btnEditar.Text = "Editar";
            this.btnEditar.UseVisualStyleBackColor = true;
            // 
            // btnNuevo
            // 
            this.btnNuevo.Location = new System.Drawing.Point(282, 376);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(130, 84);
            this.btnNuevo.TabIndex = 1;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.UseVisualStyleBackColor = true;
            // 
            // FrmGestionUsuarios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1403, 660);
            this.Controls.Add(this.panelContenido);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmGestionUsuarios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestión de Usuarios";
            this.Load += new System.EventHandler(this.FrmGestionUsuarios_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelContenido.ResumeLayout(false);
            this.panelContenido.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).EndInit();
            this.panelEdicion.ResumeLayout(false);
            this.panelEdicion.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Panel panelContenido;
        private System.Windows.Forms.Panel panelEdicion;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblIcono;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label lblDatosUsuario;
        private System.Windows.Forms.Button btnRestablecerPassword;
        private System.Windows.Forms.Button btnInhabilitarReactivar;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtNombreCompleto;
        private System.Windows.Forms.TextBox txtNombreUsuario;
        private System.Windows.Forms.ComboBox cmbEstado;
        private System.Windows.Forms.Label lblFechaUltimoAcceso;
        private System.Windows.Forms.Label lblFechaCreacion;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.Label lblNombreCompleto;
        private System.Windows.Forms.Label lblNombreUsuario;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.Label lblAccionesRapidas;
        private System.Windows.Forms.Panel panelLineaIzquierda;
        private System.Windows.Forms.Panel panelLineaDerecha;
        private System.Windows.Forms.TextBox txtFechaUltimoAcceso;
        private System.Windows.Forms.TextBox txtFechaCreacion;
        private System.Windows.Forms.DataGridView dgvUsuarios;
    }
}
