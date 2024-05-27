namespace Futebol
{

    internal class Jogo
    {
        private readonly string _campeonato;
        private readonly string _temporada;
        private readonly Equipe _timeCasa;
        private readonly Equipe _timeVisitante;
        private int _golsTimeCasa;
        private int _golsTimeVisitante;

        public string Campeonato => _campeonato;
        public string Temporada => _temporada;
        public Equipe TimeCasa => _timeCasa;
        public Equipe TimeVisitante => _timeVisitante;
        public int GolsTimeCasa => _golsTimeCasa;
        public int GolsTimeVisitante => _golsTimeVisitante;

        public Jogo(string campeonato, string temporada, Equipe timeCasa, Equipe timeVisitante)
        {
            _campeonato = campeonato;
            _temporada = temporada;
            _timeCasa = timeCasa;
            _timeVisitante = timeVisitante;
            _golsTimeCasa = 0;
            _golsTimeVisitante = 0;
        }

        public Jogo(string campeonato, string temporada, Equipe timeCasa, Equipe timeVisitante, int golsTimeCasa, int golsTimeVisitante) : this(campeonato, temporada, timeCasa, timeVisitante)
        {
            _golsTimeCasa = golsTimeCasa;
            _golsTimeVisitante = golsTimeVisitante;
        }

        public void Jogar()
        {
            var random = new Random();
            _golsTimeCasa = random.Next(10);
            _golsTimeVisitante = random.Next(10);
        }

        public void Jogar(int golsTimeCasa, int golsTimeVisitante)
        {
            _golsTimeCasa = golsTimeCasa;
            _golsTimeVisitante = golsTimeVisitante;
        }


        public string MostrarJogo(bool ExibeCampeonato)
        {
            string str = "";

            str += $"Jogo..................: {TimeCasa.Nome} x {TimeVisitante.Nome}\n";

            if (ExibeCampeonato)
            {
                str += $"Campeonato............: {Campeonato}\n";
                str += $"Temporada.............: {Temporada}\n";
            }
            str += $"Time da casa..........: {TimeCasa.Nome}\n";
            str += $"Time visitante........: {TimeVisitante.Nome}\n";
            str += $"Gols do time da casa..: {GolsTimeCasa}\n";
            str += $"Gols do time visitante: {GolsTimeVisitante}\n";

            return str;
        }

    }
}