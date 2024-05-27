using Microsoft.Data.SqlClient;

namespace Futebol.Campeonato
{
    internal class Campeonato
    {
        private readonly string _nome;
        private readonly string _temporada;
        private StatusCampeonato _status;
        private List<Equipe> _equipesParticipantes;
        private List<Jogo> _jogos;

        public string Nome => _nome;
        public string Temporada => _temporada;
        public string Status => _status.ToString();
        public List<Jogo> Jogos
        {
            get => _jogos;
            set { _jogos = value; }
        }
        public List<Equipe> Equipes
        {
            get => _equipesParticipantes;
            set { _equipesParticipantes = value; }
        }


        public Campeonato(string nome, string temporada)
        {
            _nome = nome;
            _temporada = temporada;
            _status = StatusCampeonato.Iniciado;
            _equipesParticipantes = new();
            _jogos = new();
        }

        public Campeonato(string nome, string temporada, string status) : this(nome, temporada)
        {
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
                case StatusCampeonato.Acontecendo:
                    _status = StatusCampeonato.Acontecendo;
                    ExecutarCampeonato();
                    break;
                case StatusCampeonato.Finalizado:
                    MenuExibicaoCampeonato();
                    break;
                default:
                    break;
            }
        }

        private void ExecutarCampeonato()
        {
            bool sair = false;

            string str = "Opcoes:\n1- Ver informacoes do campeonato.\n2- Executar jogos.\n0- Voltar ao menu principal.";
            bool executouTodosJogos = false;

            while (!sair)
            {
                Console.Clear();
                Console.WriteLine($"--| {Nome} |--");
                int opcao = Utils.LerInt(str);

                switch (opcao)
                {
                    case 1:
                        MenuExibicaoCampeonato();
                        break;
                    case 2:
                        executouTodosJogos = ExecutarJogos();
                        if (!executouTodosJogos)
                        {
                            Console.WriteLine("O campeonato nao foi finalizado!");
                            Console.WriteLine("O campeonato pode ser retomado a partir do menu principal!");
                        }
                        else
                        {
                            Console.WriteLine("Todos os jogos foram finalizados!");
                            sair = true;
                        }
                        break;
                    case 0:
                        sair = true;
                        Console.WriteLine("Opcao: Sair");
                        Console.WriteLine("Caso queira voltar a esse campeonato, acesse-o pelo menu principal");
                        break;
                    default:
                        Console.WriteLine("Opcao invalida");
                        break;
                }

                if (!sair)
                {
                    Console.WriteLine("Pressione qualquer tecla para voltar ao menu...");
                    Console.ReadKey();
                }

            }

            if (executouTodosJogos)
            {
                Console.Clear();
                Console.WriteLine("Campeonato finalizado!");
                _status = StatusCampeonato.Finalizado;
                ExibirPlacar();
            }
        }



        // Metodos de cadastro dos jogos do campeonato

        private bool ExecutarJogos()
        {

            int totalJogos = (Equipes.Count * Equipes.Count) - Equipes.Count;

            while (_jogos.Count < totalJogos)
            {
                for (int i = 0; i < Equipes.Count; i++)
                {
                    for (int j = i + 1; j < Equipes.Count; j++)
                    {
                        Equipe equipe1 = Equipes[i];
                        Equipe equipe2 = Equipes[j];

                        if (equipe1.Nome.Equals(equipe2.Nome))
                            continue;



                        bool contemJogoIda = false;
                        bool contemJogoVolta = false;

                        foreach (var jogo in _jogos)
                        {
                            string nomeTimeCasa = jogo.TimeCasa.Nome;
                            string nomeTimeVisitante = jogo.TimeVisitante.Nome;

                            if (nomeTimeCasa.Equals(equipe1.Nome) && nomeTimeVisitante.Equals(equipe2.Nome))
                                contemJogoIda = true;

                            if (nomeTimeCasa.Equals(equipe2.Nome) && nomeTimeVisitante.Equals(equipe1.Nome))
                                contemJogoVolta = true;
                        }

                        if (!contemJogoIda)
                        {
                            Jogo? jogo = CriarJogo(equipe1, equipe2);

                            if (jogo == null)
                                return false;

                            _jogos.Add(jogo);
                        }

                        if (!contemJogoVolta)
                        {
                            Jogo? jogo = CriarJogo(equipe2, equipe1);

                            if (jogo == null)
                                return false;

                            _jogos.Add(jogo);
                        }
                    }
                }
            }

            return _jogos.Count == totalJogos;
        }

        Jogo? CriarJogo(Equipe a, Equipe b)
        {
            Console.Clear();
            Console.WriteLine($"Jogo: {a.Nome} x {b.Nome}");
            int resposta = Utils.LerInt("Opcoes: \n1- Jogo manual (voce escolhe os resultados)\n2- Jogo automatico\n3- Sair");

            if (resposta == 3)
                return null;

            if (resposta != 1 && resposta != 2)
                return CriarJogo(a, b);


            var jogo = new Jogo(Nome, Temporada, a, b);
            if (resposta == 1)
            {
                Console.WriteLine("Digite o resultado do jogo: ");
                int golsTimeCasa = Utils.LerInt("Numero de gols do time da casa: ");
                int golsTimeVisitante = Utils.LerInt("Numero de gols do time visitante: ");

                jogo.Jogar(golsTimeCasa, golsTimeVisitante);
            }
            else if (resposta == 2)
            {
                jogo.Jogar();
            }

            return jogo;
        }



        // Metodos de exibir informacoes do campeonato

