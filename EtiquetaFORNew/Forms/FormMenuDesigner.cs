using System;
using System.Windows.Forms;

namespace EtiquetaFORNew.Forms
{
    public partial class FormMenuDesigner : Form
    {
        public FormMenuDesigner()
        {
            InitializeComponent();

            // Passo 2: Aplica configurações para remover botões de controle da janela
            ConfigurarVisualForm();
        }

        // Método para o botão "Novo Template"
        private void BtnNovo_Click(object sender, EventArgs e)
        {
            // Retorna 'Yes' para sinalizar a escolha de criar um NOVO template.
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        // Método para o botão "Carregar Existente"
        private void btnCarregar_Click(object sender, EventArgs e)
        {
            // Retorna 'No' para sinalizar a escolha de CARREGAR um existente.
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        // Método para o botão "Cancelar" ou "Sair"
        private void button2_Click(object sender, EventArgs e)
        {
            // Retorna 'Cancel' (ou 'Abort') para sair sem abrir o Designer.
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Método adicionado para configurar a aparência da janela
        private void ConfigurarVisualForm()
        {
            // Remove os botões Maximizar/Minimizar/Fechar e a borda de redimensionamento.
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            // Se quiser remover TUDO (incluindo o ícone e a barra de título):
            // this.FormBorderStyle = FormBorderStyle.None;

            // Garante que a janela inicie no centro da tela.
            this.StartPosition = FormStartPosition.CenterScreen;

            // Define a cor de fundo (sugestão do SmartPrint)
            //this.BackColor = System.Drawing.Color.White;

            // Ação principal: Remove toda a moldura, barra de título e botões.
            this.FormBorderStyle = FormBorderStyle.None;

            // As linhas MinimizeBox/MaximizeBox são redundantes com FormBorderStyle.None, mas não atrapalham.
            this.MinimizeBox = false;
            this.MaximizeBox = false;

            this.ControlBox = false; // Garante que nenhum controle de sistema seja exibido.
            this.Text = string.Empty; // Define o título como vazio, apenas por garantia.
            this.StartPosition = FormStartPosition.CenterParent;
        }

    }
}
