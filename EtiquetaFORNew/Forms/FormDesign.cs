using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace EtiquetaFORNew
{
    public partial class FormDesigner : Form
    {
        private TemplateEtiqueta template;
        private ElementoEtiqueta elementoSelecionado;
        private Point pontoInicial;
        private bool arrastando = false;
        private bool redimensionando = false;
        private string handleRedimensionamento = "";
        private float escala = 4.0f;
        private Point offsetArrastar;

        public FormDesigner(TemplateEtiqueta templateAtual)
        {
            InitializeComponent();

            this.template = new TemplateEtiqueta
            {
                Largura = templateAtual.Largura,
                Altura = templateAtual.Altura,
                Elementos = new List<ElementoEtiqueta>(templateAtual.Elementos.Select(e => ClonarElemento(e)))
            };

            this.KeyPreview = true; // Permite que o Form "escute" o teclado
            this.KeyDown += FormDesigner_KeyDown; // Liga o evento de pressionar tecla

            ConfigurarLimitesNumericUpDown();

            CarregarConfiguracoes();
            AtualizarListaElementos();

            // Configurar eventos do canvas
            panelCanvas.Paint += PanelCanvas_Paint;
            panelCanvas.MouseDown += PanelCanvas_MouseDown;
            panelCanvas.MouseMove += PanelCanvas_MouseMove;
            panelCanvas.MouseUp += PanelCanvas_MouseUp;

            // Salvar automaticamente ao fechar com OK
            this.FormClosing += FormDesigner_FormClosing;

        }

        private void FormDesigner_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                // Salvar como último template usado
                TemplateManager.SalvarUltimoTemplate(template);
            }
        }

        private void CarregarConfiguracoes()
        {
            numLargura.Value = (decimal)template.Largura;
            numAltura.Value = (decimal)template.Altura;
            AtualizarTamanhoCanvas();
        }

        private void AtualizarTamanhoCanvas()
        {
            if (numLargura == null || numAltura == null || panelCanvas == null)
                return;

            // ⭐ Validação de dimensões mínimas
            decimal larguraMinima = 5M;
            decimal alturaMinima = 5M;

            if (numLargura.Value < larguraMinima)
            {
                numLargura.Value = larguraMinima;
                MessageBox.Show(
                    $"A largura mínima permitida é {larguraMinima}mm.",
                    "Atenção",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (numAltura.Value < alturaMinima)
            {
                numAltura.Value = alturaMinima;
                MessageBox.Show(
                    $"A altura mínima permitida é {alturaMinima}mm.",
                    "Atenção",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            template.Largura = (float)numLargura.Value;
            template.Altura = (float)numAltura.Value;

            // ⭐ Ajusta a escala dinamicamente para etiquetas muito pequenas
            // Para etiquetas menores que 30mm, aumenta a escala para visualização
            if (template.Largura < 30 || template.Altura < 30)
            {
                escala = 8.0f; // Escala maior para etiquetas pequenas
            }
            else if (template.Largura < 60 || template.Altura < 60)
            {
                escala = 6.0f; // Escala média
            }
            else
            {
                escala = 4.0f; // Escala padrão
            }

            panelCanvas.Size = new Size(
                (int)(template.Largura * escala),
                (int)(template.Altura * escala)
            );

            panelCanvas.Invalidate();
        }

        private void btnTexto_Click(object sender, EventArgs e)
        {
            AdicionarElemento(TipoElemento.Texto);
        }

        private void btnCampoNome_Click(object sender, EventArgs e)
        {
            AdicionarCampo("Nome");
        }

        private void btnCampoCodigo_Click(object sender, EventArgs e)
        {
            AdicionarCampo("Codigo");
        }

        private void btnCampoPreco_Click(object sender, EventArgs e)
        {
            AdicionarCampo("Preco");
        }

        private void btnCodigoBarras_Click(object sender, EventArgs e)
        {
            AdicionarElemento(TipoElemento.CodigoBarras);
        }

        private void btnImagem_Click(object sender, EventArgs e)
        {
            AdicionarImagem();
        }

        private void AdicionarElemento(TipoElemento tipo)
        {
            var elemento = new ElementoEtiqueta
            {
                Tipo = tipo,
                Fonte = new Font("Arial", 10),
                Cor = Color.Black
            };

            if (tipo == TipoElemento.Texto)
            {
                elemento.Conteudo = "Texto";
                using (Graphics g = panelCanvas.CreateGraphics())
                {
                    SizeF tamanhoTexto = g.MeasureString(elemento.Conteudo, elemento.Fonte);

                    // ⭐ Ajusta o tamanho inicial baseado na etiqueta
                    int largura = Math.Min(
                        (int)(tamanhoTexto.Width / escala) + 2,
                        (int)template.Largura - 2
                    );
                    int altura = Math.Min(
                        (int)(tamanhoTexto.Height / escala) + 2,
                        (int)template.Altura - 2
                    );

                    elemento.Bounds = new Rectangle(1, 1, Math.Max(3, largura), Math.Max(2, altura));
                }
            }
            else if (tipo == TipoElemento.CodigoBarras)
            {
                elemento.Conteudo = "CodFabricante";

                // ⭐ Para etiquetas pequenas, ajusta proporcionalmente
                int largura = (int)(template.Largura * 0.8f);
                int altura = (int)(template.Altura * 0.4f);

                largura = Math.Max(10, Math.Min(largura, (int)template.Largura - 2));
                altura = Math.Max(5, Math.Min(altura, (int)template.Altura - 2));

                elemento.Bounds = new Rectangle(1, 1, largura, altura);
            }

            template.Elementos.Add(elemento);
            AtualizarListaElementos();
            panelCanvas.Invalidate();
        }

        private void AdicionarCampo(string campo)
        {
            var elemento = new ElementoEtiqueta
            {
                Tipo = TipoElemento.Campo,
                Conteudo = campo,
                Fonte = new Font("Arial", 8), // ⭐ Fonte menor para etiquetas pequenas
                Cor = Color.Black
            };

            string textoExemplo = "[" + campo + "]";
            using (Graphics g = panelCanvas.CreateGraphics())
            {
                SizeF tamanhoTexto = g.MeasureString(textoExemplo, elemento.Fonte);

                int largura = Math.Min(
                    (int)(tamanhoTexto.Width / escala) + 2,
                    (int)template.Largura - 2
                );
                int altura = Math.Min(
                    (int)(tamanhoTexto.Height / escala) + 2,
                    (int)template.Altura - 2
                );

                elemento.Bounds = new Rectangle(1, 1, Math.Max(3, largura), Math.Max(2, altura));
            }

            template.Elementos.Add(elemento);
            AtualizarListaElementos();
            panelCanvas.Invalidate();
        }

        private void AdicionarImagem()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Imagens|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var elemento = new ElementoEtiqueta
                    {
                        Tipo = TipoElemento.Imagem,
                        Imagem = Image.FromFile(ofd.FileName),
                        Bounds = new Rectangle(10, 10, 20, 15),
                        Conteudo = Path.GetFileName(ofd.FileName)
                    };

                    template.Elementos.Add(elemento);
                    AtualizarListaElementos();
                    panelCanvas.Invalidate();
                }
            }
        }

        private void numLargura_ValueChanged(object sender, EventArgs e)
        {
            AtualizarTamanhoCanvas();
        }

        private void numAltura_ValueChanged(object sender, EventArgs e)
        {
            AtualizarTamanhoCanvas();
        }

        private void cmbPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbPresets.SelectedIndex)
            {
                case 0: // Personalizado
                    break;
                case 1: // Etiqueta Pequena (Joias)
                    numLargura.Value = 20M;
                    numAltura.Value = 8M;
                    break;
                case 2: // Etiqueta Média (Gondola)
                    numLargura.Value = 50M;
                    numAltura.Value = 30M;
                    break;
                case 3: // Etiqueta Grande
                    numLargura.Value = 60M;
                    numAltura.Value = 40M;
                    break;
                case 4: // Etiqueta Prateleira
                    numLargura.Value = 70M;
                    numAltura.Value = 30M;
                    break;
                case 5: // Etiqueta A6
                    numLargura.Value = 100M;
                    numAltura.Value = 50M;
                    break;
            }
        }

        private void AtualizarListaElementos()
        {
            lstElementos.Items.Clear();
            for (int i = 0; i < template.Elementos.Count; i++)
            {
                var elem = template.Elementos[i];
                string descricao = $"{i + 1}. {elem.Tipo}";
                if (!string.IsNullOrEmpty(elem.Conteudo))
                {
                    descricao += $": {elem.Conteudo}";
                }
                lstElementos.Items.Add(descricao);
            }
        }

        private void lstElementos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstElementos.SelectedIndex >= 0)
            {
                elementoSelecionado = template.Elementos[lstElementos.SelectedIndex];
                CarregarPropriedades();
                panelCanvas.Invalidate();
            }
        }

        private void CarregarPropriedades()
        {
            if (elementoSelecionado == null) return;

            bool podeEditar = elementoSelecionado.Tipo != TipoElemento.Imagem;

            txtConteudo.Text = elementoSelecionado.Conteudo ?? "";
            txtConteudo.Enabled = elementoSelecionado.Tipo == TipoElemento.Texto;

            numFonte.Value = (decimal)elementoSelecionado.Fonte.Size;
            numFonte.Enabled = podeEditar;

            chkNegrito.Checked = elementoSelecionado.Negrito;
            chkNegrito.Enabled = podeEditar;

            chkItalico.Checked = elementoSelecionado.Italico;
            chkItalico.Enabled = podeEditar;

            btnCor.BackColor = elementoSelecionado.Cor;
            btnCor.Enabled = podeEditar;
        }

        private void txtConteudo_TextChanged(object sender, EventArgs e)
        {
            if (elementoSelecionado != null && elementoSelecionado.Tipo == TipoElemento.Texto)
            {
                elementoSelecionado.Conteudo = txtConteudo.Text;

                using (Graphics g = panelCanvas.CreateGraphics())
                {
                    SizeF tamanhoTexto = g.MeasureString(elementoSelecionado.Conteudo, elementoSelecionado.Fonte);
                    int novaLargura = Math.Min((int)(tamanhoTexto.Width / escala) + 2, (int)template.Largura - elementoSelecionado.Bounds.X);
                    int novaAltura = Math.Min((int)(tamanhoTexto.Height / escala) + 2, (int)template.Altura - elementoSelecionado.Bounds.Y);

                    var bounds = elementoSelecionado.Bounds;
                    bounds.Width = Math.Max(5, novaLargura);
                    bounds.Height = Math.Max(3, novaAltura);
                    elementoSelecionado.Bounds = bounds;
                }

                panelCanvas.Invalidate();
            }
        }

        private void numFonte_ValueChanged(object sender, EventArgs e)
        {
            if (elementoSelecionado != null)
            {
                FontStyle estilo = FontStyle.Regular;
                if (elementoSelecionado.Negrito) estilo |= FontStyle.Bold;
                if (elementoSelecionado.Italico) estilo |= FontStyle.Italic;

                elementoSelecionado.Fonte = new Font(elementoSelecionado.Fonte.FontFamily, (float)numFonte.Value, estilo);

                if (elementoSelecionado.Tipo == TipoElemento.Texto || elementoSelecionado.Tipo == TipoElemento.Campo)
                {
                    using (Graphics g = panelCanvas.CreateGraphics())
                    {
                        string texto = elementoSelecionado.Tipo == TipoElemento.Texto ?
                            elementoSelecionado.Conteudo :
                            "[" + elementoSelecionado.Conteudo + "]";

                        SizeF tamanhoTexto = g.MeasureString(texto, elementoSelecionado.Fonte);
                        int novaLargura = Math.Min((int)(tamanhoTexto.Width / escala) + 2, (int)template.Largura - elementoSelecionado.Bounds.X);
                        int novaAltura = Math.Min((int)(tamanhoTexto.Height / escala) + 2, (int)template.Altura - elementoSelecionado.Bounds.Y);

                        var bounds = elementoSelecionado.Bounds;
                        bounds.Width = Math.Max(5, novaLargura);
                        bounds.Height = Math.Max(3, novaAltura);
                        elementoSelecionado.Bounds = bounds;
                    }
                }

                panelCanvas.Invalidate();
            }
        }

        private void chkNegrito_CheckedChanged(object sender, EventArgs e)
        {
            if (elementoSelecionado != null)
            {
                elementoSelecionado.Negrito = chkNegrito.Checked;
                AtualizarFonte();
            }
        }

        private void chkItalico_CheckedChanged(object sender, EventArgs e)
        {
            if (elementoSelecionado != null)
            {
                elementoSelecionado.Italico = chkItalico.Checked;
                AtualizarFonte();
            }
        }

        private void AtualizarFonte()
        {
            if (elementoSelecionado == null) return;

            FontStyle estilo = FontStyle.Regular;
            if (elementoSelecionado.Negrito) estilo |= FontStyle.Bold;
            if (elementoSelecionado.Italico) estilo |= FontStyle.Italic;

            elementoSelecionado.Fonte = new Font(elementoSelecionado.Fonte.FontFamily, elementoSelecionado.Fonte.Size, estilo);
            panelCanvas.Invalidate();
        }

        private void btnCor_Click(object sender, EventArgs e)
        {
            if (elementoSelecionado != null)
            {
                using (ColorDialog cd = new ColorDialog())
                {
                    cd.Color = elementoSelecionado.Cor;
                    if (cd.ShowDialog() == DialogResult.OK)
                    {
                        elementoSelecionado.Cor = cd.Color;
                        btnCor.BackColor = cd.Color;
                        panelCanvas.Invalidate();
                    }
                }
            }
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            if (lstElementos.SelectedIndex >= 0)
            {
                if (MessageBox.Show("Deseja remover este elemento?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    template.Elementos.RemoveAt(lstElementos.SelectedIndex);
                    elementoSelecionado = null;
                    AtualizarListaElementos();
                    panelCanvas.Invalidate();
                }
            }
        }

        private void PanelCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using (Pen penBorda = new Pen(Color.FromArgb(41, 128, 185), 3))
            {
                g.DrawRectangle(penBorda, 0, 0, panelCanvas.Width - 1, panelCanvas.Height - 1);
            }

            using (Pen penGrid = new Pen(Color.FromArgb(220, 220, 220)))
            {
                for (int i = 0; i <= template.Largura; i += 5)
                {
                    int x = (int)(i * escala);
                    g.DrawLine(penGrid, x, 0, x, panelCanvas.Height);
                }
                for (int i = 0; i <= template.Altura; i += 5)
                {
                    int y = (int)(i * escala);
                    g.DrawLine(penGrid, 0, y, panelCanvas.Width, y);
                }
            }

            if (template.Elementos.Count == 0)
            {
                using (Font font = new Font("Segoe UI", 10, FontStyle.Italic))
                using (SolidBrush brush = new SolidBrush(Color.Gray))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString("Clique nos botões à esquerda para adicionar elementos",
                        font, brush, new RectangleF(0, 0, panelCanvas.Width, panelCanvas.Height), sf);
                }
            }

            foreach (var elem in template.Elementos)
            {
                DesenharElemento(g, elem, null);

                if (elem == elementoSelecionado)
                {
                    Rectangle bounds = ConverterParaPixels(elem.Bounds);
                    using (Pen penSelecao = new Pen(Color.Blue, 2))
                    {
                        penSelecao.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawRectangle(penSelecao, bounds);
                    }

                    DesenharHandles(g, bounds);
                }
            }
        }

        private void DesenharElemento(Graphics g, ElementoEtiqueta elem, Produto produto)
        {
            Rectangle bounds = ConverterParaPixels(elem.Bounds);

            switch (elem.Tipo)
            {
                case TipoElemento.Texto:
                    using (SolidBrush brush = new SolidBrush(elem.Cor))
                    {
                        StringFormat sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                        g.DrawString(elem.Conteudo ?? "Texto", elem.Fonte, brush, bounds, sf);
                    }
                    break;

                case TipoElemento.Campo:
                    string valor = ObterValorCampo(elem.Conteudo, produto);
                    using (SolidBrush brush = new SolidBrush(elem.Cor))
                    {
                        StringFormat sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
                        g.DrawString(valor, elem.Fonte, brush, bounds, sf);
                    }
                    break;

                case TipoElemento.CodigoBarras:
                    // ✅ CORREÇÃO: Usa ObterValorCampo, que retorna o placeholder "[CodFabricante]" 
                    // no designer (onde 'produto' é null) e o valor real na impressão.
                    string codigoBarras;

                    if (produto == null)
                    {
                        // No designer, exibe o placeholder (Ex: [CodFabricante])
                        codigoBarras = ObterValorCampo(elem.Conteudo, null);
                    }
                    else
                    {
                        // Na impressão, exibe o valor real
                        codigoBarras = ObterValorCampo(elem.Conteudo, produto);
                    }

                    DesenharCodigoBarras(g, codigoBarras, bounds);
                    break;

                case TipoElemento.Imagem:
                    if (elem.Imagem != null)
                    {
                        g.DrawImage(elem.Imagem, bounds);
                    }
                    else
                    {
                        g.FillRectangle(Brushes.LightGray, bounds);
                        g.DrawString("Imagem", new Font("Arial", 8), Brushes.Black, bounds);
                    }
                    break;
            }

            g.DrawRectangle(Pens.LightGray, bounds);
        }

        private string ObterValorCampo(string campo, Produto produto)
        {
            if (produto == null)
            {
                return $"[{campo}]";
            }

            switch (campo)
            {
                case "Nome": return produto.Nome;
                case "Codigo": return produto.Codigo;
                case "Preco": return produto.Preco.ToString("C2");
                case "CodFabricante": return produto.CodFabricante;
                default: return "";
            }
        }

        private void DesenharCodigoBarras(Graphics g, string codigo, Rectangle bounds)
        {
            string codigoLimpo = new string(Array.FindAll(codigo.ToCharArray(), c => char.IsDigit(c)));
            if (string.IsNullOrEmpty(codigoLimpo)) codigoLimpo = "0000000000";
            if (codigoLimpo.Length < 8) codigoLimpo = codigoLimpo.PadLeft(8, '0');

            float larguraBarra = (float)bounds.Width / (codigoLimpo.Length * 2);
            float alturaBarras = bounds.Height * 0.7f;

            for (int i = 0; i < codigoLimpo.Length; i++)
            {
                int digito = int.Parse(codigoLimpo[i].ToString());
                float larguraAtual = (digito % 2 == 0) ? larguraBarra * 1.5f : larguraBarra * 0.8f;
                g.FillRectangle(Brushes.Black, bounds.X + (i * larguraBarra * 2), bounds.Y, larguraAtual, alturaBarras);
            }

            using (Font fontBarcode = new Font("Courier New", 7))
            {
                StringFormat sf = new StringFormat { Alignment = StringAlignment.Center };
                g.DrawString(codigo, fontBarcode, Brushes.Black,
                    new RectangleF(bounds.X, bounds.Y + alturaBarras, bounds.Width, bounds.Height - alturaBarras), sf);
            }
        }

        private void DesenharHandles(Graphics g, Rectangle bounds)
        {
            int tamanhoHandle = 10;
            using (SolidBrush brush = new SolidBrush(Color.White))
            using (Pen pen = new Pen(Color.Blue, 2))
            {
                DesenharHandle(g, brush, pen, bounds.Left, bounds.Top, tamanhoHandle);
                DesenharHandle(g, brush, pen, bounds.Right, bounds.Top, tamanhoHandle);
                DesenharHandle(g, brush, pen, bounds.Left, bounds.Bottom, tamanhoHandle);
                DesenharHandle(g, brush, pen, bounds.Right, bounds.Bottom, tamanhoHandle);
                DesenharHandle(g, brush, pen, bounds.Left + bounds.Width / 2, bounds.Top, tamanhoHandle);
                DesenharHandle(g, brush, pen, bounds.Left + bounds.Width / 2, bounds.Bottom, tamanhoHandle);
                DesenharHandle(g, brush, pen, bounds.Left, bounds.Top + bounds.Height / 2, tamanhoHandle);
                DesenharHandle(g, brush, pen, bounds.Right, bounds.Top + bounds.Height / 2, tamanhoHandle);
            }
        }

        private void DesenharHandle(Graphics g, SolidBrush brush, Pen pen, int x, int y, int tamanho)
        {
            // Adiciona uma sombra leve ou borda dupla para visibilidade
            g.FillRectangle(Brushes.White, x - tamanho / 2, y - tamanho / 2, tamanho, tamanho);
            g.DrawRectangle(Pens.Black, x - tamanho / 2, y - tamanho / 2, tamanho, tamanho);
            // Desenha o interior menor para parecer profissional
            g.FillRectangle(Brushes.RoyalBlue, x - tamanho / 2 + 2, y - tamanho / 2 + 2, tamanho - 4, tamanho - 4);
        }

        private void PanelCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            pontoInicial = e.Location;

            for (int i = template.Elementos.Count - 1; i >= 0; i--)
            {
                var elem = template.Elementos[i];
                Rectangle bounds = ConverterParaPixels(elem.Bounds);

                string handle = ObterHandle(bounds, e.Location);
                if (handle != null && elem == elementoSelecionado)
                {
                    redimensionando = true;
                    handleRedimensionamento = handle;
                    panelCanvas.Cursor = Cursors.SizeAll;
                    return;
                }

                if (bounds.Contains(e.Location))
                {
                    elementoSelecionado = elem;
                    lstElementos.SelectedIndex = i;
                    arrastando = true;

                    offsetArrastar = new Point(e.X - bounds.X, e.Y - bounds.Y);

                    panelCanvas.Cursor = Cursors.SizeAll;
                    panelCanvas.Invalidate();
                    return;
                }
            }

            elementoSelecionado = null;
            lstElementos.SelectedIndex = -1;
            panelCanvas.Cursor = Cursors.Default;
            panelCanvas.Invalidate();
        }

        private void PanelCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (elementoSelecionado == null)
            {
                panelCanvas.Cursor = Cursors.Default;
                return;
            }

            if (arrastando)
            {
                int gridSize = 5;
                int novoX = e.X - offsetArrastar.X;
                int novoY = e.Y - offsetArrastar.Y;

                float xMM = novoX / escala;
                float yMM = novoY / escala;

                xMM = (float)Math.Round(xMM / gridSize) * gridSize; // Arredonda para 0, 5, 10...
                yMM = (float)Math.Round(yMM / gridSize) * gridSize;

                var bounds = elementoSelecionado.Bounds;
                bounds.X = (int)xMM;
                bounds.Y = (int)yMM;
                //bounds.X = (int)(novoX / escala);
                //bounds.Y = (int)(novoY / escala);

                bounds.X = Math.Max(0, Math.Min(bounds.X, (int)template.Largura - bounds.Width));
                bounds.Y = Math.Max(0, Math.Min(bounds.Y, (int)template.Altura - bounds.Height));

                elementoSelecionado.Bounds = bounds;
                panelCanvas.Invalidate();
            }
            else if (redimensionando)
            {
                var bounds = elementoSelecionado.Bounds;

                int mouseXmm = (int)(e.X / escala);
                int mouseYmm = (int)(e.Y / escala);

                mouseXmm = Math.Max(0, Math.Min(mouseXmm, (int)template.Largura));
                mouseYmm = Math.Max(0, Math.Min(mouseYmm, (int)template.Altura));

                int novoX = bounds.X;
                int novoY = bounds.Y;
                int novaLargura = bounds.Width;
                int novaAltura = bounds.Height;

                switch (handleRedimensionamento)
                {
                    case "TopLeft":
                        novoX = mouseXmm;
                        novoY = mouseYmm;
                        novaLargura = (bounds.X + bounds.Width) - mouseXmm;
                        novaAltura = (bounds.Y + bounds.Height) - mouseYmm;
                        break;
                    case "TopRight":
                        novoY = mouseYmm;
                        novaLargura = mouseXmm - bounds.X;
                        novaAltura = (bounds.Y + bounds.Height) - mouseYmm;
                        break;
                    case "BottomLeft":
                        novoX = mouseXmm;
                        novaLargura = (bounds.X + bounds.Width) - mouseXmm;
                        novaAltura = mouseYmm - bounds.Y;
                        break;
                    case "BottomRight":
                        novaLargura = mouseXmm - bounds.X;
                        novaAltura = mouseYmm - bounds.Y;
                        break;
                    case "Top":
                        novoY = mouseYmm;
                        novaAltura = (bounds.Y + bounds.Height) - mouseYmm;
                        break;
                    case "Bottom":
                        novaAltura = mouseYmm - bounds.Y;
                        break;
                    case "Left":
                        novoX = mouseXmm;
                        novaLargura = (bounds.X + bounds.Width) - mouseXmm;
                        break;
                    case "Right":
                        novaLargura = mouseXmm - bounds.X;
                        break;
                }

                if (novaLargura < 5)
                {
                    if (handleRedimensionamento.Contains("Left"))
                    {
                        novoX = (bounds.X + bounds.Width) - 5;
                    }
                    novaLargura = 5;
                }

                if (novaAltura < 3)
                {
                    if (handleRedimensionamento.Contains("Top"))
                    {
                        novoY = (bounds.Y + bounds.Height) - 3;
                    }
                    novaAltura = 3;
                }

                if (novoX < 0)
                {
                    novaLargura += novoX;
                    novoX = 0;
                }
                if (novoY < 0)
                {
                    novaAltura += novoY;
                    novoY = 0;
                }
                if (novoX + novaLargura > template.Largura)
                {
                    novaLargura = (int)template.Largura - novoX;
                }
                if (novoY + novaAltura > template.Altura)
                {
                    novaAltura = (int)template.Altura - novoY;
                }

                elementoSelecionado.Bounds = new Rectangle(novoX, novoY, novaLargura, novaAltura);
                panelCanvas.Invalidate();
            }
            else
            {
                Rectangle bounds = ConverterParaPixels(elementoSelecionado.Bounds);
                string handle = ObterHandle(bounds, e.Location);

                if (handle != null)
                {
                    switch (handle)
                    {
                        case "TopLeft":
                        case "BottomRight":
                            panelCanvas.Cursor = Cursors.SizeNWSE;
                            break;
                        case "TopRight":
                        case "BottomLeft":
                            panelCanvas.Cursor = Cursors.SizeNESW;
                            break;
                        case "Top":
                        case "Bottom":
                            panelCanvas.Cursor = Cursors.SizeNS;
                            break;
                        case "Left":
                        case "Right":
                            panelCanvas.Cursor = Cursors.SizeWE;
                            break;
                    }
                }
                else if (bounds.Contains(e.Location))
                {
                    panelCanvas.Cursor = Cursors.SizeAll;
                }
                else
                {
                    panelCanvas.Cursor = Cursors.Default;
                }
            }
        }

        private void PanelCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            arrastando = false;
            redimensionando = false;
            handleRedimensionamento = "";

            if (elementoSelecionado != null)
            {
                Rectangle bounds = ConverterParaPixels(elementoSelecionado.Bounds);
                string handle = ObterHandle(bounds, e.Location);

                if (handle != null)
                {
                }
                else if (bounds.Contains(e.Location))
                {
                    panelCanvas.Cursor = Cursors.SizeAll;
                }
                else
                {
                    panelCanvas.Cursor = Cursors.Default;
                }
            }
            else
            {
                panelCanvas.Cursor = Cursors.Default;
            }
        }

        private string ObterHandle(Rectangle bounds, Point ponto)
        {
            int margem = 12;

            if (Math.Abs(ponto.X - bounds.Left) <= margem && Math.Abs(ponto.Y - bounds.Top) <= margem)
                return "TopLeft";
            if (Math.Abs(ponto.X - bounds.Right) <= margem && Math.Abs(ponto.Y - bounds.Top) <= margem)
                return "TopRight";
            if (Math.Abs(ponto.X - bounds.Left) <= margem && Math.Abs(ponto.Y - bounds.Bottom) <= margem)
                return "BottomLeft";
            if (Math.Abs(ponto.X - bounds.Right) <= margem && Math.Abs(ponto.Y - bounds.Bottom) <= margem)
                return "BottomRight";

            if (Math.Abs(ponto.X - (bounds.Left + bounds.Width / 2)) <= margem && Math.Abs(ponto.Y - bounds.Top) <= margem)
                return "Top";
            if (Math.Abs(ponto.X - (bounds.Left + bounds.Width / 2)) <= margem && Math.Abs(ponto.Y - bounds.Bottom) <= margem)
                return "Bottom";
            if (Math.Abs(ponto.X - bounds.Left) <= margem && Math.Abs(ponto.Y - (bounds.Top + bounds.Height / 2)) <= margem)
                return "Left";
            if (Math.Abs(ponto.X - bounds.Right) <= margem && Math.Abs(ponto.Y - (bounds.Top + bounds.Height / 2)) <= margem)
                return "Right";

            return null;
        }

        private Rectangle ConverterParaPixels(Rectangle mmRect)
        {
            return new Rectangle(
                (int)(mmRect.X * escala),
                (int)(mmRect.Y * escala),
                (int)(mmRect.Width * escala),
                (int)(mmRect.Height * escala)
            );
        }

        private ElementoEtiqueta ClonarElemento(ElementoEtiqueta original)
        {
            return new ElementoEtiqueta
            {
                Tipo = original.Tipo,
                Conteudo = original.Conteudo,
                Bounds = original.Bounds,
                Fonte = (Font)original.Fonte.Clone(),
                Cor = original.Cor,
                Imagem = original.Imagem,
                Negrito = original.Negrito,
                Italico = original.Italico
            };
        }

        public TemplateEtiqueta ObterTemplate()
        {
            return template;
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
                    CarregarTemplate(templateCarregado);

                    MessageBox.Show($"Template '{nomeTemplate}' carregado com sucesso!",
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Falha ao carregar o template selecionado.",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
        
        public class CanvasPanel : Panel
        {
            public CanvasPanel()
            {
                this.DoubleBuffered = true;
                this.SetStyle(ControlStyles.UserPaint |
                              ControlStyles.AllPaintingInWmPaint |
                              ControlStyles.OptimizedDoubleBuffer, true);
                this.UpdateStyles();
            }
        }
        public void CarregarTemplate(TemplateEtiqueta novoTemplate)
        {
            // Atualiza o template atual
            this.template = new TemplateEtiqueta
            {
                Largura = novoTemplate.Largura,
                Altura = novoTemplate.Altura,
                Elementos = new List<ElementoEtiqueta>(novoTemplate.Elementos.Select(e => ClonarElemento(e)))
            };

            // Recarrega os controles visuais
            CarregarConfiguracoes();
            AtualizarListaElementos();

            // Limpa seleção atual
            elementoSelecionado = null;
            lstElementos.ClearSelected();

            // Re-renderiza o canvas
            panelCanvas.Invalidate();
        }
        private void ConfigurarLimitesNumericUpDown()
        {
            // ⭐ LARGURA
            numLargura.Minimum = 5M;      // Mínimo: 5mm (etiquetas muito pequenas)
            numLargura.Maximum = 500M;    // Máximo: 500mm (folhas grandes)
            numLargura.DecimalPlaces = 1; // Permite 0.5mm de precisão
            numLargura.Increment = 1M;    // Incremento de 1mm

            // ⭐ ALTURA - CORREÇÃO CRÍTICA PARA ETIQUETAS PEQUENAS
            numAltura.Minimum = 5M;       // Mínimo: 5mm (etiquetas de joias podem ter 8-10mm)
            numAltura.Maximum = 500M;     // Máximo: 500mm
            numAltura.DecimalPlaces = 1;  // Permite 0.5mm de precisão
            numAltura.Increment = 1M;     // Incremento de 1mm

            // Se você quiser permitir etiquetas ainda menores (3mm), use:
            // numAltura.Minimum = 3M;
            // numLargura.Minimum = 3M;
        }


        private void FormDesigner_KeyDown(object sender, KeyEventArgs e)
        {
            // 1. Se nada estiver selecionado, não faz nada
            if (elementoSelecionado == null) return;

            // 2. Se o usuário estiver digitando na caixa de texto de conteúdo, 
            // não queremos mover o elemento (queremos que ele digite o texto)
            if (txtConteudo.Focused) return;

            // Variável para saber se precisamos redesenhar a tela
            bool houveAlteracao = false;

            // Pega a posição atual (Bounds é uma struct, precisamos copiar, mudar e devolver)
            var novaPosicao = elementoSelecionado.Bounds;
            int passo = 1; // Quantos 'pixels' move por vez (ajuste fino)

            // Verifica as Setas (Movimento)
            if (e.KeyCode == Keys.Left)
            {
                novaPosicao.X -= passo;
                houveAlteracao = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                novaPosicao.X += passo;
                houveAlteracao = true;
            }
            if (e.KeyCode == Keys.Up)
            {
                novaPosicao.Y -= passo;
                houveAlteracao = true;
            }
            if (e.KeyCode == Keys.Down)
            {
                novaPosicao.Y += passo;
                houveAlteracao = true;
            }

            // Verifica o Delete (Remover)
            if (e.KeyCode == Keys.Delete)
            {
                // Chama o mesmo botão de remover que já existe
                btnRemover.PerformClick();
                return; // Sai do método pois o elemento já foi excluído
            }

            // Se moveu, atualiza o objeto e a tela
            if (houveAlteracao)
            {
                elementoSelecionado.Bounds = novaPosicao;
                panelCanvas.Invalidate(); // Força o Panel a se redesenhar na nova posição
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
{
    try
    {
        // Atualiza as dimensões no objeto template
        template.Largura = (float)numLargura.Value;
        template.Altura = (float)numAltura.Value;

        // Aqui você pode escolher onde salvar.
        // Se quiser salvar no mesmo arquivo do template original:
        string caminho = template.CaminhoArquivo; // precisa existir dentro de TemplateEtiqueta

        // Se TemplateEtiqueta não tiver esse campo, podemos salvar sempre como “último template”
        if (string.IsNullOrEmpty(caminho))
        {
            TemplateManager.SalvarUltimoTemplate(template);
            MessageBox.Show("Template salvo com sucesso (como último template usado).", 
                            "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Caso contrário, salve no mesmo arquivo
        TemplateManager.SalvarTemplate(template, caminho);

        MessageBox.Show("Template salvo com sucesso!", 
                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Erro ao salvar o template: {ex.Message}", 
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

    }
}