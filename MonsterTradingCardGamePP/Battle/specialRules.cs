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
        public static int checkSpecial(Card own, Card enemy, int damage, List<string> BattleLog)
        {
            switch(own.MonsterType)
            {
                case MonsterType.Dragon: damage = DragonCheck(damage, enemy, BattleLog);  break;
                case MonsterType.Goblin: damage = GoblinCheck(damage, enemy, BattleLog); break;
                case MonsterType.Ork: damage = OrkCheck(damage, enemy, BattleLog); break;
                default: 
                    //check if Card is a Spell (for Kraken)
                    if (own.CardType == CardType.Spell)
                    {
                        damage = SpellCheck(damage, enemy, BattleLog);
                    }

                    //check if Card is a WaterSpell (for knight)
                    if(own.CardType == CardType.Spell && own.Element == Element.Water)
                    {
                        damage = WaterSpellCheck(damage, enemy, BattleLog);
                    }

                    break; 
            }


            return damage;
        }

        private static int GoblinCheck(int damage,  Card enemy, List<string> BattleLog)
        {
            if (enemy.MonsterType == MonsterType.Dragon)
            {
                damage = 0;
                BattleLog.Add("Goblin is too afraid of attacking a Dragon!");
            }


            return damage;
        }        
        private static int OrkCheck(int damage, Card enemy, List<string> BattleLog)
        {
            if (enemy.MonsterType == MonsterType.Wizzard)
            {
                damage = 0;
                BattleLog.Add("Ork is mind controlled by Wizzard and can't attack!");
            }


            return damage;
        }        
        private static int SpellCheck(int damage, Card enemy, List<string> BattleLog)
        {
            if (enemy.MonsterType == MonsterType.Kraken)
            {
                damage = 0;
                BattleLog.Add("Spell fizzles out against Krakens magic immunity!");
            }


            return damage;
        }
        private static int WaterSpellCheck(int damage, Card enemy, List<string> BattleLog)
        {
            if (enemy.MonsterType == MonsterType.Knight)
            {
                damage = 99999;
                BattleLog.Add("The Water Spell drowns the Knight!");
            }

            return damage;
        }
        private static int DragonCheck(int damage, Card enemy, List<string> BattleLog)
        {
            if (enemy.MonsterType == MonsterType.Elf && enemy.Element == Element.Fire)
            {
                damage = 0;
                BattleLog.Add("Dragon cannot hit the Elf, because they are too evasive!");
            }

            return damage;
        }

    }
}
