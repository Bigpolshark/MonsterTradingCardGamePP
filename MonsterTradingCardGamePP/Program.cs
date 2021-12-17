using MonsterTradingCardGamePP.Cards;
using MonsterTradingCardGamePP.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP
{
    class Program
    {
        static void Main(string[] args)
        {
            CardType test = CardType.Spell;
            Console.WriteLine($"Check {test}");


            //test card
            Card Card1 = new Card(1, CardType.Monster, Element.Normal, MonsterType.Goblin, "Goblin Soldat", 2);
            Card Card2 = new Card(2, CardType.Monster, Element.Fire, MonsterType.Dragon, "Vulkandrache", 10000);
            Card Card3 = new Card(3, CardType.Spell, Element.Water, null, "OargerSpell", 900);
            Card Card4 = new Card(4, CardType.Monster, Element.Normal, MonsterType.Knight, "Whacker Knight", 1);

            Console.WriteLine($"Monster {Card1.Name} + {Card1.MonsterType}");
            Console.WriteLine($"Spell {Card2.Name} + {Card2.MonsterType}");


            Console.WriteLine($"Damage Check (with damage value of 10 (should be reduced to 0)) ==> {specialRules.checkSpecial(Card1, Card2, 100)}");
            Console.WriteLine($"Damage Check (with damage value of 10 (should be insta kill)) ==> {specialRules.checkSpecial(Card3, Card4, 100)}");

            Player Player1 = new Player("Hans", 1);
            Player Player2 = new Player("Deiter", 2);

            new Battle(Player1, Player2);





            //remove later
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
    }
}
