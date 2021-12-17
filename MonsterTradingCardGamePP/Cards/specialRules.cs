using MonsterTradingCardGamePP.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP.Cards
{
    class specialRules
    {
        public static int checkSpecial(Card own, Card enemy, int damage)
        {
            switch(own.MonsterType)
            {
                case MonsterType.Dragon: damage = DragonCheck(damage, enemy);  break;
                case MonsterType.Goblin: damage = GoblinCheck(damage, enemy); break;
                case MonsterType.Ork: damage = OrkCheck(damage, enemy); break;
                default: 
                    //check if Card is a Spell (for Kraken)
                    if (own.CardType == CardType.Spell)
                    {
                        damage = SpellCheck(damage, enemy);
                    }

                    //check if Card is a WaterSpell (for knight)
                    if(own.CardType == CardType.Spell && own.Element == Element.Water)
                    {
                        damage = WaterSpellCheck(damage, enemy);
                    }

                    break; 
            }


            return damage;
        }

        private static int GoblinCheck(int damage,  Card enemy)
        {
            if (enemy.MonsterType == MonsterType.Dragon)
                damage = 0;

            return damage;
        }        
        private static int OrkCheck(int damage, Card enemy)
        {
            if (enemy.MonsterType == MonsterType.Wizzard)
                damage = 0;

            return damage;
        }        
        private static int SpellCheck(int damage, Card enemy)
        {
            if (enemy.MonsterType == MonsterType.Kraken)
                damage = 0;

            return damage;
        }
        private static int WaterSpellCheck(int damage, Card enemy)
        {
            if (enemy.MonsterType == MonsterType.Knight)
                damage = 99999;

            return damage;
        }
        private static int DragonCheck(int damage, Card enemy)
        {
            if (enemy.MonsterType == MonsterType.Elf && enemy.Element == Element.Fire)
                damage = 0;

            return damage;
        }

    }
}
