using Microsoft.Data.SqlClient;
using System.Data;

namespace Futebol.Campeonato
{
    internal class ControladorCampeonato
    {
        public ControladorCampeonato() { }

        public void Executar()
        {

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=====|Campeonato de Futebol|=====");
                Console.WriteLine("1- Criar novo campeonato");
                Console.WriteLine("2- Entrar em um campeonato");
                Console.WriteLine("0- Sair");
                Console.WriteLine($"=================================");

                int escolha = Utils.LerInt("Digite sua escolha:");
                switch (escolha)
                {
                    case 1:
                        CriarCampeonato();
                        break;
                    case 2:
                        VoltarParaCampeonato();
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

        private void CriarCampeonato()
        {
            Console.Clear();
            Console.WriteLine("=====|Criar campeonato|=====");

            string nome = Utils.LerString("Digite o nome do campeonato: ");
            string temporada = Utils.LerString("Digite a temporada do campeonato: ");

            int insercao = InserirCampeonato(nome, temporada);

            if (insercao == 1)
            {
                Console.WriteLine("============================");

                Console.WriteLine("\n***Campeonato cadastrado!***");
                Console.WriteLine("Pressione qualquer tecla para continuar...");
                Console.ReadKey();

                var c = new Campeonato(nome, temporada)
                {
                    Jogos = CarregarJogos(nome, temporada),
                    Equipes = CadastrarEquipes(nome, temporada),
                };
                c.Executar();

                SalvarDadosEquipes(c.Equipes, c.Nome, c.Temporada);
                SalvarJogos(c.Jogos, c.Nome, c.Temporada);
                SalvarStatusCampeonato(c);
            }
            else if (insercao == 0)
                Console.WriteLine("Ja existe um campeonato com esse nome!");
            else
                Console.WriteLine("Houve um erro ao inserir o campeonato!");
        }

        private void VoltarParaCampeonato()
        {
            Console.Clear();
            Console.WriteLine($"=====|Voltar para um determinado campeonato|=====");

            int total = 0;
            var listaCampeonatos = new List<Campeonato>();


            LidarComException(() =>
            {
                using var conexao = new SqlConnection(new Banco().Conexao);
                using var cmd = new SqlCommand("SELECT * FROM Campeonato", conexao);

                conexao.Open();

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var c = new Campeonato(reader.GetString(0), reader.GetString(1), reader.GetString(2));
                    listaCampeonatos.Add(c);

                    Console.WriteLine($"-->Campeonato: {++total}");
                    Console.WriteLine(c.ToString());
                }
                conexao.Close();
            });


            if (total != 0)
            {
                string nomeCampEscolhido = Utils.LerString("Digite o nome do campeonato: ");
                string tempCampEscolhido = Utils.LerString("Digite a temporada do campeonato: ");
                var c = listaCampeonatos.Find(c => c.Nome.Equals(nomeCampEscolhido) && c.Temporada.Equals(tempCampEscolhido));

                if (c == null)
                    Console.WriteLine("Nome e/ou temporada invalido!");
                else
                {
                    c.Equipes = CarregarEquipesParticipando(c.Nome, c.Temporada);
                    if (c.Equipes.Count < 3)
                        c.Equipes = CadastrarEquipes(c.Nome, c.Temporada);

                    c.Jogos = CarregarJogos(c.Nome, c.Temporada);

                    c.Executar();

                    SalvarDadosEquipes(c.Equipes, c.Nome, c.Temporada);
                    SalvarJogos(c.Jogos, c.Nome, c.Temporada);
                    SalvarStatusCampeonato(c);
                }
            }
            else
                Console.WriteLine("Nao existem campeonatos em andamento!");
        }


        // Metodos de cadastro e insercao de equipes no campeonato e banco de dados 
        private List<Equipe> CadastrarEquipes(string nomeCamp, string tempCamp)
        {
            var lista = CarregarEquipesParticipando(nomeCamp, tempCamp);
            var equipesJaCadastradas = CarregarEquipesCadastradas();
            int total;

            Console.Clear();
            Console.WriteLine($"=====| {nomeCamp} |=====");
            Console.WriteLine("-->Inserir equipes:");

            if (lista.Count == 0)
            {
                total = Utils.LerInt("Digite o numero de times competindo (3-5):");

                while (total < 3 || total > 5)
                {
                    Console.WriteLine("Resposta invalida! Digite novamente: ");
                    total = Utils.LerInt("Digite o numero de times competindo (3-5):");
                }
            }
            else
            {
                Console.WriteLine("Total de equipes ja cadastradas no campeonato: " + lista.Count);
                Console.WriteLine("Faltam " + (3 - lista.Count) + " equipe(s) para atingir o minimo");

                total = Utils.LerInt("Digite o numero de equipes que voce deseja adicionar.");

                while (total + lista.Count < 3 || total + lista.Count > 5)
                {
                    Console.WriteLine("Resposta invalida! Digite novamente: ");
                    Console.WriteLine("Minimo 3 equipes e maximo 5");
                    total = Utils.LerInt("Digite o numero de equipes que voce deseja adicionar.");
                }
                total = lista.Count + total;
            }
            bool inseriuEquipe = false;
            bool primeiraVez = true;
            while (lista.Count < total)
            {
                Console.Clear();
                Console.WriteLine($"=====| {nomeCamp} |=====");
                Console.WriteLine("-->Inserir equipes:\n");

                if (primeiraVez == false)
                    if (inseriuEquipe)
                        Console.WriteLine("*** Equipe adicionada com sucesso! ***");
                    else
                        Console.WriteLine("*** Nao foi possivel adicionar essa equipe! ***");

                primeiraVez = false;
                Console.WriteLine("Total de equipes no campeonato: " + lista.Count);
                Console.WriteLine("Opcoes:");
                int op = Utils.LerInt("1- Inserir uma nova equipe\n2- Inserir uma equipe ja cadastrada");

                if (op == 1)
                {
                    var e1 = CadastrarNovaEquipe(equipesJaCadastradas);
                    Console.Clear();
                    if (e1 != null)
                    {
                        lista.Add(e1);
                        inseriuEquipe = true;
                    }
                    else
                        inseriuEquipe = false;
                }
                else if (op == 2)
                {
                    var e2 = CadastrarEquipeJaExistente(equipesJaCadastradas, lista);
                    Console.Clear();
                    if (e2 != null)
                    {
                        lista.Add(e2);
                        Console.WriteLine();
                        inseriuEquipe = true;
                    }
                    else
                        inseriuEquipe = false;
                }
                else
                    Console.WriteLine("Resposta invalida!");
            }

            Console.Clear();
            Console.WriteLine($"=====| {nomeCamp} |=====");
            Console.WriteLine("\n***Equipes cadastradas com sucesso!***");
            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadLine();
            return lista;
        }

        private static Equipe? CadastrarNovaEquipe(List<Equipe> equipesJaCadastradas)
        {

            string nome = Utils.LerString("Digite o nome da equipe: ");

            if (equipesJaCadastradas.Find(e => e.Nome.Equals(nome)) == null)
            {
                string apelido = Utils.LerString("Digite o apelido da equipe: ");
                DateOnly dataCriacao = Utils.LerData("Digite a data de criacao da equipe.");

                var e = new Equipe(nome, apelido, dataCriacao);

                equipesJaCadastradas.Add(e);
                return e;
            }
            return null;
        }

        private static Equipe? CadastrarEquipeJaExistente(List<Equipe> equipesJaCadastradas, List<Equipe> participando)
        {

            Console.Clear();
            Console.WriteLine("=====Inserir uma equipe ja cadastrada no campeonato=====");

            if (equipesJaCadastradas.Count == 0)
            {
                Console.WriteLine("-->Nao existe equipes cadastradas atualmente!\n");
                Console.Write("Pressione qualquer tecla para voltar ao menu...");
                Console.ReadKey();
                return null;
            }


            int total = 0;

            foreach (var item in equipesJaCadastradas)
                Console.WriteLine($"{++total} - {item.Nome}");


            int escolhida = Utils.LerInt("Digite o numero referente a qual equipe deseja adicionar: ");


            while (escolhida - 1 > equipesJaCadastradas.Count)
            {
                Console.WriteLine("Numero invalido!");
                escolhida = Utils.LerInt("Digite o numero referente a qual equipe deseja adicionar: ");
            }



            Equipe equipeEscolhida = equipesJaCadastradas[escolhida - 1];

            if (participando.Find(e => e.Nome.Equals(equipeEscolhida.Nome)) == null)
                return equipeEscolhida;


            return null;
        }



        // Metodos que inserem informacoes no banco de dados
        private int InserirCampeonato(string nome, string temporada)
        {
            using var conexao = new SqlConnection(new Banco().Conexao);

            using var proc = new SqlCommand("InserirCampeonato", conexao) { CommandType = CommandType.StoredProcedure };

            proc.Parameters.AddWithValue("@nome", nome);
            proc.Parameters.AddWithValue("@temporada", temporada);
            proc.Parameters.AddWithValue("@status", StatusCampeonato.Iniciado.ToString());

            var retorno = new SqlParameter("resultado", SqlDbType.Int) { Direction = ParameterDirection.Output };
            proc.Parameters.Add(retorno);

            LidarComException(() =>
            {
                conexao.Open();
                proc.ExecuteNonQuery();
                conexao.Close();
            });

            return (int)retorno.Value;
        }

        private void SalvarDadosEquipes(List<Equipe> equipes, string nomeCamp, string tempCamp)
        {

            LidarComException(() =>
            {
                using var conexao = new SqlConnection(new Banco().Conexao);

                using var proc1 = new SqlCommand("InserirEquipe", conexao) { CommandType = CommandType.StoredProcedure };
                using var proc2 = new SqlCommand("AdicionarTimeAoCampeonato", conexao) { CommandType = CommandType.StoredProcedure };


                conexao.Open();
                foreach (var equipe in equipes)
                {
                    proc1.Parameters.Clear();
                    proc1.Parameters.AddWithValue("@nome", equipe.Nome);
                    proc1.Parameters.AddWithValue("@apelido", equipe.Apelido);
                    proc1.Parameters.AddWithValue("@data_criacao_str", equipe.DataCriacao.ToString());

                    proc2.Parameters.Clear();
                    proc2.Parameters.AddWithValue("@nome_camp", nomeCamp);
                    proc2.Parameters.AddWithValue("@temp_camp", tempCamp);
                    proc2.Parameters.AddWithValue("@nome_equipe", equipe.Nome);

                    LidarComException(() =>
                    {
                        proc1.ExecuteNonQuery();
                        proc2.ExecuteNonQuery();
                    });
                }
                conexao.Close();
            });
        }

        public static void SalvarJogos(List<Jogo> jogos, string nomeCamp, string tempCamp)
        {

            LidarComException(() =>
            {
                using var conexao = new SqlConnection(new Banco().Conexao);

                conexao.Open();
                foreach (var jogo in jogos)
                {
                    using var proc = new SqlCommand("InserirJogo", conexao)
                    {
                        CommandType = CommandType.StoredProcedure,
                        Parameters =
                        {
                        new SqlParameter("@nome_camp", nomeCamp),
                        new SqlParameter("@temp_camp", tempCamp),
                        new SqlParameter("@time_casa", jogo.TimeCasa.Nome),
                        new SqlParameter("@time_visitante", jogo.TimeVisitante.Nome),
                        new SqlParameter("@gols_time_casa", jogo.GolsTimeCasa),
                        new SqlParameter("@gols_time_visitante", jogo.GolsTimeVisitante)
                        }
                    };

                    LidarComException(() =>
                    {
                        proc.ExecuteNonQuery();
                    });
                }
            });
        }

        private void SalvarStatusCampeonato(Campeonato c)
        {

            LidarComException(() =>
            {
                using var conexao = new SqlConnection(new Banco().Conexao);

                using var proc = new SqlCommand("AtualizarCampeonato", conexao)
                {
                    CommandType = CommandType.StoredProcedure,
                    Parameters = {
                        new SqlParameter("@nome", c.Nome),
                        new SqlParameter("@temporada", c.Temporada),
                        new SqlParameter("@status", c.Status),
                    }
                };

                conexao.Open();
                proc.ExecuteNonQuery();
            });
        }



        // Metodos de recuperar informacoes do banco de dados
        private List<Equipe> CarregarEquipesCadastradas()
        {
            var lista = new List<Equipe>();


            LidarComException(() =>
            {
                using var conexao = new SqlConnection(new Banco().Conexao);

                using var cmd = new SqlCommand("SELECT * FROM Equipe", conexao);

                conexao.Open();

                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string nome = reader.GetString(0);
                    string apelido = reader.GetString(1);
                    DateOnly data = DateOnly.FromDateTime(reader.GetDateTime(2));

                    var equipe = new Equipe(nome, apelido, data);

                    lista.Add(equipe);
                }
                conexao.Close();
            });

            return lista;
        }

