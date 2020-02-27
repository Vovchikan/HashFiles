using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Запуск без парамметров.");
            }
            else
            {
                // Начать поиска файлов
                Task.Run(() => FindFiles.GetFiles(args));
            }

            Console.ReadKey();
        }
    }
}
