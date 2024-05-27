using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Futebol
{
    internal class Utils
    {
        public Utils()
        {

        }

        public static string LerString(string titulo)
        {
            Console.WriteLine(titulo);
            Console.Write("R: ");

            string? str = Console.ReadLine();

            if (str == null)
            {
                return "";
            }

            return str;
        }

        public static int LerInt(string titulo)
        {
            int resposta;

            Console.WriteLine(titulo);
            Console.Write("R: ");

            string? s = Console.ReadLine();


            while (!int.TryParse(s, out resposta))
            {
                Console.WriteLine("Resposta invalida! Tente novamente.");
                Console.WriteLine(titulo);
                Console.Write("R: ");
                s = Console.ReadLine();
            }

            return resposta;
        }

        public static string FormatarStr(string texto, int tamanho)
        {
            if (texto.Length > tamanho)
            {
                texto = texto.Substring(0, tamanho);
            }

            return texto;
        }


        public static DateOnly LerData(string texto)
        {
            Console.WriteLine(texto);
            Console.Write("R: ");

            DateOnly dateOnly;
            string? s = Console.ReadLine();

            while (DateOnly.TryParse(s, out dateOnly) == false || dateOnly > DateOnly.FromDateTime(DateTime.Now))
            {
                Console.WriteLine("Data invalida!");
                Console.WriteLine(texto);
                Console.Write("R: ");
                s = Console.ReadLine();
            }

            return dateOnly;
        }
    }
}
