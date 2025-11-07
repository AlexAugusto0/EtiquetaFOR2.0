using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace EtiquetaFORNew
{
    public partial class telaTecnico : Form
    {
        private List<ImpressoraInfo> impressoras = new List<ImpressoraInfo>();

        public telaTecnico()
        {
            InitializeComponent();

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
                comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged; // garante que não haja duplicidade
                comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

                // Define o item selecionado (dispara o evento imediatamente)
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
                // Atualiza a imagem usando o método da classe ImpressoraInfo
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

                    // Se não conseguiu carregar a imagem, mostra uma mensagem no PictureBox
                    if (pictureBox1.Image == null)
                    {
                        // Você pode criar uma imagem placeholder aqui se desejar
                        // ou apenas deixar vazio
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Tag is string url && !string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    // Remove possíveis quebras de linha ou espaços extras
                    url = url.Trim();
                    System.Diagnostics.Process.Start(url);
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

                    // Filtra apenas impressoras USB pelo DeviceID
                    if (deviceId.StartsWith("USBPRINT", StringComparison.OrdinalIgnoreCase))
                    {
                        var item = new ListViewItem(new[] { nome, deviceId, statusTexto });
                        item.Tag = deviceId;

                        // Destaca em vermelho se houver problema
                        if (erro != 0)
                        {
                            item.ForeColor = Color.Red;
                        }

                        listViewDispositivos.Items.Add(item);
                    }
                }

                if (listViewDispositivos.Items.Count == 0)
                {
                    MessageBox.Show(
                        "Nenhuma impressora USB detectada.",
                        "Procurar impressoras",
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
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string nomeDispositivo = listViewDispositivos.SelectedItems[0].SubItems[0].Text;
            string deviceId = listViewDispositivos.SelectedItems[0].Tag?.ToString() ?? "";

            // Aqui você pode implementar a lógica de instalação do driver
            // Por exemplo, procurar na lista de impressoras qual driver baixar

            MessageBox.Show(
                $"Função de instalação automática de driver em desenvolvimento.\n\n" +
                $"Dispositivo: {nomeDispositivo}\n" +
                $"Device ID: {deviceId}\n\n" +
                $"Por favor, use a aba 'Consultar Modelos' para baixar o driver manualmente.",
                "Informação",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // Exemplo de como implementar:
            // 1. Identificar o modelo da impressora pelo nome ou deviceId
            // 2. Buscar na lista de impressoras o driver correspondente
            // 3. Fazer download do driver
            // 4. Executar o instalador silenciosamente
            // System.Diagnostics.Process.Start(@"C:\Drivers\instalador.exe", "/S");
        }

        /// <summary>
        /// Método auxiliar para buscar dispositivos não instalados (não utilizado atualmente)
        /// </summary>
        private void BuscarDispositivosNaoInstalados()
        {
            try
            {
                listViewDispositivos.Items.Clear();

                // Obtém todos os dispositivos Plug and Play
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");

                foreach (ManagementObject obj in searcher.Get())
                {
                    string nome = obj["Name"]?.ToString();
                    string deviceId = obj["DeviceID"]?.ToString();
                    string status = obj["Status"]?.ToString();
                    int? erro = obj["ConfigManagerErrorCode"] as int?;

                    // Filtra dispositivos USB e com erro (sem driver)
                    if (deviceId != null && deviceId.Contains("USB") && erro != 0)
                    {
                        var item = new ListViewItem(new[] { nome ?? "Desconhecido", deviceId, status ?? "Desconhecido" });
                        item.Tag = deviceId;
                        item.ForeColor = Color.Red;
                        listViewDispositivos.Items.Add(item);
                    }
                }

                if (listViewDispositivos.Items.Count == 0)
                {
                    MessageBox.Show(
                        "Nenhum dispositivo novo ou sem driver foi encontrado.",
                        "Procurar dispositivos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao buscar dispositivos: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Cleanup ao fechar o form
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Libera a imagem do PictureBox
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            base.OnFormClosing(e);
        }
    }
}