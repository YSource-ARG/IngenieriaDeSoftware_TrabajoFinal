namespace UI
{
    partial class FrmLogin
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

        private void InitializeComponent()
        {
            this.panelLogin = new System.Windows.Forms.Panel();
            this.lblIdiomaLogin = new System.Windows.Forms.Label();
            this.cboIdiomaLogin = new System.Windows.Forms.ComboBox();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtNombreUsuario = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnIngresar = new System.Windows.Forms.Button();
            this.lblMensajeSeguridad = new System.Windows.Forms.Label();
            this.panelLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLogin
            // 
            this.panelLogin.Controls.Add(this.lblIdiomaLogin);
            this.panelLogin.Controls.Add(this.cboIdiomaLogin);
            this.panelLogin.Controls.Add(this.lblTitulo);
            this.panelLogin.Controls.Add(this.lblSubtitulo);
            this.panelLogin.Controls.Add(this.lblUsuario);
            this.panelLogin.Controls.Add(this.txtNombreUsuario);
            this.panelLogin.Controls.Add(this.lblPassword);
            this.panelLogin.Controls.Add(this.txtPassword);
            this.panelLogin.Controls.Add(this.btnIngresar);
            this.panelLogin.Controls.Add(this.lblMensajeSeguridad);
            this.panelLogin.Location = new System.Drawing.Point(165, 41);
            this.panelLogin.Margin = new System.Windows.Forms.Padding(2);
            this.panelLogin.Name = "panelLogin";
            this.panelLogin.Size = new System.Drawing.Size(282, 418);
            this.panelLogin.TabIndex = 0;
            // 
            // lblIdiomaLogin
            // 
            this.lblIdiomaLogin.AutoSize = true;
            this.lblIdiomaLogin.Location = new System.Drawing.Point(34, 116);
            this.lblIdiomaLogin.Name = "lblIdiomaLogin";
            this.lblIdiomaLogin.Size = new System.Drawing.Size(38, 13);
            this.lblIdiomaLogin.TabIndex = 1;
            this.lblIdiomaLogin.Tag = "Login.Idioma";
            this.lblIdiomaLogin.Text = "Idioma";
            // 
            // cboIdiomaLogin
            // 
            this.cboIdiomaLogin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboIdiomaLogin.FormattingEnabled = true;
            this.cboIdiomaLogin.Location = new System.Drawing.Point(37, 132);
            this.cboIdiomaLogin.Name = "cboIdiomaLogin";
            this.cboIdiomaLogin.Size = new System.Drawing.Size(211, 21);
            this.cboIdiomaLogin.TabIndex = 2;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Location = new System.Drawing.Point(35, 34);
            this.lblTitulo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(68, 13);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Tag = "Login.Titulo";
            this.lblTitulo.Text = "Iniciar sesión";
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.Location = new System.Drawing.Point(35, 62);
            this.lblSubtitulo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(214, 32);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Tag = "Login.Subtitulo";
            this.lblSubtitulo.Text = "Ingresá tus credenciales para acceder al sistema.";
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(34, 183);
            this.lblUsuario.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(43, 13);
            this.lblUsuario.TabIndex = 2;
            this.lblUsuario.Tag = "Login.Usuario";
            this.lblUsuario.Text = "Usuario";
            // 
            // txtNombreUsuario
            // 
            this.txtNombreUsuario.Location = new System.Drawing.Point(34, 201);
            this.txtNombreUsuario.Margin = new System.Windows.Forms.Padding(2);
            this.txtNombreUsuario.Name = "txtNombreUsuario";
            this.txtNombreUsuario.Size = new System.Drawing.Size(215, 20);
            this.txtNombreUsuario.TabIndex = 0;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(34, 248);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(61, 13);
            this.lblPassword.TabIndex = 4;
            this.lblPassword.Tag = "Login.Password";
            this.lblPassword.Text = "Contraseña";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(34, 266);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(2);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(215, 20);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // btnIngresar
            // 
            this.btnIngresar.Location = new System.Drawing.Point(34, 321);
            this.btnIngresar.Margin = new System.Windows.Forms.Padding(2);
            this.btnIngresar.Name = "btnIngresar";
            this.btnIngresar.Size = new System.Drawing.Size(214, 37);
            this.btnIngresar.TabIndex = 2;
            this.btnIngresar.Tag = "Login.Ingresar";
            this.btnIngresar.Text = "Ingresar";
            this.btnIngresar.UseVisualStyleBackColor = false;
            this.btnIngresar.Click += new System.EventHandler(this.btnIngresar_Click);
            // 
            // lblMensajeSeguridad
            // 
            this.lblMensajeSeguridad.Location = new System.Drawing.Point(34, 363);
            this.lblMensajeSeguridad.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMensajeSeguridad.Name = "lblMensajeSeguridad";
            this.lblMensajeSeguridad.Size = new System.Drawing.Size(214, 20);
            this.lblMensajeSeguridad.TabIndex = 7;
            this.lblMensajeSeguridad.Tag = "Login.MensajeSeguridad";
            this.lblMensajeSeguridad.Text = "Acceso protegido.";
            this.lblMensajeSeguridad.Visible = false;
            // 
            // FrmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 492);
            this.Controls.Add(this.panelLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "FrmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "Login.TituloVentana";
            this.Text = "Acceso al sistema";
            this.panelLogin.ResumeLayout(false);
            this.panelLogin.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLogin;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtNombreUsuario;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnIngresar;
        private System.Windows.Forms.Label lblMensajeSeguridad;
        private System.Windows.Forms.Label lblIdiomaLogin;
        private System.Windows.Forms.ComboBox cboIdiomaLogin;
    }
}