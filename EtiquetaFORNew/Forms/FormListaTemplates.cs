using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace EtiquetaFORNew
{
    public partial class FormListaTemplates : Form
    {
        public string TemplateSelecionado { get; private set; }
        private ListBox lstTemplates;

        public FormListaTemplates()
        {
            InitializeComponent();
            CarregarLista();
        }

        private void InitializeComponent()
        {
            this.Text = "Carregar Template";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblInstrucao = new Label
            {
                Text = "Selecione um template para carregar:",
                Location = new Point(20, 20),
                Size = new Size(450, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            lstTemplates = new ListBox
            {
                Name = "lstTemplates",
                Location = new Point(20, 50),
                Size = new Size(440, 230),
                Font = new Font("Segoe UI", 10)
            };
            lstTemplates.DoubleClick += (s, e) => CarregarSelecionado();

            Label lblInfo = new Label
            {
                Text = $"📁 Local: {TemplateManager.ObterPastaTemplates()}",
                Location = new Point(20, 290),
                Size = new Size(440, 20),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray
            };

            Button btnAbrir = new Button
            {
                Text = "Abrir Pasta",
                Location = new Point(20, 320),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAbrir.FlatAppearance.BorderSize = 0;
            btnAbrir.Click += (s, e) =>
            {
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", TemplateManager.ObterPastaTemplates());
                }
                catch { }
            };

            Button btnExcluir = new Button
            {
                Text = "Excluir",
                Location = new Point(130, 320),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExcluir.FlatAppearance.BorderSize = 0;
            btnExcluir.Click += BtnExcluir_Click;

            Button btnCarregar = new Button
            {
                Text = "Carregar",
                Location = new Point(260, 320),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnCarregar.FlatAppearance.BorderSize = 0;
            btnCarregar.Click += (s, e) => CarregarSelecionado();

            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(370, 320),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancelar.FlatAppearance.BorderSize = 0;

            this.Controls.AddRange(new Control[] {
                lblInstrucao, lstTemplates, lblInfo,
                btnAbrir, btnExcluir, btnCarregar, btnCancelar
            });
        }

        private void CarregarLista()
        {
            lstTemplates.Items.Clear();
            var templates = TemplateManager.ListarTemplates();

            // Filtrar o template temporário
            templates = templates.Where(t => t != "_ultimo_template").ToList();

            if (templates.Count == 0)
            {
                lstTemplates.Items.Add("(Nenhum template salvo)");
                return;
            }

            foreach (var template in templates)
            {
                lstTemplates.Items.Add(template);
            }

            if (lstTemplates.Items.Count > 0)
            {
                lstTemplates.SelectedIndex = 0;
            }
        }

        private void CarregarSelecionado()
        {
            if (lstTemplates.SelectedItem == null ||
                lstTemplates.SelectedItem.ToString() == "(Nenhum template salvo)")
            {
                MessageBox.Show("Selecione um template!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            TemplateSelecionado = lstTemplates.SelectedItem.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnExcluir_Click(object sender, EventArgs e)
        {
            if (lstTemplates.SelectedItem == null ||
                lstTemplates.SelectedItem.ToString() == "(Nenhum template salvo)")
            {
                MessageBox.Show("Selecione um template para excluir!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nomeTemplate = lstTemplates.SelectedItem.ToString();

            if (MessageBox.Show($"Deseja realmente excluir o template '{nomeTemplate}'?",
                "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (TemplateManager.ExcluirTemplate(nomeTemplate))
                {
                    MessageBox.Show("Template excluído com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CarregarLista();
                }
            }
        }
    }
}