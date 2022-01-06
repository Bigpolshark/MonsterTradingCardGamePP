using MonsterTradingCardGamePP.Cards;
using MonsterTradingCardGamePP.Enum;
using MonsterTradingCardGamePP.Misc;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGamePP.Database
{
    class DB
    {
        //as singleton
        private static DB Instance = new DB();
        private NpgsqlConnection Connection;
        private RNG randomDB = RNG.getInstance();

        public static DB getInstance()
        {
            return Instance;
        }
        private void Connect()
        {
            Connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=;Database=postgres");
            Connection.Open();
        }

        private void Disconnect()
        {
            Connection.Close();
        }

        public Player AddUser(string username, string password)
        {
            if (checkUsername(username))
                return null;

            //Hashfunktion einbauen, aber erstmal plaintext

            int ELO = 100; //starting value 100
            int coins = 20; //starting value 20
            int tokenNumber;
            string authtoken;

            do
            {
                tokenNumber = randomDB.RandomNumber(1, 999999);
                authtoken = "" + username[0] + username[username.Length - 1] + tokenNumber.ToString();
            } while (checkToken(authtoken) == false);

            //hashPW
            string hashedPassword = miscFunctions.hashPassword(password);

            Connect();
            using (var sql = new NpgsqlCommand("INSERT INTO player (username, password, authtoken, coins, elo) VALUES (@user, @pass, @auth, @coin, @elo)", Connection))
            {
                sql.Parameters.AddWithValue("user", username);
                sql.Parameters.AddWithValue("pass", hashedPassword);
                sql.Parameters.AddWithValue("auth", authtoken);
                sql.Parameters.AddWithValue("coin", coins);
                sql.Parameters.AddWithValue("elo", ELO);
                sql.ExecuteNonQuery();
            }
            Disconnect();

            //call login() directly after registering and return it to the one calling AddUser()
            return login(username, password);
        }

        public bool checkToken(string token)
        {
            Connect();
            using (var sql = new NpgsqlCommand("SELECT authtoken FROM player WHERE authtoken = @auth", Connection))
            {
                sql.Parameters.AddWithValue("auth", token);
                NpgsqlDataReader reader = sql.ExecuteReader();

                if (reader.HasRows)
                {
                    Disconnect();
                    return false;
                }

                Disconnect();
                return true;
            }
        }
        public bool checkUsername(string username)
        {
            Connect();
            using (var sql = new NpgsqlCommand("SELECT username FROM player WHERE username = @user", Connection))
            {
                sql.Parameters.AddWithValue("user", username);
                NpgsqlDataReader reader = sql.ExecuteReader();

                if (reader.HasRows)
                {
                    Disconnect();
                    return true; //User exists
                }

                Disconnect();
                return false; //User does not exist
            }
        }

        //only used to automatically add cards to DB, only left if Cards need to be added later
        /*
        public void AddCardToDB(Card card)
        {
            Connect();
            using (var sql = new NpgsqlCommand("INSERT INTO cards (name, damage, cardtype, element, monstertype) VALUES (@name, @dmg, @ctyp, @ele, @mtyp)", Connection))
            {
                sql.Parameters.AddWithValue("name", card.Name);
                sql.Parameters.AddWithValue("dmg", card.Damage);
                sql.Parameters.AddWithValue("ctyp", card.CardType.ToString());
                sql.Parameters.AddWithValue("ele", card.Element.ToString());
                sql.Parameters.AddWithValue("mtyp", card.MonsterType.ToString());
                sql.ExecuteNonQuery();
            }
            Disconnect();
        }
        */

        public Player login(string username, string password)
        {
            string hashedPassword = miscFunctions.hashPassword(password);

            Connect();
            using (var sql = new NpgsqlCommand("SELECT * FROM player WHERE username = @user AND password = @pass", Connection))
            {
                sql.Parameters.AddWithValue("user", username);
                sql.Parameters.AddWithValue("pass", hashedPassword);
                NpgsqlDataReader reader = sql.ExecuteReader();

                Player tempPlayer = null;

                if (reader.HasRows)
                {
                    reader.Read();
                    tempPlayer = new Player((int)reader["id"], reader["username"].ToString(), reader["authtoken"].ToString(), (int)reader["coins"], (int)reader["elo"]);
                }

                Disconnect();
                return tempPlayer;
            }
        }          
        public List<Card> getAllCardsFromDB()
        {
            Connect();
            using (var sql = new NpgsqlCommand("SELECT * FROM cards ORDER BY cardid ASC", Connection))
            {
                NpgsqlDataReader reader = sql.ExecuteReader();

                List<Card> cardList = null;

                if (reader.HasRows)
                {
                    cardList = new List<Card>();
                    while (reader.Read())
                    {
                        if(reader["monstertype"].ToString() != "")
                            cardList.Add(new Card((int)reader["cardid"], (CardType)System.Enum.Parse(typeof(CardType),reader["cardtype"].ToString()), (MonsterType?)System.Enum.Parse(typeof(MonsterType), reader["monstertype"].ToString()), (Element)System.Enum.Parse(typeof(Element), reader["element"].ToString()), reader["name"].ToString(), (int)reader["damage"]));
                        else
                            cardList.Add(new Card((int)reader["cardid"], (CardType)System.Enum.Parse(typeof(CardType),reader["cardtype"].ToString()), null, (Element)System.Enum.Parse(typeof(Element), reader["element"].ToString()), reader["name"].ToString(), (int)reader["damage"]));
                    }
                }

                Disconnect();
                return cardList;
            }
        }         
        public List<Card> getUserStack(int UserID)
        {
            Connect();
            using (var sql = new NpgsqlCommand("SELECT cardid FROM playerstack WHERE playerID = @pID ORDER BY cardid ASC", Connection))
            {
                sql.Parameters.AddWithValue("pID", UserID);
                NpgsqlDataReader reader = sql.ExecuteReader();

                List<Card> cardList = null;

                if (reader.HasRows)
                {
                    cardList = new List<Card>();
                    while (reader.Read())
                    {
                        cardList.Add(new Card(getCardFromID((int)reader["cardid"])));
                    }
                    
                }

                Disconnect();
                return cardList;
            }
        }     
        
        public void buyPack(int id)
        {
            Connect();
            using (var sql = new NpgsqlCommand("UPDATE player SET coins = coins - 5 WHERE id = @id", Connection))
            {
                sql.Parameters.AddWithValue("id", id);
                sql.ExecuteNonQuery();
            }
            Disconnect();
        }

        public void addToStack(int cardID, int playerID)
        {
            Connect();
            using (var sql = new NpgsqlCommand("INSERT INTO playerstack (playerid, cardid) VALUES (@pID, @cID)", Connection))
            {
                sql.Parameters.AddWithValue("pID", playerID);
                sql.Parameters.AddWithValue("cID", cardID);
                sql.ExecuteNonQuery();
            }
            Disconnect();
        }

        public int getCoins(int id)
        {
            Connect();
            using (var sql = new NpgsqlCommand("SELECT coins FROM player WHERE id = @id", Connection))
            {
                sql.Parameters.AddWithValue("id", id);
                NpgsqlDataReader reader = sql.ExecuteReader();

                int coins = 0;

                if (reader.HasRows)
                {
                    reader.Read();
                    coins = (int)reader["coins"];
                }

                Disconnect();
                return coins;
            }
        }

        public List<Card> getUserDeck(int UserID)
        {
            Connect();
            using (var sql = new NpgsqlCommand("SELECT cardid FROM playerdeck WHERE playerid = @pID ORDER BY cardid ASC", Connection))
            {
                sql.Parameters.AddWithValue("pID", UserID);
                NpgsqlDataReader reader = sql.ExecuteReader();

                List<Card> cardList = null;

                if (reader.HasRows)
                {
                    cardList = new List<Card>();
                    while (reader.Read())
                    {
                        cardList.Add(new Card(getCardFromID((int)reader["cardid"])));
                    }

                }

                Disconnect();
                return cardList;
            }
        }

        public void addToDeck(int cardID, int playerID)
        {
            //add to Deck
            Connect();
            using (var sql = new NpgsqlCommand("INSERT INTO playerdeck (playerid, cardid) VALUES (@pID, @cID)", Connection))
            {
                sql.Parameters.AddWithValue("pID", playerID);
                sql.Parameters.AddWithValue("cID", cardID);
                sql.ExecuteNonQuery();
            }
            Disconnect();

            //remove from Stack
            Connect();
            using (var sql = new NpgsqlCommand("DELETE FROM playerstack WHERE number IN(SELECT number FROM playerstack WHERE cardid = @cID AND playerid = @pID LIMIT 1)", Connection))
            {
                sql.Parameters.AddWithValue("cID", cardID);
                sql.Parameters.AddWithValue("pID", playerID);
                sql.ExecuteNonQuery();
            }
            Disconnect();
        }

        public void removeFromDeck(int cardID, int playerID)
        {
            //add to Stack
            Connect();
            using (var sql = new NpgsqlCommand("INSERT INTO playerstack (playerid, cardid) VALUES (@pID, @cID)", Connection))
            {
                sql.Parameters.AddWithValue("pID", playerID);
                sql.Parameters.AddWithValue("cID", cardID);
                sql.ExecuteNonQuery();
            }
            Disconnect();

            //remove from Deck
            Connect();
            using (var sql = new NpgsqlCommand("DELETE FROM playerdeck WHERE cardid = @cID AND playerid = @pID", Connection))
            {
                sql.Parameters.AddWithValue("cID", cardID);
                sql.Parameters.AddWithValue("pID", playerID);
                sql.ExecuteNonQuery();
            }
            Disconnect();
        }

        public Card getCardFromID(int cardID)
        {
            Connect();
            using (var sql = new NpgsqlCommand("SELECT * FROM cards WHERE cardid = @cID", Connection))
            {
                sql.Parameters.AddWithValue("cID", cardID);
                NpgsqlDataReader reader = sql.ExecuteReader();

                Card tempCard = null;

                if (reader.HasRows)
                {
                    reader.Read();

                    if (reader["monstertype"].ToString() != "")
                    {

                        tempCard = new Card((int)reader["cardid"],
                                            (CardType)System.Enum.Parse(typeof(CardType),reader["cardtype"].ToString()),
                                            (MonsterType?)System.Enum.Parse(typeof(MonsterType),reader["monstertype"].ToString()),
                                            (Element)System.Enum.Parse(typeof(Element),reader["element"].ToString()),
                                            reader["name"].ToString(),
                                            (int)reader["damage"]);
                    }
                    else
                    {

                        tempCard = new Card((int)reader["cardid"],
                                            (CardType)System.Enum.Parse(typeof(CardType),reader["cardtype"].ToString()),
                                            null,
                                            (Element)System.Enum.Parse(typeof(Element),reader["element"].ToString()),
                                            reader["name"].ToString(),
                                            (int)reader["damage"]);
                    }
                }
                Disconnect();
                return tempCard;
            }
        }

        public List<potentialOpponent> getPotentialOpponents(int me)
        {
            List<potentialOpponent> list = null;

            Connect();
            using (var sql = new NpgsqlCommand("SELECT playerid FROM playerdeck WHERE NOT playerid = @pID GROUP BY playerid HAVING COUNT(playerid) = 4", Connection))
            {
                sql.Parameters.AddWithValue("pID", me);
                NpgsqlDataReader reader = sql.ExecuteReader();

                if (reader.HasRows)
                {
                    list = new List<potentialOpponent>();

                    while (reader.Read())
                    {
                        list.Add(new potentialOpponent((int)reader["playerid"], getUsernameByID((int)reader["playerid"])));
                    }
                }
            }

            Disconnect();
            return list;
        }

        public string getUsernameByID(int id)
        {
            Connect();
            using (var sql = new NpgsqlCommand("SELECT username FROM player WHERE id = @pID", Connection))
            {
                sql.Parameters.AddWithValue("pID", id);
                NpgsqlDataReader reader = sql.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    string name = reader["username"].ToString();
                    Disconnect();
                    return name;
                }

                Disconnect();
                return "ERROR: User could not be found"; //User does not exist
            }
        }        
        public Player getUserByID(int id)
        {
            Connect();
            using (var sql = new NpgsqlCommand("SELECT * FROM player WHERE id = @uID", Connection))
            {
                sql.Parameters.AddWithValue("uID", id);
                NpgsqlDataReader reader = sql.ExecuteReader();

                Player tempPlayer = null;

                if (reader.HasRows)
                {
                    reader.Read();
                    tempPlayer = new Player((int)reader["id"], reader["username"].ToString(), reader["authtoken"].ToString(), (int)reader["coins"], (int)reader["elo"]);
                }

                Disconnect();
                return tempPlayer;
            }
        }
        public (List<Card>, List<tradeInfo>) viewAllTrades(int id)
        {
            Connect();
            using (var sql = new NpgsqlCommand("SELECT * FROM trades JOIN cards ON cards.cardid=trades.tradedcardid WHERE NOT ownerid = @uID", Connection))
            {
                sql.Parameters.AddWithValue("uID", id);
                NpgsqlDataReader reader = sql.ExecuteReader();

                List<Card> tradeList = null;
                List<tradeInfo> tradeInfo = null;

                if (reader.HasRows)
                {
                    tradeList = new List<Card>();
                    tradeInfo = new List<tradeInfo>();

                    while (reader.Read())
                    {
                        //check if values are null
                        CardType? ctype;
                        int? mindmg, coin;

                        if (reader["targetcardtype"].ToString() == "")
                        {
                            ctype = null;
                        }
                        else
                        {
                            ctype = (CardType)System.Enum.Parse(typeof(CardType), reader["targetcardtype"].ToString());
                        }

                        if (reader["mindmg"].ToString() == "")
                        {
                            mindmg = null;
                        }
                        else
                        {
                            mindmg = (int)reader["mindmg"];
                        }

                        if (reader["coinprice"].ToString() == "")
                        {
                            coin = null;
                        }
                        else
                        {
                            coin = (int)reader["coinprice"];
                        }


                        tradeInfo.Add(new tradeInfo((int)reader["tradeid"],
                                        ctype,
                                        mindmg,
                                        coin,
                                        (int)reader["ownerid"]));


                        if (reader["monstertype"].ToString() != "")
                        {

                            tradeList.Add(new Card((int)reader["cardid"],
                                                (CardType)System.Enum.Parse(typeof(CardType), reader["cardtype"].ToString()),
                                                (MonsterType?)System.Enum.Parse(typeof(MonsterType), reader["monstertype"].ToString()),
                                                (Element)System.Enum.Parse(typeof(Element), reader["element"].ToString()),
                                                reader["name"].ToString(),
                                                (int)reader["damage"]));
                        }
                        else
                        {

                            tradeList.Add(new Card((int)reader["cardid"],
                                                (CardType)System.Enum.Parse(typeof(CardType), reader["cardtype"].ToString()),
                                                null,
                                                (Element)System.Enum.Parse(typeof(Element), reader["element"].ToString()),
                                                reader["name"].ToString(),
                                                (int)reader["damage"]));
                        }
                    }
                }

                Disconnect();
                return (tradeList, tradeInfo);
            }

        }

        public void tradeByCard(Card newCard, tradeInfo info, Card oldCard, int playerID)
        {
            Connect();
            //remove tradedeal from DB
            using (var sql = new NpgsqlCommand("DELETE FROM trades WHERE tradeid = @tID", Connection))
            {
                sql.Parameters.AddWithValue("tID", info.tradeId);
                sql.ExecuteNonQuery();
            }

            //remove own card that is being traded away
            using (var sql = new NpgsqlCommand("DELETE FROM playerstack WHERE number IN(SELECT number FROM playerstack WHERE cardid = @cID AND playerid = @pID LIMIT 1)", Connection))
            {
                sql.Parameters.AddWithValue("cID", oldCard.CardID);
                sql.Parameters.AddWithValue("pID", playerID);
                sql.ExecuteNonQuery();
            }

            //add new card to own Stack
            using (var sql = new NpgsqlCommand("INSERT INTO playerstack (playerid, cardid) VALUES (@pID, @cID)", Connection))
            {
                sql.Parameters.AddWithValue("pID", playerID);
                sql.Parameters.AddWithValue("cID", newCard.CardID);
                sql.ExecuteNonQuery();
            }
            Disconnect();
        }

        public void tradeByCoin(Card newCard, tradeInfo info, int playerID)
        {
            Connect();
            //remove tradedeal from DB
            using (var sql = new NpgsqlCommand("DELETE FROM trades WHERE tradeid = @tID", Connection))
            {
                sql.Parameters.AddWithValue("tID", info.tradeId);
                sql.ExecuteNonQuery();
            }

            //remove own coins
            using (var sql = new NpgsqlCommand("UPDATE player SET coins = coins - @c WHERE id = @id", Connection))
            {
                sql.Parameters.AddWithValue("c", info.coinprice);
                sql.Parameters.AddWithValue("id", playerID);
                sql.ExecuteNonQuery();
            }

            //add coins to trade partner
            using (var sql = new NpgsqlCommand("UPDATE player SET coins = coins + @c WHERE id = @id", Connection))
            {
                sql.Parameters.AddWithValue("c", info.coinprice);
                sql.Parameters.AddWithValue("id", info.ownerID);
                sql.ExecuteNonQuery();
            }

            //add new card to own Stack
            using (var sql = new NpgsqlCommand("INSERT INTO playerstack (playerid, cardid) VALUES (@pID, @cID)", Connection))
            {
                sql.Parameters.AddWithValue("pID", playerID);
                sql.Parameters.AddWithValue("cID", newCard.CardID);
                sql.ExecuteNonQuery();
            }
            Disconnect();
        }
    }
}
