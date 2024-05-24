using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futebol
{
    internal class Banco
    {
        private string _conexao = "Data Source=127.0.0.1; Initial Catalog=DBFutebol; User Id=sa; Password=SqlServer2019!; TrustServerCertificate=Yes";

        public string Conexao
        {
            get => _conexao;
        }

        public Banco()
        {

        }
    }
}
