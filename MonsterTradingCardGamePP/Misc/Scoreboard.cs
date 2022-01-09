using MonsterTradingCardGamePP.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP.Misc
{
    class Scoreboard
    {
        public static void viewScoreboard(Player player)
        {
            Console.Clear();
            List<Player> allPlayers = DB.getInstance().getUsersScoreboard();

            if(allPlayers == null)
            {
                Output.errorOutputCustom("Es existieren derzeit keine Spieler!");
                Output.confirm();
                return;
            }

            printScoreboardHeader();
            int rank = 0;
            foreach(Player tempPlayer in allPlayers)
            {
                printPlayerScore(tempPlayer, rank, player);
                rank++;
            }

            Output.confirm();
        }

        private static void printScoreboardHeader()
        {
            Console.WriteLine("Rank Player              Elo   Winrate TotalGames");
        }

        private static void printPlayerScore(Player player, int rank, Player currentPlayer)
        {
            double winrateCalc;
            string winrate;

            if (player.Games == 0)
            {
                winrate = "--";
            }
            else
            {
                winrateCalc = (double)player.Wins / (double)player.Games * (double)100;
                winrateCalc = Math.Round(winrateCalc, 2);
                winrate = winrateCalc.ToString();
            }


            winrate += '%';

            if (player.UserID == currentPlayer.UserID)
                Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"{rank.ToString().PadRight(5, ' ')}{player.Username.ToString().PadRight(20, ' ')}{player.Elo.ToString().PadRight(6, ' ')}{winrate.PadRight(8, ' ')}{player.Games}");

            if (player.UserID == currentPlayer.UserID)
                Console.ResetColor();
        }
    }
}
