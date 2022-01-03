using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP
{
    class potentialOpponent
    {
        public int UserID { get; }
        public string Username { get; }

        public potentialOpponent(int id, string username)
        {
            UserID = id;
            Username = username;
        }
    }
}
