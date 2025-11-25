using EtiquetaFORNew;
using EtiquetaFORNew.Data;
using EtiquetaFORNew.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;


namespace EtiquetaFORNew
{
    public partial class FormPrincipal : Form
    {
        private List<Produto> produtos = new List<Produto>();
        private TemplateEtiqueta template;

        // ⭐ NOVO: Configuração de etiqueta atual
        private ConfiguracaoEtiqueta configuracaoAtual;

        // ⭐ NOVO: Campos transferidos de FormBuscaMercadoria
        private Timer timerBusca;
        private DataTable mercadorias;

        private static readonly string CAMINHO_CONFIGURACOES =
    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "EtiquetaFornew", "configuracoes.xml");

        private static readonly string CAMINHO_MODELOS_PAPEL =
    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "EtiquetaFornew", "modelos_papel.xml");

        public FormPrincipal()
        {
            InitializeComponent();
            template = new TemplateEtiqueta();
            CarregarUltimoTemplate();
            this.DoubleBuffered = true;
            this.Load += FormPrincipal_Load;
            ConfigurarBuscaMercadoria();
            cmbBuscaNome.KeyDown += ComboBoxBusca_KeyDown;
            cmbBuscaReferencia.KeyDown += ComboBoxBusca_KeyDown;
            cmbBuscaCodigo.KeyDown += ComboBoxBusca_KeyDown;
            CarregarTemplatesDisponiveis();
            CarregarConfiguracoesPapel();
            configuracaoAtual = CarregarConfiguracaoAtual();
            CarregarComboboxModelos();
            CarregarModelosPapel();
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
            CarregarTodasMercadorias();

            // ========================================
            // 🔹 ARREDONDAR BOTÕES
            // ========================================
            ArredondarBotao(btnDesigner, 12);
            ArredondarBotao(btnImprimir, 12);
            ArredondarBotao(btnBuscarMercadoria, 12);
            ArredondarBotao(btnAdicionar, 12);
            ArredondarBotao(btnCarregarTemplate, 12);
            ArredondarBotao(btnConfigPapel, 12);
            ArredondarBotao(BtnAdicionar2, 12);
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
            if (cmbConfiguracao.SelectedItem == null)
                return;

            // Se o item selecionado for o Configuracao Atual, não faz nada (mantém configuracaoAtual)
            if (cmbConfiguracao.SelectedItem.ToString() == "(Configuração Atual)")
            {
                // Opcional: Recarrega a configuração ativa do disco se houver dúvida.
                // configuracaoAtual = GerenciadorConfiguracoesEtiqueta.CarregarConfiguracaoAtiva();
                return;
            }

            // Se o item selecionado for um modelo de papel
            if (cmbConfiguracao.SelectedItem is ConfiguracaoPapel modeloSelecionado)
            {
                // 1. Carrega as dimensões do modelo de papel para a configuração atual
                configuracaoAtual = GerenciadorConfiguracoesEtiqueta.ConverterPapelParaConfig(
                    modeloSelecionado, configuracaoAtual?.ImpressoraPadrao
                );

                // 2. Atualiza a tela principal com as novas dimensões e nome
                //AtualizarCamposDaTelaComConfiguracaoAtual(); // (Assumindo que este método existe e atualiza os campos na tela)

                // Opcional: Exibir info
                // MessageBox.Show($"Modelo '{modeloSelecionado.NomePapel}' carregado.", "Sucesso");
            }
        }

        // ========================================
        // 🔹 SINCRONIZAR MERCADORIAS DO SQL SERVER
        // ========================================
        public void SincronizarMercadorias() // MUDADO PARA PUBLIC
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // Sincronizar todas as mercadorias (pode adicionar filtro se necessário)
                int total = LocalDatabaseManager.SincronizarMercadorias(); // Atualiza o banco local

                // ⭐ CHAVE DA SOLUÇÃO: RECARREGA O DATATABLE 'mercadorias' E ATUALIZA OS COMBOBOXES
                CarregarTodasMercadorias();

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
            //var formDesigner = new FormDesigner(template);
            //if (formDesigner.ShowDialog() == DialogResult.OK)
            //{
            //    template = formDesigner.ObterTemplate();
            //    MessageBox.Show("Template salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //1.Pergunta ao usuário a intenção
            // Sim = Novo, Não = Carregar, Cancelar = Sair
            // Pergunta ao usuário a intenção
            // SIM = NOVO Template / NÃO = CARREGAR Existente / CANCEL = Sair

            TemplateEtiqueta templateParaAbrir = null;
            using (var formMenu = new EtiquetaFORNew.Forms.FormMenuDesigner())
            {
                // Abre o formulário e verifica o resultado (Yes, No, ou Cancel)
                var escolha = formMenu.ShowDialog();

                if (escolha == DialogResult.Cancel)
                    return; // Usuário cancelou ou fechou a janela.

                if (escolha == DialogResult.Yes) // Escolheu NOVO (BtnNovo_Click)
                {
                    // Cria um template padrão inicial (ajuste as dimensões)
                    templateParaAbrir = new TemplateEtiqueta
                    {
                        Largura = 50,
                        Altura = 30,
                        Elementos = new List<ElementoEtiqueta>()
                    };
                }
                else if (escolha == DialogResult.No) // Escolheu CARREGAR (btnCarregar_Click)
                {
                    // 1. Abre a tela de lista de templates
                    using (var formLista = new FormListaTemplates())
                    {
                        if (formLista.ShowDialog() == DialogResult.OK)
                        {
                            string nomeTemplate = formLista.TemplateSelecionado;
                            // Assumindo que TemplateManager.CarregarTemplate existe
                            templateParaAbrir = TemplateManager.CarregarTemplate(nomeTemplate);
                        }
                        else
                        {
                            return; // Cancelou na lista de templates
                        }
                    }
                }
            }

            // Abre o Designer se um template foi preparado
            if (templateParaAbrir != null)
            {
                var formDesigner = new FormDesigner(templateParaAbrir);

                if (formDesigner.ShowDialog() == DialogResult.OK)
                {
                    // O FormDesigner salvou o template
                    MessageBox.Show("Template salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }


        }

        // ========================================
        // ⭐ MODIFICADO: IMPRIMIR COM CONFIGURAÇÃO
        // ========================================
        //private void btnImprimir_Click(object sender, EventArgs e)
        //{
        //    var produtosSelecionados = ObterProdutosSelecionados();
        //    if (produtosSelecionados.Count == 0)
        //    {
        //        MessageBox.Show("Selecione pelo menos um produto!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    if (template.Elementos.Count == 0)
        //    {
        //        MessageBox.Show("Configure o template primeiro usando o Designer!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    // ⭐ VERIFICA SE HÁ CONFIGURAÇÃO
        //    if (configuracaoAtual == null)
        //    {
        //        var resultado = MessageBox.Show(
        //            "Nenhuma configuração de impressão foi definida.\n\n" +
        //            "Deseja configurar agora?",
        //            "Configuração Necessária",
        //            MessageBoxButtons.YesNo,
        //            MessageBoxIcon.Question);

        //        if (resultado == DialogResult.Yes)
        //        {
        //            btnConfigPapel_Click(sender, e);
        //            return;
        //        }
        //        else
        //        {
        //            return;
        //        }

        //    }

        //    //// ⭐ PASSA A CONFIGURAÇÃO PARA O FORM DE IMPRESSÃO
        //    var formImpressao = new FormImpressao(produtosSelecionados, template, configuracaoAtual);
        //    formImpressao.ShowDialog();
        //}

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            // 1. OBTÉM O TEMPLATE SELECIONADO NA COMBOBOX (NOVA LÓGICA)
            if (cmbTemplates.SelectedItem == null || cmbTemplates.SelectedItem.ToString().Contains("Nenhum Template"))
            {
                MessageBox.Show("Por favor, selecione um template de etiqueta para impressão.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Carrega o template selecionado dinamicamente
            string nomeTemplateSelecionado = cmbTemplates.SelectedItem.ToString();
            TemplateEtiqueta templateAtual = TemplateManager.CarregarTemplate(nomeTemplateSelecionado);

            if (templateAtual == null)
            {
                MessageBox.Show($"Falha ao carregar o template: {nomeTemplateSelecionado}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. OBTÉM OS PRODUTOS SELECIONADOS
            var produtosSelecionados = ObterProdutosSelecionados();
            if (produtosSelecionados.Count == 0)
            {
                MessageBox.Show("Selecione pelo menos um produto!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. VALIDA TEMPLATE (AGORA USANDO templateAtual)
            if (templateAtual.Elementos.Count == 0)
            {
                MessageBox.Show("O template selecionado não possui elementos configurados. Configure-o primeiro usando o Designer!",
                                "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. VERIFICA SE HÁ CONFIGURAÇÃO DE PAPEL (LÓGICA EXISTENTE)
            if (configuracaoAtual == null)
            {
                var resultado = MessageBox.Show(
                    "Nenhuma configuração de impressão (papel/impressora) foi definida.\n\n" +
                    "Deseja configurar agora?",
                    "Configuração Necessária",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    btnConfigPapel_Click(sender, e);
                    // Se o usuário configurar e salvar, o configuracaoAtual será definido. 
                    // O botão btnConfigPapel_Click deve recarregar a tela para que o usuário clique em imprimir novamente.
                    return;
                }
                else
                {
                    return;
                }

            }

            // 5. ABRE O FORM DE IMPRESSÃO
            // Passa o template recém-carregado (templateAtual)
            using (var formImpressao = new FormImpressao(produtosSelecionados, templateAtual, configuracaoAtual))
            {
                formImpressao.ShowDialog();
            }
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
            //// Usa a configuração atual ou cria uma nova baseada no template
            //var configParaEditar = configuracaoAtual ?? new ConfiguracaoEtiqueta
            //{
            //    NomeEtiqueta = "Etiqueta Atual",
            //    ImpressoraPadrao = "BTP-L42(D)",
            //    PapelPadrao = "Tamanho do papel-SoftcomGondBar",
            //    LarguraEtiqueta = template.Largura,
            //    AlturaEtiqueta = template.Altura,
            //    NumColunas = 1,
            //    NumLinhas = 1,
            //    EspacamentoColunas = 0,
            //    EspacamentoLinhas = 0,
            //    MargemSuperior = 0,
            //    MargemInferior = 0,
            //    MargemEsquerda = 0,
            //    MargemDireita = 0
            //};

            //var formConfig = new FormConfigEtiqueta(configParaEditar);
            //if (formConfig.ShowDialog() == DialogResult.OK)
            //{
            //    configuracaoAtual = formConfig.Configuracao;

            //    // Atualiza o template com as novas dimensões
            //    template.Largura = configuracaoAtual.LarguraEtiqueta;
            //    template.Altura = configuracaoAtual.AlturaEtiqueta;

            //    // Salva como configuração padrão
            //    GerenciadorConfiguracoesEtiqueta.SalvarConfiguracaoPadrao(configuracaoAtual);

            //    // Atualiza a lista de configurações
            //    AtualizarListaConfiguracoes();

            //    MessageBox.Show($"✅ Configuração de etiqueta aplicada com sucesso!\n\n" +
            //        $"📏 Dimensões: {configuracaoAtual.LarguraEtiqueta} x {configuracaoAtual.AlturaEtiqueta} mm\n" +
            //        $"📐 Layout: {configuracaoAtual.NumColunas} coluna(s) x {configuracaoAtual.NumLinhas} linha(s)\n" +
            //        $"🖨️ Impressora: {configuracaoAtual.ImpressoraPadrao}",
            //        "Configuração Aplicada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            ConfiguracaoPapel papelParaAbrir = null;

            // 1. Abre o Menu de Configuração (NOVO ou CARREGAR)
            using (var formMenu = new FormMenuConfiguracao())
            {
                var escolha = formMenu.ShowDialog(this);

                if (escolha == DialogResult.Cancel)
                    return;

                if (escolha == DialogResult.Yes) // NOVO
                {
                    papelParaAbrir = GerenciadorConfiguracoesEtiqueta.ConverterConfigParaPapel(
                        GerenciadorConfiguracoesEtiqueta.CarregarConfiguracaoPadrao()
                    );
                    papelParaAbrir.NomePapel = "Nova Configuração";
                }
                else if (escolha == DialogResult.No) // CARREGAR
                {
                    using (var formListaConfig = new FormListaConfiguracoes())
                    {
                        if (formListaConfig.ShowDialog(this) == DialogResult.OK)
                        {
                            string nomeConfig = formListaConfig.ConfiguracaoSelecionada;
                            papelParaAbrir = GerenciadorConfiguracoesEtiqueta.CarregarConfiguracao(nomeConfig);
                        }
                        else
                        {
                            return;
                        }
                    }
                }

            }

            // 2. Abre o FormConfigEtiqueta (o editor)
            if (papelParaAbrir != null)
            {
                ConfiguracaoEtiqueta configParaEditar = GerenciadorConfiguracoesEtiqueta.ConverterPapelParaConfig(
                    papelParaAbrir, configuracaoAtual?.ImpressoraPadrao
                );

                ConfiguracaoEtiqueta novaConfig = GerenciadorConfiguracoesEtiqueta.AbrirDialogoConfiguracao(this, configParaEditar);

                if (novaConfig != null && novaConfig != configParaEditar)
                {
                    // O FormConfigEtiqueta já salvou a nova configuração no XML e como padrão.
                    configuracaoAtual = novaConfig;

                    // 3. ⭐ CORREÇÃO AQUI: Recarrega a lista do ComboBox chamando o método correto.
                    CarregarComboboxModelos();

                    // 4. Seleciona o item recém-salvo no ComboBox
                    // Passamos o nome da etiqueta/modelo que acabamos de salvar
                    SelecionarConfiguracaoNaLista(configuracaoAtual.NomeEtiqueta);

                    MessageBox.Show("Configuração de papel salva e atualizada com sucesso!", "Sucesso",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
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
        private void ConfigurarBuscaMercadoria()
        {
            // 1. Configurar Timer para delay na busca
            timerBusca = new Timer();
            timerBusca.Interval = 300; // 300ms de delay
            timerBusca.Tick += TimerBusca_Tick;

            // 2. Configurar ComboBoxes
            // Assumindo que os ComboBoxes se chamam: cmbBuscaNome, cmbBuscaReferencia, cmbBuscaCodigo

            // Configuração comum para todos os ComboBoxes (AutoCompleteSource deve ser CustomSource)
            Action<ComboBox> setupComboBox = (cmb) =>
            {
                if (cmb != null)
                {
                    cmb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    cmb.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    cmb.DropDownStyle = ComboBoxStyle.DropDown;
                    cmb.TextUpdate += cmbBusca_TextUpdate;
                }
            };

            setupComboBox(cmbBuscaNome);
            setupComboBox(cmbBuscaReferencia);
            setupComboBox(cmbBuscaCodigo);

            // Adicionar handlers de seleção
            if (cmbBuscaNome != null) cmbBuscaNome.SelectedIndexChanged += cmbBuscaNome_SelectedIndexChanged;
            if (cmbBuscaReferencia != null) cmbBuscaReferencia.SelectedIndexChanged += cmbBuscaReferencia_SelectedIndexChanged;
            if (cmbBuscaCodigo != null) cmbBuscaCodigo.SelectedIndexChanged += cmbBuscaCodigo_SelectedIndexChanged;
        }
        private void CarregarTodasMercadorias()
        {
            try
            {
                mercadorias = LocalDatabaseManager.BuscarMercadorias("");

                // Listas para AutoComplete
                AutoCompleteStringCollection acscNome = new AutoCompleteStringCollection();
                AutoCompleteStringCollection acscReferencia = new AutoCompleteStringCollection();
                AutoCompleteStringCollection acscCodigo = new AutoCompleteStringCollection();

                // ⭐ NOVO: Listas para popular os Items de cada ComboBox (corrige dropdown vazio)
                List<string> listaNome = new List<string>();
                List<string> listaReferencia = new List<string>();
                List<string> listaCodigo = new List<string>();

                foreach (DataRow row in mercadorias.Rows)
                {
                    string nome = row["Mercadoria"]?.ToString();
                    string referencia = row["CodFabricante"]?.ToString();
                    string codigo = row["CodigoMercadoria"]?.ToString();

                    if (!string.IsNullOrEmpty(nome)) { acscNome.Add(nome); listaNome.Add(nome); }
                    if (!string.IsNullOrEmpty(referencia)) { acscReferencia.Add(referencia); listaReferencia.Add(referencia); }
                    if (!string.IsNullOrEmpty(codigo)) { acscCodigo.Add(codigo); listaCodigo.Add(codigo); }
                }

                // 1. Configurar AutoComplete Custom Source
                if (cmbBuscaNome != null) cmbBuscaNome.AutoCompleteCustomSource = acscNome;
                if (cmbBuscaReferencia != null) cmbBuscaReferencia.AutoCompleteCustomSource = acscReferencia;
                if (cmbBuscaCodigo != null) cmbBuscaCodigo.AutoCompleteCustomSource = acscCodigo;

                // 2. ⭐ NOVO: Configurar Items para que a lista apareça ao clicar
                if (cmbBuscaNome != null)
                {
                    cmbBuscaNome.Items.Clear();
                    // Adicionamos valores únicos e ordenados
                    cmbBuscaNome.Items.AddRange(listaNome.Distinct().OrderBy(s => s).ToArray());
                }
                if (cmbBuscaReferencia != null)
                {
                    cmbBuscaReferencia.Items.Clear();
                    cmbBuscaReferencia.Items.AddRange(listaReferencia.Distinct().OrderBy(s => s).ToArray());
                }
                if (cmbBuscaCodigo != null)
                {
                    cmbBuscaCodigo.Items.Clear();
                    cmbBuscaCodigo.Items.AddRange(listaCodigo.Distinct().OrderBy(s => s).ToArray());
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar lista de mercadorias: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cmbBusca_TextUpdate(object sender, EventArgs e)
        {
            // Inicia/Reinicia o timer a cada tecla digitada
            timerBusca.Stop();
            timerBusca.Start();
        }
        private void TimerBusca_Tick(object sender, EventArgs e)
        {
            // O timer serve apenas para dar tempo do AutoComplete agir.
            timerBusca.Stop();
        }
        private void cmbBuscaNome_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBuscaNome.SelectedIndex != -1)
            {
                string termoSelecionado = cmbBuscaNome.SelectedItem.ToString();
                // ⭐ PASSANDO O COMBOBOX DE ORIGEM
                AdicionarProdutoSelecionado(termoSelecionado, "Mercadoria", cmbBuscaNome);
            }
        }
        private void cmbBuscaReferencia_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBuscaReferencia.SelectedIndex != -1)
            {
                string termoSelecionado = cmbBuscaReferencia.SelectedItem.ToString();
                // ⭐ PASSANDO O COMBOBOX DE ORIGEM
                AdicionarProdutoSelecionado(termoSelecionado, "CodFabricante", cmbBuscaReferencia);
            }
        }
        private void cmbBuscaCodigo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBuscaCodigo.SelectedIndex != -1)
            {
                string termoSelecionado = cmbBuscaCodigo.SelectedItem.ToString();
                // ⭐ PASSANDO O COMBOBOX DE ORIGEM
                AdicionarProdutoSelecionado(termoSelecionado, "CodigoMercadoria", cmbBuscaCodigo);
            }
        }
        // Em FormPrincipal.cs

        private void AdicionarProdutoSelecionado(string termo, string nomeCampo, ComboBox cmbOrigem)
        {
            if (string.IsNullOrEmpty(termo)) return;

            // Importante: Remove os eventos para evitar recursão ao setar .Text
            RemoverEventosSelecao();

            try
            {
                string termoFiltrado = termo.Replace("'", "''");
                DataRow[] resultados = mercadorias.Select($"{nomeCampo} = '{termoFiltrado}'");

                if (resultados.Length > 0)
                {
                    DataRow row = resultados[0];

                    // Obter dados
                    string codigo = row["CodigoMercadoria"]?.ToString();
                    string nome = row["Mercadoria"]?.ToString();
                    string referencia = row["CodFabricante"]?.ToString();
                    decimal preco = row["PrecoVenda"] != DBNull.Value ? Convert.ToDecimal(row["PrecoVenda"]) : 0m;

                    // 1. SINCRONIZAR OS COMBOBOXES (Atualiza as 3 buscas)
                    cmbBuscaNome.Text = nome;
                    cmbBuscaReferencia.Text = referencia;
                    cmbBuscaCodigo.Text = codigo;

                    // 2. PREENCHER OS CAMPOS DE CADASTRO MANUAL
                    txtNome.Text = nome;
                    txtCodigo.Text = codigo;
                    txtPreco.Text = preco.ToString("F2");
                    numQtd.Value = 1; // Define quantidade inicial como 1

                    // 3. ⭐ Foco no botão de Adicionar. O próximo ENTER acionará este botão.
                    btnAdicionar.Focus();
                }
                else
                {
                    MessageBox.Show($"Nenhum produto encontrado com o valor '{termo}' no campo '{nomeCampo}'.", "Busca Vazia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao processar o produto: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Adiciona os eventos de volta
                AdicionarEventosSelecao();
            }
        }
        private void AdicionarProdutoNaLista(Produto produto)
        {
            // Implementação Placeholder: Substitua pela sua lógica real de adição ao DataGridView.
            // O ideal é adicionar à lista 'produtos' e redefinir o DataSource do dgvProdutos.

            // 1. Adicionar à lista interna
            produtos.Add(produto);

            // 2. Atualizar o DataGridView (assumindo que o controle se chama dgvProdutos)
            // Se você usar BindingSource, a atualização é automática. Caso contrário:
            dgvProdutos.DataSource = null;
            dgvProdutos.DataSource = produtos;

            // ... (Atualizar resumo/total)
        }
        private void RemoverEventosSelecao()
        {
            if (cmbBuscaNome != null) cmbBuscaNome.SelectedIndexChanged -= cmbBuscaNome_SelectedIndexChanged;
            if (cmbBuscaReferencia != null) cmbBuscaReferencia.SelectedIndexChanged -= cmbBuscaReferencia_SelectedIndexChanged;
            if (cmbBuscaCodigo != null) cmbBuscaCodigo.SelectedIndexChanged -= cmbBuscaCodigo_SelectedIndexChanged;
        }
        private void AdicionarEventosSelecao()
        {
            if (cmbBuscaNome != null) cmbBuscaNome.SelectedIndexChanged += cmbBuscaNome_SelectedIndexChanged;
            if (cmbBuscaReferencia != null) cmbBuscaReferencia.SelectedIndexChanged += cmbBuscaReferencia_SelectedIndexChanged;
            if (cmbBuscaCodigo != null) cmbBuscaCodigo.SelectedIndexChanged += cmbBuscaCodigo_SelectedIndexChanged;
        }

        private void BtnAdicionar2_Click(object sender, EventArgs e)
        {
            AdicionarProdutoPelaBusca();
        }
        private void AdicionarProdutoPelaBusca()
        {
            // Lógica de Validação (Reutilizada da resposta anterior)
            if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Nome e Código são obrigatórios!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal precoDecimal;
            // O CultureInfo.InvariantCulture e Replace(",", ".") garantem que o preço seja lido corretamente
            if (!decimal.TryParse(txtPreco.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out precoDecimal))
            {
                MessageBox.Show("Preço inválido!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Criação do objeto Produto
            var produto = new Produto
            {
                Nome = txtNome.Text,
                Codigo = txtCodigo.Text,
                Preco = precoDecimal,
                Quantidade = (int)numQtd.Value // Assume que numQtd é o seu NumericUpDown de quantidade
            };

            // Adiciona o produto à lista e ao DataGridView
            produtos.Add(produto); // Assume que 'produtos' é o List<Produto>
            dgvProdutos.Rows.Add(false, produto.Nome, produto.Codigo, produto.Preco.ToString("C2"), produto.Quantidade);

            // Limpeza dos campos de cadastro manual
            txtNome.Clear();
            txtCodigo.Clear();
            txtPreco.Clear();
            numQtd.Value = 1;

            // Limpeza das ComboBoxes de busca (⭐ Essencial para que a busca funcione para o próximo item)
            if (cmbBuscaNome != null) cmbBuscaNome.Text = "";
            if (cmbBuscaReferencia != null) cmbBuscaReferencia.Text = "";
            if (cmbBuscaCodigo != null) cmbBuscaCodigo.Text = "";

            // Foco para o próximo item
            cmbBuscaNome.Focus(); // ou o campo que você deseja que comece a próxima busca
        }
        private void ComboBoxBusca_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;

            if (e.KeyCode == Keys.Enter)
            {
                // 1. Bloqueia a propagação imediata do Enter
                e.Handled = true;
                e.SuppressKeyPress = true;

                string nomeCampo = GetNomeCampoBusca(cmb);
                if (nomeCampo == null) return;

                // Pega o texto atual (parcial ou completo) digitado pelo usuário.
                string termoDigitado = cmb.Text.Trim();
                string termoCompleto = termoDigitado; // Valor padrão para o caso de falha na busca

                if (string.IsNullOrWhiteSpace(termoDigitado)) return;

                // 2. FORÇA A FINALIZAÇÃO DO AUTOCOMPLETE (Ainda importante para atualizar índices)
                if (cmb.DroppedDown)
                {
                    cmb.DroppedDown = false;
                    Application.DoEvents(); // Força o processamento de eventos pendentes
                }

                // 3. TENTA PEGAR O NOME COMPLETO PELO SelectedItem
                if (cmb.SelectedIndex >= 0 && cmb.SelectedItem != null)
                {
                    // Tenta pegar a string completa do item que foi selecionado
                    termoCompleto = cmb.GetItemText(cmb.SelectedItem);
                }
                else
                {
                    // 4. BUSCA MANUALMENTE O NOME COMPLETO NA LISTA (A CHAVE DA CORREÇÃO)
                    // Itera sobre todos os itens e procura por um que comece com o que o usuário digitou.
                    foreach (object item in cmb.Items)
                    {
                        string itemText = cmb.GetItemText(item);

                        // Compara se o item completo da lista começa com o texto digitado
                        if (itemText.StartsWith(termoDigitado, StringComparison.OrdinalIgnoreCase))
                        {
                            // Encontramos o termo completo e correto (Ex: "Fone de Ouvido GameNote (s/fio)")
                            termoCompleto = itemText;
                            break;
                        }
                    }
                }

                // 5. ATUALIZA O TEXTO VISUAL DO COMBOBOX PARA O NOME COMPLETO
                // Isso resolve o problema de visualização truncada (opcional, mas recomendado).
                cmb.Text = termoCompleto;

                // 6. EXECUTA A LÓGICA DE SELEÇÃO com o termo garantido
                AdicionarProdutoSelecionado(termoCompleto, nomeCampo, cmb);

                // Move o foco para a quantidade ou próximo campo
                numQtd.Focus();
                numQtd.Select(0, numQtd.Text.Length);
            }
        }
        private string GetNomeCampoBusca(ComboBox cmb)
        {
            if (cmb == cmbBuscaNome) return "Mercadoria";
            if (cmb == cmbBuscaReferencia) return "CodFabricante";
            if (cmb == cmbBuscaCodigo) return "CodigoMercadoria";
            return null;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(
                    "Deseja sincronizar as mercadorias do SQL Server?\n\n" +
                    "Isso pode levar alguns minutos dependendo da quantidade de registros.",
                    "Confirmar Sincronização",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                Cursor = Cursors.WaitCursor;
                pictureBox2.Enabled = false;
                //pictureBox2.Text = "Sincronizando...";

                // 1. SINCRONIZAR O BANCO DE DADOS LOCAL
                int total = LocalDatabaseManager.SincronizarMercadorias();

                // ⭐ 2. RECARREGAR AS MERCADORIAS NA MEMÓRIA E ATUALIZAR OS COMBOBOXES
                CarregarTodasMercadorias();

                // Se a linha a seguir for para atualizar status no rodapé, mantenha-a.
                //CarregarEstatisticas();

                Cursor = Cursors.Default;
                pictureBox2.Enabled = true;
                //pictureBox2.Text = "🔄 Sincronizar";

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
                pictureBox2.Enabled = true;
                //btnSincronizar.Text = "🔄 Sincronizar";

                MessageBox.Show(
                    $"Erro ao sincronizar:\n\n{ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void CarregarTemplatesDisponiveis()
        {
            // Verifica se a ComboBox existe antes de usar (se foi adicionada no Designer)
            if (cmbTemplates == null) return;

            cmbTemplates.Items.Clear();

            try
            {
                // ⭐ Necessário: TemplateManager deve ter um método que retorne uma lista de nomes de templates
                // O FormListaTemplates implica que essa função existe.
                List<string> nomesTemplates = TemplateManager.ListarTemplates();

                if (nomesTemplates != null && nomesTemplates.Any())
                {
                    cmbTemplates.Items.AddRange(nomesTemplates.ToArray());

                    // Seleciona o primeiro item por padrão
                    if (cmbTemplates.Items.Count > 0)
                    {
                        cmbTemplates.SelectedIndex = 0;
                    }
                }
                else
                {
                    cmbTemplates.Items.Add("(Nenhum Template Encontrado)");
                    cmbTemplates.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar lista de templates: {ex.Message}");
            }
        }
        private void CarregarConfiguracoesPapel()
        {
            cmbConfiguracao.Items.Clear();

            // 1. Usa o Gerenciador para listar os nomes
            List<string> nomesConfig = GerenciadorConfiguracoesEtiqueta.ListarNomesConfiguracoes();

            if (nomesConfig != null && nomesConfig.Any())
            {
                cmbConfiguracao.Items.AddRange(nomesConfig.ToArray());
            }

            // 2. Tenta selecionar a última configuração salva como padrão
            if (configuracaoAtual != null)
            {
                SelecionarConfiguracaoNaLista(configuracaoAtual.PapelPadrao);
            }

            // 3. Se ainda não houver seleção, selecione o primeiro item
            if (cmbConfiguracao.Items.Count > 0 && cmbConfiguracao.SelectedIndex == -1)
            {
                cmbConfiguracao.SelectedIndex = 0;
            }

            // Carrega o objeto completo da configuração que foi selecionada/padrão
            CarregarConfiguracaoSelecionada();
        }

        /// <summary>
        /// Procura e seleciona um nome de configuração no ComboBox.
        /// </summary>
        private void SelecionarConfiguracaoNaLista(string nomeConfiguracao)
        {
            if (string.IsNullOrEmpty(nomeConfiguracao))
            {
                nomeConfiguracao = "(Configuração Atual)";
            }

            for (int i = 0; i < cmbConfiguracao.Items.Count; i++)
            {
                if (cmbConfiguracao.Items[i] is ConfiguracaoPapel papel)
                {
                    if (papel.NomePapel.Equals(nomeConfiguracao, StringComparison.OrdinalIgnoreCase)) // Compara com NomePapel
                    {
                        cmbConfiguracao.SelectedIndex = i;
                        return;
                    }
                }
                else if (cmbConfiguracao.Items[i].ToString().Equals(nomeConfiguracao, StringComparison.OrdinalIgnoreCase))
                {
                    cmbConfiguracao.SelectedIndex = i;
                    return;
                }
            }

            // Fallback
            if (cmbConfiguracao.Items.Count > 0 && cmbConfiguracao.Items[0].ToString() == "(Configuração Atual)")
            {
                cmbConfiguracao.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Carrega o objeto de configuração completo quando o usuário seleciona um item no ComboBox.
        /// </summary>
        private void CarregarConfiguracaoSelecionada()
        {
            if (cmbConfiguracao.SelectedItem == null) return;

            string nomeConfig = cmbConfiguracao.SelectedItem.ToString();

            // 1. Carrega o objeto ConfiguracaoPapel completo
            ConfiguracaoPapel papel = GerenciadorConfiguracoesEtiqueta.CarregarConfiguracao(nomeConfig);

            if (papel != null)
            {
                // 2. Define a impressora padrão (se já tiver uma, mantém)
                string impressoraPadraoAtual = configuracaoAtual != null ? configuracaoAtual.ImpressoraPadrao : null;

                // 3. Converte ConfiguracaoPapel para o objeto de trabalho (ConfiguracaoEtiqueta)
                configuracaoAtual = GerenciadorConfiguracoesEtiqueta.ConverterPapelParaConfig(papel, impressoraPadraoAtual);

                // 4. Salva a nova configuração como padrão (última usada)
                GerenciadorConfiguracoesEtiqueta.SalvarConfiguracaoPadrao(configuracaoAtual);

                // 5. Atualiza a exibição no form principal (se necessário)
                // AtualizarDisplayConfiguracao(configuracaoAtual); 
            }

        }
        private ConfiguracaoEtiqueta CarregarConfiguracaoAtual()
        {
            if (File.Exists(CAMINHO_CONFIGURACOES))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ConfiguracaoEtiqueta));
                    using (StreamReader reader = new StreamReader(CAMINHO_CONFIGURACOES))
                    {
                        return (ConfiguracaoEtiqueta)serializer.Deserialize(reader);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar configuração salva: {ex.Message}",
                                    "Erro de Leitura", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // Retorna uma configuração padrão (assumindo que ConfiguracaoEtiqueta tem um construtor sem argumentos)
            return new ConfiguracaoEtiqueta();
        }

        private List<ConfiguracaoPapel> CarregarModelosPapel()
        {
            if (!File.Exists(CAMINHO_MODELOS_PAPEL))
            {
                // Se o arquivo não existe, é normal retornar vazio.
                return new List<ConfiguracaoPapel>();
            }

            // ⭐ NOVO: Verificação de arquivo vazio
            FileInfo info = new FileInfo(CAMINHO_MODELOS_PAPEL);
            if (info.Length == 0)
            {
                // Se o arquivo estiver vazio (0 bytes), a desserialização falhará.
                // Isso pode indicar que o salvamento falhou ou o arquivo foi corrompido.
                return new List<ConfiguracaoPapel>();
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<ConfiguracaoPapel>));
                using (StreamReader reader = new StreamReader(CAMINHO_MODELOS_PAPEL))
                {
                    // Tenta desserializar
                    var modelos = (List<ConfiguracaoPapel>)serializer.Deserialize(reader);
                    return modelos ?? new List<ConfiguracaoPapel>(); // Garante que não retorne null
                }
            }
            catch (Exception ex)
            {
                // Se a leitura falhar, mostre o erro e retorne vazio
                MessageBox.Show($"Erro CRÍTICO ao ler o arquivo de modelos ({CAMINHO_MODELOS_PAPEL}). O arquivo pode estar corrompido ou o formato da classe mudou. Detalhes: {ex.Message}",
                                "Erro de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<ConfiguracaoPapel>();
            }
        }

        private void CarregarComboboxModelos()
        {
            // 1. Carrega os modelos salvos
            var modelos = CarregarModelosPapel();

            // 2. Limpa e popula o ComboBox
            cmbConfiguracao.Items.Clear();

            // Adiciona a opção de CONFIGURAÇÃO ATUAL
            cmbConfiguracao.Items.Add("(Configuração Atual)");

            // Adiciona TODAS as configurações salvas do arquivo
            foreach (var modelo in modelos)
            {
                cmbConfiguracao.Items.Add(modelo);
            }

            // 3. Chama a seleção
            //SelecionarConfiguracaoNaLista(nomeParaSelecionar);
        }




    }
}