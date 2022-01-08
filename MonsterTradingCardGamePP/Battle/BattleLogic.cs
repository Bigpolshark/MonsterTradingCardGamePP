using MonsterTradingCardGamePP.Cards;
using MonsterTradingCardGamePP.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP
{
    public class BattleLogic
    {
        //public static List<string> log;

        public static (int,int) fight(List<Card> player1Deck, List<Card> player2Deck, List<string> log, Player player1, Player player2)
        {

            for (int i = 1; i <= 100; i++)
            {
                log.Add($"\n\n------------------------------------------------");
                log.Add($"Round {i} has started!");

                int selection1 = RNG.getInstance().RandomNumber(0, player1Deck.Count());
                Card card1 = selectCard(player1Deck, selection1);
                log.Add($"{player1.Username} has choosen {card1.Name}");
                
                int selection2 = RNG.getInstance().RandomNumber(0, player2Deck.Count());
                Card card2 = selectCard(player2Deck, selection2);
                log.Add($"{player2.Username} has choosen {card2.Name}");

                int damage1 = calcDamage(card1, card2, log);
                int damage2 = calcDamage(card2, card1, log);
                log.Add($"{card1.Name} is attacking with an damage value of {damage1}");
                log.Add($"{card2.Name} is attacking with an damage value of {damage2}");

                //check who won
                if (damage1 > damage2)
                {
                    log.Add($"{player1.Username} has won this round!");
                    log.Add($"{card1.Name} and {card2.Name} have been added to {player1.Username}'s deck!");

                    if(card1.CardType == Enum.CardType.Monster)
                    {
                        card1.increaseExhaustion();
                        log.Add($"{card1.Name} has gained 1 stack of Exhaustion from the win");
                    }

                    //add both cards to player1
                    player1Deck.Add(new Card(card1));
                    player1Deck.Add(new Card(card2));
                }
                else if(damage2 > damage1)
                {
                    log.Add($"{player2.Username} has won this round!");
                    log.Add($"{card1.Name} and {card2.Name} have been added to {player2.Username}'s deck!");

                    if (card2.CardType == Enum.CardType.Monster)
                    {
                        card2.increaseExhaustion();
                        log.Add($"{card2.Name} has gained 1 stack of Exhaustion from the win");
                    }

                    //add both cards to player2
                    player2Deck.Add(new Card(card1));
                    player2Deck.Add(new Card(card2));
                }
                else //draw
                {
                    //add both card back to their respective decks
                    player1Deck.Add(new Card (card1));
                    player2Deck.Add(new Card (card2));
                    log.Add("This round ended in a Draw!");
                    log.Add($"{card1.Name} and {card2.Name} have been added back to their respective owners decks");
                }

                //check who won
                if(player1Deck.Count() == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    log.Add($"\n\n------------------------------------------------");
                    log.Add($"{player2.Username} has won the whole battle !");
                    log.Add($"{player2.Username} has gained 3 elo points from this win");
                    log.Add($"{player1.Username} has lost 5 elo points from this loss");
                    log.Add($"------------------------------------------------");
                    Console.ResetColor();
                    //elo noch machen
                    return (player2.UserID, player1.UserID);
                }
                else if(player2Deck.Count() == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    log.Add($"\n\n------------------------------------------------");
                    log.Add($"{player1.Username} has won the whole battle !");
                    log.Add($"{player1.Username} has gained 3 elo points from this win");
                    log.Add($"{player2.Username} has lost 5 elo points from this loss");
                    log.Add($"------------------------------------------------");
                    Console.ResetColor();
                    //elo noch machen
                    return (player1.UserID, player2.UserID);
                }
            }

            //can only get here if draw
            log.Add("\n\nUnentschieden!");
            log.Add("Es konnte nach 100 Runden kein Sieger gefunden!");
            log.Add("Die ELO - Werte beider Spieler bleiben unverändert");

            return (0, 0); //check outside ==> if 0,0 it is a Draw

        }

        public static Card selectCard(List<Card> deck, int selection)
        {
            //create a copy of the chosen card and at the same time remove it from the Deck

            Card tempCard = new Card(deck[selection]);

            deck.RemoveAt(selection);

            return tempCard;
        }

        private static int calcDamage(Card ownCard, Card enemyCard, List<string> log)
        {
            int damage = ownCard.Damage; //base value


            if(ownCard.CardType == Enum.CardType.Spell || enemyCard.CardType == Enum.CardType.Spell)
            {
                damage = spellCombat(ownCard, enemyCard, damage, log); 
            }
            else //monster only combat
            {
                //normally nothing to calculate, but here comes the unique mechanic "exhaustion" to play
                damage = monsterCombat(ownCard, enemyCard, damage, log);
            }



            //check specialties after damage //because of the weird way I coded them before (they set the given damage to either 0, or 99999, certainly not the best way)
            damage = specialRules.checkSpecial(ownCard, enemyCard, damage, log);

            return damage;
        }


        public static int spellCombat(Card ownCard, Card enemyCard, int damage, List<string> log)
        {
            //for Element, can check Weakness first, then if same element, and if it isnt any of the above, it has to be resisant to it
            if (ownCard.Element == enemyCard.Weakness)
            {
                damage *= 2;
                log.Add($"{ownCard.Name}'s damage doubled, because {enemyCard.Name} is weak against {ownCard.Element}");
            }
            else if(ownCard.Element == enemyCard.Element)
            {
                //damage stays the same
            }
            else
            {
                damage /= 2;
                log.Add($"{ownCard.Name}'s damage is halved, because {enemyCard.Name} is resistant against {ownCard.Element}");
            }


            return damage;
        }
        public static int monsterCombat(Card ownCard, Card enemyCard, int damage, List<string> log)
        {
            if(ownCard.Exhaustion > enemyCard.Exhaustion)
            {
                int difference = ownCard.Exhaustion - enemyCard.Exhaustion;
                damage = (damage * (100 - 20 * difference)) / 100; //damage reduced by 20% per Stack difference, only written like that to not have to use double/float

                log.Add($"{ownCard.Name} is more Exhausted than {enemyCard.Name} by {difference} Stacks");
                log.Add($"{ownCard.Name}'s is reduced by {20*difference}% because of Exhaustion!");
            }

            return damage;
        }
    }
}
