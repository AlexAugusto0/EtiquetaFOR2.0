using System;
using System.Data;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using EtiquetaFORNew.Data;

namespace EtiquetaFORNew.Data
{
    /// <summary>
    /// Gerencia o banco de dados SQLite local para cache de mercadorias
    /// </summary>
    public class LocalDatabaseManager
    {
        public static bool isConfeccao = false;
        private static readonly string DbPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "LocalData.db");
        
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";
  
        /// <summary>
        /// Inicializa o banco local e cria as tabelas se não existirem
        /// </summary>
        public static void InicializarBanco()
        {
            try
            {
                // Criar arquivo do banco se não existir
                if (!File.Exists(DbPath))
                {
                    SQLiteConnection.CreateFile(DbPath);
                }

                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    // ⭐ CORRIGIDO: Índices agora são criados separadamente
                    string createTable = @"
                        CREATE TABLE IF NOT EXISTS Mercadorias (
                            CodigoMercadoria INTEGER,
                            CodFabricante TEXT,
                            CodBarras TEXT,
                            Mercadoria TEXT NOT NULL,
                            PrecoVenda REAL,
                            VendaA REAL,
                            VendaB REAL,
                            VendaC REAL,
                            Fornecedor TEXT,
                            Fabricante TEXT,
                            Grupo TEXT,
                            Prateleira TEXT,
                            Tam TEXT,
                            Cores TEXT,
                            CodBarras_Grade TEXT,
                            Registro INTEGER,
                            UltimaAtualizacao DATETIME DEFAULT CURRENT_TIMESTAMP
                        );

                        CREATE INDEX IF NOT EXISTS idx_codfabricante ON Mercadorias(CodFabricante);
                        CREATE INDEX IF NOT EXISTS idx_mercadoria ON Mercadorias(Mercadoria);
                        CREATE INDEX IF NOT EXISTS idx_codbarras ON Mercadorias(CodBarras);
                        CREATE INDEX IF NOT EXISTS idx_fornecedor ON Mercadorias(Fornecedor);
                        CREATE INDEX IF NOT EXISTS idx_fabricante ON Mercadorias(Fabricante);
                        CREATE INDEX IF NOT EXISTS idx_grupo ON Mercadorias(Grupo);

                        CREATE TABLE IF NOT EXISTS ProdutosSelecionados (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            CodigoMercadoria INTEGER,
                            Nome TEXT NOT NULL,
                            Codigo TEXT NOT NULL,
                            Preco REAL NOT NULL,
                            Quantidade INTEGER NOT NULL,
                            DataSelecao DATETIME DEFAULT CURRENT_TIMESTAMP,
                            FOREIGN KEY (CodigoMercadoria) REFERENCES Mercadorias(CodigoMercadoria)
                        );

                        CREATE TABLE IF NOT EXISTS ConfiguracaoSync (
                            Id INTEGER PRIMARY KEY,
                            UltimaSincronizacao DATETIME,
                            TotalRegistros INTEGER
                        );
                    ";

                    using (var cmd = new SQLiteCommand(createTable, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao inicializar banco local: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sincroniza mercadorias do SQL Server para o SQLite local
        /// </summary>
        /// <param name="filtro">Filtro opcional (ex: WHERE PrecoVenda > 0)</param>
        /// <param name="limite">Limite de registros (0 = todos)</param>
        public static int SincronizarMercadorias(string filtro = "", int limite = 0)
        {
            try
            {
                string sqlServerConnStr = DatabaseConfig.GetConnectionString();
                DatabaseConfig.ConfigData config = DatabaseConfig.LoadConfiguration();

                if (string.IsNullOrEmpty(sqlServerConnStr))
                {
                    throw new Exception("Configuração do SQL Server não encontrada!");
                }

                int registrosImportados = 0;

                // Buscar dados do SQL Server
                using (var sqlConn = new SqlConnection(sqlServerConnStr))
                {
                    sqlConn.Open();

                    string query = @"
                        SELECT TOP " + (limite > 0 ? limite.ToString() : "999999") + @"
                            [Código da Mercadoria] as CodigoMercadoria,
                            [Cód Fabricante] as CodFabricante,
                            [Cód Barra] as CodBarras,
                            [Mercadoria],
                            [Preço de Venda] as PrecoVenda,
                            [VendaA] as VendaA,
                            [VendaB] as VendaB,
                            [VendaC] as VendaC,
                            [Fornecedor] as Fornecedor,
                            [Fabricante] as Fabricante,
                            [Grupo] as Grupo,
                            [Prateleira] as Prateleira,
                            [Tam] as Tam,
                            [Cores] as Cores,
                            [CodBarras] as CodBarras_Grade
                        FROM [memoria_MercadoriasLojas]
                        WHERE [Loja] = '" + config.Loja + @"'
                        " + (string.IsNullOrEmpty(filtro) ? "" : "AND " + filtro) + @"
                        ORDER BY [Código da Mercadoria]
                    ";

                    using (var sqlCmd = new SqlCommand(query, sqlConn))
                    using (var reader = sqlCmd.ExecuteReader())
                    {
                        // Inserir no SQLite
                        using (var localConn = new SQLiteConnection(ConnectionString))
                        {
                            localConn.Open();

                            // Limpar dados antigos
                            using (var deleteCmd = new SQLiteCommand("DELETE FROM Mercadorias", localConn))
                            {
                                deleteCmd.ExecuteNonQuery();
                            }

                            // Iniciar transação para performance
                            using (var transaction = localConn.BeginTransaction())
                            {
                                string insertQuery = @"
                                    INSERT INTO Mercadorias 
                                    (CodigoMercadoria, CodFabricante, CodBarras, Mercadoria, PrecoVenda, VendaA, VendaB, VendaC, Fornecedor, Fabricante, Grupo, Prateleira, Tam, Cores,CodBarras_Grade)
                                    VALUES (@cod, @fabr, @barras, @merc, @preco, @vendaA, @vendaB, @vendaC, @fornecedor, @fabricante, @grupo, @prateleira,@tam ,@cores, @codbarras_grade)
                                ";

                                using (var insertCmd = new SQLiteCommand(insertQuery, localConn))
                                {
                                    while (reader.Read())
                                    {
                                        insertCmd.Parameters.Clear();
                                        insertCmd.Parameters.AddWithValue("@cod", reader["CodigoMercadoria"]);
                                        insertCmd.Parameters.AddWithValue("@fabr", reader["CodFabricante"] ?? DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@barras", reader["CodBarras"] ?? DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@merc", reader["Mercadoria"]);
                                        insertCmd.Parameters.AddWithValue("@preco", reader["PrecoVenda"] ?? DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@vendaA", reader["VendaA"] ?? DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@vendaB", reader["VendaB"] ?? DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@vendaC", reader["VendaC"] ?? DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@fornecedor", reader["Fornecedor"] ?? DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@fabricante", reader["Fabricante"] ?? DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@grupo", reader["Grupo"] ?? DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@prateleira", reader["Prateleira"] ?? DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@tam", reader["Tam"] ?? DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@cores", reader["Cores"] ?? DBNull.Value);
                                        insertCmd.Parameters.AddWithValue("@codbarras_grade", reader["CodBarras_Grade"] ?? DBNull.Value);
                                        //insertCmd.Parameters.AddWithValue("@reg", reader["Registro"] ?? DBNull.Value);

                                        insertCmd.ExecuteNonQuery();
                                        registrosImportados++;
                                    }
                                }

                                transaction.Commit();
                            }

                            // Atualizar info de sincronização
                            string updateSync = @"
                                INSERT OR REPLACE INTO ConfiguracaoSync (Id, UltimaSincronizacao, TotalRegistros)
                                VALUES (1, datetime('now'), @total)
                            ";
                            using (var syncCmd = new SQLiteCommand(updateSync, localConn))
                            {
                                syncCmd.Parameters.AddWithValue("@total", registrosImportados);
                                syncCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                return registrosImportados;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao sincronizar mercadorias: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Busca mercadorias no banco local
        /// </summary>
        public static DataTable BuscarMercadorias(string termoBusca = "", int limite = 100)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            CodigoMercadoria,
                            CodFabricante,
                            CodBarras,
                            Mercadoria,
                            PrecoVenda,
                            VendaA,
                            VendaB,
                            VendaC,
                            Fornecedor,
                            Fabricante,
                            Grupo,
                            Prateleira,
                            Tam,
                            Cores,
                            CodBarras_Grade,
                            Registro
                        FROM Mercadorias
                    ";

                    if (!string.IsNullOrEmpty(termoBusca))
                    {
                        query += @" 
                            WHERE Mercadoria LIKE @termo 
                            OR CodFabricante LIKE @termo 
                            OR CodBarras LIKE @termo
                            OR CodigoMercadoria = @codigo
                        ";
                    }

                    query += " ORDER BY Mercadoria LIMIT " + limite;

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(termoBusca))
                        {
                            cmd.Parameters.AddWithValue("@termo", "%" + termoBusca + "%");

                            // Tentar converter para número
                            if (int.TryParse(termoBusca, out int codigo))
                            {
                                cmd.Parameters.AddWithValue("@codigo", codigo);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@codigo", -1);
                            }
                        }

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar mercadorias: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Busca mercadoria por código
        /// </summary>
        ///         /// <summary>
        /// ⭐ NOVO: Busca mercadorias filtrando por um campo específico (busca dinâmica)
        /// </summary>
        /// <param name="termoBusca">Termo a ser buscado</param>
        /// <param name="nomeCampo">Nome do campo específico (Mercadoria, CodFabricante, CodigoMercadoria)</param>
        /// <param name="limite">Limite de resultados (padrão 500 para performance)</param>
        public static DataTable BuscarMercadorias(string termoBusca, string nomeCampo, int limite = 500)
        {
            if (string.IsNullOrWhiteSpace(nomeCampo))
                return BuscarMercadorias(termoBusca, limite); // Usa busca genérica

            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    // ⭐ BUSCA ESPECÍFICA POR CAMPO COM LIMIT CONFIGURÁVEL
                    string query = $@"
                        SELECT DISTINCT 
                            CodigoMercadoria,
                            CodFabricante,
                            CodBarras,
                            Mercadoria,
                            PrecoVenda,
                            VendaA,
                            VendaB,
                            VendaC,
                            Fornecedor,
                            Fabricante,
                            Grupo,
                            Prateleira,
                            Tam,
                            Cores,
                            CodBarras_Grade,
                            Registro
                        FROM Mercadorias
                        WHERE {nomeCampo} LIKE @termo
                        ORDER BY {nomeCampo}
                        LIMIT {limite}";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@termo", "%" + termoBusca + "%");

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar mercadorias por {nomeCampo}: {ex.Message}", ex);
            }
        }
        public static DataRow BuscarMercadoriaPorCodigo(int codigo)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT * FROM Mercadorias 
                        WHERE CodigoMercadoria = @codigo
                    ";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@codigo", codigo);

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar mercadoria: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Adiciona produto à lista de selecionados
        /// </summary>
        public static void AdicionarProdutoSelecionado(int codigoMercadoria, string nome,
            string codigo, decimal preco, int quantidade)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO ProdutosSelecionados 
                        (CodigoMercadoria, Nome, Codigo, Preco, Quantidade)
                        VALUES (@codMerc, @nome, @cod, @preco, @qtd)
                    ";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@codMerc", codigoMercadoria);
                        cmd.Parameters.AddWithValue("@nome", nome);
                        cmd.Parameters.AddWithValue("@cod", codigo);
                        cmd.Parameters.AddWithValue("@preco", preco);
                        cmd.Parameters.AddWithValue("@qtd", quantidade);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar produto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtém produtos selecionados
        /// </summary>
        public static DataTable ObterProdutosSelecionados()
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT * FROM ProdutosSelecionados 
                        ORDER BY DataSelecao DESC
                    ";

                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter produtos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Limpa produtos selecionados
        /// </summary>
        public static void LimparProdutosSelecionados()
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("DELETE FROM ProdutosSelecionados", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao limpar produtos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Remove produto selecionado por ID
        /// </summary>
        public static void RemoverProdutoSelecionado(int id)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("DELETE FROM ProdutosSelecionados WHERE Id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao remover produto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtém informações da última sincronização
        /// </summary>
        public static (DateTime? ultima, int total) ObterInfoSincronizacao()
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = "SELECT UltimaSincronizacao, TotalRegistros FROM ConfiguracaoSync WHERE Id = 1";

                    using (var cmd = new SQLiteCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DateTime? ultima = reader["UltimaSincronizacao"] != DBNull.Value
                                ? Convert.ToDateTime(reader["UltimaSincronizacao"])
                                : (DateTime?)null;

                            int total = reader["TotalRegistros"] != DBNull.Value
                                ? Convert.ToInt32(reader["TotalRegistros"])
                                : 0;

                            return (ultima, total);
                        }
                    }
                }

                return (null, 0);
            }
            catch
            {
                return (null, 0);
            }
        }

        /// <summary>
        /// Verifica se precisa sincronizar (última sync > 24h)
        /// </summary>
        public static bool PrecisaSincronizar()
        {
            var (ultima, _) = ObterInfoSincronizacao();

            if (ultima == null) return true;

            return (DateTime.Now - ultima.Value).TotalHours > 24;
        }

        /// <summary>
        /// Obtém total de mercadorias no banco local
        /// </summary>
        public static int ObterTotalMercadorias()
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Mercadorias", conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Busca todos os tamanhos e cores disponíveis para um produto específico
        /// Retorna listas distintas de TODOS os registros daquele produto
        /// </summary>
        public static (List<string> tamanhos, List<string> cores) BuscarTamanhosECoresPorCodigo(int codigo)
        {
            var tamanhos = new List<string>();
            var cores = new List<string>();
            

            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))

                {
                    conn.Open();

                    string query;
                    

                        query = @"
                        SELECT DISTINCT Tam, Cores
                        FROM Mercadorias
                        WHERE CodigoMercadoria = @codigo
                        AND (Tam IS NOT NULL OR Cores IS NOT NULL)
                    ";


                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@codigo", codigo);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Processar Tamanhos
                                string tam = reader["Tam"]?.ToString();
                                if (!string.IsNullOrEmpty(tam))
                                {
                                    if (!tamanhos.Contains(tam))
                                        tamanhos.Add(tam);
                                }

                                // Processar Cores
                                string cor = reader["Cores"]?.ToString();
                                if (!string.IsNullOrEmpty(cor))
                                {
                                    if (!cores.Contains(cor))
                                        cores.Add(cor);
                                }
                            }
                        }
                    }
                }

                // Ordenar para melhor apresentação
                tamanhos.Sort();
                cores.Sort();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar tamanhos e cores: {ex.Message}", ex);
            }

            return (tamanhos, cores);
        }
    }
}