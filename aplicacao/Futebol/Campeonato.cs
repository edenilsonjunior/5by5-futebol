using Microsoft.Data.SqlClient;
using System.Data;

namespace Futebol
{
    enum StatusCampeonato
    {
        Iniciado,
        Acontecendo,
        Finalizado
    }

    internal class Campeonato
    {
        private readonly string _nome;
        private readonly string _temporada;
        private StatusCampeonato _status;
        private readonly SqlConnection _conexao;
        private List<Equipe> _equipes;

        public string Nome => _nome;

        public string Temporada => _temporada;

        public string Status => _status.ToString();

        public Campeonato(string nome, string temporada, SqlConnection conexaoSql)
        {
            _nome = nome;
            _temporada = temporada;
            _status = StatusCampeonato.Iniciado;
            _conexao = conexaoSql;
            _equipes = new();
        }

        public Campeonato(string nome, string temporada, string status, SqlConnection conexao)
        {
            _nome = nome;
            _temporada = temporada;
            _conexao = conexao;
            _equipes = new();

            switch (status)
            {
                case "Iniciado":
                    _status = StatusCampeonato.Iniciado;
                    break;
                case "Acontecendo":
                    _status = StatusCampeonato.Acontecendo;
                    break;
                case "Finalizado":
                    _status = StatusCampeonato.Finalizado;
                    break;
                default:
                    break;
            }
        }

        public void Executar()
        {

            switch (_status)
            {
                case StatusCampeonato.Iniciado:
                    _status = StatusCampeonato.Acontecendo;
                    IniciarCampeonato();
                    break;
                case StatusCampeonato.Acontecendo:
                    ContinuarCampeonato();
                    break;
                case StatusCampeonato.Finalizado:
                    Console.WriteLine("O campeonato ja foi finalizado!");
                    break;
                default:
                    break;
            }
        }


        private void IniciarCampeonato()
        {
            int qntTimes;
            string? s;

            Console.Write("Digite o numero de times competindo (3-5):");
            s = Console.ReadLine();

            while (int.TryParse(s, out qntTimes) == false && (qntTimes < 3 || qntTimes > 5))
            {
                Console.WriteLine("Resposta invalida! Digite novamente: ");
                Console.Write("R: ");
                s = Console.ReadLine();
            }

            for (int i = 0; i < qntTimes; i++)
            {
                Equipe p = CriarEquipe();

                _equipes.Add(p);
            }

            InserirEquipes();

            /*
                1- Fazer todos os jogos (a cada jogo, perguntar se deseja continuar
                2- Mudar o status do campeonato para StatusCampeonato.Finalizado e atualizar no banco
                3- Re-pensar o metodo CriarEquipe para que seja possivel adicionar uma equipe que ja existe.
                4- Adicionar todos os jogos ao banco
                5- Mostrar placar
                6- Pensar em uma maneira de fazer apenas um metodo de jogos tanto para IniciarCampeonato() quanto para ContinuarCampeonato()
             */

            throw new NotImplementedException("Implementar metodo de iniciar campeonato");
        }


        private void ContinuarCampeonato()
        {
            throw new NotImplementedException("Implementar metodo de continuar um campeonato");
            // apos o termino de todos os jogos, o _status muda para StatusCampeonato.Finalizado
        }


