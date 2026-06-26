namespace IDS_TPFinal
{
    partial class FrmAsignacionPermisosUsuario
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label _lblTitulo;
        private System.Windows.Forms.Label _lblUsuario;
        private System.Windows.Forms.Label _lblAyuda;
        private System.Windows.Forms.TreeView _treeComponentes;
        private System.Windows.Forms.Button _btnGuardar;
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
            this._lblUsuario = new System.Windows.Forms.Label();
            this._lblAyuda = new System.Windows.Forms.Label();
            this._treeComponentes = new System.Windows.Forms.TreeView();
            this._btnGuardar = new System.Windows.Forms.Button();
            this._btnCerrar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _lblTitulo
            // 
            this._lblTitulo.AutoSize = true;
            this._lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this._lblTitulo.Location = new System.Drawing.Point(25, 20);
            this._lblTitulo.Name = "_lblTitulo";
            this._lblTitulo.Size = new System.Drawing.Size(263, 32);
            this._lblTitulo.TabIndex = 0;
            this._lblTitulo.Tag = "Permisos.Asignacion.Titulo";
            this._lblTitulo.Text = "Asignación de permisos";
            // 
            // _lblUsuario
            // 
            this._lblUsuario.AutoSize = true;
            this._lblUsuario.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._lblUsuario.Location = new System.Drawing.Point(28, 62);
            this._lblUsuario.Name = "_lblUsuario";
            this._lblUsuario.Size = new System.Drawing.Size(59, 19);
            this._lblUsuario.TabIndex = 1;
            this._lblUsuario.Text = "Usuario:";
            // 
            // _lblAyuda
            // 
            this._lblAyuda.AutoSize = true;
            this._lblAyuda.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._lblAyuda.Location = new System.Drawing.Point(28, 86);
            this._lblAyuda.MaximumSize = new System.Drawing.Size(690, 0);
            this._lblAyuda.Name = "_lblAyuda";
            this._lblAyuda.Size = new System.Drawing.Size(664, 15);
            this._lblAyuda.TabIndex = 2;
            this._lblAyuda.Tag = "Permisos.Asignacion.Ayuda";
            this._lblAyuda.Text = "Al marcar un rol, sus hijos se marcan visualmente en forma recursiva dentro del árbol Composite.";
            // 
            // _treeComponentes
            // 
            this._treeComponentes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._treeComponentes.CheckBoxes = true;
            this._treeComponentes.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._treeComponentes.HideSelection = false;
            this._treeComponentes.Location = new System.Drawing.Point(30, 120);
            this._treeComponentes.Name = "_treeComponentes";
            this._treeComponentes.Size = new System.Drawing.Size(690, 360);
            this._treeComponentes.TabIndex = 3;
            this._treeComponentes.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeComponentes_AfterCheck);
            // 
            // _btnGuardar
            // 
            this._btnGuardar.Location = new System.Drawing.Point(420, 500);
            this._btnGuardar.Name = "_btnGuardar";
            this._btnGuardar.Size = new System.Drawing.Size(140, 40);
            this._btnGuardar.TabIndex = 4;
            this._btnGuardar.Tag = "General.Guardar";
            this._btnGuardar.Text = "Guardar";
            this._btnGuardar.UseVisualStyleBackColor = true;
            this._btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // _btnCerrar
            // 
            this._btnCerrar.Location = new System.Drawing.Point(580, 500);
            this._btnCerrar.Name = "_btnCerrar";
            this._btnCerrar.Size = new System.Drawing.Size(140, 40);
            this._btnCerrar.TabIndex = 5;
            this._btnCerrar.Tag = "General.Cerrar";
            this._btnCerrar.Text = "Cerrar";
            this._btnCerrar.UseVisualStyleBackColor = true;
            this._btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FrmAsignacionPermisosUsuario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 560);
            this.Controls.Add(this._btnCerrar);
            this.Controls.Add(this._btnGuardar);
            this.Controls.Add(this._treeComponentes);
            this.Controls.Add(this._lblAyuda);
            this.Controls.Add(this._lblUsuario);
            this.Controls.Add(this._lblTitulo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAsignacionPermisosUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "Permisos.Asignacion.Titulo";
            this.Text = "Asignación de permisos";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
