namespace EtiquetaFORNew
{
    partial class FormPrincipal
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnDesigner;
        private System.Windows.Forms.Button btnImprimir;
        private System.Windows.Forms.Button btnSalvarTemplate;
        private System.Windows.Forms.Button btnCarregarTemplate;
        private System.Windows.Forms.GroupBox groupProduto;
        private System.Windows.Forms.Label lblNome;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.Label lblCodigo;
        private System.Windows.Forms.TextBox txtCodigo;
        private System.Windows.Forms.Label lblPreco;
        private System.Windows.Forms.TextBox txtPreco;
        private System.Windows.Forms.Label lblQtd;
        private System.Windows.Forms.NumericUpDown numQtd;
        private System.Windows.Forms.Button btnAdicionar;
        private System.Windows.Forms.DataGridView dgvProdutos;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSelecionar;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNome;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCodigo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPreco;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantidade;
        private System.Windows.Forms.DataGridViewButtonColumn colRemover;

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
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnDesigner = new System.Windows.Forms.Button();
            this.btnImprimir = new System.Windows.Forms.Button();
            this.btnSalvarTemplate = new System.Windows.Forms.Button();
            this.btnCarregarTemplate = new System.Windows.Forms.Button();
            this.groupProduto = new System.Windows.Forms.GroupBox();
            this.lblNome = new System.Windows.Forms.Label();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.lblCodigo = new System.Windows.Forms.Label();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.lblPreco = new System.Windows.Forms.Label();
            this.txtPreco = new System.Windows.Forms.TextBox();
            this.lblQtd = new System.Windows.Forms.Label();
            this.numQtd = new System.Windows.Forms.NumericUpDown();
            this.btnAdicionar = new System.Windows.Forms.Button();
            this.dgvProdutos = new System.Windows.Forms.DataGridView();
            this.colSelecionar = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colNome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCodigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPreco = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuantidade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRemover = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panelTop.SuspendLayout();
            this.groupProduto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQtd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.panelTop.Controls.Add(this.btnCarregarTemplate);
            this.panelTop.Controls.Add(this.btnSalvarTemplate);
            this.panelTop.Controls.Add(this.btnImprimir);
            this.panelTop.Controls.Add(this.btnDesigner);
            this.panelTop.Controls.Add(this.lblTitulo);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(900, 80);
            this.panelTop.TabIndex = 0;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(20, 10);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(287, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "SISTEMA DE ETIQUETAS";
            // 
            // btnDesigner
            // 
            this.btnDesigner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnDesigner.FlatAppearance.BorderSize = 0;
            this.btnDesigner.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDesigner.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDesigner.ForeColor = System.Drawing.Color.White;
            this.btnDesigner.Location = new System.Drawing.Point(20, 45);
            this.btnDesigner.Name = "btnDesigner";
            this.btnDesigner.Size = new System.Drawing.Size(180, 30);
            this.btnDesigner.TabIndex = 1;
            this.btnDesigner.Text = "Designer de Etiqueta";
            this.btnDesigner.UseVisualStyleBackColor = false;
            this.btnDesigner.Click += new System.EventHandler(this.btnDesigner_Click);
            // 
            // btnImprimir
            // 
            this.btnImprimir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnImprimir.FlatAppearance.BorderSize = 0;
            this.btnImprimir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImprimir.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnImprimir.ForeColor = System.Drawing.Color.White;
            this.btnImprimir.Location = new System.Drawing.Point(210, 45);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(150, 30);
            this.btnImprimir.TabIndex = 2;
            this.btnImprimir.Text = "Imprimir Etiquetas";
            this.btnImprimir.UseVisualStyleBackColor = false;
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // btnSalvarTemplate
            // 
            this.btnSalvarTemplate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnSalvarTemplate.FlatAppearance.BorderSize = 0;
            this.btnSalvarTemplate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvarTemplate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSalvarTemplate.ForeColor = System.Drawing.Color.White;
            this.btnSalvarTemplate.Location = new System.Drawing.Point(370, 45);
            this.btnSalvarTemplate.Name = "btnSalvarTemplate";
            this.btnSalvarTemplate.Size = new System.Drawing.Size(140, 30);
            this.btnSalvarTemplate.TabIndex = 3;
            this.btnSalvarTemplate.Text = "💾 Salvar Template";
            this.btnSalvarTemplate.UseVisualStyleBackColor = false;
            this.btnSalvarTemplate.Click += new System.EventHandler(this.btnSalvarTemplate_Click);
            // 
            // btnCarregarTemplate
            // 
            this.btnCarregarTemplate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnCarregarTemplate.FlatAppearance.BorderSize = 0;
            this.btnCarregarTemplate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCarregarTemplate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCarregarTemplate.ForeColor = System.Drawing.Color.White;
            this.btnCarregarTemplate.Location = new System.Drawing.Point(520, 45);
            this.btnCarregarTemplate.Name = "btnCarregarTemplate";
            this.btnCarregarTemplate.Size = new System.Drawing.Size(160, 30);
            this.btnCarregarTemplate.TabIndex = 4;
            this.btnCarregarTemplate.Text = "📂 Carregar Template";
            this.btnCarregarTemplate.UseVisualStyleBackColor = false;
            this.btnCarregarTemplate.Click += new System.EventHandler(this.btnCarregarTemplate_Click);
            // 
            // groupProduto
            // 
            this.groupProduto.Controls.Add(this.btnAdicionar);
            this.groupProduto.Controls.Add(this.numQtd);
            this.groupProduto.Controls.Add(this.lblQtd);
            this.groupProduto.Controls.Add(this.txtPreco);
            this.groupProduto.Controls.Add(this.lblPreco);
            this.groupProduto.Controls.Add(this.txtCodigo);
            this.groupProduto.Controls.Add(this.lblCodigo);
            this.groupProduto.Controls.Add(this.txtNome);
            this.groupProduto.Controls.Add(this.lblNome);
            this.groupProduto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupProduto.Location = new System.Drawing.Point(10, 90);
            this.groupProduto.Name = "groupProduto";
            this.groupProduto.Size = new System.Drawing.Size(860, 100);
            this.groupProduto.TabIndex = 1;
            this.groupProduto.TabStop = false;
            this.groupProduto.Text = "Adicionar Produto";
            // 
            // lblNome
            // 
            this.lblNome.AutoSize = true;
            this.lblNome.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNome.Location = new System.Drawing.Point(10, 30);
            this.lblNome.Name = "lblNome";
            this.lblNome.Size = new System.Drawing.Size(43, 15);
            this.lblNome.TabIndex = 0;
            this.lblNome.Text = "Nome:";
            // 
            // txtNome
            // 
            this.txtNome.Location = new System.Drawing.Point(70, 27);
            this.txtNome.Name = "txtNome";
            this.txtNome.Size = new System.Drawing.Size(200, 23);
            this.txtNome.TabIndex = 1;
            // 
            // lblCodigo
            // 
            this.lblCodigo.AutoSize = true;
            this.lblCodigo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCodigo.Location = new System.Drawing.Point(280, 30);
            this.lblCodigo.Name = "lblCodigo";
            this.lblCodigo.Size = new System.Drawing.Size(49, 15);
            this.lblCodigo.TabIndex = 2;
            this.lblCodigo.Text = "Código:";
            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(340, 27);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(120, 23);
            this.txtCodigo.TabIndex = 3;
            // 
            // lblPreco
            // 
            this.lblPreco.AutoSize = true;
            this.lblPreco.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPreco.Location = new System.Drawing.Point(470, 30);
            this.lblPreco.Name = "lblPreco";
            this.lblPreco.Size = new System.Drawing.Size(40, 15);
            this.lblPreco.TabIndex = 4;
            this.lblPreco.Text = "Preço:";
            // 
            // txtPreco
            // 
            this.txtPreco.Location = new System.Drawing.Point(520, 27);
            this.txtPreco.Name = "txtPreco";
            this.txtPreco.Size = new System.Drawing.Size(100, 23);
            this.txtPreco.TabIndex = 5;
            // 
            // lblQtd
            // 
            this.lblQtd.AutoSize = true;
            this.lblQtd.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblQtd.Location = new System.Drawing.Point(630, 30);
            this.lblQtd.Name = "lblQtd";
            this.lblQtd.Size = new System.Drawing.Size(32, 15);
            this.lblQtd.TabIndex = 6;
            this.lblQtd.Text = "Qtd:";
            // 
            // numQtd
            // 
            this.numQtd.Location = new System.Drawing.Point(670, 27);
            this.numQtd.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numQtd.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numQtd.Name = "numQtd";
            this.numQtd.Size = new System.Drawing.Size(60, 23);
            this.numQtd.TabIndex = 7;
            this.numQtd.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // btnAdicionar
            // 
            this.btnAdicionar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnAdicionar.FlatAppearance.BorderSize = 0;
            this.btnAdicionar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdicionar.ForeColor = System.Drawing.Color.White;
            this.btnAdicionar.Location = new System.Drawing.Point(750, 25);
            this.btnAdicionar.Name = "btnAdicionar";
            this.btnAdicionar.Size = new System.Drawing.Size(90, 25);
            this.btnAdicionar.TabIndex = 8;
            this.btnAdicionar.Text = "Adicionar";
            this.btnAdicionar.UseVisualStyleBackColor = false;
            this.btnAdicionar.Click += new System.EventHandler(this.btnAdicionar_Click);
            // 
            // dgvProdutos
            // 
            this.dgvProdutos.AllowUserToAddRows = false;
            this.dgvProdutos.BackgroundColor = System.Drawing.Color.White;
            this.dgvProdutos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvProdutos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProdutos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSelecionar,
            this.colNome,
            this.colCodigo,
            this.colPreco,
            this.colQuantidade,
            this.colRemover});
            this.dgvProdutos.Location = new System.Drawing.Point(10, 200);
            this.dgvProdutos.MultiSelect = true;
            this.dgvProdutos.Name = "dgvProdutos";
            this.dgvProdutos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProdutos.Size = new System.Drawing.Size(860, 340);
            this.dgvProdutos.TabIndex = 2;
            this.dgvProdutos.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProdutos_CellContentClick);
            // 
            // colSelecionar
            // 
            this.colSelecionar.HeaderText = "Sel.";
            this.colSelecionar.Name = "colSelecionar";
            this.colSelecionar.Width = 40;
            // 
            // colNome
            // 
            this.colNome.HeaderText = "Nome do Produto";
            this.colNome.Name = "colNome";
            this.colNome.ReadOnly = true;
            this.colNome.Width = 300;
            // 
            // colCodigo
            // 
            this.colCodigo.HeaderText = "Código";
            this.colCodigo.Name = "colCodigo";
            this.colCodigo.ReadOnly = true;
            this.colCodigo.Width = 120;
            // 
            // colPreco
            // 
            this.colPreco.HeaderText = "Preço";
            this.colPreco.Name = "colPreco";
            this.colPreco.ReadOnly = true;
            // 
            // colQuantidade
            // 
            this.colQuantidade.HeaderText = "Qtd";
            this.colQuantidade.Name = "colQuantidade";
            this.colQuantidade.ReadOnly = true;
            this.colQuantidade.Width = 60;
            // 
            // colRemover
            // 
            this.colRemover.HeaderText = "Ação";
            this.colRemover.Name = "colRemover";
            this.colRemover.Text = "Remover";
            this.colRemover.UseColumnTextForButtonValue = true;
            this.colRemover.Width = 80;
            // 
            // FormPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.dgvProdutos);
            this.Controls.Add(this.groupProduto);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "FormPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sistema de Etiquetas - Menu Principal";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.groupProduto.ResumeLayout(false);
            this.groupProduto.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQtd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).EndInit();
            this.ResumeLayout(false);
        }
    }
}