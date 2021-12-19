using MonsterTradingCardGamePP.Cards;
using MonsterTradingCardGamePP.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP
{
    class Shop
    {
        public static void openShop(Player player)
        {
            while(true)
            {
                player.updateCoins();
                Console.Clear();
                Console.WriteLine("Hier koennen Sie Kartenpakete erwerben!");
                Console.WriteLine("Ein Kartenpaket kostet 5 coins\n");
                Console.WriteLine($"Derzeit besitzen Sie {player.Coins} coins\n");

                if(player.Coins >= 5)
                {
                    Console.WriteLine("Moechten Sie ein Kartenpaket kaufen? (write 'y' or 'n')\n");

                    string input = Console.ReadLine();

                    if(input == "y")
                    {
                        buyPack(player);
                    }
                    else if(input == "n")
                    {
                        break;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nSie besitzen derzeit leider nicht mehr genug coins, bitte besuchen die den Shop spaeter nochmal!\n");
                    Console.ResetColor();
                    break;
                }
            }

            Console.WriteLine("\nDruecke Enter um fortzufahren");
            Console.ReadLine();
        }

        private static void buyPack(Player player)
        {
            DB.getInstance().buyPack(player.UserID);

            Console.Clear();

            Console.WriteLine("Sie haben folgende Karten erhalten!\n");
            Console.WriteLine("CardID  Name              Damage  CardType  MonsterType  Element");

            for(int i = 0; i<5; i++)
            {
                int random = RNG.getInstance().RandomNumber(0, Program.AllCards.Count());

                Card temp = new Card(Program.AllCards[random]);
                Console.WriteLine($"{temp.CardID.ToString().PadRight(8, ' ')}{temp.Name.PadRight(18, ' ')}{temp.Damage.ToString().PadRight(8, ' ')}{temp.CardType.ToString().PadRight(10, ' ')}{temp.MonsterType.ToString().PadRight(13, ' ')}{temp.Element}");
                
                //add card to DB
                DB.getInstance().addToStack(temp.CardID, player.UserID);
            }

            //get new Stack from DB
            player.getStack();

            Console.WriteLine("\nDruecke Enter um fortzufahren");
            Console.ReadLine();
        }
    }
}
