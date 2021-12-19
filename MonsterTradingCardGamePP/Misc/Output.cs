using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP.Misc
{
    class Output
    {
        public static void errorOutputWrongSelection()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nBitte geben Sie einen der oben gegebenen Werte an!\n");
            Console.ResetColor();
        }

        public static void confirm()
        {
            Console.WriteLine("\nDruecke Enter um fortzufahren");
            Console.ReadLine();
        }

        public static void errorOutputCustom(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{text}\n");
            Console.ResetColor();
        }

        public static void errorNotInt()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nBitte geben Sie eine Zahl an!\n");
            Console.ResetColor();
        }
    }
}
