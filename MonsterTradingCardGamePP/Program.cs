using MonsterTradingCardGamePP.Cards;
using MonsterTradingCardGamePP.Enum;
using MonsterTradingCardGamePP.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGamePP.Misc;

namespace MonsterTradingCardGamePP
{
    public class Program
    {
        public static List<Card> AllCards;
        public static string token;
        public static int userID;

        static void Main(string[] args)
        {
            //get All cards that exist in the DB for easier handling
            AllCards = DB.getInstanceWithoutToken().getAllCardsFromDB();

            //go to Menu
            Menu.startMenu(AllCards);

            //remove later
            Console.WriteLine("\nDruecke Enter um fortzufahren");
            Console.ReadLine();
        }

    }
}
