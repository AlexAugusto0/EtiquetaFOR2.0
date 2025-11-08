using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace SistemaEtiquetas
{
    public partial class FormConfigEtiqueta : Form
    {
        private ConfiguracaoEtiqueta configuracao;
        private const float ESCALA_PREVIEW = 3.0f;
        private System.Drawing.Printing.PaperSize papelSelecionado;

        // Caminho para salvar as configurações
        private static readonly string CAMINHO_CONFIGURACOES =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SistemaEtiquetas", "configuracoes.xml");

        public ConfiguracaoEtiqueta Configuracao => configuracao;

        public FormConfigEtiqueta(ConfiguracaoEtiqueta configAtual = null)
        {
            InitializeComponent();

            // Inicializa com configuração salva, atual, ou valores padrão
            if (configAtual != null)
            {
                configuracao = configAtual;
            }
            else
            {
                configuracao = CarregarConfiguracaoSalva() ?? new ConfiguracaoEtiqueta
                {
                    NomeEtiqueta = "Gondola com Barras",
                    ImpressoraPadrao = "BTP-L42(D)",
                    PapelPadrao = "Tamanho do papel-SoftcomGondBar",
                    LarguraEtiqueta = 100,
                    AlturaEtiqueta = 30,
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

            CarregarConfiguracoes();
            ConfigurarEventos();
            AtualizarPreview();
        }

        private void ConfigurarEventos()
        {
            // Eventos de mudança de valores
            txtNomeEtiqueta.TextChanged += (s, e) => AtualizarConfiguracao();

            // Quando a impressora mudar, recarrega os papéis disponíveis
            cmbImpressora.SelectedIndexChanged += (s, e) =>
            {
                CarregarTiposPapelDaImpressora();
                AtualizarConfiguracao();
            };

            cmbPapel.SelectedIndexChanged += (s, e) =>
            {
                AtualizarConfiguracao();
                AtualizarPreview();
            };

            numLargura.ValueChanged += (s, e) => { AtualizarConfiguracao(); AtualizarPreview(); };
            numAltura.ValueChanged += (s, e) => { AtualizarConfiguracao(); AtualizarPreview(); };
            numColunas.ValueChanged += (s, e) => { AtualizarConfiguracao(); AtualizarPreview(); };
            numLinhas.ValueChanged += (s, e) => { AtualizarConfiguracao(); AtualizarPreview(); };
            numEspacamentoColunas.ValueChanged += (s, e) => { AtualizarConfiguracao(); AtualizarPreview(); };
            numEspacamentoLinhas.ValueChanged += (s, e) => { AtualizarConfiguracao(); AtualizarPreview(); };
            numMargemSuperior.ValueChanged += (s, e) => { AtualizarConfiguracao(); AtualizarPreview(); };
            numMargemInferior.ValueChanged += (s, e) => { AtualizarConfiguracao(); AtualizarPreview(); };
            numMargemEsquerda.ValueChanged += (s, e) => { AtualizarConfiguracao(); AtualizarPreview(); };
            numMargemDireita.ValueChanged += (s, e) => { AtualizarConfiguracao(); AtualizarPreview(); };

            // Evento de pintura do preview
            panelPreview.Paint += PanelPreview_Paint;
        }

        private void CarregarConfiguracoes()
        {
            // Carrega valores na interface
            txtNomeEtiqueta.Text = configuracao.NomeEtiqueta;

            // Carrega impressoras disponíveis
            CarregarImpressoras();
            if (cmbImpressora.Items.Contains(configuracao.ImpressoraPadrao))
                cmbImpressora.SelectedItem = configuracao.ImpressoraPadrao;
            else if (cmbImpressora.Items.Count > 0)
                cmbImpressora.SelectedIndex = 0;

            // Carrega tipos de papel da impressora selecionada
            CarregarTiposPapelDaImpressora();
            if (cmbPapel.Items.Contains(configuracao.PapelPadrao))
                cmbPapel.SelectedItem = configuracao.PapelPadrao;
            else if (cmbPapel.Items.Count > 0)
                cmbPapel.SelectedIndex = 0;

            // Dimensões da etiqueta
            numLargura.Value = (decimal)configuracao.LarguraEtiqueta;
            numAltura.Value = (decimal)configuracao.AlturaEtiqueta;

            // Layout
            numColunas.Value = configuracao.NumColunas;
            numLinhas.Value = configuracao.NumLinhas;
            numEspacamentoColunas.Value = (decimal)configuracao.EspacamentoColunas;
            numEspacamentoLinhas.Value = (decimal)configuracao.EspacamentoLinhas;

            // Margens
            numMargemSuperior.Value = (decimal)configuracao.MargemSuperior;
            numMargemInferior.Value = (decimal)configuracao.MargemInferior;
            numMargemEsquerda.Value = (decimal)configuracao.MargemEsquerda;
            numMargemDireita.Value = (decimal)configuracao.MargemDireita;
        }

        private void CarregarImpressoras()
        {
            cmbImpressora.Items.Clear();

            // Adiciona impressoras instaladas no sistema
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                cmbImpressora.Items.Add(printer);
            }

            // Adiciona impressora padrão do exemplo
            if (!cmbImpressora.Items.Contains("BTP-L42(D)"))
                cmbImpressora.Items.Add("BTP-L42(D)");
        }

        private void CarregarTiposPapelDaImpressora()
        {
            cmbPapel.Items.Clear();

            // Verifica se há uma impressora selecionada
            if (cmbImpressora.SelectedItem == null)
            {
                cmbPapel.Items.Add("Selecione uma impressora primeiro");
                cmbPapel.Enabled = false;
                return;
            }

            cmbPapel.Enabled = true;
            string impressoraSelecionada = cmbImpressora.SelectedItem.ToString();

            try
            {
                // Cria PrinterSettings para a impressora selecionada
                System.Drawing.Printing.PrinterSettings printerSettings =
                    new System.Drawing.Printing.PrinterSettings();
                printerSettings.PrinterName = impressoraSelecionada;

                // Verifica se a impressora é válida
                if (!printerSettings.IsValid)
                {
                    MessageBox.Show($"A impressora '{impressoraSelecionada}' não está disponível.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cmbPapel.Items.Add("Impressora não disponível");
                    return;
                }

                // Obtém os tamanhos de papel suportados pela impressora
                foreach (System.Drawing.Printing.PaperSize paperSize in printerSettings.PaperSizes)
                {
                    // Adiciona o nome do papel com suas dimensões (convertidas de centésimos de polegada para mm)
                    // 1 polegada = 25.4 mm, então dividimos por 100 (centésimos) e multiplicamos por 25.4
                    double larguraMM = (paperSize.Width / 100.0) * 25.4;
                    double alturaMM = (paperSize.Height / 100.0) * 25.4;

                    string itemTexto = $"{paperSize.PaperName} ({larguraMM:F0} x {alturaMM:F0} mm)";
                    cmbPapel.Items.Add(new PaperSizeItem(paperSize, itemTexto));
                }

                // Se não encontrou papéis, adiciona mensagem
                if (cmbPapel.Items.Count == 0)
                {
                    cmbPapel.Items.Add("Nenhum tamanho de papel disponível");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar tamanhos de papel: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbPapel.Items.Add("Erro ao carregar papéis");
            }
        }

        private void AtualizarConfiguracao()
        {
            configuracao.NomeEtiqueta = txtNomeEtiqueta.Text;
            configuracao.ImpressoraPadrao = cmbImpressora.SelectedItem?.ToString() ?? "";

            // Armazena o PaperSize selecionado
            if (cmbPapel.SelectedItem is PaperSizeItem psi)
            {
                papelSelecionado = psi.PaperSize;
                configuracao.PapelPadrao = psi.DisplayText;
            }
            else
            {
                configuracao.PapelPadrao = cmbPapel.SelectedItem?.ToString() ?? "";
            }

            configuracao.LarguraEtiqueta = (float)numLargura.Value;
            configuracao.AlturaEtiqueta = (float)numAltura.Value;
            configuracao.NumColunas = (int)numColunas.Value;
            configuracao.NumLinhas = (int)numLinhas.Value;
            configuracao.EspacamentoColunas = (float)numEspacamentoColunas.Value;
            configuracao.EspacamentoLinhas = (float)numEspacamentoLinhas.Value;
            configuracao.MargemSuperior = (float)numMargemSuperior.Value;
            configuracao.MargemInferior = (float)numMargemInferior.Value;
            configuracao.MargemEsquerda = (float)numMargemEsquerda.Value;
            configuracao.MargemDireita = (float)numMargemDireita.Value;
        }

        private void AtualizarPreview()
        {
            panelPreview.Invalidate();
        }

        private void PanelPreview_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.LightGray);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Calcula dimensões totais do layout
            float larguraTotal = (configuracao.NumColunas * configuracao.LarguraEtiqueta) +
                                ((configuracao.NumColunas - 1) * configuracao.EspacamentoColunas) +
                                configuracao.MargemEsquerda + configuracao.MargemDireita;

            float alturaTotal = (configuracao.NumLinhas * configuracao.AlturaEtiqueta) +
                               ((configuracao.NumLinhas - 1) * configuracao.EspacamentoLinhas) +
                               configuracao.MargemSuperior + configuracao.MargemInferior;

            // Se há papel selecionado, mostra as dimensões do papel
            float larguraPapel = larguraTotal;
            float alturaPapel = alturaTotal;

            if (papelSelecionado != null)
            {
                // Converte dimensões do papel de centésimos de polegada para mm
                larguraPapel = (papelSelecionado.Width / 100.0f) * 25.4f;
                alturaPapel = (papelSelecionado.Height / 100.0f) * 25.4f;
            }

            // Calcula escala para caber na tela
            float escalaLargura = (panelPreview.Width - 20) / larguraPapel;
            float escalaAltura = (panelPreview.Height - 50) / alturaPapel;
            float escala = Math.Min(escalaLargura, escalaAltura);

            // Centraliza no painel
            float offsetX = (panelPreview.Width - larguraPapel * escala) / 2;
            float offsetY = 10;

            // Desenha fundo do papel (branco)
            using (Brush fundoPapelBrush = new SolidBrush(Color.White))
            {
                e.Graphics.FillRectangle(fundoPapelBrush,
                    offsetX, offsetY,
                    larguraPapel * escala,
                    alturaPapel * escala);
            }

            // Desenha borda do papel
            using (Pen paperPen = new Pen(Color.Black, 2))
            {
                e.Graphics.DrawRectangle(paperPen,
                    offsetX, offsetY,
                    larguraPapel * escala,
                    alturaPapel * escala);
            }

            // Desenha as margens
            using (Brush margemBrush = new SolidBrush(Color.FromArgb(50, 128, 128, 128)))
            {
                // Margem Superior
                if (configuracao.MargemSuperior > 0)
                {
                    e.Graphics.FillRectangle(margemBrush,
                        offsetX,
                        offsetY,
                        larguraPapel * escala,
                        configuracao.MargemSuperior * escala);
                }

                // Margem Inferior
                if (configuracao.MargemInferior > 0)
                {
                    e.Graphics.FillRectangle(margemBrush,
                        offsetX,
                        offsetY + (alturaPapel - configuracao.MargemInferior) * escala,
                        larguraPapel * escala,
                        configuracao.MargemInferior * escala);
                }

                // Margem Esquerda
                if (configuracao.MargemEsquerda > 0)
                {
                    e.Graphics.FillRectangle(margemBrush,
                        offsetX,
                        offsetY,
                        configuracao.MargemEsquerda * escala,
                        alturaPapel * escala);
                }

                // Margem Direita
                if (configuracao.MargemDireita > 0)
                {
                    e.Graphics.FillRectangle(margemBrush,
                        offsetX + (larguraPapel - configuracao.MargemDireita) * escala,
                        offsetY,
                        configuracao.MargemDireita * escala,
                        alturaPapel * escala);
                }
            }

            // Desenha etiquetas
            using (Pen etiquetaPen = new Pen(Color.FromArgb(231, 76, 60), 2))
            using (Brush etiquetaBrush = new SolidBrush(Color.FromArgb(30, 231, 76, 60)))
            {
                for (int linha = 0; linha < configuracao.NumLinhas; linha++)
                {
                    for (int coluna = 0; coluna < configuracao.NumColunas; coluna++)
                    {
                        float x = offsetX + (configuracao.MargemEsquerda +
                                            (coluna * (configuracao.LarguraEtiqueta + configuracao.EspacamentoColunas)))
                                            * escala;

                        float y = offsetY + (configuracao.MargemSuperior +
                                            (linha * (configuracao.AlturaEtiqueta + configuracao.EspacamentoLinhas)))
                                            * escala;

                        float largura = configuracao.LarguraEtiqueta * escala;
                        float altura = configuracao.AlturaEtiqueta * escala;

                        RectangleF etiqueta = new RectangleF(x, y, largura, altura);

                        // Preenche
                        e.Graphics.FillRectangle(etiquetaBrush, etiqueta);

                        // Contorno
                        e.Graphics.DrawRectangle(etiquetaPen, Rectangle.Round(etiqueta));

                        // Desenha "rascunho" de código de barras
                        if (altura > 15)
                        {
                            float barrasY = y + (altura * 0.3f);
                            float barrasAltura = altura * 0.4f;
                            float barrasX = x + (largura * 0.1f);
                            float barrasLargura = largura * 0.8f;

                            using (Pen barraPen = new Pen(Color.FromArgb(150, 231, 76, 60), 1))
                            {
                                // Desenha algumas barras simulando código de barras
                                for (float i = 0; i < barrasLargura; i += 3)
                                {
                                    if ((int)(i / 3) % 3 != 0)
                                    {
                                        e.Graphics.DrawLine(barraPen,
                                            barrasX + i, barrasY,
                                            barrasX + i, barrasY + barrasAltura);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Desenha informações do papel e layout
            using (Font infoFont = new Font("Segoe UI", 9))
            using (Brush textBrush = new SolidBrush(Color.DarkSlateGray))
            {
                string infoPapel = "";
                if (papelSelecionado != null)
                {
                    infoPapel = $"Papel: {larguraPapel:F0} x {alturaPapel:F0} mm";
                }

                string infoLayout = $"Layout: {larguraPapel:F1} x {alturaPapel:F1} mm | " +
                                   $"{configuracao.NumColunas}x{configuracao.NumLinhas} etiquetas";

                float yText = panelPreview.Height - 35;

                if (!string.IsNullOrEmpty(infoPapel))
                {
                    e.Graphics.DrawString(infoPapel, infoFont, textBrush, 10, yText);
                    yText += 18;
                }

                e.Graphics.DrawString(infoLayout, infoFont, textBrush, 10, yText);

                // Aviso se não caber
                if (larguraTotal > larguraPapel || alturaTotal > alturaPapel)
                {
                    using (Brush avisbBrush = new SolidBrush(Color.Red))
                    using (Font boldFont = new Font("Segoe UI", 9, FontStyle.Bold))
                    {
                        string aviso = "⚠️ ATENÇÃO: Layout NÃO cabe no papel!";
                        yText += 18;
                        e.Graphics.DrawString(aviso, boldFont, avisbBrush, 10, yText);
                    }
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(txtNomeEtiqueta.Text))
            {
                MessageBox.Show("Por favor, informe o nome da etiqueta.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNomeEtiqueta.Focus();
                return;
            }

            if (cmbImpressora.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, selecione uma impressora.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbImpressora.Focus();
                return;
            }

            if (cmbPapel.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, selecione um tipo de papel.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbPapel.Focus();
                return;
            }

            // Valida se o layout cabe no papel
            if (papelSelecionado != null)
            {
                float larguraPapel = (papelSelecionado.Width / 100.0f) * 25.4f;
                float alturaPapel = (papelSelecionado.Height / 100.0f) * 25.4f;

                float larguraTotal = (configuracao.NumColunas * configuracao.LarguraEtiqueta) +
                                    ((configuracao.NumColunas - 1) * configuracao.EspacamentoColunas) +
                                    configuracao.MargemEsquerda + configuracao.MargemDireita;

                float alturaTotal = (configuracao.NumLinhas * configuracao.AlturaEtiqueta) +
                                   ((configuracao.NumLinhas - 1) * configuracao.EspacamentoLinhas) +
                                   configuracao.MargemSuperior + configuracao.MargemInferior;

                if (larguraTotal > larguraPapel || alturaTotal > alturaPapel)
                {
                    DialogResult resultado = MessageBox.Show(
                        $"⚠️ O layout não cabe no papel selecionado:\n\n" +
                        $"Layout: {larguraTotal:F1}mm x {alturaTotal:F1}mm\n" +
                        $"Papel: {larguraPapel:F0}mm x {alturaPapel:F0}mm\n\n" +
                        $"Deseja continuar mesmo assim?",
                        "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (resultado == DialogResult.No)
                    {
                        return;
                    }
                }
            }

            // Pergunta se deseja salvar como modelo de papel
            DialogResult salvarModelo = MessageBox.Show(
                $"Deseja salvar esta configuração como modelo de papel?\n\n" +
                $"Isso permite reutilizar estas configurações later.",
                "Salvar Modelo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (salvarModelo == DialogResult.Yes)
            {
                // Abre dialog para entrar nome do modelo
                Form dialogNome = new Form
                {
                    Text = "Nome do Modelo de Papel",
                    Width = 400,
                    Height = 150,
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false
                };

                Label lblNome = new Label
                {
                    Text = "Nome do modelo:",
                    Left = 20,
                    Top = 20,
                    Width = 350
                };

                TextBox txtNomeModelo = new TextBox
                {
                    Left = 20,
                    Top = 45,
                    Width = 350,
                    Text = $"{configuracao.NomeEtiqueta} - {papelSelecionado?.PaperName ?? "Padrão"}"
                };

                Button btnOkNome = new Button
                {
                    Text = "✓ OK",
                    Left = 220,
                    Top = 80,
                    Width = 70,
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(46, 204, 113),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                Button btnCancelNome = new Button
                {
                    Text = "✕ Cancelar",
                    Left = 300,
                    Top = 80,
                    Width = 70,
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.FromArgb(149, 165, 166),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                dialogNome.Controls.Add(lblNome);
                dialogNome.Controls.Add(txtNomeModelo);
                dialogNome.Controls.Add(btnOkNome);
                dialogNome.Controls.Add(btnCancelNome);
                dialogNome.AcceptButton = btnOkNome;
                dialogNome.CancelButton = btnCancelNome;

                if (dialogNome.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(txtNomeModelo.Text))
                {
                    SalvarConfiguracacoPapel(txtNomeModelo.Text);
                    MessageBox.Show("Modelo de papel salvo com sucesso!",
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                dialogNome.Dispose();
            }

            // Salva as configurações principais
            SalvarConfiguracao();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnTestarImpressao_Click(object sender, EventArgs e)
        {
            TestarImpressao();
        }

        /// <summary>
        /// Executa teste de impressão com visualização prévia
        /// </summary>
        private void TestarImpressao()
        {
            try
            {
                // Validações básicas
                if (papelSelecionado == null)
                {
                    MessageBox.Show("Por favor, selecione um tipo de papel primeiro.",
                        "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cmbImpressora.SelectedIndex < 0)
                {
                    MessageBox.Show("Por favor, selecione uma impressora primeiro.",
                        "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cria PrintDocument com as configurações atuais
                System.Drawing.Printing.PrintDocument printDoc = new System.Drawing.Printing.PrintDocument();

                // Configura impressora
                printDoc.PrinterSettings.PrinterName = cmbImpressora.SelectedItem.ToString();

                // Configura papel
                printDoc.DefaultPageSettings.PaperSize = papelSelecionado;

                // IMPORTANTE: Reseta as margens para 0 (deixa a função ImprimirTesteEtiqueta controlar)
                printDoc.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);

                // Marca como visualização (teste)
                printDoc.PrintPage += PrintDoc_PrintPage;

                // Abre Print Preview Dialog
                System.Windows.Forms.PrintPreviewDialog previewDialog = new System.Windows.Forms.PrintPreviewDialog();
                previewDialog.Document = printDoc;
                previewDialog.ShowIcon = false;
                previewDialog.Text = $"Teste de Impressão - {this.txtNomeEtiqueta.Text}";
                previewDialog.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                previewDialog.ClientSize = new System.Drawing.Size(900, 700);

                if (previewDialog.ShowDialog(this) == DialogResult.OK)
                {
                    // Se usuário clicou em "Imprimir", executa
                    printDoc.Print();
                    MessageBox.Show("Teste de impressão enviado com sucesso!",
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                printDoc.Dispose();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Impressora não encontrada ou não disponível.",
                    "Erro de Impressora", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao testar impressão:\n{ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento de PrintPage para desenhar as etiquetas
        /// </summary>
        private void PrintDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            ImprimirTesteEtiqueta(e.Graphics, e);
        }

        /// <summary>
        /// Desenha etiquetas de teste para visualização de impressão
        /// </summary>
        private void ImprimirTesteEtiqueta(Graphics g, System.Drawing.Printing.PrintPageEventArgs e)
        {
            g.Clear(Color.White);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Usar PageBounds para obter as dimensões reais da página
            float pageWidthPixels = e.PageBounds.Width;
            float pageHeightPixels = e.PageBounds.Height;

            // Dimensões do papel em milímetros
            float paperWidthMm = (papelSelecionado.Width / 100.0f) * 25.4f;
            float paperHeightMm = (papelSelecionado.Height / 100.0f) * 25.4f;

            // Calcular pixels por milímetro (escala uniforme)
            float pixelsPorMmX = pageWidthPixels / paperWidthMm;
            float pixelsPorMmY = pageHeightPixels / paperHeightMm;
            float pixelsPorMm = Math.Min(pixelsPorMmX, pixelsPorMmY);  // Usar o MÍNIMO para manter proporções

            // Posição inicial com margens
            float xPosInicial = configuracao.MargemEsquerda * pixelsPorMm;
            float yPosInicial = configuracao.MargemSuperior * pixelsPorMm;

            // Dimensões das etiquetas em pixels
            float larguraEtiqueta = configuracao.LarguraEtiqueta * pixelsPorMm;
            float alturaEtiqueta = configuracao.AlturaEtiqueta * pixelsPorMm;

            // Espaçamento em pixels
            float espacamentoColunas = configuracao.EspacamentoColunas * pixelsPorMm;
            float espacamentoLinhas = configuracao.EspacamentoLinhas * pixelsPorMm;

            // Desenha grid de etiquetas conforme configuração
            using (Pen borderPen = new Pen(Color.Red, 2))
            using (Brush fillBrush = new SolidBrush(Color.FromArgb(30, 231, 76, 60)))
            {
                int numEtiqueta = 1;

                for (int linha = 0; linha < configuracao.NumLinhas; linha++)
                {
                    for (int coluna = 0; coluna < configuracao.NumColunas; coluna++)
                    {
                        // Calcula posição de cada etiqueta
                        float x = xPosInicial + (coluna * (larguraEtiqueta + espacamentoColunas));
                        float y = yPosInicial + (linha * (alturaEtiqueta + espacamentoLinhas));

                        // Desenha retângulo preenchido (como no preview)
                        RectangleF etiquetaRect = new RectangleF(x, y, larguraEtiqueta, alturaEtiqueta);
                        g.FillRectangle(fillBrush, etiquetaRect);
                        g.DrawRectangle(borderPen, x, y, larguraEtiqueta, alturaEtiqueta);

                        // Desenha código de barras simulado (como no preview)
                        if (alturaEtiqueta > 15)
                        {
                            float barrasY = y + (alturaEtiqueta * 0.3f);
                            float barrasAltura = alturaEtiqueta * 0.4f;
                            float barrasX = x + (larguraEtiqueta * 0.1f);
                            float barrasLargura = larguraEtiqueta * 0.8f;

                            using (Pen barraPen = new Pen(Color.FromArgb(150, 231, 76, 60), 1))
                            {
                                // Desenha barras simulando código de barras
                                for (float i = 0; i < barrasLargura; i += 3)
                                {
                                    if ((int)(i / 3) % 3 != 0)
                                    {
                                        g.DrawLine(barraPen,
                                            barrasX + i, barrasY,
                                            barrasX + i, barrasY + barrasAltura);
                                    }
                                }
                            }
                        }

                        numEtiqueta++;
                    }
                }
            }

            // Desenha informações de rodapé
            using (Font footerFont = new Font("Segoe UI", 8))
            using (Brush footerBrush = new SolidBrush(Color.DarkSlateGray))
            {
                string footer = $"Teste - {DateTime.Now:dd/MM/yyyy HH:mm} | " +
                              $"Papel: {paperWidthMm:F0}×{paperHeightMm:F0}mm | " +
                              $"Layout: {configuracao.NumColunas}×{configuracao.NumLinhas} | " +
                              $"Etiqueta: {configuracao.LarguraEtiqueta:F0}×{configuracao.AlturaEtiqueta:F0}mm";

                RectangleF footerRect = new RectangleF(
                    10,
                    e.PageBounds.Height - 25,
                    e.PageBounds.Width - 20,
                    20
                );

                g.DrawString(footer, footerFont, footerBrush, footerRect);
            }

            e.HasMorePages = false;
        }

        private void groupDimensoes_Enter(object sender, EventArgs e)
        {
        }

        private void groupMargens_Enter(object sender, EventArgs e)
        {
        }

        // ==================== GERENCIAMENTO DE PAPÉIS SALVOS ====================

        /// <summary>
        /// Abre dialog para listar e gerenciar papéis salvos
        /// </summary>
        private void ListarPapeisGerenciador()
        {
            try
            {
                // Cria lista de papéis salvos
                List<ConfiguracaoPapel> papeisSalvos = CarregarListaPapeisSalvos();

                if (papeisSalvos.Count == 0)
                {
                    MessageBox.Show("Nenhuma configuração de papel foi salva ainda.",
                        "Lista de Papéis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Cria formulário de listagem
                Form listaForm = new Form
                {
                    Text = "Gerenciador de Papéis Salvos",
                    Size = new System.Drawing.Size(600, 400),
                    StartPosition = FormStartPosition.CenterParent,
                    Font = new Font("Segoe UI", 9)
                };

                // ListBox para mostrar papéis
                ListBox listaPapeis = new ListBox
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(10),
                    SelectionMode = SelectionMode.One,
                    Font = new Font("Segoe UI", 10)
                };

                // Popula lista
                foreach (var papel in papeisSalvos)
                {
                    string item = $"{papel.NomePapel} ({papel.Largura:F0} x {papel.Altura:F0} mm) - " +
                                 $"Etiquetas: {papel.NumColunas}x{papel.NumLinhas}";
                    listaPapeis.Items.Add(papel);
                    listaPapeis.DisplayMember = "ToString";
                }

                // Painel de botões
                Panel painelBotoes = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 50,
                    BackColor = Color.FromArgb(240, 240, 240),
                    Padding = new Padding(5)
                };

                Button btnCarregar = new Button
                {
                    Text = "📂 Carregar",
                    Dock = DockStyle.Left,
                    Width = 100,
                    BackColor = Color.FromArgb(52, 152, 219),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                Button btnExcluir = new Button
                {
                    Text = "🗑️ Excluir",
                    Dock = DockStyle.Left,
                    Width = 100,
                    BackColor = Color.FromArgb(231, 76, 60),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Margin = new Padding(5, 0, 0, 0)
                };

                Button btnFechar = new Button
                {
                    Text = "Fechar",
                    Dock = DockStyle.Right,
                    Width = 100,
                    BackColor = Color.FromArgb(149, 165, 166),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                // Eventos dos botões
                btnCarregar.Click += (s, e) =>
                {
                    if (listaPapeis.SelectedItem is ConfiguracaoPapel papelSelecionado)
                    {
                        CarregarConfiguracacoPapel(papelSelecionado);
                        AtualizarPreview();
                        listaForm.Close();
                        MessageBox.Show("Configuração de papel carregada com sucesso!",
                            "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Por favor, selecione um papel.",
                            "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };

                btnExcluir.Click += (s, e) =>
                {
                    if (listaPapeis.SelectedItem is ConfiguracaoPapel papelExcluir)
                    {
                        DialogResult resultado = MessageBox.Show(
                            $"Deseja excluir a configuração:\n{papelExcluir.NomePapel}?",
                            "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (resultado == DialogResult.Yes)
                        {
                            ExcluirConfiguracacoPapel(papelExcluir.NomePapel);
                            listaPapeis.Items.Remove(papelExcluir);
                            MessageBox.Show("Configuração excluída com sucesso!",
                                "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Por favor, selecione um papel.",
                            "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                };

                btnFechar.Click += (s, e) => listaForm.Close();

                painelBotoes.Controls.Add(btnFechar);
                painelBotoes.Controls.Add(btnExcluir);
                painelBotoes.Controls.Add(btnCarregar);

                listaForm.Controls.Add(listaPapeis);
                listaForm.Controls.Add(painelBotoes);

                listaForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao listar papéis salvos:\n{ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Carrega lista de papéis salvos do arquivo
        /// </summary>
        private List<ConfiguracaoPapel> CarregarListaPapeisSalvos()
        {
            List<ConfiguracaoPapel> papeis = new List<ConfiguracaoPapel>();

            try
            {
                string caminhoLista = Path.Combine(
                    Path.GetDirectoryName(CAMINHO_CONFIGURACOES),
                    "papeis_salvos.xml");

                if (!File.Exists(caminhoLista))
                    return papeis;

                XmlSerializer serializer = new XmlSerializer(typeof(List<ConfiguracaoPapel>));
                using (StreamReader reader = new StreamReader(caminhoLista))
                {
                    papeis = (List<ConfiguracaoPapel>)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar papéis salvos: {ex.Message}");
            }

            return papeis;
        }

        /// <summary>
        /// Salva configuração de papel na lista
        /// </summary>
        private void SalvarConfiguracacoPapel(string nomePapel)
        {
            try
            {
                List<ConfiguracaoPapel> papeis = CarregarListaPapeisSalvos();

                // Verifica se já existe
                var papelExistente = papeis.FirstOrDefault(p => p.NomePapel == nomePapel);
                if (papelExistente != null)
                {
                    papeis.Remove(papelExistente);
                }

                // Cria nova configuração de papel
                float larguraPapel = 210;
                float alturaPapel = 297;

                if (papelSelecionado != null)
                {
                    larguraPapel = (papelSelecionado.Width / 100.0f) * 25.4f;
                    alturaPapel = (papelSelecionado.Height / 100.0f) * 25.4f;
                }

                ConfiguracaoPapel novoPapel = new ConfiguracaoPapel
                {
                    NomePapel = nomePapel,
                    NomeEtiqueta = configuracao.NomeEtiqueta,
                    Largura = larguraPapel,
                    Altura = alturaPapel,
                    NumColunas = configuracao.NumColunas,
                    NumLinhas = configuracao.NumLinhas,
                    EspacamentoColunas = configuracao.EspacamentoColunas,
                    EspacamentoLinhas = configuracao.EspacamentoLinhas,
                    MargemSuperior = configuracao.MargemSuperior,
                    MargemInferior = configuracao.MargemInferior,
                    MargemEsquerda = configuracao.MargemEsquerda,
                    MargemDireita = configuracao.MargemDireita,
                    DataCriacao = DateTime.Now
                };

                papeis.Add(novoPapel);

                // Salva lista
                string diretorio = Path.GetDirectoryName(CAMINHO_CONFIGURACOES);
                string caminhoLista = Path.Combine(diretorio, "papeis_salvos.xml");

                XmlSerializer serializer = new XmlSerializer(typeof(List<ConfiguracaoPapel>));
                using (StreamWriter writer = new StreamWriter(caminhoLista))
                {
                    serializer.Serialize(writer, papeis);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar configuração de papel:\n{ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Carrega configuração de papel salva
        /// </summary>
        private void CarregarConfiguracacoPapel(ConfiguracaoPapel papel)
        {
            try
            {
                configuracao.NomeEtiqueta = papel.NomeEtiqueta;
                configuracao.NumColunas = papel.NumColunas;
                configuracao.NumLinhas = papel.NumLinhas;
                configuracao.EspacamentoColunas = papel.EspacamentoColunas;
                configuracao.EspacamentoLinhas = papel.EspacamentoLinhas;
                configuracao.MargemSuperior = papel.MargemSuperior;
                configuracao.MargemInferior = papel.MargemInferior;
                configuracao.MargemEsquerda = papel.MargemEsquerda;
                configuracao.MargemDireita = papel.MargemDireita;

                // Atualiza UI
                txtNomeEtiqueta.Text = papel.NomeEtiqueta;
                numColunas.Value = papel.NumColunas;
                numLinhas.Value = papel.NumLinhas;
                numEspacamentoColunas.Value = (decimal)papel.EspacamentoColunas;
                numEspacamentoLinhas.Value = (decimal)papel.EspacamentoLinhas;
                numMargemSuperior.Value = (decimal)papel.MargemSuperior;
                numMargemInferior.Value = (decimal)papel.MargemInferior;
                numMargemEsquerda.Value = (decimal)papel.MargemEsquerda;
                numMargemDireita.Value = (decimal)papel.MargemDireita;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar configuração:\n{ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Exclui configuração de papel salva
        /// </summary>
        private void ExcluirConfiguracacoPapel(string nomePapel)
        {
            try
            {
                List<ConfiguracaoPapel> papeis = CarregarListaPapeisSalvos();
                papeis.RemoveAll(p => p.NomePapel == nomePapel);

                string diretorio = Path.GetDirectoryName(CAMINHO_CONFIGURACOES);
                string caminhoLista = Path.Combine(diretorio, "papeis_salvos.xml");

                XmlSerializer serializer = new XmlSerializer(typeof(List<ConfiguracaoPapel>));
                using (StreamWriter writer = new StreamWriter(caminhoLista))
                {
                    serializer.Serialize(writer, papeis);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao excluir configuração:\n{ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== PERSISTÊNCIA DE DADOS ====================

        /// <summary>
        /// Salva a configuração atual em arquivo XML
        /// </summary>
        private void SalvarConfiguracao()
        {
            try
            {
                // Cria diretório se não existir
                string diretorio = Path.GetDirectoryName(CAMINHO_CONFIGURACOES);
                if (!Directory.Exists(diretorio))
                {
                    Directory.CreateDirectory(diretorio);
                }

                // Serializa e salva
                XmlSerializer serializer = new XmlSerializer(typeof(ConfiguracaoEtiqueta));
                using (StreamWriter writer = new StreamWriter(CAMINHO_CONFIGURACOES))
                {
                    serializer.Serialize(writer, configuracao);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar configuração: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Carrega a configuração salva em arquivo XML
        /// </summary>
        private ConfiguracaoEtiqueta CarregarConfiguracaoSalva()
        {
            try
            {
                if (!File.Exists(CAMINHO_CONFIGURACOES))
                {
                    return null;
                }

                XmlSerializer serializer = new XmlSerializer(typeof(ConfiguracaoEtiqueta));
                using (StreamReader reader = new StreamReader(CAMINHO_CONFIGURACOES))
                {
                    return (ConfiguracaoEtiqueta)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar configuração salva: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }

    // Classe para armazenar a configuração da etiqueta
    [Serializable]
    public class ConfiguracaoEtiqueta
    {
        public string NomeEtiqueta { get; set; }
        public string ImpressoraPadrao { get; set; }
        public string PapelPadrao { get; set; }
        public float LarguraEtiqueta { get; set; }
        public float AlturaEtiqueta { get; set; }
        public int NumColunas { get; set; }
        public int NumLinhas { get; set; }
        public float EspacamentoColunas { get; set; }
        public float EspacamentoLinhas { get; set; }
        public float MargemSuperior { get; set; }
        public float MargemInferior { get; set; }
        public float MargemEsquerda { get; set; }
        public float MargemDireita { get; set; }
    }

    // Classe para armazenar configuração de papéis salvos
    [Serializable]
    public class ConfiguracaoPapel
    {
        public string NomePapel { get; set; }
        public string NomeEtiqueta { get; set; }
        public float Largura { get; set; }
        public float Altura { get; set; }
        public int NumColunas { get; set; }
        public int NumLinhas { get; set; }
        public float EspacamentoColunas { get; set; }
        public float EspacamentoLinhas { get; set; }
        public float MargemSuperior { get; set; }
        public float MargemInferior { get; set; }
        public float MargemEsquerda { get; set; }
        public float MargemDireita { get; set; }
        public DateTime DataCriacao { get; set; }

        public override string ToString()
        {
            return $"{NomePapel} ({Largura:F0}x{Altura:F0}mm) - {DataCriacao:dd/MM/yyyy}";
        }
    }

    // Classe auxiliar para armazenar informações do papel
    internal class PaperSizeItem
    {
        public System.Drawing.Printing.PaperSize PaperSize { get; set; }
        public string DisplayText { get; set; }

        public PaperSizeItem(System.Drawing.Printing.PaperSize paperSize, string displayText)
        {
            PaperSize = paperSize;
            DisplayText = displayText;
        }

        public override string ToString()
        {
            return DisplayText;
        }
    }
}