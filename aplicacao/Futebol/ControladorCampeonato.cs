using Microsoft.Data.SqlClient;
using System.Data;

namespace Futebol
{
    internal class ControladorCampeonato
    {
        private readonly string _iniciado;
        private readonly string _acontecendo;
        private readonly string _finalizado;
        private SqlConnection _conexao;

        public ControladorCampeonato()
        {
            _iniciado = StatusCampeonato.Iniciado.ToString();
            _acontecendo = StatusCampeonato.Acontecendo.ToString();
            _finalizado = StatusCampeonato.Acontecendo.ToString();

            var conn = new Banco();
            _conexao = new SqlConnection(conn.Conexao);
        }

        public void Executar()
        {

            while (true)
            {
                switch (Menu())
                {
                    case 1:
                        CriarCampeonato();
                        break;
                    case 2:
                        VoltarParaCampeonato();
                        break;
                    case 3:
                        ListarFinalizados();
                        break;
                    case 0:
                        if (_conexao.State == ConnectionState.Open)
                            _conexao.Close();

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
            Console.WriteLine("2- Entrar em um campeonato nao finalizado");
            Console.WriteLine("3- Listar campeonatos finalizados");
            Console.WriteLine("0- Sair");
            Console.Write("R: ");

            if (int.TryParse(Console.ReadLine(), out int option))
                return option;

            Console.WriteLine("Voce deve digitar um numero!");
            Console.Write("Pressione qualquer tecla para continuar...");
            Console.ReadKey();
            return Menu();
        }


        private void CriarCampeonato()
        {
            Console.Clear();
            Console.WriteLine("---|Criar campeonato|---");

            string nome = LerString("Digite o nome do campeonato: ");
            string temporada = LerString("Digite a temporada do campeonato: ");
            int resultadoProc = -1;

            using var cmd = new SqlCommand("InserirCampeonato", _conexao);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@nome", nome);
            cmd.Parameters.AddWithValue("@temporada", temporada);
            cmd.Parameters.AddWithValue("@status", StatusCampeonato.Iniciado.ToString());

            // adicionando uma variavel que vai ser o retorno da procedure
            var retorno = new SqlParameter("@resultado", SqlDbType.Int);
            retorno.Direction = ParameterDirection.Output;

            try
            {
                using (_conexao)
                {
                    _conexao.Open();
                    cmd.ExecuteNonQuery();
                    resultadoProc = (int)retorno.Value;
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine($"Erro SQL: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro: {e.Message}");
            }

            if (resultadoProc == 0)
                Console.WriteLine("Ja existe um campeonato com esse nome!");
            else if (resultadoProc > 0)
            {
                Console.WriteLine("Campeonato cadastrado!");
                new Campeonato(nome, temporada, _conexao).Executar();
            }
        }


        private void VoltarParaCampeonato()
        {
            Console.WriteLine($"---|Voltar para um determinado campeonato|---");

            int totalCampeonatos = 0;
            var listaCampeonatos = new List<Campeonato>();

            using var cmd = new SqlCommand("SELECT * FROM Campeonato WHERE status = @iniciado OR status = @acontecendo", _conexao);
            cmd.Parameters.AddWithValue("@iniciado", _iniciado);
            cmd.Parameters.AddWithValue("@acontecendo", _acontecendo);

            try
            {
                using (_conexao)
                {
                    _conexao.Open();
                    using SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string nome = reader.GetString(0);
                            string temporada = reader.GetString(1);
                            string status = reader.GetString(2);

                            Console.WriteLine($"-->Campeonato: {totalCampeonatos + 1}");
                            Console.WriteLine("Nome......: " + nome);
                            Console.WriteLine("Temporada.: " + temporada);
                            Console.WriteLine("Status....: " + status);

                            listaCampeonatos.Add(new(nome, temporada, status, _conexao));
                            totalCampeonatos++;
                        }
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

            if (totalCampeonatos == 0)
            {
                Console.WriteLine("Nao existem campeonatos em andamento!");
                return;
            }


            string escolhido = LerString("Digite o nome do campeonato: ");
            Campeonato? c = listaCampeonatos.Find(c => c.Nome.Equals(escolhido));

            if (c == null)
                Console.WriteLine("Nome invalido!");
            else
                c.Executar();

        }


        private void ListarFinalizados()
        {
            var cmd = new SqlCommand("SELECT * FROM Campeonato WHERE status = @status", _conexao);
            cmd.Parameters.AddWithValue("@status", _finalizado);

            try
            {
                using (_conexao)
                {
                    _conexao.Open();
                    using var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        int atual = 1;
                        while (reader.Read())
                        {
                            Console.WriteLine($"-->Campeonato: " + atual++);
                            ExibirCampeonato(reader.GetString(0), reader.GetString(1), reader.GetString(2));

                        }
                    }
                    else
                        Console.WriteLine("Nao existem campeonatos finalizados!");
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
        }


        private static void ExibirCampeonato(string nome, string temporada, string status)
        {
            Console.WriteLine($"Nome......: {nome}");
            Console.WriteLine($"Temporada.: {temporada}");
            Console.WriteLine($"Status....: {status}");
            Console.WriteLine("=====================");
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
