using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace EtiquetaFORNew
{
    public partial class FormPrincipal : Form
    {
        private List<Produto> produtos = new List<Produto>();
        private TemplateEtiqueta template;

        public FormPrincipal()
        {
            InitializeComponent();
            template = new TemplateEtiqueta();
            CarregarUltimoTemplate();
        }

        private void CarregarUltimoTemplate()
        {
            var ultimoTemplate = TemplateManager.CarregarUltimoTemplate();
            if (ultimoTemplate != null)
            {
                template = ultimoTemplate;
            }
        }

        private void btnSalvarTemplate_Click(object sender, EventArgs e)
        {
            if (template.Elementos.Count == 0)
            {
                MessageBox.Show("Crie um template no Designer primeiro!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var formNome = new FormNomeTemplate();
            if (formNome.ShowDialog() == DialogResult.OK)
            {
                string nomeTemplate = formNome.NomeTemplate;

                if (TemplateManager.SalvarTemplate(template, nomeTemplate))
                {
                    MessageBox.Show($"Template '{nomeTemplate}' salvo com sucesso!\n\nLocal: {TemplateManager.ObterPastaTemplates()}",
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnCarregarTemplate_Click(object sender, EventArgs e)
        {
            var formLista = new FormListaTemplates();
            if (formLista.ShowDialog() == DialogResult.OK)
            {
                string nomeTemplate = formLista.TemplateSelecionado;

                var templateCarregado = TemplateManager.CarregarTemplate(nomeTemplate);
                if (templateCarregado != null)
                {
                    template = templateCarregado;
                    MessageBox.Show($"Template '{nomeTemplate}' carregado com sucesso!",
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnDesigner_Click(object sender, EventArgs e)
        {
            var formDesigner = new FormDesigner(template);
            if (formDesigner.ShowDialog() == DialogResult.OK)
            {
                template = formDesigner.ObterTemplate();
                MessageBox.Show("Template salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            var produtosSelecionados = ObterProdutosSelecionados();
            if (produtosSelecionados.Count == 0)
            {
                MessageBox.Show("Selecione pelo menos um produto!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (template.Elementos.Count == 0)
            {
                MessageBox.Show("Configure o template primeiro usando o Designer!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var formImpressao = new FormImpressao(produtosSelecionados, template);
            formImpressao.ShowDialog();
        }

        private void btnAdicionar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Nome e Código são obrigatórios!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal precoDecimal;
            if (!decimal.TryParse(txtPreco.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out precoDecimal))
            {
                MessageBox.Show("Preço inválido!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var produto = new Produto
            {
                Nome = txtNome.Text,
                Codigo = txtCodigo.Text,
                Preco = precoDecimal,
                Quantidade = (int)numQtd.Value
            };

            produtos.Add(produto);
            dgvProdutos.Rows.Add(false, produto.Nome, produto.Codigo, produto.Preco.ToString("C2"), produto.Quantidade);

            // Limpar campos
            txtNome.Clear();
            txtCodigo.Clear();
            txtPreco.Clear();
            numQtd.Value = 1;
            txtNome.Focus();
        }

        private void dgvProdutos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvProdutos.Columns[e.ColumnIndex].Name == "colRemover")
            {
                if (MessageBox.Show("Deseja remover este produto?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    produtos.RemoveAt(e.RowIndex);
                    dgvProdutos.Rows.RemoveAt(e.RowIndex);
                }
            }
        }

        private List<Produto> ObterProdutosSelecionados()
        {
            var selecionados = new List<Produto>();

            for (int i = 0; i < dgvProdutos.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dgvProdutos.Rows[i].Cells["colSelecionar"].Value))
                {
                    selecionados.Add(produtos[i]);
                }
            }

            return selecionados;
        }
    }
}