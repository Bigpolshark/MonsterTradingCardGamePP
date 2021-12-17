using MonsterTradingCardGamePP.Cards;
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
        public int AuthToken { get; }
        public List<Card> Stack { get; set; }
        public List<Card> Deck { get; set; }
        public int Coins { get; } //need to add subtract coins method

        public Player(string username, int authToken)
        {
            Username = username;
            AuthToken = authToken;
            //getStack();
            //getDeck();
            //getCoins();
        }

        private void getCoins()
        {
            throw new NotImplementedException();
        }

        private void getDeck()
        {
            throw new NotImplementedException();
        }

        private void getStack()
        {
            throw new NotImplementedException();
        }
    }
}
