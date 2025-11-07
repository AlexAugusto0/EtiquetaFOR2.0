using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
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

            // Esconder controles inicialmente
            comboBox1.Visible = false;
            pictureBox1.Visible = false;
            panel2.Visible = false;
            btnInstalarDriver.Visible = false;
            btnProcurar.Visible = false;
            listViewDispositivos.Visible = false;
            button1.Visible = false;

            // Limpar valores
            comboBox1.SelectedIndex = -1;
            pictureBox1.Image = null;

            CarregarImpressoras();
            this.Text = Main.AppInfo.GetTituloAplicacao();
            InicializarListView();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
                btnProcurar.Visible = true;
                btnInstalarDriver.Visible = true;
                panel2.Visible = true;
                listViewDispositivos.Visible = true;
                pictureBox1.Visible = false;
                comboBox1.Visible = false;
                button1.Visible = false;
            }
            else
            {
                comboBox1.Visible = false;
                pictureBox1.Visible = false;
                panel2.Visible = false;
                button1.Visible = false;
                btnProcurar.Visible = false;
                btnInstalarDriver.Visible = false;
                listViewDispositivos.Visible = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
                comboBox1.Visible = true;
                pictureBox1.Visible = true;
                panel2.Visible = true;
                button1.Visible = true;
                listViewDispositivos.Visible = false;
                btnProcurar.Visible = false;
                btnInstalarDriver.Visible = false;
            }
            else
            {
                comboBox1.Visible = false;
                pictureBox1.Visible = false;
                panel2.Visible = false;
                button1.Visible = false;
                listViewDispositivos.Visible = false;
            }
        }

        /// <summary>
        /// Carrega as impressoras usando o ImpressoraManager
        /// </summary>
        private void CarregarImpressoras()
        {
            try
            {
                // Carrega as impressoras do JSON
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

                // Limpa e adiciona itens no ComboBox
                comboBox1.Items.Clear();
                foreach (var imp in impressoras)
                    comboBox1.Items.Add(imp.Nome);

                // Registrar o evento antes de definir SelectedIndex
                comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
                comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

                // Define o item selecionado
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
                    // Libera a imagem anterior se existir
                    if (pictureBox1.Image != null)
                    {
                        var imagemAnterior = pictureBox1.Image;
                        pictureBox1.Image = null;
                        imagemAnterior.Dispose();
                    }

                    // Carrega a nova imagem
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
                        $"Erro ao carregar imagem da impressora: {ex.Message}",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                // Atualiza o link do botão
                button1.Tag = info.DriverUrl;
            }
        }

        /// <summary>
        /// Botão para abrir link do driver no navegador (modo consulta)
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Tag is string url && !string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    url = url.Trim();
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Não foi possível abrir o link:\n{ex.Message}",
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(
                    "Nenhum link de driver disponível para esta impressora.",
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
            listViewDispositivos.Columns.Add("Nome", 250);
            listViewDispositivos.Columns.Add("Device ID", 300);
            listViewDispositivos.Columns.Add("Status", 100);
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
                    string statusTexto = erro == 0 ? "Instalado" : "Sem driver / Problema";

                    // Filtra apenas impressoras USB
                    if (deviceId.StartsWith("USBPRINT", StringComparison.OrdinalIgnoreCase))
                    {
                        var item = new ListViewItem(new[] { nome, deviceId, statusTexto });
                        item.Tag = new { DeviceId = deviceId, Info = obj };

                        // Destaca em vermelho se houver problema
                        if (erro != 0)
                        {
                            item.ForeColor = Color.Red;
                            item.Font = new Font(item.Font, FontStyle.Bold);
                        }
                        else
                        {
                            item.ForeColor = Color.Green;
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
                        "Procurar impressoras",
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

        /// <summary>
        /// Botão de instalar driver - IMPLEMENTAÇÃO COMPLETA
        /// </summary>
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

            // Tenta identificar a impressora automaticamente pelo nome
            var impressoraEncontrada = TentarIdentificarImpressora(nomeDispositivo);

            if (impressoraEncontrada != null)
            {
                // Encontrou correspondência automática
                DialogResult resultado = MessageBox.Show(
                    $"Dispositivo detectado: {nomeDispositivo}\n\n" +
                    $"Impressora identificada: {impressoraEncontrada.Nome}\n\n" +
                    $"Deseja baixar e instalar o driver automaticamente?",
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
                // Não identificou automaticamente - mostra lista para seleção manual
                MostrarSelecaoManualDriver(nomeDispositivo);
            }
        }

        /// <summary>
        /// Tenta identificar a impressora pelo nome do dispositivo
        /// </summary>
        private ImpressoraInfo TentarIdentificarImpressora(string nomeDispositivo)
        {
            string nomeNormalizado = nomeDispositivo.ToLower().Replace(" ", "");

            foreach (var impressora in impressoras)
            {
                string nomeImpressoraNormalizado = impressora.Nome.ToLower().Replace(" ", "");

                // Verifica se há correspondência parcial
                if (nomeNormalizado.Contains(nomeImpressoraNormalizado) ||
                    nomeImpressoraNormalizado.Contains(nomeNormalizado))
                {
                    return impressora;
                }

                // Verifica partes do nome
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

        /// <summary>
        /// Mostra formulário para seleção manual do driver
        /// </summary>
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
            // Libera recursos
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
            this.Size = new Size(500, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblTitulo = new Label
            {
                Text = "Não foi possível identificar automaticamente a impressora.",
                Location = new Point(20, 20),
                Size = new Size(450, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            lblInfo = new Label
            {
                Text = $"Dispositivo: {nomeDispositivo}\n\nSelecione o modelo correto da impressora:",
                Location = new Point(20, 50),
                Size = new Size(450, 40)
            };

            comboImpressoras = new ComboBox
            {
                Location = new Point(20, 100),
                Size = new Size(450, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboImpressoras.SelectedIndexChanged += ComboImpressoras_SelectedIndexChanged;

            picturePreview = new PictureBox
            {
                Location = new Point(20, 140),
                Size = new Size(450, 120),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            btnOK = new Button
            {
                Text = "Baixar e Instalar",
                Location = new Point(250, 275),
                Size = new Size(110, 30),
                DialogResult = DialogResult.OK
            };
            btnOK.Click += BtnOK_Click;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(370, 275),
                Size = new Size(100, 30),
                DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[] {
                lblTitulo, lblInfo, comboImpressoras, picturePreview, btnOK, btnCancelar
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