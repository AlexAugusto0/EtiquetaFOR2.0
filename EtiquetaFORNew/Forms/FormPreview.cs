using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace EtiquetaFORNew
{
    public class FormPreview : Form
    {
        private TemplateEtiqueta template;
        private ConfiguracaoEtiqueta configuracao;
        private PictureBox pbPreview;
        private const float PIXELS_POR_MM = 3.0f;
        private string nomePapel;
        private float larguraPapelMM;
        private float alturaPapelMM;

        public FormPreview(TemplateEtiqueta template, ConfiguracaoEtiqueta configuracao,
                          string nomePapel, float larguraPapelMM, float alturaPapelMM)
        {
            this.template = template;
            this.configuracao = configuracao;
            this.nomePapel = nomePapel;
            this.larguraPapelMM = larguraPapelMM;
            this.alturaPapelMM = alturaPapelMM;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = string.Format("Preview de Impressão ({0} elementos - {1}x{2} - Papel: {3} [{4:0}x{5:0}mm])",
                template.Elementos.Count, configuracao.NumColunas, configuracao.NumLinhas,
                nomePapel, larguraPapelMM, alturaPapelMM);
            this.Size = new Size(1100, 850);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(70, 70, 70);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = false;

            // Painel superior
            Panel panelTop = new Panel();
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 60;
            panelTop.BackColor = Color.FromArgb(52, 73, 94);
            this.Controls.Add(panelTop);

            Label lblTitulo = new Label();
            lblTitulo.Text = "PREVIEW DE IMPRESSÃO";
            lblTitulo.Location = new Point(20, 15);
            lblTitulo.Size = new Size(400, 30);
            lblTitulo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            panelTop.Controls.Add(lblTitulo);

            // Botão Imprimir
            Button btnImprimir = new Button();
            btnImprimir.Text = "🖨 Imprimir";
            btnImprimir.Location = new Point(850, 15);
            btnImprimir.Size = new Size(120, 30);
            btnImprimir.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnImprimir.BackColor = Color.FromArgb(46, 204, 113);
            btnImprimir.ForeColor = Color.White;
            btnImprimir.FlatStyle = FlatStyle.Flat;
            btnImprimir.Cursor = Cursors.Hand;
            btnImprimir.FlatAppearance.BorderSize = 0;
            btnImprimir.Click += BtnImprimir_Click;
            panelTop.Controls.Add(btnImprimir);

            // Botão Fechar
            Button btnFechar = new Button();
            btnFechar.Text = "✕ Fechar";
            btnFechar.Location = new Point(980, 15);
            btnFechar.Size = new Size(100, 30);
            btnFechar.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnFechar.BackColor = Color.FromArgb(231, 76, 60);
            btnFechar.ForeColor = Color.White;
            btnFechar.FlatStyle = FlatStyle.Flat;
            btnFechar.Cursor = Cursors.Hand;
            btnFechar.FlatAppearance.BorderSize = 0;
            btnFechar.Click += delegate { this.Close(); };
            panelTop.Controls.Add(btnFechar);

            // Painel de scroll
            Panel panelScroll = new Panel();
            panelScroll.Dock = DockStyle.Fill;
            panelScroll.AutoScroll = true;
            panelScroll.BackColor = Color.FromArgb(70, 70, 70);
            panelScroll.Padding = new Padding(50, 80, 50, 50);  // Top maior
            this.Controls.Add(panelScroll);

            // PictureBox para desenhar
            pbPreview = new PictureBox();
            pbPreview.Location = new Point(50, 80);  // Top mais afastado
            pbPreview.BackColor = Color.White;
            pbPreview.BorderStyle = BorderStyle.FixedSingle;
            pbPreview.Paint += PbPreview_Paint;
            panelScroll.Controls.Add(pbPreview);

            CalcularTamanhoPreview();
        }

        private void CalcularTamanhoPreview()
        {
            // Usar as dimensões recebidas do papel
            int larguraPixels = (int)(larguraPapelMM * PIXELS_POR_MM);
            int alturaPixels = (int)(alturaPapelMM * PIXELS_POR_MM);
            pbPreview.Size = new Size(larguraPixels, alturaPixels);
        }

        private void PbPreview_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.Clear(Color.White);

            // Desenhar margens
            DesenharMargens(g);

            // Desenhar etiquetas
            for (int linha = 0; linha < configuracao.NumLinhas; linha++)
            {
                for (int coluna = 0; coluna < configuracao.NumColunas; coluna++)
                {
                    DesenharEtiqueta(g, linha, coluna);
                }
            }
        }

        private void DesenharMargens(Graphics g)
        {
            Pen penMargem = new Pen(Color.Red, 1);
            penMargem.DashStyle = DashStyle.Dot;

            if (configuracao.MargemSuperior > 0)
            {
                float y = configuracao.MargemSuperior * PIXELS_POR_MM;
                g.DrawLine(penMargem, 0, y, pbPreview.Width, y);
            }

            if (configuracao.MargemInferior > 0)
            {
                float y = (alturaPapelMM - configuracao.MargemInferior) * PIXELS_POR_MM;
                g.DrawLine(penMargem, 0, y, pbPreview.Width, y);
            }

            if (configuracao.MargemEsquerda > 0)
            {
                float x = configuracao.MargemEsquerda * PIXELS_POR_MM;
                g.DrawLine(penMargem, x, 0, x, pbPreview.Height);
            }

            if (configuracao.MargemDireita > 0)
            {
                float x = (larguraPapelMM - configuracao.MargemDireita) * PIXELS_POR_MM;
                g.DrawLine(penMargem, x, 0, x, pbPreview.Height);
            }

            penMargem.Dispose();
        }

        private void DesenharEtiqueta(Graphics g, int linha, int coluna)
        {
            // Posição em MM
            float xMM = configuracao.MargemEsquerda +
                       (coluna * (configuracao.LarguraEtiqueta + configuracao.EspacamentoColunas));
            float yMM = configuracao.MargemSuperior +
                       (linha * (configuracao.AlturaEtiqueta + configuracao.EspacamentoLinhas));

            // Converter para pixels
            float x = xMM * PIXELS_POR_MM;
            float y = yMM * PIXELS_POR_MM;
            float largura = configuracao.LarguraEtiqueta * PIXELS_POR_MM;
            float altura = configuracao.AlturaEtiqueta * PIXELS_POR_MM;

            RectangleF rectEtiqueta = new RectangleF(x, y, largura, altura);

            // Borda da etiqueta (mais destacada)
            Pen penBorda = new Pen(Color.FromArgb(100, 100, 100), 2);
            penBorda.DashStyle = DashStyle.Dash;
            g.DrawRectangle(penBorda, x, y, largura, altura);
            penBorda.Dispose();

            // Texto indicando "ETIQUETA" no centro
            Font fonte = new Font("Segoe UI", 10, FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.FromArgb(180, 180, 180));
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            string texto = string.Format("ETIQUETA\n{0}x{1} mm",
                configuracao.LarguraEtiqueta, configuracao.AlturaEtiqueta);
            g.DrawString(texto, fonte, brush, rectEtiqueta, sf);

            fonte.Dispose();
            brush.Dispose();
            sf.Dispose();
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDocument printDoc = new PrintDocument();

                // Configurar impressora se houver uma selecionada
                if (!string.IsNullOrEmpty(configuracao.ImpressoraPadrao))
                {
                    printDoc.PrinterSettings.PrinterName = configuracao.ImpressoraPadrao;
                }

                // Configurar tamanho do papel
                bool papelEncontrado = false;
                foreach (PaperSize paperSize in printDoc.PrinterSettings.PaperSizes)
                {
                    if (paperSize.PaperName == nomePapel)
                    {
                        printDoc.DefaultPageSettings.PaperSize = paperSize;
                        papelEncontrado = true;
                        break;
                    }
                }

                if (!papelEncontrado)
                {
                    MessageBox.Show("Papel " + nomePapel + " não encontrado na impressora.\nUsando papel padrão.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Configurar margens
                printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

                // Evento de impressão
                printDoc.PrintPage += PrintDoc_PrintPage;

                // Mostrar diálogo de impressão
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDoc;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDoc.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao imprimir: " + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // Calcular escala de impressão (100 DPI / 25.4 = pixels por mm)
            const float PIXELS_POR_MM_IMPRESSAO = 100f / 25.4f;

            // Desenhar etiquetas
            for (int linha = 0; linha < configuracao.NumLinhas; linha++)
            {
                for (int coluna = 0; coluna < configuracao.NumColunas; coluna++)
                {
                    DesenharEtiquetaImpressao(g, linha, coluna, PIXELS_POR_MM_IMPRESSAO);
                }
            }

            e.HasMorePages = false;
        }

        private void DesenharEtiquetaImpressao(Graphics g, int linha, int coluna, float pixelsPorMM)
        {
            // Posição em MM
            float xMM = configuracao.MargemEsquerda +
                       (coluna * (configuracao.LarguraEtiqueta + configuracao.EspacamentoColunas));
            float yMM = configuracao.MargemSuperior +
                       (linha * (configuracao.AlturaEtiqueta + configuracao.EspacamentoLinhas));

            // Converter para pixels de impressão
            float x = xMM * pixelsPorMM;
            float y = yMM * pixelsPorMM;
            float largura = configuracao.LarguraEtiqueta * pixelsPorMM;
            float altura = configuracao.AlturaEtiqueta * pixelsPorMM;

            RectangleF rectEtiqueta = new RectangleF(x, y, largura, altura);

            // Desenhar apenas a borda da etiqueta
            Pen penBorda = new Pen(Color.Black, 1);
            g.DrawRectangle(penBorda, x, y, largura, altura);
            penBorda.Dispose();

            // Desenhar texto "ETIQUETA" no centro
            Font fonte = new Font("Segoe UI", 10, FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            string texto = string.Format("ETIQUETA\n{0}x{1} mm",
                configuracao.LarguraEtiqueta, configuracao.AlturaEtiqueta);
            g.DrawString(texto, fonte, brush, rectEtiqueta, sf);

            fonte.Dispose();
            brush.Dispose();
            sf.Dispose();
        }

    }
}