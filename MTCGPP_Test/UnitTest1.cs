using NUnit.Framework;
using MonsterTradingCardGamePP;
using System;
using Moq;
using System.IO;
using MonsterTradingCardGamePP.Cards;
using MonsterTradingCardGamePP.Enum;
using System.Collections.Generic;
using System.Linq;

namespace MTCGPP_Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void test_battle_winner()
        {
            //arrange
            //manually set Player with only a single Card in deck, so to guarantee the winner is testPlayer1
            Player testPlayer1 = new Player(1,"test1","token",0,0,0,0);
            Player testPlayer2 = new Player(2,"test2","token",0,0,0,0);
            List<Card> deck1 = new List<Card>();
            List<Card> deck2 = new List<Card>();
            deck1.Add(new Card(1, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 99999));
            deck2.Add(new Card(2, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin2", 1));
            int winnerID, loserID;
            List<string> log = new List<string>();

            //act
            (winnerID, loserID) = BattleLogic.fight(deck1, deck2, log, testPlayer1, testPlayer2);

            //assert
            Assert.AreEqual(1, winnerID);
        }   
        
        [Test]
        public void test_battle_draw()
        {
            //arrange
            //manually set Player with same card, to force draw
            Player testPlayer1 = new Player(1,"test1","token",0,0,0,0);
            Player testPlayer2 = new Player(2,"test2","token",0,0,0,0);
            List<Card> deck1 = new List<Card>();
            List<Card> deck2 = new List<Card>();
            deck1.Add(new Card(1, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 99999));
            deck2.Add(new Card(2, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin2", 99999));
            int winnerID, loserID;
            List<string> log = new List<string>();

            //act
            (winnerID, loserID) = BattleLogic.fight(deck1, deck2, log, testPlayer1, testPlayer2);

            //assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(0, winnerID); //BattleLogic should return winnerID and loserID as 0 when there is a draw (no player exists with ID 0)
                Assert.AreEqual(0, loserID);
            });
        }

        [Test]
        public void test_battle_swapCards()
        {
            //arrange
            //Player combat with 4 cards each, winner should have all 8 cards in the end
            Player testPlayer1 = new Player(1, "test1", "token", 0, 0,0,0);
            Player testPlayer2 = new Player(2, "test2", "token", 0, 0,0,0);
            List<Card> deck1 = new List<Card>();
            List<Card> deck2 = new List<Card>();
            deck1.Add(new Card(1, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 1));
            deck1.Add(new Card(2, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 2));
            deck1.Add(new Card(3, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 3));
            deck1.Add(new Card(4, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 6));
            deck2.Add(new Card(1, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 1));
            deck2.Add(new Card(2, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 2));
            deck2.Add(new Card(3, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 3));
            deck2.Add(new Card(4, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 6));
            int winnerID, loserID;
            List<string> log = new List<string>();

            //act
            (winnerID, loserID) = BattleLogic.fight(deck1, deck2, log, testPlayer1, testPlayer2);

            //assert
            if (winnerID == testPlayer1.UserID) //check who is winner | 8 cards should be in the winners deck
            {
                Assert.Multiple(() =>
                {
                    Assert.AreEqual(8, deck1.Count());
                    Assert.AreEqual(0, deck2.Count());
                });
            }
            else if(winnerID == testPlayer2.UserID)
            {
                Assert.Multiple(() =>
                {
                    Assert.AreEqual(0, deck1.Count());
                    Assert.AreEqual(8, deck2.Count());
                });
            } 
            else //test needs to be repeated, battle ended in draw
            {
                Assert.Warn("Repeat Test, Battle ended in Draw");
            }
        }

        
        [Test]
        public void test_selectCardInBattle()
        {
            //arrange
            List<Card> deck1 = new List<Card>();
            deck1.Add(new Card(1, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 1));
            deck1.Add(new Card(2, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 1));
            deck1.Add(new Card(3, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 1));
            deck1.Add(new Card(4, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 1));
            int select = 0; //0 => first card on first try, and second card on second try

            //act
            Card returnCard1 =BattleLogic.selectCard(deck1, select);
            Card returnCard2 =BattleLogic.selectCard(deck1, select);

            //assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, returnCard1.CardID); 
                Assert.AreEqual(2, returnCard2.CardID);
            });
        }

        [Test]
        public void test_calcMonsterCombat()
        {
            //calculate Monster damage with exhaustion stacks
            //arrange
            Card goblin = new Card(1, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 1);
            Card knight = new Card(1, CardType.Monster, MonsterType.Knight, Element.Normal, "testKnight", 1);
            int damage = 10;
            //set exhaustion to 3 //should equal a 60% damage reduction ==> to 4 damage
            goblin.increaseExhaustion();
            goblin.increaseExhaustion();
            goblin.increaseExhaustion();
            List<string> log = new List<string>();

            //act
            damage = BattleLogic.monsterCombat(goblin, knight, 10, log);


            //assert
            Assert.AreEqual(4,damage);
        }

        [Test]
        public void test_calcSpellCombatFire()
        {
            //calculate Spell damage (elements)
            //arrange
            Card fireSpell = new Card(1, CardType.Spell, null , Element.Fire, "testFireSpell", 1);

            Card normalKnight = new Card(1, CardType.Monster, MonsterType.Knight, Element.Normal, "testNormalKnight", 1);
            Card fireKnight = new Card(1, CardType.Monster, MonsterType.Knight, Element.Fire, "testFireKnight", 1);
            Card waterKnight = new Card(1, CardType.Monster, MonsterType.Knight, Element.Water, "testWaterKnight", 1);
            int damage = 10;

            List<string> log = new List<string>();

            //act
            int doubleDamage = BattleLogic.spellCombat(fireSpell, normalKnight, damage, log);
            int normalDamage = BattleLogic.spellCombat(fireSpell, fireKnight, damage, log);
            int halveDamage = BattleLogic.spellCombat(fireSpell, waterKnight, damage, log);


            //assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(20, doubleDamage);
                Assert.AreEqual(10, normalDamage);
                Assert.AreEqual(5, halveDamage);
            });
        }

        [Test]
        public void test_exhaustion()
        {
            //arrange
            Card temp = new Card(1, CardType.Monster, MonsterType.Goblin, Element.Normal, "Goblin Soldat", 2);

            //act
            temp.increaseExhaustion();

            //assert
            Assert.AreEqual(1, temp.Exhaustion);
        }

        [Test]
        public void test_goblinVsDragon()
        {
            //Goblins are too afraid of Dragons to attack.
            // ==> Damage from goblin to dragon should be set to zero

            //arrange
            Card goblin = new Card(1, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 1);
            Card dragon = new Card(1, CardType.Monster, MonsterType.Dragon, Element.Normal, "testDragon", 1);
            List<string> log = new List<string>();
            int damage = 111;

            //act
            damage = specialRules.checkSpecial(goblin, dragon, damage, log);

            //assert
            Assert.AreEqual(0, damage);
        }        
        
        [Test]
        public void test_orkVsWizzard()
        {
            //Wizzard can control Orks so they are not able to damage them. 
            // ==> Damage from ork to wizzard should be set to zero

            //arrange
            Card ork = new Card(1, CardType.Monster, MonsterType.Ork, Element.Normal, "testOrk", 1);
            Card wizzard = new Card(1, CardType.Monster, MonsterType.Wizzard, Element.Normal, "testWizzard", 1);
            List<string> log = new List<string>();
            int damage = 111;

            //act
            damage = specialRules.checkSpecial(ork, wizzard, damage, log);

            //assert
            Assert.AreEqual(0, damage);
        }

        [Test]
        public void test_waterSpellVsKnight()
        {
            //The armor of Knights is so heavy that WaterSpells make them drown them instantly. 
            // ==> Damage from waterSpell to knight should be set to a value that instantly kills them

            //arrange
            Card waterSpell = new Card(1, CardType.Spell, null, Element.Water, "testWaterSpell", 1);
            Card knight = new Card(1, CardType.Monster, MonsterType.Knight, Element.Normal, "testKnight", 1);
            List<string> log = new List<string>();
            int damage = 111;

            //act
            damage = specialRules.checkSpecial(waterSpell, knight, damage, log);

            //assert
            Assert.AreEqual(99999, damage);
        }

        [Test]
        public void test_spellVsKraken()
        {
            //The Kraken is immune against spells.  
            // ==> Damage from spell to kraken should be set to zero

            //arrange
            Card spell = new Card(1, CardType.Spell, null, Element.Normal, "testSpell", 1);
            Card kraken = new Card(1, CardType.Monster, MonsterType.Kraken, Element.Normal, "testKraken", 1);
            List<string> log = new List<string>();
            int damage = 111;

            //act
            damage = specialRules.checkSpecial(spell, kraken, damage, log);

            //assert
            Assert.AreEqual(0, damage);
        }

        [Test]
        public void test_dragonVsFireElf()
        {
            //The FireElves know Dragons since they were little and can evade their attacks.   
            // ==> Damage from spell to kraken should be set to zero

            //arrange
            Card dragon = new Card(1, CardType.Monster, MonsterType.Dragon, Element.Normal, "testDragon", 1);
            Card fireElf = new Card(1, CardType.Monster, MonsterType.Elf, Element.Fire, "testFireElf", 1);
            List<string> log = new List<string>();
            int damage = 111;

            //act
            damage = specialRules.checkSpecial(dragon, fireElf, damage, log);

            //assert
            Assert.AreEqual(0, damage);
        }

        [Test]
        public void test_exhaustionInBattle()
        {
            //arrange
            //manually set Player with only a single Card in deck, so to guarantee the winner is testPlayer1
            Player testPlayer1 = new Player(1, "test1", "token", 0, 0,0,0);
            Player testPlayer2 = new Player(2, "test2", "token", 0, 0,0,0);
            List<Card> deck1 = new List<Card>();
            List<Card> deck2 = new List<Card>();
            deck1.Add(new Card(1, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 99999));
            deck2.Add(new Card(2, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin2", 1));
            int winnerID, loserID;
            List<string> log = new List<string>();

            //act
            (winnerID, loserID) = BattleLogic.fight(deck1, deck2, log, testPlayer1, testPlayer2);

            //assert

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, deck1[0].Exhaustion); //1st card in deck1 should have 1 exhaustion after combat
                Assert.AreEqual(0, deck1[1].Exhaustion); //2nd card in deck1 should have 0 exhaustion after combat (because it lost and was transferred over)
            });
        }

        [Test]
        public void test_RNG()
        {
            //arrange
            RNG random = RNG.getInstance();
            List<int> randomList = new List<int>();
            int check = 0;

            //act

            //randomly add 1000 numbers, from 1-99
            for(int i = 0; i< 1000; i++)
            {
                randomList.Add(random.RandomNumber(1,100));
            }

            //assert
            for (int i = 0; i < 1000; i++)
            {
                if(randomList[i] >= 1 && randomList[i] <= 99)
                {
                    check++;
                }
            }

            Assert.AreEqual(1000, check);
        }

        [Test]
        public void test_battlelogWin()
        {
            //arrange
            //manually set Player with only a single Card in deck, so to guarantee the winner is testPlayer1
            Player testPlayer1 = new Player(1, "test1", "token", 0, 0, 0, 0);
            Player testPlayer2 = new Player(2, "test2", "token", 0, 0, 0, 0);
            List<Card> deck1 = new List<Card>();
            List<Card> deck2 = new List<Card>();
            deck1.Add(new Card(1, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 99999));
            deck2.Add(new Card(2, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin2", 1));
            List<string> log = new List<string>();
            List<string> testLog = new List<string>();
            testLog.Add("\n\n------------------------------------------------");
            testLog.Add("Round 1 has started!");
            testLog.Add("test1 has choosen testGoblin");
            testLog.Add("test2 has choosen testGoblin2");
            testLog.Add("testGoblin is attacking with an damage value of 99999");
            testLog.Add("testGoblin2 is attacking with an damage value of 1");
            testLog.Add("test1 has won this round!");
            testLog.Add("testGoblin and testGoblin2 have been added to test1's deck!");
            testLog.Add("testGoblin has gained 1 stack of Exhaustion from the win");
            testLog.Add("\n\n------------------------------------------------");
            testLog.Add("test1 has won the whole battle !");
            testLog.Add("test1 has gained 3 elo points from this win");
            testLog.Add("test2 has lost 5 elo points from this loss");            
            testLog.Add("------------------------------------------------");


            //act
            BattleLogic.fight(deck1, deck2, log, testPlayer1, testPlayer2);

            //assert
            Assert.Multiple(() =>
            {
                int count = 0;
                foreach(string txt in log)
                {
                    Assert.AreEqual(testLog[count], txt);
                    count++;
                }
            });
        }

        [Test]
        public void test_weakness()
        {
            //arrange
            Card normalCard = new Card(1, CardType.Monster, MonsterType.Goblin, Element.Normal, "testGoblin", 99999);
            Card fireCard = new Card(1, CardType.Monster, MonsterType.Goblin, Element.Fire, "testGoblin", 99999);
            Card waterCard = new Card(1, CardType.Monster, MonsterType.Goblin, Element.Water, "testGoblin", 99999);

            //act
            Element weakness1 = Card.getWeakness(normalCard.Element);
            Element weakness2 = Card.getWeakness(fireCard.Element);
            Element weakness3 = Card.getWeakness(waterCard.Element);

            //assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(weakness1, Element.Fire);
                Assert.AreEqual(weakness2, Element.Water);
                Assert.AreEqual(weakness3, Element.Normal);

            });
        }

        [Test]
        public void test_chooseCoinsTrade()
        {
            //arrange
            int? coin = null;
            using (var sr = new StringReader("50"))
            {
                Console.SetIn(sr);

                //act
                coin = Trade.chooseCoinForTrade();

                //assert
                Assert.AreEqual(coin, 50);
            }
        }

        [Test]
        public void test_chooseCoinsTradeReturnNull()
        {
            //arrange
            int? coin = null;
            using (var sr = new StringReader("x"))
            {
                Console.SetIn(sr);

                //act
                coin = Trade.chooseCoinForTrade();

                //assert
                Assert.AreEqual(coin, null);
            }
        }
    }
}