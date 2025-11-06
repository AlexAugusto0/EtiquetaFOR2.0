using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static EtiquetaFORNew.Main;

namespace EtiquetaFORNew
{
    public partial class configuracoes : Form
    {
        public configuracoes()
        {
            InitializeComponent();
            CarregarConfiguracao();
            this.Text = AppInfo.GetTituloAplicacao();
        }

        public class ConfiguracaoBD
        {
            public string Servidor { get; set; }
            public string Porta { get; set; }
            public string Usuario { get; set; }
            public string Senha { get; set; }
            public string Banco { get; set; }
        }
        private void SalvarConfiguracao(ConfiguracaoBD config)
        {
            try
            {
                string caminhoArquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(caminhoArquivo, json);

                MessageBox.Show("💾 Configuração salva com sucesso!", "Salvo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar configuração:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void TestarConexao_Click(object sender, EventArgs e)
        {
            string servidor = textServidor.Text;
            string porta = textPorta.Text;
            string usuario = textUsuario.Text;
            string senha = textSenha.Text;
            string banco = textBanco.Text;

            string servidorCompleto = string.IsNullOrEmpty(porta) ? servidor : $"{servidor},{porta}";
            string connectionString =
                $"Server={servidorCompleto};Database={banco};User Id={usuario};Password={senha};TrustServerCertificate=True;";

            using (SqlConnection conexao = new SqlConnection(connectionString))
            {
                try
                {
                    conexao.Open();
                    MessageBox.Show("✅ Conexão realizada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    SalvarConfiguracao(new ConfiguracaoBD
                    {
                        Servidor = servidor,
                        Porta = porta,
                        Usuario = usuario,
                        Senha = senha,
                        Banco = banco
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ Falha ao conectar:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void CarregarConfiguracao()
        {
            try
            {
                string caminhoArquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                if (!File.Exists(caminhoArquivo))
                    return;

                string json = File.ReadAllText(caminhoArquivo);
                var config = JsonConvert.DeserializeObject<ConfiguracaoBD>(json);

                if (config != null)
                {
                    textServidor.Text = config.Servidor;
                    textPorta.Text = config.Porta;
                    textUsuario.Text = config.Usuario;
                    textSenha.UseSystemPasswordChar = true;
                    textSenha.Text = config.Senha;
                    textBanco.Text = config.Banco;

                    MessageBox.Show("💡 Uma configuração existente foi carregada automaticamente.",
                                    "Configuração Existente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar configuração:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textSenha_TextChanged(object sender, EventArgs e)
        {
            textSenha.UseSystemPasswordChar = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Alterna entre oculto e visível
            textSenha.UseSystemPasswordChar = !textSenha.UseSystemPasswordChar;

            // Opcional: trocar ícone do olho aberto/fechado
            if (textSenha.UseSystemPasswordChar)
                btnExibirSenha.Text = "👁"; // oculto
            else
                btnExibirSenha.Text = "🙈"; // visível
        }
    }
}
