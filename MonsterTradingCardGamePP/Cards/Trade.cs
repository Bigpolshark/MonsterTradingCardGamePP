using MonsterTradingCardGamePP.Database;
using MonsterTradingCardGamePP.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP.Cards
{
    public class Trade
    {
        public static void tradeMenu(Player player)
        {
            string input = null;
            while (input != "4")
            {
                Console.Clear();
                Console.WriteLine("\nWas wollen Sie tun?");
                Console.WriteLine("\n1 - Verfuegbare Tauschangebote ansehen");
                Console.WriteLine("2 - Karte zum Tauschen anbieten");
                Console.WriteLine("3 - Erstellte Tauschangebote verwalten");
                Console.WriteLine("4 - Menue verlassen\n");

                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        //view trades, except of own ones
                        viewAllTrades(player);
                        break;
                    case "2":
                        //publish trade deal
                        publishTrade(player);
                        break;
                    case "3":
                        //see own trade deals
                        viewOwnTrades(player);
                        break;
                    case "4": break;
                    default:
                        Output.errorOutputWrongSelection();
                        Output.confirm();
                        break;
                }
            }

            Output.confirm();
        }

        private static void viewAllTrades(Player player)
        {
            Console.Clear();
            List<Card> tradeableCards;
            List<tradeInfo> cardInfo;

            (tradeableCards, cardInfo) = DB.getInstance().viewAllTrades(player.UserID);

            if(tradeableCards == null)
            {
                Output.errorOutputCustom("Derzeit befinden sich keine Karten auf der Tauschboerse!");
                Output.confirm();
                return;
            }

            int position = -1; //starts at -1, so it can be incremented at the beginning of the loop, and the variable can be used later to see how many trade offers exist
            Console.WriteLine("Position  CardID  Name              Damage  CardType  MonsterType  Element || TargetCardType MinDdmg Coinprice");
            foreach (Card card in tradeableCards)
            {
                position++;
                Console.Write(position.ToString().PadRight(10, ' '));
                printTrade(card, cardInfo[position]); 
            }

            Console.WriteLine("\nWelche Karte wollen Sie erwerben? Geben Sie bitte die 'Position' an!\nFalls sie diese Menue verlassen wollen, geben Sie bitte 'x' ein!");

            string input;
            int selection = 0;
            while (true)
            {
                while (true)
                {
                    input = Console.ReadLine();
                    if (input == "x")
                        return;

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

                if (selection < 0 || selection > position)
                {
                    Output.errorOutputWrongSelection();
                    continue;
                }

                break;
            }

            buyCardTrade(player, tradeableCards[selection], cardInfo[selection]);

            //update Stack and Coins
            player.getStack();
            player.updateCoins();

            Output.confirm();
        }

        private static void printTrade(Card card, tradeInfo info)
        {
            Console.Write($"{card.CardID.ToString().PadRight(8, ' ')}{card.Name.PadRight(18, ' ')}{card.Damage.ToString().PadRight(8, ' ')}{card.CardType.ToString().PadRight(10, ' ')}{card.MonsterType.ToString().PadRight(13, ' ')}{card.Element.ToString().PadRight(8, ' ')}");
            Console.WriteLine($"|| {info.targetCardType.ToString().PadRight(15, ' ')}{info.minDmg.ToString().PadRight(8, ' ')}{info.coinprice}");
        }

        private static void buyCardTrade(Player player, Card card, tradeInfo info)
        {
            bool forCoin = false;
            bool forCard = false;

            Console.Write("\nSie koennen diese Karte gegen ");

            if(info.minDmg != null)
            {
                forCard = true;
                Console.Write($"eine Karte mit <Typ: {info.targetCardType} | MinDmg: {info.minDmg}> ");
            }

            if (info.coinprice != null)
            {
                if(forCard == true)
                {
                    Console.Write(" ODER ");
                }

                forCoin = true;
                Console.Write($"{info.coinprice} coins ");
            }

            Console.WriteLine("tauschen!");

            string input = null;
            while (true)
            {

                Console.WriteLine("\nWas wollen Sie tun?\n");
                if(forCard == true)
                    Console.WriteLine("1 - Gegen eigene Karte tauschen");
                if(forCoin == true)
                    Console.WriteLine("2 - Um Coins kaufen");
                Console.WriteLine("3 - Menue verlassen\n");

                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        if (forCard == false)
                        {
                            Output.errorOutputWrongSelection();
                            break;
                        }

                        //return on successfull trade
                        if (buyWithCard(player, card, info))
                            return;

                        break;
                    case "2":
                        if (forCoin == false)
                        {
                            Output.errorOutputWrongSelection();
                            break;
                        }

                        //return on successfull trade
                        if (buyWithCoin(player, card, info))
                            return;

                        break;
                    case "3": return;
                    default:
                        Output.errorOutputWrongSelection();
                        break;
                }
            }
        }

        private static bool buyWithCard(Player player, Card card, tradeInfo info)
        {
            if(player.Stack == null)
            {
                Output.errorOutputCustom("Sie besitzen derzeit keine Karten!\n");

                return false;
            }

            List<Card> tradeableCards = new List<Card>();
            bool foundCard = false;

            //check if a tradeable card is in possession
            foreach (Card tempCard in player.Stack)
            {
                if(info.targetCardType == tempCard.CardType)
                {
                    if(info.minDmg <= tempCard.Damage)
                    {
                        tradeableCards.Add(new Card(tempCard));
                        foundCard = true;
                    }
                }
            }

            if(foundCard == false)
            {
                Output.errorOutputCustom("Sie besitzen derzeit keine Karten, welche den Tausch erfuellen koennen!\n");

                return false;
            }

            int selection0 = -1;
            //print all tradeable Cards
            Card.printCardListHeader();
            foreach(Card tempCard in tradeableCards)
            {
                selection0++;
                tempCard.printCard(selection0);
            }

            Console.WriteLine("\nWelche Karte wollen Sie eintauschen? Geben Sie bitte die 'Position' an!\nFalls sie den Tausch abbrechen wollen, geben Sie bitte 'x' ein!");

            string input;
            int selection = 0;
            while (true)
            {
                while (true)
                {
                    input = Console.ReadLine();
                    if (input == "x")
                        return false;

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

                if (selection < 0 || selection >= tradeableCards.Count())
                {
                    Output.errorOutputWrongSelection();
                    continue;
                }

                break;
            }

            //Karte die durch "selection" ausgewählt wurde wird aus dem Stack entfernt, und neue Karte kommt dazu
            DB.getInstance().tradeByCard(card, info, tradeableCards[selection], player.UserID);

            return true;
        }

        private static bool buyWithCoin(Player player, Card card, tradeInfo info)
        {
            if (player.Coins < info.coinprice)
            {
                Output.errorOutputCustom("Sie besitzen derzeit nicht genug coins!\n");

                return false;
            }

            //Karte wird hinzugefügt, Geld wird abgezogen und dem Karten besitzer überwiesen
            DB.getInstance().tradeByCoin(card, info, player.UserID);

            Console.WriteLine($"{card.Name} wurde erfolgreich gekauft !");
            
            return true;

        }

        private static void publishTrade(Player player)
        {
            //Karte vom Stack auswählen
            player.showStack();

            Console.WriteLine("\nWelche Karte aus ihrem Stack wollen Sie ihrem Deck hinzufügen? Geben Sie bitte die 'Position' an!\nFalls sie den Tausch abbrechen wollen, geben Sie bitte 'x' ein!");

            string input;
            int selection = 0;
            while (true)
            {
                while (true)
                {
                    input = Console.ReadLine();
                    if (input == "x")
                        return;

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

                if (selection < 0 || selection >= player.Stack.Count())
                {
                    Output.errorOutputWrongSelection();
                    continue;
                }

                break;
            }

            //auswählen was mit karte machen
            bool loop = true;
            input = null;
            while (loop)
            {
                Console.Clear();
                Console.WriteLine("\nFuer was wollen Sie die Karte anbieten ?");
                Console.WriteLine("\n1 - Gegen eine andere Karte");
                Console.WriteLine("2 - Gegen coins");
                Console.WriteLine("3 - Gegen beides");
                Console.WriteLine("4 - Abbrechen\n");

                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        tradeForCard(player, player.Stack[selection]);
                        loop = false;
                        break;
                    case "2":
                        tradeForCoin(player, player.Stack[selection]);
                        loop = false;
                        break;
                    case "3":
                        tradeForCardAndCoin(player, player.Stack[selection]);
                        loop = false;
                        break;
                    case "4": break;
                    default:
                        Output.errorOutputWrongSelection();
                        Output.confirm();
                        break;
                }
            }

            Output.confirm();
        }

        private static void tradeForCard(Player player, Card card)
        {
            Enum.CardType? ctype;
            int? damage;
            (ctype, damage) = chooseCardForTrade();

            if (ctype == null)
                return;

            //add trade to DB, remove Card from stack
            DB.getInstance().tradeForCard(card.CardID, player.UserID, (Enum.CardType)ctype, (int)damage);

        }

        private static void tradeForCoin(Player player, Card card)
        {
            int? coin;
            coin = chooseCoinForTrade();

            if (coin == null)
                return;

            //add trade to DB, remove Card from stack
            DB.getInstance().tradeForCoin(card.CardID, player.UserID, (int)coin);
        }

        private static void tradeForCardAndCoin(Player player, Card card)
        {


            Enum.CardType? ctype;
            int? damage;
            (ctype, damage) = chooseCardForTrade();

            if (ctype == null)
                return;

            int? coin;
            coin = chooseCoinForTrade();

            if (coin == null)
                return;

            //add trade to DB, remove Card from stack
            DB.getInstance().tradeForCardAndCoin(card.CardID, player.UserID, (int)coin, (Enum.CardType)ctype, (int)damage);
        }

        private static (Enum.CardType?, int?) chooseCardForTrade()
        {
            Enum.CardType? ctype = null; ;

            bool loop = true;
            string input = null;
            while (loop)
            {
                Console.Clear();
                Console.WriteLine("\nSoll die Karten ein Monster oder ein Spell sein?");
                Console.WriteLine("\n1 - Monster");
                Console.WriteLine("2 - Spell");
                Console.WriteLine("3 - Abbrechen\n");

                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        ctype = Enum.CardType.Monster;
                        loop = false;
                        break;
                    case "2":
                        ctype = Enum.CardType.Spell;
                        loop = false;
                        break;
                    case "3": return (null,null);
                    default:
                        Output.errorOutputWrongSelection();
                        Output.confirm();
                        break;
                }
            }

            Console.WriteLine("\nWelchen Damage soll die Karte mindestens haben (0-100)?\nFalls sie den Tausch abbrechen wollen, geben Sie bitte 'x' ein!");

            input = null;
            int selection = 0;
            while (true)
            {
                while (true)
                {
                    input = Console.ReadLine();
                    if (input == "x")
                        return (null, null);

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

                if (selection < 0 || selection >= 100)
                {
                    Output.errorOutputWrongSelection();
                    continue;
                }

                break;
            }

            return (ctype, selection);
        }

        public static int? chooseCoinForTrade()
        {
            Console.WriteLine("\nFuer welchen coin-Wert soll die Karte angeboten werden (0-100)?\nFalls sie den Tausch abbrechen wollen, geben Sie bitte 'x' ein!");

            string input = null;
            int selection;
            while (true)
            {
                while (true)
                {
                    input = Console.ReadLine();
                    if (input == "x")
                        return null;

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

                if (selection < 0 || selection >= 100)
                {
                    Output.errorOutputWrongSelection();
                    continue;
                }

                break;
            }

            return selection;
        }

        private static void viewOwnTrades(Player player)
        {

            Console.Clear();
            List<Card> tradeableCards;
            List<tradeInfo> cardInfo;

            (tradeableCards, cardInfo) = DB.getInstance().viewOwnTrades(player.UserID);

            if (tradeableCards == null)
            {
                Output.errorOutputCustom("Derzeit befinden sich keine Karten von Ihnen auf der Tauschboerse!");
                Output.confirm();
                return;
            }

            int position = -1; //starts at -1, so it can be incremented at the beginning of the loop, and the variable can be used later to see how many trade offers exist
            Console.WriteLine("Position  CardID  Name              Damage  CardType  MonsterType  Element || TargetCardType MinDdmg Coinprice");
            foreach (Card card in tradeableCards)
            {
                position++;
                Console.Write(position.ToString().PadRight(10, ' '));
                printTrade(card, cardInfo[position]);
            }

            Console.WriteLine("\nWollen Sie einen Tausch zurueckziehen ? Wenn ja, geben Sie bitte die 'Position' an!\nFalls sie diese Menue verlassen wollen, geben Sie bitte 'x' ein!");

            string input;
            int selection = 0;
            while (true)
            {
                while (true)
                {
                    input = Console.ReadLine();
                    if (input == "x")
                        return;

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

                if (selection < 0 || selection > position)
                {
                    Output.errorOutputWrongSelection();
                    continue;
                }

                break;
            }

            DB.getInstance().removeTrade(player.UserID, tradeableCards[selection], cardInfo[selection]);

            //update Stack and Coins
            player.getStack();
            player.updateCoins();

            Output.confirm();
        }
    }

}
