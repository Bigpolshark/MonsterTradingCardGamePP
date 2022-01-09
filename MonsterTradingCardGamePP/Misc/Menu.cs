using MonsterTradingCardGamePP.Cards;
using MonsterTradingCardGamePP.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP
{
    class Menu
    {
        public static void startMenu(List<Card> AllCards)
        {

            //stay in loop, until either login, Registration or exit
            Player currentPlayer = null; //sets current Player for the future
            string input;


            while (currentPlayer == null)
            {
                Console.WriteLine("Willkommen beim MonsterTradingCardGamePP !");
                Console.WriteLine("Um eine Funktion zu wählen, geben Sie bitte die zugehörige Zahl an!");
                Console.WriteLine("\n1 - Login");
                Console.WriteLine("2 - Neues Profil erstellen");
                Console.WriteLine("3 - Programm Beenden\n");

                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        //login
                        currentPlayer = Player.login();
                        break;
                    case "2":
                        //register
                        currentPlayer = Player.register();
                        break;
                    case "3": return;
                    default:
                        Output.errorOutputWrongSelection();
                        Output.confirm();
                        break;
                }
            }

            Program.token = currentPlayer.AuthToken;
            Program.userID = currentPlayer.UserID;

            input = "";
            while (input != "7")
            {
                Console.Clear();
                Console.WriteLine($"Eingeloggt als User {currentPlayer.Username}\n");

                //update Stack/Deck when in this menu
                currentPlayer.getStack();
                currentPlayer.getDeck();

                Console.WriteLine("Was wollen Sie tun ?");
                Console.WriteLine("\n1 - Stack ansehen");
                Console.WriteLine("2 - Deck verwalten");
                Console.WriteLine("3 - Start Random Battle");
                Console.WriteLine("4 - Kartentausch");
                Console.WriteLine("5 - Scoreboard");
                Console.WriteLine("6 - Karten Shop");
                Console.WriteLine("7 - Programm Beenden\n");

                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        //Stack ansehen
                        currentPlayer.showStack();
                        Output.confirm();
                        break;
                    case "2":
                        //Deck verwalten
                        currentPlayer.manageDeck();
                        break;
                    case "3":
                        //Battle starten
                        if (currentPlayer.Deck == null)
                        {
                            Output.errorOutputCustom("Sie brauchen ein Deck mit 4 Karten um spielen zu koennen!");
                            Output.confirm();
                            break;
                        }
                        else if (currentPlayer.Deck.Count() != 4)
                        {
                            Output.errorOutputCustom("Sie brauchen ein Deck mit 4 Karten um spielen zu koennen!");
                            Output.confirm();
                            break;
                        }
                        Battle currentBattle = Battle.setupBattle(currentPlayer);
                        currentBattle.printLog();
                        Output.confirm();
                        break;
                    case "4":
                        //Karten tauschen
                        Trade.tradeMenu(currentPlayer);
                        break;
                    case "5":
                        //scoreboard sehen
                        Scoreboard.viewScoreboard(currentPlayer);
                        break;
                    case "6":
                        //Karten Shop
                        Shop.openShop(currentPlayer);
                        break;
                    case "7": break;
                    default: Output.errorOutputWrongSelection(); Output.confirm();  break;
                }


            }
        }
    }
}
