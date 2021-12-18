using MonsterTradingCardGamePP.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP.Cards
{
    class Card
    {
        /* At first it was planned to split Monster and Spell cards, and using an ICard Interface. However after thinking about it, I came to the conclusion to use only a single
         * Class for the Cards, and differentiate between the Types by using an additional Enum. This made checking for the exhaustion mechanic easier too (can just set
         * exhaustion 0 to spells and compare it like it is a monster card). */

        public int CardID { get; } //only used when getting Card from Database, may be needed for Trading etc ? Not sure, but to be safe for later 
        public CardType CardType { get; } //Monster/Spell, for easier differentation
        public MonsterType? MonsterType { get; } //? ==> nullable ==> can be NULL
        public Element Element { get; }
        public string Name { get; } //custom Name of Card
        public int Damage { get; }

        //add Unique mechanic - exhaustion
        //Exhaustion occurs on a Monster card, after combat. One stack of exhaustion is added per combat.
        //During combat, the current exhaustion stacks are compared between the units, and depending on the difference, the more exhausted monster has 
        //its (post-calculation) damage value reduced
        public int Exhaustion { get; } //no setter, can only be increased by one with function increaseExhaustion(), and after battle the battleDeck with the exausted card is thrown out anyway

        public Card(int cardID, CardType cardType, MonsterType? monsterType, Element element,  string name, int damage)
        {
            CardID = cardID;
            CardType = cardType;
            MonsterType = monsterType;
            Element = element;
            Name = name;
            Damage = damage;
            Exhaustion = 0;
        }
    }

}
