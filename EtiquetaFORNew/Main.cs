using EtiquetaFORNew;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace EtiquetaFORNew
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            //testerepositorio
            senhaBox.UseSystemPasswordChar = true; // Oculta caracteres
            senhaBox.KeyDown += senhaBox_KeyDown;  // Detecta F11
            this.Text = AppInfo.GetTituloAplicacao();
            this.KeyPreview = true; // faz o formulário "enxergar" as teclas antes dos campos
            // Conecta o evento KeyDown
            this.KeyDown += Main_KeyDown;

            LoadUsuarios();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            usuarioBox.Focus();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            configuracoes tela = new configuracoes();
            tela.ShowDialog();
        }

        private void senhaBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                e.SuppressKeyPress = true; // evita beep do F11

                if (senhaBox.Text == "suporte@softcom")
                {
                    telaTecnico tela = new telaTecnico();
                    tela.ShowDialog();
                    senhaBox.Clear();
                }
                else
                {
                    MessageBox.Show(
                        "Ops! A senha digitada não confere. Verifique e tente novamente, por favor.",
                        "Senha incorreta",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    senhaBox.Clear();
                    senhaBox.Focus();
                }
            }
        }

        private void btnLogar_Click(object sender, EventArgs e)
        {
            string senha = senhaBox.Text.Trim();

            if (usuarioBox.SelectedItem == null || string.IsNullOrEmpty(senha))
            {
                MessageBox.Show("Por favor, preencha o usuário e a senha.",
                                "Campos obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string codigoSelecionado = ((ComboItem)usuarioBox.SelectedItem).Value;

            try
            {
                // Caminho da configuração
                string caminhoArquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                if (!File.Exists(caminhoArquivo))
                {
                    MessageBox.Show("⚠️ Configuração de banco não encontrada. Configure primeiro nas Configurações.",
                                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string json = File.ReadAllText(caminhoArquivo);
                var config = JsonConvert.DeserializeObject<ConfiguracaoBD>(json);

                // Monta connection string
                string servidorCompleto = string.IsNullOrEmpty(config.Porta)
                    ? config.Servidor
                    : $"{config.Servidor},{config.Porta}";

                string connectionString =
                    $"Server={servidorCompleto};Database={config.Banco};User Id={config.Usuario};Password={config.Senha};";

                // Verificação de login
                using (SqlConnection conexao = new SqlConnection(connectionString))
                {
                    conexao.Open();

                    string query = @"SELECT [Nome] 
                                     FROM [Cadastro De Vendedores] 
                                     WHERE [Código do Vendedor] = @codigo AND [Senha] = @Senha";

                    using (SqlCommand cmd = new SqlCommand(query, conexao))
                    {
                        cmd.Parameters.Add("@Codigo", SqlDbType.NVarChar, 50).Value = codigoSelecionado.Trim();
                        cmd.Parameters.Add("@Senha", SqlDbType.NVarChar, 50).Value = senha.Trim();

                        string nomeVendedor = cmd.ExecuteScalar()?.ToString();

                        if (!string.IsNullOrEmpty(nomeVendedor))
                        {
                            MessageBox.Show($"✅ Bem-vindo, {nomeVendedor}!", "Login realizado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Abre a próxima tela e esconde a principal
                            telaEntrada Entrada = new telaEntrada();
                            Entrada.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("❌ Usuário ou senha incorretos.\nVerifique e tente novamente.",
                                            "Falha no login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao tentar logar:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUsuarios()
        {
            try
            {
                string caminhoArquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
                if (!File.Exists(caminhoArquivo)) return;

                string json = File.ReadAllText(caminhoArquivo);
                var config = JsonConvert.DeserializeObject<ConfiguracaoBD>(json);

                string servidorCompleto = string.IsNullOrEmpty(config.Porta) ? config.Servidor : $"{config.Servidor},{config.Porta}";
                string connectionString = $"Server={servidorCompleto};Database={config.Banco};User Id={config.Usuario};Password={config.Senha};";

                using (SqlConnection conexao = new SqlConnection(connectionString))
                {
                    conexao.Open();

                    string query = "SELECT [Código do Vendedor], [Nome] FROM [Cadastro De Vendedores] ORDER BY [Nome]";

                    using (SqlCommand cmd = new SqlCommand(query, conexao))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        usuarioBox.Items.Clear();
                        while (reader.Read())
                        {
                            usuarioBox.Items.Add(new ComboItem
                            {
                                Text = reader["Nome"].ToString(),
                                Value = reader["Código do Vendedor"].ToString()
                            });
                        }
                    }
                }

                usuarioBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                usuarioBox.AutoCompleteSource = AutoCompleteSource.ListItems;
                usuarioBox.DisplayMember = "Text";
                usuarioBox.ValueMember = "Value";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar usuários:\n{ex.Message}");
            }
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // evita beep
                this.SelectNextControl(this.ActiveControl, true, true, true, true); // simula o Tab
            }
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public static class AppInfo
        {
            public static string GetTituloAplicacao()
            {
                string nome = Assembly.GetExecutingAssembly().GetName().Name;
                return $"{nome} - v1.0";
            }
        }

        public class ComboItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public override string ToString() => Text;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            configuracoes tela = new configuracoes();
            tela.ShowDialog();
        }

    }

    // Classe para mapear configuração do JSON
    public class ConfiguracaoBD
    {
        public string Servidor { get; set; }
        public string Porta { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string Banco { get; set; }
    }


}
