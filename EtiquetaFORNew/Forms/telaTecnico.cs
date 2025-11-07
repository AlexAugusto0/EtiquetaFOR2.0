using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Windows.Forms;

namespace EtiquetaFORNew
{
    public partial class telaTecnico : Form
    {
        private List<ImpressoraInfo> impressoras = new List<ImpressoraInfo>();
        private DriverInstaller driverInstaller;

        public telaTecnico()
        {
            InitializeComponent();

            // Inicializa o instalador de drivers
            driverInstaller = new DriverInstaller(this);

            CarregarImpressoras();
            this.Text = Main.AppInfo.GetTituloAplicacao();
            InicializarListView();

            // Aplica efeitos hover nos botões
            AplicarEfeitosHover();
        }

        private void AplicarEfeitosHover()
        {
            // Efeito hover para checkBox1 (Detecção Automática)
            checkBox1.MouseEnter += (s, e) =>
            {
                if (!checkBox1.Checked)
                    checkBox1.BackColor = Color.FromArgb(189, 224, 254);
            };
            checkBox1.MouseLeave += (s, e) =>
            {
                if (!checkBox1.Checked)
                    checkBox1.BackColor = Color.FromArgb(236, 240, 241);
            };

            // Efeito hover para checkBox2 (Instalação Manual)
            checkBox2.MouseEnter += (s, e) =>
            {
                if (!checkBox2.Checked)
                    checkBox2.BackColor = Color.FromArgb(162, 238, 195);
            };
            checkBox2.MouseLeave += (s, e) =>
            {
                if (!checkBox2.Checked)
                    checkBox2.BackColor = Color.FromArgb(236, 240, 241);
            };

            // Efeito hover para botões
            AplicarHoverBotao(btnProcurar, Color.FromArgb(52, 152, 219), Color.FromArgb(41, 128, 185));
            AplicarHoverBotao(btnInstalarDriver, Color.FromArgb(46, 204, 113), Color.FromArgb(39, 174, 96));
            AplicarHoverBotao(btnDownloadDriver, Color.FromArgb(46, 204, 113), Color.FromArgb(39, 174, 96));
        }

        private void AplicarHoverBotao(Button btn, Color corNormal, Color corHover)
        {
            btn.MouseEnter += (s, e) => { btn.BackColor = corHover; };
            btn.MouseLeave += (s, e) => { btn.BackColor = corNormal; };
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;

                // Mostra modo de detecção automática
                groupBoxDeteccao.Visible = true;
                groupBoxManual.Visible = false;

                // Atualiza cor do botão selecionado
                checkBox1.ForeColor = Color.White;
                checkBox2.ForeColor = Color.FromArgb(52, 73, 94);
            }
            else
            {
                groupBoxDeteccao.Visible = false;
                checkBox1.ForeColor = Color.FromArgb(52, 73, 94);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;

                // Mostra modo manual
                groupBoxManual.Visible = true;
                groupBoxDeteccao.Visible = false;

                // Atualiza cor do botão selecionado
                checkBox2.ForeColor = Color.White;
                checkBox1.ForeColor = Color.FromArgb(52, 73, 94);
            }
            else
            {
                groupBoxManual.Visible = false;
                checkBox2.ForeColor = Color.FromArgb(52, 73, 94);
            }
        }

