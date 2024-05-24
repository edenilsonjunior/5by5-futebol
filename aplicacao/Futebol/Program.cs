using Microsoft.Data.SqlClient;
using System.Data;

namespace Futebol
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var conn = new Banco();
                SqlConnection conexaoSql = new SqlConnection(conn.Conexao);
                switch (Menu())
                {
                    case 1:
                        CriarCampeonato(conexaoSql);
                        break;
                    case 2:
                        VoltarParaCampeonato(conexaoSql);
                        break;
                    case 3:
                        ListarFinalizados(conexaoSql);
                        break;
                    case 0:
                        Console.Write("Saindo...");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Opcao invalida!");
                        break;
                }
                Console.Write("Pressione qualquer tecla para voltar ao menu...");
                Console.ReadKey();
            }

        }


        private static int Menu()
        {
            Console.Clear();
            Console.WriteLine($"---|Campeonato de Futebol|---");

            Console.WriteLine("1- Criar novo campeonato");
            Console.WriteLine("2- Entrar em um campeonato em andamento");
            Console.WriteLine("3- Listar campeonatos finalizados");
            Console.WriteLine("0- Voltar");
            Console.Write("R: ");

            if (int.TryParse(Console.ReadLine(), out int option))
            {
                Console.WriteLine($"Opcao: {option}");
                return option;
            }
            else
            {
                Console.WriteLine("Voce deve digitar um numero!");
                Console.Write("Pressione qualquer tecla para continuar...");
                Console.ReadKey();
                return Menu();
            }
        }


        private static void CriarCampeonato(SqlConnection conexaoSql)
        {
            Console.Clear();
            Console.WriteLine($"---|Criar campeonato|---");

            string nome = LerString("Digite o nome do campeonato: ");
            string temporada = LerString("Digite a temporada do campeonato: ");
            int totalCampeonatos = 0;

            try
            {
                conexaoSql.Open();
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Campeonato WHERE nome = @nome AND temporada = @temporada", conexaoSql))
                {
                    cmd.Parameters.AddWithValue("@nome", SqlDbType.VarChar).Value = nome;
                    cmd.Parameters.AddWithValue("@temporada", SqlDbType.VarChar).Value = temporada;

                    totalCampeonatos = (int)cmd.ExecuteScalar();
                }

                if (totalCampeonatos == 0)
                {
                    using var cmd = new SqlCommand("INSERT INTO Campeonato VALUES (@nome, @temporada, @status);", conexaoSql);

                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@temporada", temporada);
                    cmd.Parameters.AddWithValue("@status", "Iniciado");

                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Erro SQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
            finally
            {
                if (conexaoSql.State == ConnectionState.Open)
                    conexaoSql.Close();
            }

            if (totalCampeonatos != 0)
                Console.WriteLine("Ja existe um campeonato com esse mesmo nome e temporada!");
            else
                new Campeonato(nome, temporada, conexaoSql).Executar();
        }


        private static void VoltarParaCampeonato(SqlConnection conexaoSql)
        {
            Console.WriteLine($"---|Voltar para um determinado campeonato|---");

            int totalCampeonatos = 0;
            var listaCampeonatos = new List<Campeonato>();

            try
            {
                conexaoSql.Open();

                using SqlCommand cmd = new SqlCommand("SELECT * FROM Campeonato WHERE status = @status", conexaoSql);

                cmd.Parameters.AddWithValue("@status", SqlDbType.VarChar).Value = StatusCampeonato.Acontecendo.ToString();

                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string nome = reader.GetString(0);
                        string temporada = reader.GetString(1);
                        string status = reader.GetString(2);

                        listaCampeonatos.Add(new(nome, temporada, status, conexaoSql));

                        Console.WriteLine($"-->Campeonato: {totalCampeonatos + 1}");
                        Console.WriteLine("Nome......: " + nome);
                        Console.WriteLine("Temporada.: " + temporada);
                        Console.WriteLine("Status....: " + status);

                        totalCampeonatos++;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Erro SQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
            finally
            {
                if (conexaoSql.State == ConnectionState.Open)
                    conexaoSql.Close();
            }

            if (totalCampeonatos != 0)
            {
                string nomeCampeonatoEscolhido = LerString("Digite o nome do campeonato: ");

                Campeonato? c = listaCampeonatos.Find(c => c.Nome.Equals(nomeCampeonatoEscolhido));

                if (c == null)
                    Console.WriteLine("Nome invalido!");
                else
                {
                    c.Executar();
                }
            }
            else
            {
                Console.WriteLine("Nao existem campeonatos em andamento!");
            }

        }


        private static void ListarFinalizados(SqlConnection conexaoSql)
        {
            try
            {
                conexaoSql.Open();

                using SqlCommand cmd = new SqlCommand("SELECT * FROM Campeonato WHERE status = @status", conexaoSql);

                cmd.Parameters.AddWithValue("@status", SqlDbType.VarChar).Value = StatusCampeonato.Acontecendo.ToString();

                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    int atual = 1;
                    while (reader.Read())
                    {

                        Console.WriteLine($"-->Campeonato: " + atual++);
                        Console.WriteLine("Nome......: " + reader.GetString(0));
                        Console.WriteLine("Temporada.: " + reader.GetString(1));
                        Console.WriteLine("Status....: " + reader.GetString(2));
                        Console.WriteLine("=====================");
                    }
                }
                else
                    Console.WriteLine("Nao existem campeonatos finalizados!");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Erro SQL: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
            finally
            {
                if (conexaoSql.State == ConnectionState.Open)
                    conexaoSql.Close();
            }
        }


        private static string LerString(string titulo)
        {
            Console.Write(titulo);
            string? str = Console.ReadLine();

            if (str == null)
            {
                return "";
            }

            return str;
        }
    }
}
