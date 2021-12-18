using MonsterTradingCardGamePP.Cards;
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

            Connect();
            using (var sql = new NpgsqlCommand("INSERT INTO player (username, password, authtoken, coins, elo) VALUES (@user, @pass, @auth, @coin, @elo)", Connection))
            {
                sql.Parameters.AddWithValue("user", username);
                sql.Parameters.AddWithValue("pass", password);
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
            Connect();
            using (var sql = new NpgsqlCommand("SELECT * FROM player WHERE username = @user AND password = @pass", Connection))
            {
                sql.Parameters.AddWithValue("user", username);
                sql.Parameters.AddWithValue("pass", password);
                NpgsqlDataReader reader = sql.ExecuteReader();

                Player tempPlayer = null;

                if (reader.HasRows)
                {
                    reader.Read();
                    tempPlayer = new Player(reader["username"].ToString(), reader["authtoken"].ToString(), (int)reader["coins"], (int)reader["elo"]);
                }

                Disconnect();
                return tempPlayer;
            }
        }        

    }
}
