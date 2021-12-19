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
    class Program
    {
        public static List<Card> AllCards;

        static void Main(string[] args)
        {
            //stay in loop, until either login, Registration or exit
            Player currentPlayer = null; //sets current Player for the future
            string input;

            //get All cards that exist in the DB for easier handling
            AllCards = DB.getInstance().getAllCardsFromDB();

            while (currentPlayer == null)
            {
                Console.WriteLine("Willkommen beim MonsterTradingCardGamePP !");
                Console.WriteLine("Um eine Funktion zu wählen, geben Sie bitte die zugehörige Zahl an!");
                Console.WriteLine("\n1 - Login");
                Console.WriteLine("2 - Neues Profil erstellen");
                Console.WriteLine("3 - Programm Beenden\n");

                input = Console.ReadLine();

                switch(input)
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
                    default: Console.WriteLine("Bitte geben Sie einen der oben gegebenen Werte an!\n"); break;
                }
            }

            input = "";
            while(input != "9")
            {
                Console.Clear();
                Console.WriteLine($"Eingeloggt als User {currentPlayer.Username}\n");

                Console.WriteLine("Was wollen Sie tun ?");
                Console.WriteLine("\n1 - Stack ansehen");
                Console.WriteLine("2 - Deck verwalten");                
                Console.WriteLine("8 - Karten Shop");
                Console.WriteLine("9 - Programm Beenden\n");

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
                    case "8":
                        //Karten Shop
                        Shop.openShop(currentPlayer);
                        break;
                    case "9": break;
                    default: Console.WriteLine("Bitte geben Sie einen der oben gegebenen Werte an!\n"); break;
                }


            }



            /*
            CardType test = CardType.Spell;
            Console.WriteLine($"Check {test}");


            //test card
            Card Card1 = new Card(1, CardType.Monster, MonsterType.Goblin, Element.Normal, "Goblin Soldat", 2);
            Card Card2 = new Card(2, CardType.Monster,  MonsterType.Dragon, Element.Fire, "Vulkandrache", 10000);
            Card Card3 = new Card(3, CardType.Spell, null, Element.Water,"OargerSpell", 900);
            Card Card4 = new Card(4, CardType.Monster,  MonsterType.Knight, Element.Normal, "Whacker Knight", 1);

            Console.WriteLine($"Monster {Card1.Name} + {Card1.MonsterType}");
            Console.WriteLine($"Spell {Card2.Name} + {Card2.MonsterType}");


            Console.WriteLine($"Damage Check (with damage value of 10 (should be reduced to 0)) ==> {specialRules.checkSpecial(Card1, Card2, 100)}");
            Console.WriteLine($"Damage Check (with damage value of 10 (should be insta kill)) ==> {specialRules.checkSpecial(Card3, Card4, 100)}");

            Player Player1 = new Player("Hans", 1);
            Player Player2 = new Player("Deiter", 2);

            new Battle(Player1, Player2);

            DB Database = DB.getInstance();
            //Database.AddUser("TestUser", "hallo123");
            //Database.AddCardToDB(Card3);

            Console.WriteLine($"jo wer ist denn da an der ersten Stelle ?????? ==> {Database.GetUsernameByID(1)}");

            List<Card> Cardlist = new List<Card>();



        */
            //remove later
            Console.WriteLine("\nDruecke Enter um fortzufahren");
            Console.ReadLine();
        }

    }
}
