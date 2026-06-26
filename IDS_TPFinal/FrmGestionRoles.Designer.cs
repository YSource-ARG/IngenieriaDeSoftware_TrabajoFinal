namespace IDS_TPFinal
{
    partial class FrmGestionRoles
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label _lblTitulo;
        private System.Windows.Forms.Label _lblAyuda;
        private System.Windows.Forms.Label _lblRolExistente;
        private System.Windows.Forms.ComboBox _cboRoles;
        private System.Windows.Forms.Button _btnNuevoRol;
        private System.Windows.Forms.Label _lblNombre;
        private System.Windows.Forms.TextBox _txtNombre;
        private System.Windows.Forms.Label _lblCodigo;
        private System.Windows.Forms.TextBox _txtCodigo;
        private System.Windows.Forms.Label _lblDescripcion;
        private System.Windows.Forms.TextBox _txtDescripcion;
        private System.Windows.Forms.CheckBox _chkActivo;
        private System.Windows.Forms.TreeView _treeComponentes;
        private System.Windows.Forms.Button _btnGuardar;
        private System.Windows.Forms.Button _btnCancelarNuevo;
        private System.Windows.Forms.Button _btnCerrar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this._lblTitulo = new System.Windows.Forms.Label();
            this._lblAyuda = new System.Windows.Forms.Label();
            this._lblRolExistente = new System.Windows.Forms.Label();
            this._cboRoles = new System.Windows.Forms.ComboBox();
            this._btnNuevoRol = new System.Windows.Forms.Button();
            this._lblNombre = new System.Windows.Forms.Label();
            this._txtNombre = new System.Windows.Forms.TextBox();
            this._lblCodigo = new System.Windows.Forms.Label();
            this._txtCodigo = new System.Windows.Forms.TextBox();
            this._lblDescripcion = new System.Windows.Forms.Label();
            this._txtDescripcion = new System.Windows.Forms.TextBox();
            this._chkActivo = new System.Windows.Forms.CheckBox();
            this._treeComponentes = new System.Windows.Forms.TreeView();
            this._btnGuardar = new System.Windows.Forms.Button();
            this._btnCancelarNuevo = new System.Windows.Forms.Button();
            this._btnCerrar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _lblTitulo
            // 
            this._lblTitulo.AutoSize = true;
            this._lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this._lblTitulo.Location = new System.Drawing.Point(28, 20);
            this._lblTitulo.Name = "_lblTitulo";
            this._lblTitulo.Size = new System.Drawing.Size(199, 32);
            this._lblTitulo.TabIndex = 0;
            this._lblTitulo.Tag = "Roles.Gestion.Titulo";
            this._lblTitulo.Text = "Gestión de roles";
            // 
            // _lblAyuda
            // 
            this._lblAyuda.AutoSize = true;
            this._lblAyuda.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._lblAyuda.Location = new System.Drawing.Point(30, 58);
            this._lblAyuda.MaximumSize = new System.Drawing.Size(880, 0);
            this._lblAyuda.Name = "_lblAyuda";
            this._lblAyuda.Size = new System.Drawing.Size(775, 15);
            this._lblAyuda.TabIndex = 1;
            this._lblAyuda.Tag = "Roles.Gestion.Ayuda";
            this._lblAyuda.Text = "Seleccione un rol existente para administrar su composición o cree uno nuevo carg" +
    "ando sus atributos y marcando permisos o subroles en el árbol.";
            // 
            // _lblRolExistente
            // 
            this._lblRolExistente.AutoSize = true;
            this._lblRolExistente.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._lblRolExistente.Location = new System.Drawing.Point(30, 112);
            this._lblRolExistente.Name = "_lblRolExistente";
            this._lblRolExistente.Size = new System.Drawing.Size(95, 19);
            this._lblRolExistente.TabIndex = 2;
            this._lblRolExistente.Tag = "Roles.Gestion.RolExistente";
            this._lblRolExistente.Text = "Rol existente";
            // 
            // _cboRoles
            // 
            this._cboRoles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cboRoles.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._cboRoles.FormattingEnabled = true;
            this._cboRoles.Location = new System.Drawing.Point(30, 136);
            this._cboRoles.Name = "_cboRoles";
            this._cboRoles.Size = new System.Drawing.Size(330, 25);
            this._cboRoles.TabIndex = 3;
            this._cboRoles.SelectedIndexChanged += new System.EventHandler(this.cboRoles_SelectedIndexChanged);
            // 
            // _btnNuevoRol
            // 
            this._btnNuevoRol.Location = new System.Drawing.Point(380, 130);
            this._btnNuevoRol.Name = "_btnNuevoRol";
            this._btnNuevoRol.Size = new System.Drawing.Size(140, 36);
            this._btnNuevoRol.TabIndex = 4;
            this._btnNuevoRol.Tag = "Roles.Gestion.NuevoRol";
            this._btnNuevoRol.Text = "Nuevo rol";
            this._btnNuevoRol.UseVisualStyleBackColor = true;
            this._btnNuevoRol.Click += new System.EventHandler(this.btnNuevoRol_Click);
            // 
            // _lblNombre
            // 
            this._lblNombre.AutoSize = true;
            this._lblNombre.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._lblNombre.Location = new System.Drawing.Point(30, 190);
            this._lblNombre.Name = "_lblNombre";
            this._lblNombre.Size = new System.Drawing.Size(65, 19);
            this._lblNombre.TabIndex = 5;
            this._lblNombre.Tag = "Roles.Gestion.Nombre";
            this._lblNombre.Text = "Nombre";
            // 
            // _txtNombre
            // 
            this._txtNombre.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._txtNombre.Location = new System.Drawing.Point(30, 214);
            this._txtNombre.Name = "_txtNombre";
            this._txtNombre.Size = new System.Drawing.Size(300, 25);
            this._txtNombre.TabIndex = 6;
            // 
            // _lblCodigo
            // 
            this._lblCodigo.AutoSize = true;
            this._lblCodigo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._lblCodigo.Location = new System.Drawing.Point(360, 190);
            this._lblCodigo.Name = "_lblCodigo";
            this._lblCodigo.Size = new System.Drawing.Size(58, 19);
            this._lblCodigo.TabIndex = 7;
            this._lblCodigo.Tag = "Roles.Gestion.Codigo";
            this._lblCodigo.Text = "Código";
            // 
            // _txtCodigo
            // 
            this._txtCodigo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._txtCodigo.Location = new System.Drawing.Point(360, 214);
            this._txtCodigo.Name = "_txtCodigo";
            this._txtCodigo.Size = new System.Drawing.Size(250, 25);
            this._txtCodigo.TabIndex = 8;
            // 
            // _lblDescripcion
            // 
            this._lblDescripcion.AutoSize = true;
            this._lblDescripcion.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._lblDescripcion.Location = new System.Drawing.Point(30, 255);
            this._lblDescripcion.Name = "_lblDescripcion";
            this._lblDescripcion.Size = new System.Drawing.Size(87, 19);
            this._lblDescripcion.TabIndex = 10;
            this._lblDescripcion.Tag = "Roles.Gestion.Descripcion";
            this._lblDescripcion.Text = "Descripción";
            // 
            // _txtDescripcion
            // 
            this._txtDescripcion.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._txtDescripcion.Location = new System.Drawing.Point(30, 279);
            this._txtDescripcion.Multiline = true;
            this._txtDescripcion.Name = "_txtDescripcion";
            this._txtDescripcion.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._txtDescripcion.Size = new System.Drawing.Size(880, 60);
            this._txtDescripcion.TabIndex = 11;
            // 
            // _chkActivo
            // 
            this._chkActivo.AutoSize = true;
            this._chkActivo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._chkActivo.Location = new System.Drawing.Point(640, 217);
            this._chkActivo.Name = "_chkActivo";
            this._chkActivo.Size = new System.Drawing.Size(66, 23);
            this._chkActivo.TabIndex = 9;
            this._chkActivo.Tag = "Roles.Gestion.Activo";
            this._chkActivo.Text = "Activo";
            this._chkActivo.UseVisualStyleBackColor = true;
            // 
            // _treeComponentes
            // 
            this._treeComponentes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._treeComponentes.CheckBoxes = true;
            this._treeComponentes.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._treeComponentes.HideSelection = false;
            this._treeComponentes.Location = new System.Drawing.Point(30, 370);
            this._treeComponentes.Name = "_treeComponentes";
            this._treeComponentes.Size = new System.Drawing.Size(880, 270);
            this._treeComponentes.TabIndex = 12;
            this._treeComponentes.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeComponentes_AfterCheck);
            // 
            // _btnGuardar
            // 
            this._btnGuardar.Location = new System.Drawing.Point(430, 660);
            this._btnGuardar.Name = "_btnGuardar";
            this._btnGuardar.Size = new System.Drawing.Size(150, 40);
            this._btnGuardar.TabIndex = 13;
            this._btnGuardar.Text = "Guardar composición";
            this._btnGuardar.UseVisualStyleBackColor = true;
            this._btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // _btnCancelarNuevo
            // 
            this._btnCancelarNuevo.Location = new System.Drawing.Point(600, 660);
            this._btnCancelarNuevo.Name = "_btnCancelarNuevo";
            this._btnCancelarNuevo.Size = new System.Drawing.Size(150, 40);
            this._btnCancelarNuevo.TabIndex = 14;
            this._btnCancelarNuevo.Tag = "Roles.Gestion.Cancelar";
            this._btnCancelarNuevo.Text = "Cancelar";
            this._btnCancelarNuevo.UseVisualStyleBackColor = true;
            this._btnCancelarNuevo.Visible = false;
            this._btnCancelarNuevo.Click += new System.EventHandler(this.btnCancelarNuevo_Click);
            // 
            // _btnCerrar
            // 
            this._btnCerrar.Location = new System.Drawing.Point(760, 660);
            this._btnCerrar.Name = "_btnCerrar";
            this._btnCerrar.Size = new System.Drawing.Size(150, 40);
            this._btnCerrar.TabIndex = 15;
            this._btnCerrar.Tag = "Roles.Gestion.Cerrar";
            this._btnCerrar.Text = "Cerrar";
            this._btnCerrar.UseVisualStyleBackColor = true;
            this._btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FrmGestionRoles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 720);
            this.Controls.Add(this._btnCerrar);
            this.Controls.Add(this._btnCancelarNuevo);
            this.Controls.Add(this._btnGuardar);
            this.Controls.Add(this._treeComponentes);
            this.Controls.Add(this._txtDescripcion);
            this.Controls.Add(this._lblDescripcion);
            this.Controls.Add(this._chkActivo);
            this.Controls.Add(this._txtCodigo);
            this.Controls.Add(this._lblCodigo);
            this.Controls.Add(this._txtNombre);
            this.Controls.Add(this._lblNombre);
            this.Controls.Add(this._btnNuevoRol);
            this.Controls.Add(this._cboRoles);
            this.Controls.Add(this._lblRolExistente);
            this.Controls.Add(this._lblAyuda);
            this.Controls.Add(this._lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmGestionRoles";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "Roles.Gestion.TituloVentana";
            this.Text = "Gestión de roles";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
