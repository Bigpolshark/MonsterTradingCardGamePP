using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP
{
    class RNG
    {
        //singleton
        private static RNG Instance = new RNG();
        //One universa Random() to prevent possible "duplication" of randomness
        private static Random _random = new Random();

        public static RNG getInstance()
        {
            return Instance;
        }

        public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}
