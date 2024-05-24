using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futebol
{

    internal class Jogo
    {
        private string _campeonato;
        private string _temporada;
        private Equipe _timeCasa;
        private Equipe _timeVisitante;
        private int _golsTimeCasa;
        private int _golsTimeVisitante;

        public string Campeonato => _campeonato;
        public string Temporada => _campeonato;
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
    }
}