        private List<Equipe> CarregarEquipesParticipando(string nomeCamp, string tempCamp)
        {
            var lista = new List<Equipe>();


            LidarComException(() =>
            {
                using var conexao = new SqlConnection(new Banco().Conexao);
                using var cmd = new SqlCommand("RecuperarEquipesPorCampeonato", conexao)
                {
                    Parameters = {
                        new SqlParameter("@nome_camp", nomeCamp),
                        new SqlParameter("@temp_camp", tempCamp)
                },
                    CommandType = CommandType.StoredProcedure
                };

                conexao.Open();
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string nome = reader.GetString(0);
                    string apelido = reader.GetString(1);
                    DateOnly data = DateOnly.FromDateTime(reader.GetDateTime(2));

                    var equipe = new Equipe(nome, apelido, data);

                    lista.Add(equipe);
                }
                conexao.Close();
            });

            return lista;
        }

        private List<Jogo> CarregarJogos(string nomeCamp, string tempCamp)
        {
            var jogos = new List<Jogo>();
            var equipes = CarregarEquipesCadastradas();


            LidarComException(() =>
            {
                using var conexao = new SqlConnection(new Banco().Conexao);
                using var cmd = new SqlCommand("SELECT * FROM Jogo WHERE nome_camp = @nome_camp AND temp_camp = @temp_camp ", conexao)
                {
                    Parameters = {
                        new SqlParameter("@nome_camp", nomeCamp),
                        new SqlParameter("@temp_camp", tempCamp)
                    }
                };

                conexao.Open();

                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        string campeonato = reader.GetString(0);
                        string temporada = reader.GetString(1);
                        string timeCasa = reader.GetString(2);
                        string timeVisitante = reader.GetString(3);
                        int golsTimeCasa = reader.GetInt32(4);
                        int golsTimeVisitante = reader.GetInt32(5);

                        Equipe? casa = equipes.Find(e => e.Nome.Equals(timeCasa));
                        Equipe? visitante = equipes.Find(e => e.Nome.Equals(timeVisitante));

                        var j = new Jogo(campeonato, temporada, casa, visitante, golsTimeCasa, golsTimeVisitante);

                        jogos.Add(j);
                    }
                }
            });

            return jogos;
        }

        private static void LidarComException(Action acaoExecutada)
        {
            try
            {
                acaoExecutada();
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
    }
}