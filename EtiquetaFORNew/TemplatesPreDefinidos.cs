using System;
using System.IO;
using System.Collections.Generic;

namespace EtiquetaFORNew
{
    /// <summary>
    /// Gerenciador de templates pré-definidos
    /// Instala templates padrão na primeira execução do sistema
    /// </summary>
    public static class TemplatesPreDefinidos
    {
        private static readonly string PASTA_TEMPLATES = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "SistemaEtiquetas",
            "Templates"
        );

        private static readonly string PASTA_CONFIGURACOES = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "SistemaEtiquetas",
            "Configuracoes"
        );

        private static readonly string ARQUIVO_CONTROLE = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "SistemaEtiquetas",
            "_templates_instalados.txt"
        );

        /// <summary>
        /// Instala templates pré-definidos se necessário (primeira execução)
        /// </summary>
        public static void InstalarSeNecessario()
        {
            try
            {
                // Verifica se já foi instalado
                if (JaFoiInstalado())
                {
                    System.Diagnostics.Debug.WriteLine("Templates pré-definidos já foram instalados anteriormente.");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("Instalando templates pré-definidos...");

                // Cria pastas se não existirem
                CriarPastas();

                // Instala templates e configurações
                InstalarTemplates();
                InstalarConfiguracoes();

                // Marca como instalado
                MarcarComoInstalado();

                System.Diagnostics.Debug.WriteLine("Templates pré-definidos instalados com sucesso!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao instalar templates pré-definidos: {ex.Message}");
                // Não lança exceção para não impedir a inicialização do sistema
            }
        }

        /// <summary>
        /// Força a reinstalação dos templates (sobrescreve existentes)
        /// </summary>
        public static void ReinstalarTemplates()
        {
            try
            {
                // Remove marcação
                if (File.Exists(ARQUIVO_CONTROLE))
                {
                    File.Delete(ARQUIVO_CONTROLE);
                }

                // Instala novamente
                InstalarSeNecessario();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao reinstalar templates: {ex.Message}");
            }
        }

        #region Métodos Privados

        private static bool JaFoiInstalado()
        {
            return File.Exists(ARQUIVO_CONTROLE);
        }

        private static void MarcarComoInstalado()
        {
            File.WriteAllText(ARQUIVO_CONTROLE,
                $"Templates instalados em: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        }

        private static void CriarPastas()
        {
            if (!Directory.Exists(PASTA_TEMPLATES))
                Directory.CreateDirectory(PASTA_TEMPLATES);

            if (!Directory.Exists(PASTA_CONFIGURACOES))
                Directory.CreateDirectory(PASTA_CONFIGURACOES);
        }

        private static void InstalarTemplates()
        {
            var templates = ObterTemplatesPreDefinidos();

            foreach (var template in templates)
            {
                string caminho = Path.Combine(PASTA_TEMPLATES, template.Key + ".json");

                // Só instala se não existir (não sobrescreve templates do usuário)
                if (!File.Exists(caminho))
                {
                    File.WriteAllText(caminho, template.Value);
                    System.Diagnostics.Debug.WriteLine($"Template instalado: {template.Key}");
                }
            }
        }

        private static void InstalarConfiguracoes()
        {
            var configuracoes = ObterConfiguracoesPreDefinidas();

            foreach (var config in configuracoes)
            {
                string caminho = Path.Combine(PASTA_CONFIGURACOES, config.Key + ".json");

                // Só instala se não existir (não sobrescreve configurações do usuário)
                if (!File.Exists(caminho))
                {
                    File.WriteAllText(caminho, config.Value);
                    System.Diagnostics.Debug.WriteLine($"Configuração instalada: {config.Key}");
                }
            }
        }

        #endregion

        #region Templates Pré-definidos (JSON)

        private static Dictionary<string, string> ObterTemplatesPreDefinidos()
        {
            return new Dictionary<string, string>
            {
                { "3 Colunas", TEMPLATE_3_COLUNAS },
                { "Gondola", TEMPLATE_GONDOLA },
                { "Otica", TEMPLATE_OTICA }
            };
        }

        // Template: 3 Colunas (32x22mm)
        private const string TEMPLATE_3_COLUNAS = @"{
  ""Largura"": 32.0,
  ""Altura"": 22.0,
  ""Elementos"": [
    {
      ""Tipo"": ""Campo"",
      ""Conteudo"": ""Mercadoria"",
      ""X"": 0,
      ""Y"": 0,
      ""Largura"": 31,
      ""Altura"": 5,
      ""FonteNome"": ""Arial"",
      ""FonteTamanho"": 8.0,
      ""Negrito"": false,
      ""Italico"": false,
      ""CorR"": 0,
      ""CorG"": 0,
      ""CorB"": 0,
      ""ImagemBase64"": null
    },
    {
      ""Tipo"": ""Campo"",
      ""Conteudo"": ""PrecoVenda"",
      ""X"": 6,
      ""Y"": 16,
      ""Largura"": 18,
      ""Altura"": 3,
      ""FonteNome"": ""Arial"",
      ""FonteTamanho"": 8.0,
      ""Negrito"": false,
      ""Italico"": false,
      ""CorR"": 0,
      ""CorG"": 0,
      ""CorB"": 0,
      ""ImagemBase64"": null
    },
    {
      ""Tipo"": ""CodigoBarras"",
      ""Conteudo"": ""CodigoMercadoria"",
      ""X"": 4,
      ""Y"": 6,
      ""Largura"": 22,
      ""Altura"": 9,
      ""FonteNome"": ""Arial"",
      ""FonteTamanho"": 8.0,
      ""Negrito"": false,
      ""Italico"": false,
      ""CorR"": 0,
      ""CorG"": 0,
      ""CorB"": 0,
      ""ImagemBase64"": null
    }
  ]
}";

        // Template: Gondola (100x30mm)
        private const string TEMPLATE_GONDOLA = @"{
  ""Largura"": 100.0,
  ""Altura"": 30.0,
  ""Elementos"": [
    {
      ""Tipo"": ""Campo"",
      ""Conteudo"": ""Mercadoria"",
      ""X"": 0,
      ""Y"": 0,
      ""Largura"": 99,
      ""Altura"": 6,
      ""FonteNome"": ""Arial"",
      ""FonteTamanho"": 10.0,
      ""Negrito"": false,
      ""Italico"": false,
      ""CorR"": 0,
      ""CorG"": 0,
      ""CorB"": 0,
      ""ImagemBase64"": null
    },
    {
      ""Tipo"": ""CodigoBarras"",
      ""Conteudo"": ""CodigoMercadoria"",
      ""X"": 2,
      ""Y"": 8,
      ""Largura"": 63,
      ""Altura"": 20,
      ""FonteNome"": ""Arial"",
      ""FonteTamanho"": 8.0,
      ""Negrito"": false,
      ""Italico"": false,
      ""CorR"": 0,
      ""CorG"": 0,
      ""CorB"": 0,
      ""ImagemBase64"": null
    },
    {
      ""Tipo"": ""Campo"",
      ""Conteudo"": ""PrecoVenda"",
      ""X"": 64,
      ""Y"": 7,
      ""Largura"": 33,
      ""Altura"": 20,
      ""FonteNome"": ""Arial"",
      ""FonteTamanho"": 14.0,
      ""Negrito"": false,
      ""Italico"": false,
      ""CorR"": 0,
      ""CorG"": 0,
      ""CorB"": 0,
      ""ImagemBase64"": null
    }
  ]
}";

        // Template: Otica (94.9x10mm)
        private const string TEMPLATE_OTICA = @"{
  ""Largura"": 94.9,
  ""Altura"": 10.0,
  ""Elementos"": [
    {
      ""Tipo"": ""Campo"",
      ""Conteudo"": ""Mercadoria"",
      ""X"": 1,
      ""Y"": 1,
      ""Largura"": 48,
      ""Altura"": 3,
      ""FonteNome"": ""Arial"",
      ""FonteTamanho"": 8.0,
      ""Negrito"": false,
      ""Italico"": false,
      ""CorR"": 0,
      ""CorG"": 0,
      ""CorB"": 0,
      ""ImagemBase64"": null
    },
    {
      ""Tipo"": ""CodigoBarras"",
      ""Conteudo"": ""CodigoMercadoria"",
      ""X"": 65,
      ""Y"": 2,
      ""Largura"": 25,
      ""Altura"": 7,
      ""FonteNome"": ""Arial"",
      ""FonteTamanho"": 8.0,
      ""Negrito"": false,
      ""Italico"": false,
      ""CorR"": 0,
      ""CorG"": 0,
      ""CorB"": 0,
      ""ImagemBase64"": null
    },
    {
      ""Tipo"": ""Campo"",
      ""Conteudo"": ""PrecoVenda"",
      ""X"": 1,
      ""Y"": 5,
      ""Largura"": 34,
      ""Altura"": 4,
      ""FonteNome"": ""Arial"",
      ""FonteTamanho"": 8.0,
      ""Negrito"": false,
      ""Italico"": false,
      ""CorR"": 0,
      ""CorG"": 0,
      ""CorB"": 0,
      ""ImagemBase64"": null
    }
  ]
}";

        #endregion

        #region Configurações Pré-definidas (JSON)

        private static Dictionary<string, string> ObterConfiguracoesPreDefinidas()
        {
            return new Dictionary<string, string>
            {
                { "3 Colunas", CONFIG_3_COLUNAS },
                { "Gondola", CONFIG_GONDOLA },
                { "Otica", CONFIG_OTICA }
            };
        }

        // Configuração: 3 Colunas
        private const string CONFIG_3_COLUNAS = @"{
  ""NomeEtiqueta"": ""3 Colunas"",
  ""ImpressoraPadrao"": ""Microsoft Print to PDF"",
  ""PapelPadrao"": ""A4"",
  ""LarguraEtiqueta"": 32.0,
  ""AlturaEtiqueta"": 22.0,
  ""NumColunas"": 3,
  ""NumLinhas"": 1,
  ""EspacamentoColunas"": 0.15,
  ""EspacamentoLinhas"": 0.0,
  ""MargemSuperior"": 0.0,
  ""MargemInferior"": 0.0,
  ""MargemEsquerda"": 1.29,
  ""MargemDireita"": 0.0
}";

        // Configuração: Gondola
        private const string CONFIG_GONDOLA = @"{
  ""NomeEtiqueta"": ""Gondola"",
  ""ImpressoraPadrao"": ""Microsoft Print to PDF"",
  ""PapelPadrao"": ""A4"",
  ""LarguraEtiqueta"": 100.0,
  ""AlturaEtiqueta"": 30.0,
  ""NumColunas"": 1,
  ""NumLinhas"": 1,
  ""EspacamentoColunas"": 0.0,
  ""EspacamentoLinhas"": 0.0,
  ""MargemSuperior"": 0.0,
  ""MargemInferior"": 0.0,
  ""MargemEsquerda"": 0.0,
  ""MargemDireita"": 0.0
}";

        // Configuração: Otica
        private const string CONFIG_OTICA = @"{
  ""NomeEtiqueta"": ""Otica"",
  ""ImpressoraPadrao"": ""Microsoft Print to PDF"",
  ""PapelPadrao"": ""A4"",
  ""LarguraEtiqueta"": 94.9,
  ""AlturaEtiqueta"": 10.0,
  ""NumColunas"": 1,
  ""NumLinhas"": 1,
  ""EspacamentoColunas"": 0.0,
  ""EspacamentoLinhas"": 0.0,
  ""MargemSuperior"": 0.0,
  ""MargemInferior"": 0.0,
  ""MargemEsquerda"": 1.0,
  ""MargemDireita"": 0.0
}";

        #endregion
    }
}