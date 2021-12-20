using MonsterTradingCardGamePP.Cards;
using MonsterTradingCardGamePP.Database;
using MonsterTradingCardGamePP.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP
{
    class Player
    {
        public int UserID { get; }
        public string Username { get; }
        public string AuthToken { get; }
        public List<Card> Stack { get; set; }
        public List<Card> Deck { get; set; }
        public int Coins { get; set; } //need to add subtract coins method
        public int Elo { get; }

        public Player(int id, string username, string authToken, int coins, int elo)
        {
            UserID = id;
            Username = username;
            AuthToken = authToken;
            Coins = coins;
            Elo = elo;
            getStack();
            getDeck();
        }


        private void getDeck()
        {
            Deck = DB.getInstance().getUserDeck(UserID);
        }

        public void getStack()
        {
            Stack = DB.getInstance().getUserStack(UserID);
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

        public void showStack()
        {
            Console.Clear();

            //check if Stack is empty
            if (Stack == null)
            {
                Output.errorOutputCustom("Sie besitzen derzeit keine Karten!\nKaufen Sie sich am besten neue Kartenpakete im Karten Shop!");

                return;
            }

            Console.WriteLine("Zu Beachten: Karten die sich derzeit in ihrem Deck befinden sind nicht im Stack einsichtbar!\n");

            Console.WriteLine("Position  CardID  Name              Damage  CardType  MonsterType  Element");
            int position = 0;
            foreach (Card card in Stack)
            {
                Console.WriteLine($"{position.ToString().PadRight(10, ' ')}{card.CardID.ToString().PadRight(8, ' ')}{card.Name.PadRight(18, ' ')}{card.Damage.ToString().PadRight(8,' ')}{card.CardType.ToString().PadRight(10, ' ')}{card.MonsterType.ToString().PadRight(13, ' ')}{card.Element}");
                position++;
            }
        }

        public void updateCoins()
        {
            Coins = DB.getInstance().getCoins(this.UserID);
        }

        public void showDeck()
        {
            Console.Clear();

            //check if Stack is empty
            if (Deck == null)
            {
                Output.errorOutputCustom("Ihr Deck ist derzeit noch leer!");
                return;
            }

            Console.WriteLine("Position  CardID  Name              Damage  CardType  MonsterType  Element");
            int position = 0;
            foreach (Card card in Deck)
            {
                Console.WriteLine($"{position.ToString().PadRight(10, ' ')}{card.CardID.ToString().PadRight(8, ' ')}{card.Name.PadRight(18, ' ')}{card.Damage.ToString().PadRight(8, ' ')}{card.CardType.ToString().PadRight(10, ' ')}{card.MonsterType.ToString().PadRight(13, ' ')}{card.Element}");
                position++;
            }
        }

        public void manageDeck()
        {
            string input = null;
            while(input != "3")
            {
                showDeck();

                Console.WriteLine("\nWas wollen Sie tun?");
                Console.WriteLine("\n1 - Karte hinzufuegen");
                Console.WriteLine("2 - Karte entfernen");
                Console.WriteLine("3 - Menue verlassen\n");

                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        //add card
                        addCard();
                        break;
                    case "2":
                        //remove card
                        removeCard();
                        break;
                    case "3": break;
                    default:
                        Output.errorOutputWrongSelection();
                        Output.confirm();
                        break;
                }
            }

            Output.confirm();
        }

        private void addCard()
        {
            if(Deck != null)
                if(Deck.Count() >= 4)
                {
                    Output.errorOutputCustom("Die maximale Deckgroesse ist auf 4 limitiert, Sie haben dieses Limit schon erreicht!");
                    Output.confirm();
                    return;
                }

            showStack();

            if (Stack == null)
            {
                Output.confirm();
                return;
            }

            Console.WriteLine("\nWelche Karte aus ihrem Stack wollen Sie ihrem Deck hinzufügen? Geben Sie bitte die 'Position' an!");

            string input;
            int selection = 0;
            while(true)
            {
                while(true)
                { 
                    input = Console.ReadLine();
                    try
                    {
                        selection = Int32.Parse(input);
                    }
                    catch (FormatException)
                    {
                        Output.errorNotInt();
                        continue;
                    }
                    break;
                }

                if(selection < 0 || selection >= Stack.Count())
                {
                    Output.errorOutputWrongSelection();
                    continue;
                }

                break;
            }

            //check if Card is already in Deck (no duplicates allowed)
            if(Deck != null)
                foreach(Card card in Deck)
                {
                    if(card.CardID == Stack[selection].CardID)
                    {
                        Output.errorOutputCustom("Eine Instanz dieser Karte exisitert bereits im Deck, bitte waehlen Sie eine andere!");
                        Output.confirm();
                        return;
                    }
                }

            //swap card from Stack to Deck
            DB.getInstance().addToDeck(Stack[selection].CardID, UserID);

            //update Deck/Stack at the end
            getDeck();
            getStack();

            Output.confirm();
        }
        
        private void removeCard()
        {
            if((Deck == null ) || (Deck.Count() == 0))
            {
                Output.errorOutputCustom("Ihr Deck ist leer, Sie koennen keine Karten daraus entfernen!");
                Output.confirm();
                return;
            }

            showDeck();

            Console.WriteLine("\nWelche Karte aus ihrem Deck wollen Sie ihrem Deck entfernen? Entfernte Karten werden wieder dem Stack hinzugefuegt.\nGeben Sie bitte die 'Position' an!");

            string input;
            int selection = 0;
            while(true)
            {
                while(true)
                { 
                    input = Console.ReadLine();
                    try
                    {
                        selection = Int32.Parse(input);
                    }
                    catch (FormatException)
                    {
                        Output.errorNotInt();
                        continue;
                    }
                    break;
                }

                if(selection < 0 || selection >= Deck.Count())
                {
                    Output.errorOutputWrongSelection();
                    continue;
                }

                break;
            }

            //swap card from Stack to Deck
            DB.getInstance().removeFromDeck(Deck[selection].CardID, UserID);

            //update Deck/Stack at the end
            getDeck();
            getStack();

            Output.confirm();
        }
    }
}
