namespace EtiquetaFORNew
{
    partial class configuracoes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(configuracoes));
            this.panel1 = new System.Windows.Forms.Panel();
            this.TestarConexao = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnExibirSenha = new System.Windows.Forms.PictureBox();
            this.textBanco = new System.Windows.Forms.TextBox();
            this.textSenha = new System.Windows.Forms.TextBox();
            this.textUsuario = new System.Windows.Forms.TextBox();
            this.textPorta = new System.Windows.Forms.TextBox();
            this.textServidor = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnExibirSenha)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.TestarConexao);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(21, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(568, 315);
            this.panel1.TabIndex = 0;
            // 
            // TestarConexao
            // 
            this.TestarConexao.Location = new System.Drawing.Point(370, 279);
            this.TestarConexao.Name = "TestarConexao";
            this.TestarConexao.Size = new System.Drawing.Size(141, 26);
            this.TestarConexao.TabIndex = 12;
            this.TestarConexao.Text = "Testar Conexão";
            this.TestarConexao.UseVisualStyleBackColor = true;
            this.TestarConexao.Click += new System.EventHandler(this.TestarConexao_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(67, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(109, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Login Para o Servidor";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnExibirSenha);
            this.panel2.Controls.Add(this.textBanco);
            this.panel2.Controls.Add(this.textSenha);
            this.panel2.Controls.Add(this.textUsuario);
            this.panel2.Controls.Add(this.textPorta);
            this.panel2.Controls.Add(this.textServidor);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(28, 28);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(484, 242);
            this.panel2.TabIndex = 11;
            // 
            // btnExibirSenha
            // 
            this.btnExibirSenha.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExibirSenha.Image = global::EtiquetaFORNew.Properties.Resources.olho_senha;
            this.btnExibirSenha.Location = new System.Drawing.Point(418, 167);
            this.btnExibirSenha.Name = "btnExibirSenha";
            this.btnExibirSenha.Size = new System.Drawing.Size(29, 27);
            this.btnExibirSenha.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnExibirSenha.TabIndex = 10;
            this.btnExibirSenha.TabStop = false;
            this.btnExibirSenha.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // textBanco
            // 
            this.textBanco.Location = new System.Drawing.Point(160, 209);
            this.textBanco.Name = "textBanco";
            this.textBanco.Size = new System.Drawing.Size(240, 20);
            this.textBanco.TabIndex = 9;
            // 
            // textSenha
            // 
            this.textSenha.Location = new System.Drawing.Point(160, 166);
            this.textSenha.Name = "textSenha";
            this.textSenha.Size = new System.Drawing.Size(240, 20);
            this.textSenha.TabIndex = 8;
            this.textSenha.TextChanged += new System.EventHandler(this.textSenha_TextChanged);
            // 
            // textUsuario
            // 
            this.textUsuario.Location = new System.Drawing.Point(160, 122);
            this.textUsuario.Name = "textUsuario";
            this.textUsuario.Size = new System.Drawing.Size(240, 20);
            this.textUsuario.TabIndex = 7;
            // 
            // textPorta
            // 
            this.textPorta.Location = new System.Drawing.Point(160, 72);
            this.textPorta.Name = "textPorta";
            this.textPorta.Size = new System.Drawing.Size(240, 20);
            this.textPorta.TabIndex = 6;
            // 
            // textServidor
            // 
            this.textServidor.Location = new System.Drawing.Point(160, 32);
            this.textServidor.Name = "textServidor";
            this.textServidor.Size = new System.Drawing.Size(240, 20);
            this.textServidor.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(39, 212);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Banco de Dados:";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 169);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Senha:";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Usuário:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Porta:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Servidor:";
            // 
            // configuracoes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(612, 353);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "configuracoes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configurações";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnExibirSenha)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textServidor;
        private System.Windows.Forms.TextBox textSenha;
        private System.Windows.Forms.TextBox textUsuario;
        private System.Windows.Forms.TextBox textPorta;
        private System.Windows.Forms.TextBox textBanco;
        private System.Windows.Forms.Button TestarConexao;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox btnExibirSenha;
    }
}