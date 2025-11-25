namespace EtiquetaFORNew
{
    partial class FormPrincipal
    {
        private System.ComponentModel.IContainer components = null;
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
        private System.Windows.Forms.Button btnBuscarMercadoria;

        // ⭐ NOVOS CONTROLES PARA CONFIGURAÇÃO
        private System.Windows.Forms.ComboBox cmbConfiguracao;
        private System.Windows.Forms.Label lblConfiguracao;
        private System.Windows.Forms.Label lblStatusConfig;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrincipal));
            this.groupProduto = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.BtnAdicionar2 = new System.Windows.Forms.Button();
            this.cmbBuscaReferencia = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbBuscaNome = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbBuscaCodigo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAdicionar = new System.Windows.Forms.Button();
            this.numQtd = new System.Windows.Forms.NumericUpDown();
            this.lblQtd = new System.Windows.Forms.Label();
            this.txtPreco = new System.Windows.Forms.TextBox();
            this.lblPreco = new System.Windows.Forms.Label();
            this.txtCodigo = new System.Windows.Forms.TextBox();
            this.lblCodigo = new System.Windows.Forms.Label();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.lblNome = new System.Windows.Forms.Label();
            this.dgvProdutos = new System.Windows.Forms.DataGridView();
            this.colSelecionar = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colNome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCodigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPreco = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuantidade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRemover = new System.Windows.Forms.DataGridViewButtonColumn();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnDesigner = new System.Windows.Forms.Button();
            this.btnImprimir = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnConfigPapel = new System.Windows.Forms.Button();
            this.btnCarregarTemplate = new System.Windows.Forms.Button();
            this.btnBuscarMercadoria = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelConfiguracao = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblConfiguracao = new System.Windows.Forms.Label();
            this.cmbConfiguracao = new System.Windows.Forms.ComboBox();
            this.lblStatusConfig = new System.Windows.Forms.Label();
            this.cmbTemplates = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupProduto.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQtd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).BeginInit();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panelConfiguracao.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // groupProduto
            // 
            this.groupProduto.BackColor = System.Drawing.Color.White;
            this.groupProduto.Controls.Add(this.panel1);
            this.groupProduto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.groupProduto.Location = new System.Drawing.Point(3, 3);
            this.groupProduto.Name = "groupProduto";
            this.groupProduto.Size = new System.Drawing.Size(851, 124);
            this.groupProduto.TabIndex = 1;
            this.groupProduto.TabStop = false;
            this.groupProduto.Text = "Adicionar Produto";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.BtnAdicionar2);
            this.panel1.Controls.Add(this.cmbBuscaReferencia);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.cmbBuscaNome);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.cmbBuscaCodigo);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnAdicionar);
            this.panel1.Controls.Add(this.numQtd);
            this.panel1.Controls.Add(this.lblQtd);
            this.panel1.Controls.Add(this.txtPreco);
            this.panel1.Controls.Add(this.lblPreco);
            this.panel1.Controls.Add(this.txtCodigo);
            this.panel1.Controls.Add(this.lblCodigo);
            this.panel1.Controls.Add(this.txtNome);
            this.panel1.Controls.Add(this.lblNome);
            this.panel1.Location = new System.Drawing.Point(6, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(838, 74);
            this.panel1.TabIndex = 0;
            // 
            // BtnAdicionar2
            // 
            this.BtnAdicionar2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(143)))), ((int)(((byte)(0)))));
            this.BtnAdicionar2.FlatAppearance.BorderSize = 0;
            this.BtnAdicionar2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnAdicionar2.ForeColor = System.Drawing.Color.Black;
            this.BtnAdicionar2.Location = new System.Drawing.Point(744, 5);
            this.BtnAdicionar2.Name = "BtnAdicionar2";
            this.BtnAdicionar2.Size = new System.Drawing.Size(90, 25);
            this.BtnAdicionar2.TabIndex = 14;
            this.BtnAdicionar2.Text = "Adicionar";
            this.BtnAdicionar2.UseVisualStyleBackColor = false;
            this.BtnAdicionar2.Click += new System.EventHandler(this.BtnAdicionar2_Click);
            // 
            // cmbBuscaReferencia
            // 
            this.cmbBuscaReferencia.FormattingEnabled = true;
            this.cmbBuscaReferencia.Location = new System.Drawing.Point(237, 2);
            this.cmbBuscaReferencia.Name = "cmbBuscaReferencia";
            this.cmbBuscaReferencia.Size = new System.Drawing.Size(99, 23);
            this.cmbBuscaReferencia.TabIndex = 11;
            this.cmbBuscaReferencia.SelectedIndexChanged += new System.EventHandler(this.cmbBuscaReferencia_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label3.Location = new System.Drawing.Point(166, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 15);
            this.label3.TabIndex = 13;
            this.label3.Text = "Referência:";
            // 
            // cmbBuscaNome
            // 
            this.cmbBuscaNome.FormattingEnabled = true;
            this.cmbBuscaNome.Location = new System.Drawing.Point(394, 3);
            this.cmbBuscaNome.Name = "cmbBuscaNome";
            this.cmbBuscaNome.Size = new System.Drawing.Size(258, 23);
            this.cmbBuscaNome.TabIndex = 12;
            this.cmbBuscaNome.SelectedIndexChanged += new System.EventHandler(this.cmbBuscaNome_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label2.Location = new System.Drawing.Point(343, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "Nome:";
            // 
            // cmbBuscaCodigo
            // 
            this.cmbBuscaCodigo.FormattingEnabled = true;
            this.cmbBuscaCodigo.Location = new System.Drawing.Point(60, 2);
            this.cmbBuscaCodigo.Name = "cmbBuscaCodigo";
            this.cmbBuscaCodigo.Size = new System.Drawing.Size(99, 23);
            this.cmbBuscaCodigo.TabIndex = 10;
            this.cmbBuscaCodigo.SelectedIndexChanged += new System.EventHandler(this.cmbBuscaCodigo_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.label1.Location = new System.Drawing.Point(5, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Código:";
            // 
            // btnAdicionar
            // 
            this.btnAdicionar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(143)))), ((int)(((byte)(0)))));
            this.btnAdicionar.FlatAppearance.BorderSize = 0;
            this.btnAdicionar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdicionar.ForeColor = System.Drawing.Color.Black;
            this.btnAdicionar.Location = new System.Drawing.Point(745, 46);
            this.btnAdicionar.Name = "btnAdicionar";
            this.btnAdicionar.Size = new System.Drawing.Size(90, 25);
            this.btnAdicionar.TabIndex = 8;
            this.btnAdicionar.Text = "Adicionar";
            this.btnAdicionar.UseVisualStyleBackColor = false;
            this.btnAdicionar.Visible = false;
            this.btnAdicionar.Click += new System.EventHandler(this.btnAdicionar_Click);
            // 
            // numQtd
            // 
            this.numQtd.Location = new System.Drawing.Point(698, 4);
            this.numQtd.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numQtd.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numQtd.Name = "numQtd";
            this.numQtd.Size = new System.Drawing.Size(40, 23);
            this.numQtd.TabIndex = 13;
            this.numQtd.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblQtd
            // 
            this.lblQtd.AutoSize = true;
            this.lblQtd.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblQtd.Location = new System.Drawing.Point(658, 7);
            this.lblQtd.Name = "lblQtd";
            this.lblQtd.Size = new System.Drawing.Size(30, 15);
            this.lblQtd.TabIndex = 6;
            this.lblQtd.Text = "Qtd:";
            // 
            // txtPreco
            // 
            this.txtPreco.Location = new System.Drawing.Point(530, 48);
            this.txtPreco.Name = "txtPreco";
            this.txtPreco.Size = new System.Drawing.Size(80, 23);
            this.txtPreco.TabIndex = 5;
            this.txtPreco.Visible = false;
            // 
            // lblPreco
            // 
            this.lblPreco.AutoSize = true;
            this.lblPreco.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPreco.Location = new System.Drawing.Point(479, 51);
            this.lblPreco.Name = "lblPreco";
            this.lblPreco.Size = new System.Drawing.Size(40, 15);
            this.lblPreco.TabIndex = 4;
            this.lblPreco.Text = "Preço:";
            this.lblPreco.Visible = false;
            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(62, 47);
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(100, 23);
            this.txtCodigo.TabIndex = 3;
            this.txtCodigo.Visible = false;
            // 
            // lblCodigo
            // 
            this.lblCodigo.AutoSize = true;
            this.lblCodigo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCodigo.Location = new System.Drawing.Point(0, 50);
            this.lblCodigo.Name = "lblCodigo";
            this.lblCodigo.Size = new System.Drawing.Size(49, 15);
            this.lblCodigo.TabIndex = 2;
            this.lblCodigo.Text = "Código:";
            this.lblCodigo.Visible = false;
            // 
            // txtNome
            // 
            this.txtNome.Location = new System.Drawing.Point(225, 47);
            this.txtNome.Name = "txtNome";
            this.txtNome.Size = new System.Drawing.Size(220, 23);
            this.txtNome.TabIndex = 1;
            this.txtNome.Visible = false;
            // 
            // lblNome
            // 
            this.lblNome.AutoSize = true;
            this.lblNome.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblNome.Location = new System.Drawing.Point(174, 50);
            this.lblNome.Name = "lblNome";
            this.lblNome.Size = new System.Drawing.Size(43, 15);
            this.lblNome.TabIndex = 0;
            this.lblNome.Text = "Nome:";
            this.lblNome.Visible = false;
            // 
            // dgvProdutos
            // 
            this.dgvProdutos.AllowUserToAddRows = false;
            this.dgvProdutos.BackgroundColor = System.Drawing.Color.White;
            this.dgvProdutos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProdutos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSelecionar,
            this.colNome,
            this.colCodigo,
            this.colPreco,
            this.colQuantidade,
            this.colRemover});
            this.dgvProdutos.Location = new System.Drawing.Point(12, 297);
            this.dgvProdutos.Name = "dgvProdutos";
            this.dgvProdutos.Size = new System.Drawing.Size(859, 276);
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
            this.colNome.HeaderText = "Nome";
            this.colNome.Name = "colNome";
            this.colNome.Width = 300;
            // 
            // colCodigo
            // 
            this.colCodigo.HeaderText = "Código";
            this.colCodigo.Name = "colCodigo";
            this.colCodigo.Width = 120;
            // 
            // colPreco
            // 
            this.colPreco.HeaderText = "Preço";
            this.colPreco.Name = "colPreco";
            // 
            // colQuantidade
            // 
            this.colQuantidade.HeaderText = "Qtd";
            this.colQuantidade.Name = "colQuantidade";
            this.colQuantidade.Width = 50;
            // 
            // colRemover
            // 
            this.colRemover.HeaderText = "Remover";
            this.colRemover.Name = "colRemover";
            this.colRemover.Text = "X";
            this.colRemover.UseColumnTextForButtonValue = true;
            this.colRemover.Width = 80;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.Black;
            this.lblTitulo.Location = new System.Drawing.Point(68, 6);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(260, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "SISTEMA DE ETIQUETAS";
            // 
            // btnDesigner
            // 
            this.btnDesigner.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnDesigner.FlatAppearance.BorderSize = 0;
            this.btnDesigner.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDesigner.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDesigner.ForeColor = System.Drawing.Color.Black;
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
            this.btnImprimir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnImprimir.FlatAppearance.BorderSize = 0;
            this.btnImprimir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImprimir.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnImprimir.ForeColor = System.Drawing.Color.Black;
            this.btnImprimir.Location = new System.Drawing.Point(210, 45);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(150, 30);
            this.btnImprimir.TabIndex = 2;
            this.btnImprimir.Text = "Imprimir Etiquetas";
            this.btnImprimir.UseVisualStyleBackColor = false;
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.White;
            this.panelTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTop.Controls.Add(this.btnConfigPapel);
            this.panelTop.Controls.Add(this.btnCarregarTemplate);
            this.panelTop.Controls.Add(this.btnBuscarMercadoria);
            this.panelTop.Controls.Add(this.btnImprimir);
            this.panelTop.Controls.Add(this.btnDesigner);
            this.panelTop.Controls.Add(this.lblTitulo);
            this.panelTop.Controls.Add(this.pictureBox1);
            this.panelTop.Location = new System.Drawing.Point(12, 12);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(860, 85);
            this.panelTop.TabIndex = 0;
            // 
            // btnConfigPapel
            // 
            this.btnConfigPapel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnConfigPapel.FlatAppearance.BorderSize = 0;
            this.btnConfigPapel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfigPapel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnConfigPapel.ForeColor = System.Drawing.Color.Black;
            this.btnConfigPapel.Location = new System.Drawing.Point(559, 45);
            this.btnConfigPapel.Name = "btnConfigPapel";
            this.btnConfigPapel.Size = new System.Drawing.Size(150, 30);
            this.btnConfigPapel.TabIndex = 10;
            this.btnConfigPapel.Text = "Configurar Papel";
            this.btnConfigPapel.UseVisualStyleBackColor = false;
            this.btnConfigPapel.Click += new System.EventHandler(this.btnConfigPapel_Click);
            // 
            // btnCarregarTemplate
            // 
            this.btnCarregarTemplate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnCarregarTemplate.FlatAppearance.BorderSize = 0;
            this.btnCarregarTemplate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCarregarTemplate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCarregarTemplate.ForeColor = System.Drawing.Color.Black;
            this.btnCarregarTemplate.Location = new System.Drawing.Point(715, 45);
            this.btnCarregarTemplate.Name = "btnCarregarTemplate";
            this.btnCarregarTemplate.Size = new System.Drawing.Size(140, 30);
            this.btnCarregarTemplate.TabIndex = 9;
            this.btnCarregarTemplate.Text = "📂 Carregar Template";
            this.btnCarregarTemplate.UseVisualStyleBackColor = false;
            this.btnCarregarTemplate.Click += new System.EventHandler(this.btnCarregarTemplate_Click);
            // 
            // btnBuscarMercadoria
            // 
            this.btnBuscarMercadoria.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnBuscarMercadoria.FlatAppearance.BorderSize = 0;
            this.btnBuscarMercadoria.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarMercadoria.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnBuscarMercadoria.ForeColor = System.Drawing.Color.Black;
            this.btnBuscarMercadoria.Location = new System.Drawing.Point(370, 45);
            this.btnBuscarMercadoria.Name = "btnBuscarMercadoria";
            this.btnBuscarMercadoria.Size = new System.Drawing.Size(180, 30);
            this.btnBuscarMercadoria.TabIndex = 3;
            this.btnBuscarMercadoria.Text = "🔍 Buscar Mercadoria";
            this.btnBuscarMercadoria.UseVisualStyleBackColor = false;
            this.btnBuscarMercadoria.Visible = false;
            this.btnBuscarMercadoria.Click += new System.EventHandler(this.btnBuscarMercadoria_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::EtiquetaFORNew.Properties.Resources.icone_novo_2025_PNG1;
            this.pictureBox1.Location = new System.Drawing.Point(18, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(63, 35);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.groupProduto);
            this.panel2.Location = new System.Drawing.Point(12, 163);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(859, 128);
            this.panel2.TabIndex = 3;
            // 
            // panelConfiguracao
            // 
            this.panelConfiguracao.BackColor = System.Drawing.Color.White;
            this.panelConfiguracao.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelConfiguracao.Controls.Add(this.label4);
            this.panelConfiguracao.Controls.Add(this.cmbTemplates);
            this.panelConfiguracao.Controls.Add(this.pictureBox2);
            this.panelConfiguracao.Controls.Add(this.lblConfiguracao);
            this.panelConfiguracao.Controls.Add(this.cmbConfiguracao);
            this.panelConfiguracao.Controls.Add(this.lblStatusConfig);
            this.panelConfiguracao.Location = new System.Drawing.Point(12, 103);
            this.panelConfiguracao.Name = "panelConfiguracao";
            this.panelConfiguracao.Size = new System.Drawing.Size(860, 54);
            this.panelConfiguracao.TabIndex = 4;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox2.Image = global::EtiquetaFORNew.Properties.Resources.Sincronizando;
            this.pictureBox2.Location = new System.Drawing.Point(823, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(31, 24);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // lblConfiguracao
            // 
            this.lblConfiguracao.AutoSize = true;
            this.lblConfiguracao.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblConfiguracao.Location = new System.Drawing.Point(8, 8);
            this.lblConfiguracao.Name = "lblConfiguracao";
            this.lblConfiguracao.Size = new System.Drawing.Size(99, 15);
            this.lblConfiguracao.TabIndex = 0;
            this.lblConfiguracao.Text = "⚙️ Configuração:";
            // 
            // cmbConfiguracao
            // 
            this.cmbConfiguracao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConfiguracao.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbConfiguracao.FormattingEnabled = true;
            this.cmbConfiguracao.Location = new System.Drawing.Point(113, 5);
            this.cmbConfiguracao.Name = "cmbConfiguracao";
            this.cmbConfiguracao.Size = new System.Drawing.Size(280, 23);
            this.cmbConfiguracao.TabIndex = 1;
            this.cmbConfiguracao.SelectedIndexChanged += new System.EventHandler(this.cmbConfiguracao_SelectedIndexChanged);
            // 
            // lblStatusConfig
            // 
            this.lblStatusConfig.AutoSize = true;
            this.lblStatusConfig.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblStatusConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblStatusConfig.Location = new System.Drawing.Point(148, 31);
            this.lblStatusConfig.Name = "lblStatusConfig";
            this.lblStatusConfig.Size = new System.Drawing.Size(197, 13);
            this.lblStatusConfig.TabIndex = 2;
            this.lblStatusConfig.Text = "⚠️ Nenhuma configuração carregada";
            // 
            // cmbTemplates
            // 
            this.cmbTemplates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTemplates.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbTemplates.FormattingEnabled = true;
            this.cmbTemplates.Location = new System.Drawing.Point(483, 5);
            this.cmbTemplates.Name = "cmbTemplates";
            this.cmbTemplates.Size = new System.Drawing.Size(280, 23);
            this.cmbTemplates.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(418, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "Template:";
            // 
            // FormPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(235)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(887, 582);
            this.Controls.Add(this.dgvProdutos);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelConfiguracao);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SmartPrint v1.0 - Menu Principal";
            this.Load += new System.EventHandler(this.FormPrincipal_Load);
            this.groupProduto.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQtd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdutos)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panelConfiguracao.ResumeLayout(false);
            this.panelConfiguracao.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnDesigner;
        private System.Windows.Forms.Button btnImprimir;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCarregarTemplate;
        private System.Windows.Forms.Button btnConfigPapel;

        // ⭐ NOVO: Painel de configuração
        private System.Windows.Forms.Panel panelConfiguracao;
        private System.Windows.Forms.ComboBox cmbBuscaReferencia;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbBuscaNome;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbBuscaCodigo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnAdicionar2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbTemplates;
    }
}