using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using EtiquetaFORNew.Data;

namespace EtiquetaFORNew
{
    /// <summary>
    /// Formulário para buscar e selecionar mercadorias do banco local
    /// </summary>
    public partial class FormBuscaMercadoria : Form
    {
        private Timer timerBusca;
        private DataTable mercadorias;

        public int CodigoSelecionado { get; private set; }
        public string NomeSelecionado { get; private set; }
        public string CodigoFabricanteSelecionado { get; private set; }
        public decimal PrecoSelecionado { get; private set; }

        public FormBuscaMercadoria()
        {
            InitializeComponent();
            ConfigurarTimer();
            CarregarEstatisticas();
        }

        private void ConfigurarTimer()
        {
            // Timer para delay na busca (evita buscar a cada tecla)
            timerBusca = new Timer();
            timerBusca.Interval = 300; // 300ms de delay
            timerBusca.Tick += TimerBusca_Tick;
        }

        public void CarregarEstatisticas()
        {
            var (ultimaSync, total) = LocalDatabaseManager.ObterInfoSincronizacao();

            if (ultimaSync.HasValue)
            {
                lblUltimaSync.Text = $"Última sincronização: {ultimaSync.Value:dd/MM/yyyy HH:mm}";
                lblTotalRegistros.Text = $"Total de mercadorias: {total:N0}";
            }
            else
            {
                lblUltimaSync.Text = "Nenhuma sincronização realizada";
                lblTotalRegistros.Text = "Clique em 'Sincronizar' para importar dados";
            }
        }

        private void txtBusca_TextChanged(object sender, EventArgs e)
        {
            // Reinicia o timer a cada tecla
            timerBusca.Stop();
            timerBusca.Start();
        }

        private void TimerBusca_Tick(object sender, EventArgs e)
        {
            timerBusca.Stop();
            RealizarBusca();
        }

        private void RealizarBusca()
        {
            try
            {
                string termo = txtBusca.Text.Trim();

                if (string.IsNullOrEmpty(termo))
                {
                    dgvResultados.DataSource = null;
                    lblResultados.Text = "Digite para buscar...";
                    return;
                }

                Cursor = Cursors.WaitCursor;

                mercadorias = LocalDatabaseManager.BuscarMercadorias(termo, 100);
                dgvResultados.DataSource = mercadorias;

                ConfigurarGridResultados();

                lblResultados.Text = mercadorias.Rows.Count > 0
                    ? $"{mercadorias.Rows.Count} resultado(s) encontrado(s)"
                    : "Nenhum resultado encontrado";

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"Erro ao buscar: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarGridResultados()
        {
            if (dgvResultados.Columns.Count == 0) return;

            dgvResultados.Columns["CodigoMercadoria"].HeaderText = "Código";
            dgvResultados.Columns["CodigoMercadoria"].Width = 80;

            dgvResultados.Columns["CodFabricante"].HeaderText = "Cód. Fabricante";
            dgvResultados.Columns["CodFabricante"].Width = 120;

            dgvResultados.Columns["CodBarras"].HeaderText = "Cód. Barras";
            dgvResultados.Columns["CodBarras"].Width = 120;

            dgvResultados.Columns["Mercadoria"].HeaderText = "Nome";
            dgvResultados.Columns["Mercadoria"].Width = 300;
            dgvResultados.Columns["Mercadoria"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvResultados.Columns["PrecoVenda"].HeaderText = "Preço";
            dgvResultados.Columns["PrecoVenda"].Width = 100;
            dgvResultados.Columns["PrecoVenda"].DefaultCellStyle.Format = "C2";
            dgvResultados.Columns["PrecoVenda"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            if (dgvResultados.Columns.Contains("Registro"))
            {
                dgvResultados.Columns["Registro"].Visible = false;
            }

            dgvResultados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResultados.MultiSelect = false;
            dgvResultados.AllowUserToAddRows = false;
            dgvResultados.ReadOnly = true;
        }

        private void btnSincronizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(
                    "Deseja sincronizar as mercadorias do SQL Server?\n\n" +
                    "Isso pode levar alguns minutos dependendo da quantidade de registros.",
                    "Confirmar Sincronização",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }

                Cursor = Cursors.WaitCursor;
                btnSincronizar.Enabled = false;
                btnSincronizar.Text = "Sincronizando...";

                // Sincronizar (pode adicionar filtro ou limite se necessário)
                int total = LocalDatabaseManager.SincronizarMercadorias();

                Cursor = Cursors.Default;
                btnSincronizar.Enabled = true;
                btnSincronizar.Text = "🔄 Sincronizar";

                MessageBox.Show(
                    $"Sincronização concluída com sucesso!\n\n" +
                    $"Total de mercadorias importadas: {total:N0}",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                CarregarEstatisticas();
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                btnSincronizar.Enabled = true;
                btnSincronizar.Text = "🔄 Sincronizar";

                MessageBox.Show(
                    $"Erro ao sincronizar:\n\n{ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnSelecionar_Click(object sender, EventArgs e)
        {
            SelecionarMercadoria();
        }

        private void dgvResultados_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SelecionarMercadoria();
            }
        }

        private void SelecionarMercadoria()
        {
            if (dgvResultados.CurrentRow == null)
            {
                MessageBox.Show("Selecione uma mercadoria!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataRowView row = (DataRowView)dgvResultados.CurrentRow.DataBoundItem;

                CodigoSelecionado = Convert.ToInt32(row["CodigoMercadoria"]);
                NomeSelecionado = row["Mercadoria"].ToString();
                CodigoFabricanteSelecionado = row["CodFabricante"]?.ToString() ?? "";

                if (row["PrecoVenda"] != DBNull.Value)
                {
                    PrecoSelecionado = Convert.ToDecimal(row["PrecoVenda"]);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao selecionar mercadoria: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FormBuscaMercadoria_Load(object sender, EventArgs e)
        {
            txtBusca.Focus();

            // Verificar se precisa sincronizar
            if (LocalDatabaseManager.PrecisaSincronizar())
            {
                lblUltimaSync.ForeColor = Color.Red;
                lblUltimaSync.Text += " - Recomendado sincronizar!";
            }
        }
    }
}