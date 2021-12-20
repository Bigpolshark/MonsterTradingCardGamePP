using MonsterTradingCardGamePP.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP
{
    class BattleLogic
    {
        public static void fight(List<Card> player1Deck, List<Card> player2Deck, List<string> log, Player player1, Player player2)
        {
            for (int i = 1; i <= 100; i++)
            {
                log.Add($"\n------------------------------------------------");
                log.Add($"Round {i} has started!");

                int selection1 = RNG.getInstance().RandomNumber(0, player1Deck.Count() - 1);
                Card card1 = selectCard(player1Deck, selection1);
                log.Add($"{player1.Username} has choosen {card1.Name}");
                
                int selection2 = RNG.getInstance().RandomNumber(0, player2Deck.Count() - 1);
                Card card2 = selectCard(player2Deck, selection2);
                log.Add($"{player2.Username} has choosen {card2.Name}");

                //WIP
                int damage1 = calcDamage(card1, card2, log);
                int damage2 = calcDamage(card2, card1, log);
                //
                //
                //

                //check who won
                if(damage1 > damage2)
                {
                    //add both cards to player1
                    player1Deck.Add(new Card(card1));
                    player1Deck.Add(new Card(card2));
                }
                else if(damage2 > damage1)
                {
                    //add both cards to player2
                    player2Deck.Add(new Card(card1));
                    player2Deck.Add(new Card(card2));
                }
                else //draw
                {
                    //add both card back to their respective decks
                    player1Deck.Add(new Card (card1));
                    player2Deck.Add(new Card (card2));
                }

                //check who won
                //
                //
                //
                //


            }

            //can only get here if draw
            log.Add("Unentschieden!");
            log.Add("Es konnte nach 100 Runden kein Sieger gefunden!");
            log.Add("Die ELO - Werte beider Spieler bleiben unverändert");
        }

        public static Card selectCard(List<Card> deck, int selection)
        {
            //create a copy of the chosen card and at the same time remove it from the Deck

            Card tempCard = new Card(deck[selection]);

            deck.RemoveAt(selection);

            return tempCard;
        }

        public static int calcDamage(Card ownCard, Card enemyCard, List<string> log)
        {
            int damage = ownCard.Damage;


            ///



            //check specialties after damage
            damage = specialRules.checkSpecial(ownCard, enemyCard, damage, log);

            return damage;
        }
    }
}