        private void CarregarImpressoras()
        {
            try
            {
                impressoras = ImpressoraManager.CarregarImpressoras();

                if (impressoras == null || impressoras.Count == 0)
                {
                    MessageBox.Show(
                        "Nenhuma impressora foi carregada. Verifique o arquivo impressoras.json",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    impressoras = new List<ImpressoraInfo>();
                    return;
                }

                comboBox1.Items.Clear();
                foreach (var imp in impressoras)
                    comboBox1.Items.Add(imp.Nome);

                comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
                comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

                if (comboBox1.Items.Count > 0)
                    comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao carregar impressoras: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null) return;

            string selecionada = comboBox1.SelectedItem.ToString();
            var info = impressoras.Find(i => i.Nome == selecionada);

            if (info != null)
            {
                try
                {
                    if (pictureBox1.Image != null)
                    {
                        var imagemAnterior = pictureBox1.Image;
                        pictureBox1.Image = null;
                        imagemAnterior.Dispose();
                    }

                    pictureBox1.Image = info.ObterImagem();

                    if (pictureBox1.Image == null)
                    {
                        pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                    }
                    else
                    {
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Erro ao carregar imagem: {ex.Message}",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                btnDownloadDriver.Tag = info;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (btnDownloadDriver.Tag is ImpressoraInfo impressora)
            {
                driverInstaller.BaixarEInstalarDriver(impressora);
            }
            else
            {
                MessageBox.Show(
                    "Selecione uma impressora primeiro.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void InicializarListView()
        {
            listViewDispositivos.View = View.Details;
            listViewDispositivos.FullRowSelect = true;
            listViewDispositivos.GridLines = true;
            listViewDispositivos.Columns.Add("Nome", 300);
            listViewDispositivos.Columns.Add("Device ID", 350);
            listViewDispositivos.Columns.Add("Status", 130);
        }

        private void BuscarDispositivosDeImpressoras()
        {
            try
            {
                listViewDispositivos.Items.Clear();

                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE DeviceID LIKE '%USB%'");

                foreach (ManagementObject obj in searcher.Get())
                {
                    string nome = obj["Name"]?.ToString() ?? "Desconhecido";
                    string deviceId = obj["DeviceID"]?.ToString() ?? "-";
                    int? erro = obj["ConfigManagerErrorCode"] as int?;
                    string statusTexto = erro == 0 ? "✓ Instalado" : "✗ Sem driver";

                    if (deviceId.StartsWith("USBPRINT", StringComparison.OrdinalIgnoreCase))
                    {
                        var item = new ListViewItem(new[] { nome, deviceId, statusTexto });
                        item.Tag = new { DeviceId = deviceId, Info = obj };

                        if (erro != 0)
                        {
                            item.ForeColor = Color.FromArgb(231, 76, 60);
                            item.Font = new Font(item.Font, FontStyle.Bold);
                        }
                        else
                        {
                            item.ForeColor = Color.FromArgb(39, 174, 96);
                        }

                        listViewDispositivos.Items.Add(item);
                    }
                }

                if (listViewDispositivos.Items.Count == 0)
                {
                    MessageBox.Show(
                        "Nenhuma impressora USB detectada.\n\n" +
                        "Verifique se:\n" +
                        "• A impressora está ligada\n" +
                        "• O cabo USB está conectado\n" +
                        "• O Windows detectou o dispositivo",
                        "Nenhuma Impressora Encontrada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(
                        $"{listViewDispositivos.Items.Count} impressora(s) USB encontrada(s).",
                        "Busca Concluída",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao buscar impressoras: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnProcurar_Click(object sender, EventArgs e)
        {
            listViewDispositivos.Items.Clear();
            BuscarDispositivosDeImpressoras();
        }

        private void btnInstalarDriver_Click(object sender, EventArgs e)
        {
            if (listViewDispositivos.SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    "Selecione um dispositivo na lista para instalar o driver.",
                    "Nenhum Dispositivo Selecionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string nomeDispositivo = listViewDispositivos.SelectedItems[0].SubItems[0].Text;
            var impressoraEncontrada = TentarIdentificarImpressora(nomeDispositivo);

            if (impressoraEncontrada != null)
            {
                DialogResult resultado = MessageBox.Show(
                    $"Dispositivo: {nomeDispositivo}\n\n" +
                    $"Impressora identificada: {impressoraEncontrada.Nome}\n\n" +
                    $"Deseja baixar e instalar o driver?",
                    "Driver Identificado",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    driverInstaller.BaixarEInstalarDriver(impressoraEncontrada);
                }
            }
            else
            {
                MostrarSelecaoManualDriver(nomeDispositivo);
            }
        }

        private ImpressoraInfo TentarIdentificarImpressora(string nomeDispositivo)
        {
            string nomeNormalizado = nomeDispositivo.ToLower().Replace(" ", "");

            foreach (var impressora in impressoras)
            {
                string nomeImpressoraNormalizado = impressora.Nome.ToLower().Replace(" ", "");

                if (nomeNormalizado.Contains(nomeImpressoraNormalizado) ||
                    nomeImpressoraNormalizado.Contains(nomeNormalizado))
                {
                    return impressora;
                }

                string[] partesDispositivo = nomeDispositivo.ToLower().Split(' ');
                string[] partesImpressora = impressora.Nome.ToLower().Split(' ');

                int correspondencias = partesDispositivo.Count(pd =>
                    partesImpressora.Any(pi => pi.Contains(pd) || pd.Contains(pi)));

                if (correspondencias >= 2)
                {
                    return impressora;
                }
            }

            return null;
        }

        private void MostrarSelecaoManualDriver(string nomeDispositivo)
        {
            using (FormSelecaoDriver formSelecao = new FormSelecaoDriver(impressoras, nomeDispositivo))
            {
                if (formSelecao.ShowDialog(this) == DialogResult.OK)
                {
                    var impressoraSelecionada = formSelecao.ImpressoraSelecionada;
                    if (impressoraSelecionada != null)
                    {
                        driverInstaller.BaixarEInstalarDriver(impressoraSelecionada);
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            base.OnFormClosing(e);
        }
    }

    /// <summary>
    /// Formulário para seleção manual do driver
    /// </summary>
    public class FormSelecaoDriver : Form
    {
        private ComboBox comboImpressoras;
        private Button btnOK;
        private Button btnCancelar;
        private Label lblInfo;
        private PictureBox picturePreview;

        public ImpressoraInfo ImpressoraSelecionada { get; private set; }

        public FormSelecaoDriver(List<ImpressoraInfo> impressoras, string nomeDispositivo)
        {
            InitializeComponent(nomeDispositivo);
            CarregarImpressoras(impressoras);
        }

        private void InitializeComponent(string nomeDispositivo)
        {
            this.Text = "Selecionar Driver Manualmente";
            this.Size = new Size(550, 420);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Título
            Label lblTitulo = new Label
            {
                Text = "Seleção Manual de Driver",
                Location = new Point(20, 20),
                Size = new Size(500, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            // Info do dispositivo
            lblInfo = new Label
            {
                Text = $"Dispositivo detectado:\n{nomeDispositivo}\n\nSelecione o modelo correto da impressora:",
                Location = new Point(20, 55),
                Size = new Size(500, 70),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            // Label "Modelo:"
            Label lblModelo = new Label
            {
                Text = "Modelo da Impressora:",
                Location = new Point(20, 125),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            // ComboBox
            comboImpressoras = new ComboBox
            {
                Location = new Point(20, 150),
                Size = new Size(500, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
            comboImpressoras.SelectedIndexChanged += ComboImpressoras_SelectedIndexChanged;

            // Preview da imagem
            picturePreview = new PictureBox
            {
                Location = new Point(20, 190),
                Size = new Size(500, 150),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };

            // Botão OK
            btnOK = new Button
            {
                Text = "Baixar e Instalar",
                Location = new Point(310, 345),
                Size = new Size(130, 35),
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += BtnOK_Click;

            // Hover do botão OK
            btnOK.MouseEnter += (s, e) => btnOK.BackColor = Color.FromArgb(39, 174, 96);
            btnOK.MouseLeave += (s, e) => btnOK.BackColor = Color.FromArgb(46, 204, 113);

            // Botão Cancelar
            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(450, 345),
                Size = new Size(80, 35),
                DialogResult = DialogResult.Cancel,
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;

            // Hover do botão Cancelar
            btnCancelar.MouseEnter += (s, e) => btnCancelar.BackColor = Color.FromArgb(127, 140, 141);
            btnCancelar.MouseLeave += (s, e) => btnCancelar.BackColor = Color.FromArgb(149, 165, 166);

            this.Controls.AddRange(new Control[] {
                lblTitulo, lblInfo, lblModelo, comboImpressoras, picturePreview, btnOK, btnCancelar
            });

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancelar;
        }

        private void CarregarImpressoras(List<ImpressoraInfo> impressoras)
        {
            comboImpressoras.Items.Clear();
            foreach (var imp in impressoras)
            {
                comboImpressoras.Items.Add(imp);
            }
            comboImpressoras.DisplayMember = "Nome";

            if (comboImpressoras.Items.Count > 0)
                comboImpressoras.SelectedIndex = 0;
        }

        private void ComboImpressoras_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboImpressoras.SelectedItem is ImpressoraInfo impressora)
            {
                // Atualiza preview da imagem
                if (picturePreview.Image != null)
                {
                    picturePreview.Image.Dispose();
                }
                picturePreview.Image = impressora.ObterImagem();
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            ImpressoraSelecionada = comboImpressoras.SelectedItem as ImpressoraInfo;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (picturePreview.Image != null)
            {
                picturePreview.Image.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}