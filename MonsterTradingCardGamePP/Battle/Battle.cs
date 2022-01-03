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
    class Battle
    {
        List<string> BattleLog;
        public List<Card> player1Deck { get; }
        public List<Card> player2Deck { get; }

        public Battle(Player p1, Player p2)
        {
            player1Deck = new List<Card>();
            player2Deck = new List<Card>();
            BattleLog = new List<string>();

            //copy decks, so the original doesnt get affected by battle
            foreach (Card card in p1.Deck)
            {
                player1Deck.Add(new Card(card));
            }

            foreach (Card card in p2.Deck)
            {
                player2Deck.Add(new Card(card));
            }

            BattleLog.Add($"Starte Battle zwischen {p1.Username} und {p2.Username}");
            BattleLogic.fight(player1Deck, player2Deck, BattleLog, p1, p2);
        }

        public static Battle setupBattle(Player currentPlayer)
        {
            Console.Clear();
            List<potentialOpponent> potentialOpponents = DB.getInstance().getPotentialOpponents(currentPlayer.UserID);

            if (potentialOpponents == null)
            {
                Output.errorOutputCustom("Es existiert derzeit leider kein Gegner, welcher ein volles Deck besitzt!\nBitte versuchen Sie es spaeter nocheinmal");
                Output.confirm();
                return null;
            }

            Console.WriteLine("Gegner wird aus folgenden Usern zufaellig ausgewaehlt:\n");
            Console.WriteLine("ID   Username");

            foreach (potentialOpponent op in potentialOpponents)
            {
                Console.WriteLine($"{op.UserID.ToString().PadRight(5,' ')}{op.Username}");
            }

            Output.confirm();

            //randomly choose Opponent
            int random = RNG.getInstance().RandomNumber(0, potentialOpponents.Count());
            Player enemy = DB.getInstance().getUserByID(potentialOpponents[random].UserID);
            Console.WriteLine($"Folgender Gegner wurde ermittelt: {enemy.Username}");
            Console.WriteLine("Starte Battle gegen den ausgewaehlten User!");

            Output.confirm();

            //Open Battle constructor, which automatically calls BattleLogic
            Battle newBattle = new Battle(currentPlayer, enemy);

            return newBattle;
        }

        public void printLog()
        {
            Console.Clear();
            Console.WriteLine("Battle Log:\n");
            foreach (string line in BattleLog)
            {
                Console.WriteLine(line);
            }
        }
    }
}
