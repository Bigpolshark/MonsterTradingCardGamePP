using MonsterTradingCardGamePP.Cards;
using MonsterTradingCardGamePP.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP
{
    class Player
    {
        public string Username { get; }
        public string AuthToken { get; }
        public List<Card> Stack { get; set; }
        public List<Card> Deck { get; set; }
        public int Coins { get; } //need to add subtract coins method
        public int Elo { get; }

        public Player(string username, string authToken, int coins, int elo)
        {
            Username = username;
            AuthToken = authToken;
            Coins = coins;
            Elo = elo;
            //getStack();
            //getDeck();
        }


        private void getDeck()
        {
            throw new NotImplementedException();
        }

        private void getStack()
        {
            throw new NotImplementedException();
        }

        public static Player login()
        {
            Console.WriteLine("\nUsername:");
            string username = Console.ReadLine();            
            Console.WriteLine("\nPassword: (input is hidden)");
            string password = writePassword();

            Player tempPlayer;
            DB database = DB.getInstance();

            if((tempPlayer = database.login(username, password)) == null)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nBenutzername oder Passwort ist falsch!\n");
                Console.ResetColor();
                return null;
            }
            else
            {
                return tempPlayer;
            }


        }        
        public static Player register()
        {
            Console.WriteLine("\nUsername:");
            string username = Console.ReadLine();            
            Console.WriteLine("\nPassword: (input is hidden)");
            string password = writePassword();

            Console.WriteLine("\nRepeat Password: (input is hidden)");
            string password2 = writePassword();

            if(password != password2)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nDie Passwoerter stimmen nicht ueberein!\n");
                Console.ResetColor();
                return null;
            }

            Player tempPlayer;
            DB database = DB.getInstance();

            if((tempPlayer = database.AddUser(username, password)) == null)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nEin Spieler mit diesem Namen existiert bereits!\n");
                Console.ResetColor();
                return null;
            }
            else
            {
                return tempPlayer;
            }


        }

        private static string writePassword()
        {
            string password ="";
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Backspace)
                {
                    if(password.Length > 0)
                        password = password.Remove(password.Length - 1);

                    continue;
                }
                if (key.Key == ConsoleKey.Enter)
                    break;
                password += key.KeyChar;
            }

            return password;
        }
    }
}
