using MonsterTradingCardGamePP.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP.Cards
{
    public class tradeInfo
    {
        public int tradeId { get; }
        public CardType? targetCardType { get; }
        public int? minDmg { get; }
        public int? coinprice { get; }

        public int ownerID { get; }

        public tradeInfo(int tradeId, CardType? targetCardType, int? minDmg, int? coinprice, int ownerID)
        {
            this.tradeId = tradeId;
            this.targetCardType = targetCardType;
            this.minDmg = minDmg;
            this.coinprice = coinprice;
            this.ownerID = ownerID;
        }
    }
}
