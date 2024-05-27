namespace Futebol
{
    internal class Equipe
    {
        private string _nome;
        private string _apelido;
        private DateOnly _dataCriacao;

        public string Nome => _nome;
        public string Apelido => _apelido;
        public DateOnly DataCriacao => _dataCriacao;

        public Equipe(string nome, string apelido, DateOnly dataCriacao)
        {
            _nome = nome;
            _apelido = apelido;
            _dataCriacao = dataCriacao;
        }

        public override string? ToString()
        {
            string str = "";

            str += $"Nome...........: {Nome}\n";
            str += $"Apelido........: {Apelido}\n";
            str += $"Data de Criacao: {DataCriacao}";

            return str;
        }
    }
}
