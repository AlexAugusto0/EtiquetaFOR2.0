using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace EtiquetaFORNew
{
    /// <summary>
    /// Gerenciador de download e instalação de drivers - VERSÃO OTIMIZADA
    /// Com menos confirmações e fluxo mais direto
    /// </summary>
    public class DriverInstaller
    {
        private WebClient _webClient;
        private string _caminhoDownload;
        private string _urlDriver;
        private ImpressoraInfo _impressora;
        private Form _formPai;

        public event EventHandler<DownloadProgressChangedEventArgs> ProgressoDownload;
        public event EventHandler<AsyncCompletedEventArgs> DownloadCompleto;

        public DriverInstaller(Form formPai)
        {
            _formPai = formPai;
        }

        /// <summary>
        /// Baixa e instala o driver da impressora
        /// </summary>
        public void BaixarEInstalarDriver(ImpressoraInfo impressora)
        {
            try
            {
                _impressora = impressora;
                _urlDriver = impressora.DriverUrl?.Trim();

                if (string.IsNullOrWhiteSpace(_urlDriver))
                {
                    MessageBox.Show(
                        "URL do driver não encontrada para esta impressora.",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Google Drive - abre direto no navegador
                if (_urlDriver.Contains("drive.google.com"))
                {
                    AbrirLinkNoNavegador(_urlDriver);
                    MessageBox.Show(
                        $"Download do driver de {impressora.Nome} iniciado no navegador.\n\n" +
                        "Após o download, execute o instalador.",
                        "Download Manual - Google Drive",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                // Cria pasta temporária
                string pastaTemp = Path.Combine(Path.GetTempPath(), "EtiquetaFOR_Drivers");
                if (!Directory.Exists(pastaTemp))
                    Directory.CreateDirectory(pastaTemp);

                string nomeArquivo = ObterNomeArquivo(_urlDriver, impressora.Nome);
                _caminhoDownload = Path.Combine(pastaTemp, nomeArquivo);

                // Se já existe, usa direto (sem perguntar)
                if (File.Exists(_caminhoDownload))
                {
                    ProcessarArquivoBaixado(_caminhoDownload);
                    return;
                }

                // Inicia download direto (sem confirmação)
                IniciarDownload(_urlDriver, _caminhoDownload);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao preparar instalação do driver:\n\n{ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void IniciarDownload(string url, string caminhoDestino)
        {
            try
            {
                _webClient = new WebClient();
                _webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                _webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

                FormProgresso formProgresso = new FormProgresso(_impressora.Nome);
                formProgresso.Show(_formPai);

                _webClient.DownloadFileAsync(new Uri(url), caminhoDestino, formProgresso);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao iniciar download:\n\n{ex.Message}",
                    "Erro de Download",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.UserState is FormProgresso formProgresso)
            {
                formProgresso.AtualizarProgresso(e.ProgressPercentage, e.BytesReceived, e.TotalBytesToReceive);
            }
            ProgressoDownload?.Invoke(sender, e);
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            FormProgresso formProgresso = e.UserState as FormProgresso;
            formProgresso?.Close();

            if (e.Error != null)
            {
                MessageBox.Show(
                    $"Erro ao baixar o driver:\n\n{e.Error.Message}",
                    "Erro de Download",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (e.Cancelled)
            {
                MessageBox.Show(
                    "Download cancelado.",
                    "Cancelado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // Download completo - processa direto (sem mensagem de sucesso)
            ProcessarArquivoBaixado(_caminhoDownload);

            DownloadCompleto?.Invoke(sender, e);
        }

        /// <summary>
        /// Processa o arquivo baixado (extrai e instala automaticamente)
        /// </summary>
        private void ProcessarArquivoBaixado(string caminhoArquivo)
        {
            try
            {
                string extensao = Path.GetExtension(caminhoArquivo).ToLower();

                if (extensao == ".zip")
                {
                    // ZIP - extrai e instala automaticamente
                    ExtrairEInstalarZip(caminhoArquivo);
                }
                else if (extensao == ".rar" || extensao == ".7z")
                {
                    // RAR/7Z - abre pasta (não dá pra extrair automaticamente)
                    MessageBox.Show(
                        $"Arquivo {extensao.ToUpper()} requer extração manual.\n\n" +
                        $"A pasta será aberta para você extrair e executar o instalador.",
                        "Extração Manual",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    Process.Start("explorer.exe", $"/select,\"{caminhoArquivo}\"");
                }
                else
                {
                    // EXE/MSI - executa direto
                    ExecutarInstalador(caminhoArquivo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao processar arquivo:\n\n{ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Extrai ZIP e instala automaticamente
        /// </summary>
        private void ExtrairEInstalarZip(string caminhoZip)
        {
            try
            {
                string pastaExtracao = Path.Combine(
                    Path.GetDirectoryName(caminhoZip),
                    Path.GetFileNameWithoutExtension(caminhoZip) + "_extraido");

                if (Directory.Exists(pastaExtracao))
                    Directory.Delete(pastaExtracao, true);

                // Mostra progresso de extração
                FormExtracaoProgresso formExtracao = new FormExtracaoProgresso(_impressora.Nome);
                formExtracao.Show(_formPai);

                // Extrai
                ZipFile.ExtractToDirectory(caminhoZip, pastaExtracao);

                formExtracao.Close();

                // Procura executáveis
                var executaveis = ProcurarExecutaveis(pastaExtracao);

                if (executaveis.Count == 0)
                {
                    // Nenhum instalador encontrado
                    MessageBox.Show(
                        "Nenhum instalador encontrado no arquivo ZIP.\n\n" +
                        "A pasta extraída será aberta para você procurar manualmente.",
                        "Instalador Não Encontrado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    Process.Start("explorer.exe", pastaExtracao);
                    return;
                }

                if (executaveis.Count == 1)
                {
                    // 1 instalador - executa direto
                    ExecutarInstalador(executaveis[0]);
                    return;
                }

                // Múltiplos instaladores - tenta identificar
                string instaladorPrincipal = IdentificarInstaladorPrincipal(executaveis);

                if (instaladorPrincipal != null)
                {
                    // Identificou - executa direto
                    ExecutarInstalador(instaladorPrincipal);
                }
                else
                {
                    // Não identificou - pede seleção
                    MostrarSelecaoExecutavel(executaveis, pastaExtracao);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao extrair arquivo ZIP:\n\n{ex.Message}",
                    "Erro de Extração",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Process.Start("explorer.exe", $"/select,\"{caminhoZip}\"");
            }
        }

        private System.Collections.Generic.List<string> ProcurarExecutaveis(string pasta)
        {
            var executaveis = new System.Collections.Generic.List<string>();

            try
            {
                executaveis.AddRange(Directory.GetFiles(pasta, "*.exe", SearchOption.AllDirectories));
                executaveis.AddRange(Directory.GetFiles(pasta, "*.msi", SearchOption.AllDirectories));

                executaveis = executaveis.Where(exe =>
                {
                    string nome = Path.GetFileName(exe).ToLower();
                    return !nome.Contains("uninstall") &&
                           !nome.Contains("unins") &&
                           !nome.Contains("readme") &&
                           !nome.Contains("license");
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao procurar executáveis: {ex.Message}");
            }

            return executaveis;
        }

        private string IdentificarInstaladorPrincipal(System.Collections.Generic.List<string> executaveis)
        {
            string[] palavrasChave = { "setup", "install", "driver", "printer", "instalar" };

            foreach (var palavra in palavrasChave)
            {
                var encontrado = executaveis.FirstOrDefault(exe =>
                    Path.GetFileName(exe).ToLower().Contains(palavra));

                if (encontrado != null)
                    return encontrado;
            }

            // Pega o maior arquivo
            var maiorArquivo = executaveis
                .Select(exe => new { Path = exe, Size = new FileInfo(exe).Length })
                .OrderByDescending(x => x.Size)
                .FirstOrDefault();

            return maiorArquivo?.Path;
        }

        private void MostrarSelecaoExecutavel(System.Collections.Generic.List<string> executaveis, string pastaExtracao)
        {
            using (FormSelecaoExecutavel form = new FormSelecaoExecutavel(executaveis))
            {
                if (form.ShowDialog(_formPai) == DialogResult.OK)
                {
                    string executavelSelecionado = form.ExecutavelSelecionado;
                    if (!string.IsNullOrEmpty(executavelSelecionado))
                    {
                        ExecutarInstalador(executavelSelecionado);
                    }
                }
                else
                {
                    Process.Start("explorer.exe", pastaExtracao);
                }
            }
        }

        /// <summary>
        /// Executa o instalador (direto, sem confirmação)
        /// </summary>
        private void ExecutarInstalador(string caminhoArquivo)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = caminhoArquivo,
                    UseShellExecute = true,
                    Verb = "runas" // Solicita admin
                };

                Process.Start(startInfo);

                // Mensagem informativa simples
                MessageBox.Show(
                    $"Instalador iniciado: {Path.GetFileName(caminhoArquivo)}\n\n" +
                    "Siga as instruções na tela para concluir a instalação.",
                    "Instalação Iniciada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Se falhar (UAC cancelado, etc), oferece abrir pasta
                DialogResult resultado = MessageBox.Show(
                    $"Não foi possível iniciar o instalador automaticamente.\n\n" +
                    $"Erro: {ex.Message}\n\n" +
                    $"Deseja abrir a pasta para executar manualmente?",
                    "Erro ao Executar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (resultado == DialogResult.Yes)
                {
                    Process.Start("explorer.exe", $"/select,\"{caminhoArquivo}\"");
                }
            }
        }

        private void AbrirLinkNoNavegador(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao abrir navegador:\n\n{ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private string ObterNomeArquivo(string url, string nomeImpressora)
        {
            try
            {
                Uri uri = new Uri(url);
                string nomeArquivo = Path.GetFileName(uri.LocalPath);

                if (string.IsNullOrWhiteSpace(nomeArquivo) || nomeArquivo.Length < 3)
                {
                    nomeArquivo = $"Driver_{nomeImpressora.Replace(" ", "_")}.exe";
                }

                return nomeArquivo;
            }
            catch
            {
                return $"Driver_{nomeImpressora.Replace(" ", "_")}.exe";
            }
        }

        public void CancelarDownload()
        {
            _webClient?.CancelAsync();
        }
    }

    /// <summary>
    /// Formulário de progresso - SIMPLIFICADO
    /// </summary>
    public class FormProgresso : Form
    {
        private ProgressBar progressBar;
        private Label lblStatus;
        private Label lblVelocidade;
        private Button btnCancelar;
        private DateTime _inicioDownload;

        public FormProgresso(string nomeImpressora)
        {
            InitializeComponent(nomeImpressora);
            _inicioDownload = DateTime.Now;
        }

        private void InitializeComponent(string nomeImpressora)
        {
            this.Text = "Baixando Driver";
            this.Size = new System.Drawing.Size(500, 180);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblTitulo = new Label
            {
                Text = $"Baixando: {nomeImpressora}",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(450, 20),
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };

            lblStatus = new Label
            {
                Text = "Iniciando...",
                Location = new System.Drawing.Point(20, 50),
                Size = new System.Drawing.Size(450, 20)
            };

            progressBar = new ProgressBar
            {
                Location = new System.Drawing.Point(20, 80),
                Size = new System.Drawing.Size(450, 25),
                Style = ProgressBarStyle.Continuous
            };

            lblVelocidade = new Label
            {
                Text = "",
                Location = new System.Drawing.Point(20, 110),
                Size = new System.Drawing.Size(450, 20),
                ForeColor = System.Drawing.Color.Gray
            };

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new System.Drawing.Point(195, 140),
                Size = new System.Drawing.Size(100, 30)
            };
            btnCancelar.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            this.Controls.AddRange(new Control[] {
                lblTitulo, lblStatus, progressBar, lblVelocidade, btnCancelar
            });
        }

        public void AtualizarProgresso(int porcentagem, long bytesRecebidos, long totalBytes)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AtualizarProgresso(porcentagem, bytesRecebidos, totalBytes)));
                return;
            }

            progressBar.Value = Math.Min(porcentagem, 100);

            double recebidoMB = bytesRecebidos / 1024.0 / 1024.0;
            double totalMB = totalBytes / 1024.0 / 1024.0;

            lblStatus.Text = $"{porcentagem}% - {recebidoMB:F2} MB de {totalMB:F2} MB";

            TimeSpan tempoDecorrido = DateTime.Now - _inicioDownload;
            if (tempoDecorrido.TotalSeconds > 0)
            {
                double velocidadeMBps = recebidoMB / tempoDecorrido.TotalSeconds;
                lblVelocidade.Text = $"Velocidade: {velocidadeMBps:F2} MB/s";
            }
        }
    }

    /// <summary>
    /// Formulário de extração - SIMPLIFICADO
    /// </summary>
    public class FormExtracaoProgresso : Form
    {
        public FormExtracaoProgresso(string nomeImpressora)
        {
            InitializeComponent(nomeImpressora);
        }

        private void InitializeComponent(string nomeImpressora)
        {
            this.Text = "Extraindo";
            this.Size = new System.Drawing.Size(400, 120);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;

            Label lblTitulo = new Label
            {
                Text = $"Extraindo: {nomeImpressora}",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(350, 20),
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };

            ProgressBar progressBar = new ProgressBar
            {
                Location = new System.Drawing.Point(20, 50),
                Size = new System.Drawing.Size(350, 25),
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30
            };

            this.Controls.AddRange(new Control[] { lblTitulo, progressBar });
        }
    }

    /// <summary>
    /// Seleção de executável - quando não identifica automaticamente
    /// </summary>
    public class FormSelecaoExecutavel : Form
    {
        private ListBox listExecutaveis;
        private Button btnOK;
        private Button btnCancelar;
        public string ExecutavelSelecionado { get; private set; }

        public FormSelecaoExecutavel(System.Collections.Generic.List<string> executaveis)
        {
            InitializeComponent();
            CarregarExecutaveis(executaveis);
        }

        private void InitializeComponent()
        {
            this.Text = "Selecionar Instalador";
            this.Size = new System.Drawing.Size(500, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblTitulo = new Label
            {
                Text = "Múltiplos instaladores encontrados. Selecione:",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(450, 20),
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };

            listExecutaveis = new ListBox
            {
                Location = new System.Drawing.Point(20, 50),
                Size = new System.Drawing.Size(450, 270)
            };
            listExecutaveis.DoubleClick += (s, e) => { BtnOK_Click(s, e); };

            btnOK = new Button
            {
                Text = "Executar",
                Location = new System.Drawing.Point(270, 330),
                Size = new System.Drawing.Size(90, 30),
                DialogResult = DialogResult.OK
            };
            btnOK.Click += BtnOK_Click;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new System.Drawing.Point(370, 330),
                Size = new System.Drawing.Size(100, 30),
                DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[] {
                lblTitulo, listExecutaveis, btnOK, btnCancelar
            });

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancelar;
        }

        private void CarregarExecutaveis(System.Collections.Generic.List<string> executaveis)
        {
            listExecutaveis.Items.Clear();
            foreach (var exe in executaveis)
            {
                listExecutaveis.Items.Add(exe);
            }

            if (listExecutaveis.Items.Count > 0)
                listExecutaveis.SelectedIndex = 0;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (listExecutaveis.SelectedItem != null)
            {
                ExecutavelSelecionado = listExecutaveis.SelectedItem.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}