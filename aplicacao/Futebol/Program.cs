using Microsoft.Data.SqlClient;
using System.Data;

namespace Futebol
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string[] opcoes = { "1- Criar novo campeonato", "2- Entrar em um campeonato em andamento", "3- Listar campeonatos finalizados" };

            while (true)
            {
                var conn = new Banco();
                SqlConnection conexaoSql = new SqlConnection(conn.Conexao);
                switch (Menu(opcoes))
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


        private static int Menu(string[] opcoes)
        {
            Console.Clear();
            Console.WriteLine($"---|Campeonato de Futebol|---");

            foreach (var i in opcoes)
                Console.WriteLine(i);
            Console.WriteLine("0- Voltar");
            Console.Write("R: ");

            if (int.TryParse(Console.ReadLine(), out int option))
                return option;

            Console.WriteLine("Voce deve digitar um numero!");
            Console.Write("Pressione qualquer tecla para continuar...");
            Console.ReadKey();
            return Menu(opcoes);
        }


        private static void CriarCampeonato(SqlConnection conexaoSql)
        {
            Console.Clear();
            Console.WriteLine($"---|Criar campeonato|---");

            string nome = LerString("Digite o nome do campeonato: ");
            string temporada = LerString("Digite a temporada do campeonato: ");

            try
            {
                conexaoSql.Open();

                var cmd = new SqlCommand("InserirCampeonato", conexaoSql);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@nome", SqlDbType.VarChar, 30)).Value = nome;
                cmd.Parameters.Add(new SqlParameter("@temporada", SqlDbType.VarChar, 10)).Value = temporada;
                cmd.Parameters.Add(new SqlParameter("@status", SqlDbType.VarChar, 30)).Value = StatusCampeonato.Iniciado.ToString();

                // adicionando uma variavel que vai ser o retorno da procedure
                var retorno = new SqlParameter("@resultado", SqlDbType.Int);
                retorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(retorno);

                cmd.ExecuteNonQuery();

                int resultadoProc = (int)retorno.Value;

                if (resultadoProc == 0)
                    Console.WriteLine("Ja existe um campeonato com esse nome!");
                else
                    new Campeonato(nome, temporada, conexaoSql).Executar();
            }
            catch (SqlException e)
            {
                Console.WriteLine($"Erro SQL: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro: {e.Message}");
            }
            finally
            {
                if (conexaoSql.State == ConnectionState.Open)
                    conexaoSql.Close();
            }
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
                    c.Executar();
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