        private int GetTotalEquipes()
        {
            int totalEquipes = 0;
            string sql = "SELECT COUNT(*) FROM Estatistica WHERE nome_camp = @nome_camp AND temp_camp = @temp_camp";
            var cmd = new SqlCommand(sql, _conexao);

            cmd.Parameters.AddWithValue("@nome_camp", _nome);
            cmd.Parameters.AddWithValue("@temp_camp", _temporada);

            try
            {
                using (_conexao)
                {
                    _conexao.Open();
                    totalEquipes = (int)cmd.ExecuteScalar();
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

            return totalEquipes;
        }


        private int GetTotalJogos()
        {
            int totalJogos = 0;
            string sql = "SELECT COUNT(*) FROM Jogo WHERE nome_camp = @nome_camp AND temp_camp = @temp_camp";
            var cmd = new SqlCommand(sql, _conexao);

            cmd.Parameters.AddWithValue("@nome_camp", _nome);
            cmd.Parameters.AddWithValue("@temp_camp", _temporada);

            try
            {
                using (_conexao)
                {
                    _conexao.Open();
                    totalJogos = (int)cmd.ExecuteScalar();
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

            return totalJogos;
        }


        private Equipe CriarEquipe()
        {
            Console.Clear();
            Console.WriteLine("---|Inserir equipe|---");

            string nome = LerString("Digite o nome da equipe: ");
            string apelido = LerString("Digite o apelido da equipe: ");

            DateOnly dataCriacao;

            Console.Write("Digite a data de criacao da equipe: ");
            string? s = Console.ReadLine();

            while (DateOnly.TryParse(s, out dataCriacao) == false || dataCriacao > DateOnly.FromDateTime(DateTime.Now))
            {
                Console.WriteLine("Data invalida!");
                Console.Write("Digite a data de criacao da equipe: ");
                s = Console.ReadLine();
            }

            return new Equipe(nome, apelido, dataCriacao);
        }

        public void InserirEquipes()
        {
            string comando = "INSERT INTO Estatistica (nome_camp, temp_camp, nome_equipe, pontos, gols_marcados, gols_sofridos) VALUES";
            string nomeCampFormat = FormatarStr(Nome, 30);
            string tempCampFormat = FormatarStr(Temporada, 10);

            for (int i = 0; i < _equipes.Count; i++)
            {
                comando += $" (@nome_camp, @temp_camp, @nome_equipe{i}, 0, 0, 0), ";
            }
            comando = comando.Substring(0, comando.Length - 2);


            var cmd = new SqlCommand(comando, _conexao);

            cmd.Parameters.AddWithValue($"@nome_camp", nomeCampFormat);
            cmd.Parameters.AddWithValue($"@temp_camp", tempCampFormat);

            for (int i = 0; i < _equipes.Count; i++)
            {
                string nomeEquipeFormatado = FormatarStr(_equipes[i].Nome, 30);
                cmd.Parameters.AddWithValue($"@nome_equipe{i}", nomeEquipeFormatado);
            }

            try
            {
                using (_conexao)
                {
                    _conexao.Open();
                    cmd.ExecuteNonQuery();
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
        }




        public void ExibirEquipes()
        {
            using var cmd = new SqlCommand("SELECT nome_equipe FROM Estatistica WHERE nome_camp = @nome_camp AND temp_camp = @temp_camp", _conexao);

            cmd.Parameters.Add(new SqlParameter("@nome_camp", SqlDbType.VarChar, 30)).Value = Nome;
            cmd.Parameters.Add(new SqlParameter("@temp_camp", SqlDbType.VarChar, 30)).Value = Temporada;

            try
            {
                using (_conexao)
                {
                    _conexao.Open();

                    using SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        Console.WriteLine("Equipes participantes: ");
                        int atual = 1;
                        while (reader.Read())
                        {
                            Console.WriteLine($"Equipe {atual:00}: " + reader.GetString(0));
                            atual++;
                        }
                    }
                    else
                        Console.WriteLine("Esse campeonato ainda nao possui equipes competindo!");
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


        private static string FormatarStr(string texto, int tamanho)
        {
            if (texto.Length > tamanho)
            {
                texto = texto.Substring(0, tamanho);
            }

            return texto;
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



/*
 **** Codigo nao usado para usar a procedure de inserir equipe



    var cmd = new SqlCommand("InserirEquipe", _conexao)
    {
        CommandType = CommandType.StoredProcedure
    };

    cmd.Parameters.AddWithValue("@nome", e.Nome);
    cmd.Parameters.AddWithValue("@apelido", e.Apelido);
    cmd.Parameters.AddWithValue("@data_criacao_str", e.DataCriacao.ToString());

    try
    {
        using (_conexao)
        {
            _conexao.Open();
            cmd.ExecuteNonQuery();
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
*/
