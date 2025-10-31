using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace EtiquetaFORNew
{
    public partial class FormImpressao : Form
    {
        private List<Produto> produtos;
        private TemplateEtiqueta template;
        private int paginaAtual = 0;
        private List<List<Produto>> produtosPorPagina;
        private int paginaImpressaoAtual = 0;

        public FormImpressao(List<Produto> produtos, TemplateEtiqueta template)
        {
            InitializeComponent();
            this.produtos = produtos;
            this.template = template;
            CalcularPaginacao();
            DesenharVisualizacao();
        }

        private void CalcularPaginacao()
        {
            produtosPorPagina = new List<List<Produto>>();

            float larguraA4 = 210;
            float alturaA4 = 297;
            float margem = 10;

            int etiquetasPorLinha = (int)((larguraA4 - margem * 2) / (template.Largura + 2));
            int linhasPorPagina = (int)((alturaA4 - margem * 2) / (template.Altura + 2));
            int etiquetasPorPagina = etiquetasPorLinha * linhasPorPagina;

            List<Produto> etiquetas = new List<Produto>();
            foreach (var produto in produtos)
            {
                for (int i = 0; i < produto.Quantidade; i++)
                {
                    etiquetas.Add(produto);
                }
            }

            for (int i = 0; i < etiquetas.Count; i += etiquetasPorPagina)
            {
                int count = Math.Min(etiquetasPorPagina, etiquetas.Count - i);
                produtosPorPagina.Add(etiquetas.GetRange(i, count));
            }

            if (produtosPorPagina.Count == 0)
            {
                produtosPorPagina.Add(new List<Produto>());
            }
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            MudarPagina(-1);
        }

        private void btnProxima_Click(object sender, EventArgs e)
        {
            MudarPagina(1);
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MudarPagina(int direcao)
        {
            paginaAtual += direcao;
            paginaAtual = Math.Max(0, Math.Min(paginaAtual, produtosPorPagina.Count - 1));

            DesenharVisualizacao();
            AtualizarBotoes();
        }

        private void AtualizarBotoes()
        {
            btnAnterior.Enabled = paginaAtual > 0;
            btnProxima.Enabled = paginaAtual < produtosPorPagina.Count - 1;
            lblInfo.Text = $"Página {paginaAtual + 1} de {produtosPorPagina.Count}";
        }

        private void DesenharVisualizacao()
        {
            panelVisualizacao.Controls.Clear();

            float escala = 2.8f;
            int larguraPagina = (int)(210 * escala);
            int alturaPagina = (int)(297 * escala);

            Panel paginaPanel = new Panel
            {
                Size = new Size(larguraPagina, alturaPagina),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point((panelVisualizacao.Width - larguraPagina) / 2, 20)
            };

            paginaPanel.Paint += (s, e) => DesenharPagina(e.Graphics, escala);

            panelVisualizacao.Controls.Add(paginaPanel);
            AtualizarBotoes();
        }

        private void DesenharPagina(Graphics g, float escala)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            float margem = 10 * escala;
            float xPos = margem;
            float yPos = margem;
            float larguraEtiqueta = template.Largura * escala;
            float alturaEtiqueta = template.Altura * escala;
            float espacamento = 2 * escala;

            int etiquetasPorLinha = (int)((210 * escala - margem * 2) / (larguraEtiqueta + espacamento));

            var produtosDaPagina = produtosPorPagina[paginaAtual];
            int count = 0;

            foreach (var produto in produtosDaPagina)
            {
                DesenharEtiqueta(g, produto, xPos, yPos, larguraEtiqueta, alturaEtiqueta, escala);

                count++;
                xPos += larguraEtiqueta + espacamento;

                if (count % etiquetasPorLinha == 0)
                {
                    xPos = margem;
                    yPos += alturaEtiqueta + espacamento;
                }
            }
        }

        private void DesenharEtiqueta(Graphics g, Produto produto, float x, float y, float largura, float altura, float escala)
        {
            g.DrawRectangle(new Pen(Color.LightGray, 1), x, y, largura, altura);

            foreach (var elem in template.Elementos)
            {
                DesenharElemento(g, elem, produto, x, y, escala);
            }
        }

        private void DesenharElemento(Graphics g, ElementoEtiqueta elem, Produto produto, float offsetX, float offsetY, float escala)
        {
            RectangleF bounds = new RectangleF(
                offsetX + elem.Bounds.X * escala,
                offsetY + elem.Bounds.Y * escala,
                elem.Bounds.Width * escala,
                elem.Bounds.Height * escala
            );

            Font fonte = new Font(elem.Fonte.FontFamily, elem.Fonte.Size * escala / 4, elem.Fonte.Style);

            switch (elem.Tipo)
            {
                case TipoElemento.Texto:
                    using (SolidBrush brush = new SolidBrush(elem.Cor))
                    {
                        StringFormat sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                        g.DrawString(elem.Conteudo ?? "Texto", fonte, brush, bounds, sf);
                    }
                    break;

                case TipoElemento.Campo:
                    string valor = ObterValorCampo(elem.Conteudo, produto);
                    using (SolidBrush brush = new SolidBrush(elem.Cor))
                    {
                        StringFormat sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                        g.DrawString(valor, fonte, brush, bounds, sf);
                    }
                    break;

                case TipoElemento.CodigoBarras:
                    DesenharCodigoBarras(g, produto.Codigo, bounds);
                    break;

                case TipoElemento.Imagem:
                    if (elem.Imagem != null)
                    {
                        g.DrawImage(elem.Imagem, bounds);
                    }
                    break;
            }
        }

        private string ObterValorCampo(string campo, Produto produto)
        {
            switch (campo)
            {
                case "Nome": return produto.Nome;
                case "Codigo": return produto.Codigo;
                case "Preco": return produto.Preco.ToString("C2");
                default: return "";
            }
        }

        private void DesenharCodigoBarras(Graphics g, string codigo, RectangleF bounds)
        {
            string codigoLimpo = new string(Array.FindAll(codigo.ToCharArray(), c => char.IsDigit(c)));
            if (string.IsNullOrEmpty(codigoLimpo)) codigoLimpo = "0000000000";
            if (codigoLimpo.Length < 8) codigoLimpo = codigoLimpo.PadLeft(8, '0');

            float larguraBarra = bounds.Width / (codigoLimpo.Length * 2);
            float alturaBarras = bounds.Height * 0.7f;

            for (int i = 0; i < codigoLimpo.Length; i++)
            {
                int digito = int.Parse(codigoLimpo[i].ToString());
                float larguraAtual = (digito % 2 == 0) ? larguraBarra * 1.5f : larguraBarra * 0.8f;
                g.FillRectangle(Brushes.Black, bounds.X + (i * larguraBarra * 2), bounds.Y, larguraAtual, alturaBarras);
            }

            using (Font fontBarcode = new Font("Courier New", bounds.Height * 0.15f))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(codigo, fontBarcode, Brushes.Black,
                    new RectangleF(bounds.X, bounds.Y + alturaBarras, bounds.Width, bounds.Height - alturaBarras), sf);
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            pd.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
            pd.PrintPage += ImprimirPagina;

            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = pd;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pd.Print();
                    MessageBox.Show("Impressão enviada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao imprimir: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ImprimirPagina(object sender, PrintPageEventArgs e)
        {
            if (paginaImpressaoAtual >= produtosPorPagina.Count)
            {
                e.HasMorePages = false;
                paginaImpressaoAtual = 0;
                return;
            }

            Graphics g = e.Graphics;
            float escala = g.DpiX / 25.4f;

            float xPos = e.MarginBounds.Left;
            float yPos = e.MarginBounds.Top;
            float larguraEtiqueta = template.Largura * escala;
            float alturaEtiqueta = template.Altura * escala;
            float espacamento = 2 * escala;

            int etiquetasPorLinha = (int)(e.MarginBounds.Width / (larguraEtiqueta + espacamento));

            var produtosDaPagina = produtosPorPagina[paginaImpressaoAtual];
            int count = 0;

            foreach (var produto in produtosDaPagina)
            {
                DesenharEtiqueta(g, produto, xPos, yPos, larguraEtiqueta, alturaEtiqueta, escala);

                count++;
                xPos += larguraEtiqueta + espacamento;

                if (count % etiquetasPorLinha == 0)
                {
                    xPos = e.MarginBounds.Left;
                    yPos += alturaEtiqueta + espacamento;
                }
            }

            paginaImpressaoAtual++;
            e.HasMorePages = paginaImpressaoAtual < produtosPorPagina.Count;
        }
    }
}