namespace Futebol
{
    internal class Equipe
    {
        private string _nome;
        private string _apelido;
        private DateOnly _dataCriacao;

        public string Nome
        {
            get => _nome;
            set { _nome = value; }
        }

        public string Apelido
        {
            get => _apelido;
            set { _apelido = value; }
        }

        public DateOnly DataCriacao
        {
            get => _dataCriacao;
            set { _dataCriacao = value; }
        }

        public Equipe(string nome, string apelido, DateOnly dataCriacao)
        {
            _nome = nome;
            _apelido = apelido;
            _dataCriacao = dataCriacao;
        }

    }
}
