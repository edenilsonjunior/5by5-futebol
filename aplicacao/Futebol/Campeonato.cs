using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private string _nome;
        private string _temporada;
        private StatusCampeonato _status;
        private SqlConnection _conexao;
        private List<Equipe> _equipesCompetindo;

        public string Nome => _nome;

        public string Temporada => _temporada;

        public string Status => _status.ToString();

        public Campeonato(string nome, string temporada, SqlConnection conexaoSql)
        {
            _nome = nome;
            _temporada = temporada;
            _status = StatusCampeonato.Iniciado;
            _conexao = conexaoSql;
            _equipesCompetindo = new();
        }

        public Campeonato(string nome, string temporada, string status, SqlConnection conexao)
        {
            _nome = nome;
            _temporada = temporada;
            _conexao = conexao;
            _equipesCompetindo = new();

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
            try
            {
                _conexao.Open();

                string sql = "SELECT COUNT(*) FROM Estatistica WHERE nome_camp = @nome_camp AND temp_camp = @temp_camp";
                var cmd = new SqlCommand(sql, _conexao);

                cmd.Parameters.AddWithValue("@nome_camp", SqlDbType.VarChar).Value = _nome;
                cmd.Parameters.AddWithValue("@temp_camp", SqlDbType.VarChar).Value = _temporada;

                totalEquipes = (int)cmd.ExecuteScalar();
            }
            catch (Exception)
            {
                Console.WriteLine("Nao foi possivel recuperar o total de jogos!");
            }
            finally
            {
                _conexao.Close();
            }

            return totalEquipes;
        }


        private int GetTotalJogos()
        {
            int totalJogos = 0;
            try
            {
                _conexao.Open();

                string sql = "SELECT COUNT(*) FROM Jogo WHERE nome_camp = @nome_camp AND temp_camp = @temp_camp";
                var cmd = new SqlCommand(sql, _conexao);

                cmd.Parameters.AddWithValue("@nome_camp", SqlDbType.VarChar).Value = _nome;
                cmd.Parameters.AddWithValue("@temp_camp", SqlDbType.VarChar).Value = _temporada;

                totalJogos = (int)cmd.ExecuteScalar();
            }
            catch (Exception)
            {
                Console.WriteLine("Nao foi possivel recuperar o total de jogos!");
            }
            finally
            {
                _conexao.Close();
            }

            return totalJogos;
        }


        private static void CriarEquipe(SqlConnection conexaoSql)
        {
            Console.Clear();
            Console.WriteLine("---|Inserir equipe|---");

            string nome = LerString("Digite o nome da equipe: ");
            string apelido = LerString("Digite o apelido da equipe: ");

            DateOnly dataCriacao;
            bool dataValida = false;

            while (!dataValida)
            {
                Console.Write("Digite a data de criacao da equipe: ");

                dataValida = DateOnly.TryParse(Console.ReadLine(), out dataCriacao);

                if (!dataValida)
                    Console.WriteLine("Data invalida! Digite novamente.");
            }

            var e = new Equipe(nome, apelido, dataCriacao);

            using (conexaoSql)
            {
                SqlCommand sql_cmnd = new SqlCommand("InserirEquipe", conexaoSql);
                sql_cmnd.CommandType = CommandType.StoredProcedure;

                sql_cmnd.Parameters.AddWithValue("@nome", SqlDbType.VarChar).Value = e.Nome;
                sql_cmnd.Parameters.AddWithValue("@apelido", SqlDbType.VarChar).Value = e.Apelido;
                sql_cmnd.Parameters.AddWithValue("@data_criacao_str", SqlDbType.VarChar).Value = e.DataCriacao.ToString();

                try
                {
                    sql_cmnd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Houve um problema com a sua inserção!\nNao foi possivel adicionar a equipe");
                    Console.WriteLine(ex.ToString());
                }
            }
        }


        private static void ExibirEquipes(SqlConnection conexaoSql)
        {
            conexaoSql.Open();

            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT * FROM Equipe";

            cmd.Connection = conexaoSql;


            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                Console.WriteLine("Equipes: ");
                while (reader.Read())
                {
                    Console.WriteLine("Nome: " + reader.GetString(0));
                    Console.WriteLine("Apelido: " + reader.GetString(1));
                    Console.WriteLine("DataCriacao: " + reader.GetDateTime(2));
                }
            }
            conexaoSql.Close();
        }


        private static string LerString(string titulo)
        {
            Console.Write("Digite o nome da equipe: ");
            string? str = Console.ReadLine();

            if (str == null)
            {
                return "";
            }

            return str;
        }
    }
}