        private void MenuExibicaoCampeonato()
        {
            bool sair = false;

            while (!sair)
            {
                Console.Clear();
                Console.WriteLine("======================");
                Console.WriteLine($"Campeonato: {Nome}");
                Console.WriteLine($"Status....: {_status}");

                Console.WriteLine("Opcoes:");
                Console.WriteLine("1- Exibir placar do campeonato");
                Console.WriteLine("2- Exibir jogos do campeonato");
                Console.WriteLine("3- Listar equipes participantes");
                Console.WriteLine("4- Sair");
                Console.Write("R: ");

                int opcao;

                while (!int.TryParse(Console.ReadLine(), out opcao))
                {
                    Console.WriteLine("Voce deve digitar um numero!");
                    Console.Write("R: ");
                }

                switch (opcao)
                {
                    case 1:
                        ExibirPlacar();
                        break;
                    case 2:
                        ExibirJogos();
                        break;
                    case 3:
                        ExibirEquipes();
                        break;
                    case 4:
                        sair = true;
                        break;
                    default:
                        Console.WriteLine("Opcao invalida!");
                        break;
                }

                if (!sair)
                {
                    Console.WriteLine("Digite qualquer tecla para retornar ao menu...");
                    Console.ReadLine();
                }
            }
        }

        private void ExibirPlacar()
        {
            try
            {
                using var conexao = new SqlConnection(new Banco().Conexao);

                using var cmd = new SqlCommand()
                {
                    CommandText = "SELECT * FROM Estatistica WHERE nome_camp = @nome_camp AND temp_camp = @temp_camp ORDER BY pontos DESC, gols_marcados DESC",
                    Connection = conexao,
                    Parameters =
                    {
                        new SqlParameter("@nome_camp", Nome),
                        new SqlParameter("@temp_camp", Temporada)
                    }
                };

                conexao.Open();

                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    int posicao = 1;
                    Console.WriteLine("--|Placar do campeonato|-- \n");

                    if (reader.HasRows)
                    {

                        while (reader.Read())
                        {
                            // Só exibe o campeao caso o campeonato ja esteja finalizado
                            if (posicao == 1 && _status == StatusCampeonato.Finalizado)
                                Console.WriteLine("=====CAMPEAO=====");

                            Console.WriteLine($"Equipe...........: " + reader.GetString(2));
                            Console.WriteLine($"Posicao no placar: {posicao++}");
                            Console.WriteLine($"Pontos...........: " + reader.GetInt32(3));
                            Console.WriteLine($"Gols marcados....: " + reader.GetInt32(4));
                            Console.WriteLine($"Gols sofridos....: " + reader.GetInt32(5));
                            Console.WriteLine("===================");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nao existem pontuacoes ainda atreladas a esse campeonato!");
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
        }

        private void ExibirJogos()
        {
            Console.Clear();
            Console.WriteLine("=====Listar todos os jogos=====");


            if (_jogos.Count == 0)
            {
                Console.WriteLine("Nenhum jogo cadastrado!");
                return;
            }

            int indice = 0;
            int opcao = -1;

            do
            {
                bool opcaoValida = true;
                bool isNumero = true;

                Console.Clear();
                do
                {
                    Console.WriteLine("Jogo atual:");
                    Console.WriteLine(_jogos[indice].MostrarJogo(ExibeCampeonato: false) + "\n\n");

                    ExibirMenuImprimir(isNumero, opcaoValida);

                    if (int.TryParse(Console.ReadLine(), out opcao))
                    {
                        if (opcao >= 0 && opcao <= 5)
                            opcaoValida = true;
                    }
                    else
                        isNumero = false;

                } while (!opcaoValida);

                switch (opcao)
                {
                    case 1:
                        indice = indice == _jogos.Count - 1 ? 0 : indice + 1;
                        break;
                    case 2:
                        indice = indice == 0 ? _jogos.Count - 1 : indice - 1;
                        break;
                    case 3:
                        indice = 0;
                        break;
                    case 4:
                        indice = _jogos.Count - 1;
                        break;
                    case 5:
                        Console.Clear();
                        foreach (var jogo in _jogos)
                        {
                            Console.WriteLine(jogo.MostrarJogo(false));
                            Console.WriteLine("==============");
                        }
                        Console.WriteLine("Pressione qualquer tecla para voltar ao menu de exibicao...");
                        Console.ReadKey();

                        break;
                }
            } while (opcao != 0);
        }

        private void ExibirMenuImprimir(bool isNumero, bool opcaoValida)
        {
            Console.WriteLine("Navegar pelos jogos:");
            Console.WriteLine("Opcoes: ");
            Console.WriteLine("1- Proximo da lista");
            Console.WriteLine("2- Anterior da lista");
            Console.WriteLine("3- Inicio da lista");
            Console.WriteLine("4- Final da lista");
            Console.WriteLine("5- Listar todos de uma vez");
            Console.WriteLine("0- Parar navegacao");

            if (!isNumero)
                Console.WriteLine("Voce deve digitar um numero!");

            if (!opcaoValida)
                Console.WriteLine("Opcao invalida!");

            Console.Write("R: ");
        }

        public void ExibirEquipes()
        {
            Console.WriteLine("--|Equipes participantes do campeonato|--");

            if (_equipesParticipantes.Count == 0)
            {
                Console.WriteLine("Nao existe equipes participando!");
            }
            else
            {
                foreach (var item in _equipesParticipantes)
                {
                    Console.WriteLine(item.ToString());
                    Console.WriteLine("===================");
                }
            }
        }

        public override string? ToString()
        {
            string str = "";
            str += $"Nome......: {_nome}\n";
            str += $"Temporada.: {_temporada}\n";
            str += $"Status....: {_status}\n";

            return str;
        }
    }
}
