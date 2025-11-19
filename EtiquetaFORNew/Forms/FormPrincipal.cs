using EtiquetaFORNew;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using EtiquetaFORNew.Data;

namespace EtiquetaFORNew
{
    public partial class FormPrincipal : Form
    {
        private List<Produto> produtos = new List<Produto>();
        private TemplateEtiqueta template;

        // ⭐ NOVO: Configuração de etiqueta atual
        private ConfiguracaoEtiqueta configuracaoAtual;

        public FormPrincipal()
        {
            InitializeComponent();
            template = new TemplateEtiqueta();
            CarregarUltimoTemplate();
            this.DoubleBuffered = true;
            this.Load += FormPrincipal_Load;
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            // ========================================
            // 🔹 INICIALIZAR BANCO LOCAL SQLITE
            // ========================================
            try
            {
                LocalDatabaseManager.InicializarBanco();

                // Verificar se precisa sincronizar (mais de 24h desde última sync)
                if (LocalDatabaseManager.PrecisaSincronizar())
                {
                    var result = MessageBox.Show(
                        "Detectamos que faz mais de 24 horas desde a última sincronização.\n\n" +
                        "Deseja sincronizar as mercadorias do SQL Server agora?",
                        "Sincronização Recomendada",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        SincronizarMercadorias();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao inicializar banco local:\n{ex.Message}\n\n" +
                    "O sistema continuará funcionando, mas você precisará adicionar produtos manualmente.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            // ========================================
            // 🔹 CARREGAR CONFIGURAÇÃO DE IMPRESSÃO
            // ========================================
            CarregarConfiguracaoImpressao();
            AtualizarListaConfiguracoes();

            // ========================================
            // 🔹 ARREDONDAR BOTÕES
            // ========================================
            ArredondarBotao(btnDesigner, 12);
            ArredondarBotao(btnImprimir, 12);
            ArredondarBotao(btnBuscarMercadoria, 12);
            ArredondarBotao(btnAdicionar, 12);
            ArredondarBotao(btnCarregarTemplate, 12);
            ArredondarBotao(btnConfigPapel, 12);
        }

        // ========================================
        // ⭐ NOVO: GERENCIAMENTO DE CONFIGURAÇÕES
        // ========================================

        /// <summary>
        /// Carrega a configuração de impressão ao iniciar
        /// </summary>
        private void CarregarConfiguracaoImpressao()
        {
            configuracaoAtual = GerenciadorConfiguracoesEtiqueta.CarregarConfiguracaoPadrao();

            if (configuracaoAtual == null)
            {
                // Se não houver configuração, cria uma padrão baseada no template
                configuracaoAtual = new ConfiguracaoEtiqueta
                {
                    NomeEtiqueta = "Etiqueta Padrão",
                    ImpressoraPadrao = "BTP-L42(D)",
                    PapelPadrao = "Tamanho do papel-SoftcomGondBar",
                    LarguraEtiqueta = template.Largura,
                    AlturaEtiqueta = template.Altura,
                    NumColunas = 1,
                    NumLinhas = 1,
                    EspacamentoColunas = 0,
                    EspacamentoLinhas = 0,
                    MargemSuperior = 0,
                    MargemInferior = 0,
                    MargemEsquerda = 0,
                    MargemDireita = 0
                };
            }

            AtualizarStatusConfiguracao();
        }

        /// <summary>
        /// Atualiza a lista de configurações no ComboBox
        /// </summary>
        private void AtualizarListaConfiguracoes()
        {
            cmbConfiguracao.Items.Clear();

            // Adiciona configuração atual
            cmbConfiguracao.Items.Add(new ConfiguracaoItem
            {
                Nome = "⭐ Configuração Atual",
                Configuracao = configuracaoAtual,
                IsPadrao = true
            });

            // Adiciona configurações salvas
            List<ConfiguracaoPapel> papeisSalvos = GerenciadorConfiguracoesEtiqueta.CarregarTodasConfiguracoes();

            foreach (var papel in papeisSalvos)
            {
                var config = GerenciadorConfiguracoesEtiqueta.ConverterPapelParaConfig(
                    papel,
                    configuracaoAtual.ImpressoraPadrao
                );

                cmbConfiguracao.Items.Add(new ConfiguracaoItem
                {
                    Nome = $"📄 {papel.NomePapel}",
                    Configuracao = config,
                    IsPadrao = false
                });
            }

            // Seleciona a configuração atual
            if (cmbConfiguracao.Items.Count > 0)
            {
                cmbConfiguracao.SelectedIndex = 0;
            }

            // Atualiza o status
            AtualizarStatusConfiguracao();
        }

        /// <summary>
        /// Atualiza o label de status da configuração
        /// </summary>
        private void AtualizarStatusConfiguracao()
        {
            if (configuracaoAtual != null)
            {
                lblStatusConfig.Text = $"📋 {configuracaoAtual.NomeEtiqueta} | " +
                                      $"📏 {configuracaoAtual.LarguraEtiqueta}x{configuracaoAtual.AlturaEtiqueta}mm | " +
                                      $"🖨️ {configuracaoAtual.ImpressoraPadrao}";
            }
            else
            {
                lblStatusConfig.Text = "⚠️ Nenhuma configuração carregada";
            }
        }

        /// <summary>
        /// Evento ao mudar a seleção do ComboBox
        /// </summary>
        private void cmbConfiguracao_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbConfiguracao.SelectedItem is ConfiguracaoItem item)
            {
                configuracaoAtual = item.Configuracao;

                // Atualiza o template com as dimensões da configuração
                template.Largura = item.Configuracao.LarguraEtiqueta;
                template.Altura = item.Configuracao.AlturaEtiqueta;

                AtualizarStatusConfiguracao();
            }
        }

        // ========================================
        // 🔹 SINCRONIZAR MERCADORIAS DO SQL SERVER
        // ========================================
        private void SincronizarMercadorias()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Sincronizar todas as mercadorias (pode adicionar filtro se necessário)
                int total = LocalDatabaseManager.SincronizarMercadorias();

                Cursor = Cursors.Default;

                MessageBox.Show(
                    $"Sincronização concluída com sucesso!\n\n" +
                    $"Total de mercadorias importadas: {total:N0}",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(
                    $"Erro ao sincronizar:\n{ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CarregarUltimoTemplate()
        {
            var ultimoTemplate = TemplateManager.CarregarUltimoTemplate();
            if (ultimoTemplate != null)
            {
                template = ultimoTemplate;
            }
        }

        // ========================================
        // ⭐ BUSCAR MERCADORIA
        // ========================================
        private void btnBuscarMercadoria_Click(object sender, EventArgs e)
        {
            try
            {
                var formBusca = new FormBuscaMercadoria();

                if (formBusca.ShowDialog() == DialogResult.OK)
                {
                    // Preencher os campos com a mercadoria selecionada
                    txtNome.Text = formBusca.NomeSelecionado;
                    txtCodigo.Text = formBusca.CodigoFabricanteSelecionado;
                    txtPreco.Text = formBusca.PrecoSelecionado.ToString("F2");

                    // Focar na quantidade para o usuário só digitar e adicionar
                    numQtd.Focus();
                    numQtd.Select(0, numQtd.Text.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao buscar mercadoria:\n{ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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

        // ========================================
        // ⭐ MODIFICADO: IMPRIMIR COM CONFIGURAÇÃO
        // ========================================
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

            // ⭐ VERIFICA SE HÁ CONFIGURAÇÃO
            if (configuracaoAtual == null)
            {
                var resultado = MessageBox.Show(
                    "Nenhuma configuração de impressão foi definida.\n\n" +
                    "Deseja configurar agora?",
                    "Configuração Necessária",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    btnConfigPapel_Click(sender, e);
                    return;
                }
                else
                {
                    return;
                }
                
            }

            //// ⭐ PASSA A CONFIGURAÇÃO PARA O FORM DE IMPRESSÃO
            var formImpressao = new FormImpressao(produtosSelecionados, template, configuracaoAtual);
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

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            using (LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.White,
                Color.White,
                LinearGradientMode.Vertical))
            {
                ColorBlend blend = new ColorBlend();
                blend.Positions = new float[] { 0.0f, 0.85f, 1.0f };
                blend.Colors = new Color[] {
                    Color.FromArgb(240, 235, 255),
                    Color.FromArgb(240, 235, 255),
                    Color.FromArgb(255, 255, 200, 50)
                };

                brush.InterpolationColors = blend;
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        public static void ArredondarBotao(Button botao, int raio)
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle rect = botao.ClientRectangle;

            int d = raio * 2;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            botao.Region = new Region(path);
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

        // ========================================
        // ⭐ MODIFICADO: CONFIGURAR PAPEL
        // ========================================
        private void btnConfigPapel_Click(object sender, EventArgs e)
        {
            // Usa a configuração atual ou cria uma nova baseada no template
            var configParaEditar = configuracaoAtual ?? new ConfiguracaoEtiqueta
            {
                NomeEtiqueta = "Etiqueta Atual",
                ImpressoraPadrao = "BTP-L42(D)",
                PapelPadrao = "Tamanho do papel-SoftcomGondBar",
                LarguraEtiqueta = template.Largura,
                AlturaEtiqueta = template.Altura,
                NumColunas = 1,
                NumLinhas = 1,
                EspacamentoColunas = 0,
                EspacamentoLinhas = 0,
                MargemSuperior = 0,
                MargemInferior = 0,
                MargemEsquerda = 0,
                MargemDireita = 0
            };

            var formConfig = new FormConfigEtiqueta(configParaEditar);
            if (formConfig.ShowDialog() == DialogResult.OK)
            {
                configuracaoAtual = formConfig.Configuracao;

                // Atualiza o template com as novas dimensões
                template.Largura = configuracaoAtual.LarguraEtiqueta;
                template.Altura = configuracaoAtual.AlturaEtiqueta;

                // Salva como configuração padrão
                GerenciadorConfiguracoesEtiqueta.SalvarConfiguracaoPadrao(configuracaoAtual);

                // Atualiza a lista de configurações
                AtualizarListaConfiguracoes();

                MessageBox.Show($"✅ Configuração de etiqueta aplicada com sucesso!\n\n" +
                    $"📏 Dimensões: {configuracaoAtual.LarguraEtiqueta} x {configuracaoAtual.AlturaEtiqueta} mm\n" +
                    $"📐 Layout: {configuracaoAtual.NumColunas} coluna(s) x {configuracaoAtual.NumLinhas} linha(s)\n" +
                    $"🖨️ Impressora: {configuracaoAtual.ImpressoraPadrao}",
                    "Configuração Aplicada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // ========================================
        // ⭐ CLASSE AUXILIAR PARA ITENS DO COMBOBOX
        // ========================================
        private class ConfiguracaoItem
        {
            public string Nome { get; set; }
            public ConfiguracaoEtiqueta Configuracao { get; set; }
            public bool IsPadrao { get; set; }

            public override string ToString()
            {
                return Nome;
            }
        }
    }
}