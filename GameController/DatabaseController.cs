using Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankWars;

namespace GameController
{
    public class DatabaseController
    {
        /// <summary>
        /// The connection string.
        /// </summary>
        public const string connectionString = "server=atr.eng.utah.edu;" + "database=cs3500_u1167015;" + "uid=cs3500_u1167015;" + "password=entersandman";

        /// <summary>
        /// Default constructor for the Database controller
        /// </summary>
        public DatabaseController()
        {
        }

        /// <summary>
        /// Method used when an Invalid URL is used 
        /// </summary>
        /// <returns></returns>
        public string errorMessage()
        {
            return WebViews.Get404();
        }

        /// <summary>
        /// Used to send home page html string
        /// </summary>
        /// <returns></returns>
        public string sendToHomePage()
        {
            return WebViews.GetHomePage();
        }

        /// <summary>
        /// Creates the HTML string that will be used to get the table for any individual
        /// player
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <returns></returns>
        public string individualTableCreator(String name)
        {
            // Connect to the DB
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "Select PlayerID, Game, Score, Accuracy From Players";

                    //Lists used to hold information about the players
                    List<SessionModel> games = new List<SessionModel>();
                    List<uint> currGameIDs = new List<uint>();
                    List<uint> currGameDurs = new List<uint>();
                    List<uint> currGameScore = new List<uint>();
                    List<uint> currGameAccuracy = new List<uint>();


                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //Add all the player data to its aplicable table
                            if (reader["PlayerID"].Equals(name))
                            {
                                currGameIDs.Add(uint.Parse(reader["Game"].ToString()));
                                currGameScore.Add(uint.Parse(reader["Score"].ToString()));
                                currGameAccuracy.Add(uint.Parse(reader["Accuracy"].ToString()));

                            }
                        }
                    }
                    //If the player hasn't played any games, the url is invalid
                    if (currGameIDs.Count == 0)
                    {
                        return errorMessage();
                    }
                    //Get the duration for each game the player has played
                    foreach (uint i in currGameIDs)
                    {
                        command.CommandText = "Select Duration From Games Where GameID=" + "'" + i + "'" + "";
                        currGameDurs.Add(uint.Parse(command.ExecuteScalar().ToString()));
                    }
                    //Turn each game the player has played into a session model
                    //Add those to the games list
                    for(int i = 0; i < currGameIDs.Count; i++)
                    {
                        SessionModel newGame = new SessionModel(currGameIDs[i], currGameDurs[i], currGameScore[i], currGameAccuracy[i]);
                        games.Add(newGame);
                    }
                    
                    return WebViews.GetPlayerGames(name, games);
                }
                catch (Exception e)
                {
                    return "Error: " + e.Message;
                }
            }
        }

        /// <summary>
        /// Creates the HTML string that will be used to get the tables for all games
        /// </summary>
        /// <returns></returns>
        public string allGamesTableCreator()
        {
            // Connect to the DB
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "Select GameID, Duration From Games";

                    //Dictionary used to hold all games that have been played
                    Dictionary<uint, GameModel> games = new Dictionary<uint, GameModel>();

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        //Create a gameModel out of the current game and add it to the games dictionary
                        while (reader.Read())
                        {
                            uint currGameID = uint.Parse(reader["GameID"].ToString());
                            GameModel currentGame = new GameModel(currGameID, uint.Parse(reader["Duration"].ToString()));
                            games.Add(currGameID, currentGame);
                        }
                    }

                    command.CommandText = "Select PlayerID, Game, Score, Accuracy From Players";

                    // Execute the command and cycle through the DataReader object
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        //Add every player to the game that they played
                        while (reader.Read())
                        {
                            String playerID = reader["PlayerID"].ToString();
                            uint score = uint.Parse(reader["Score"].ToString());
                            uint accuracy = uint.Parse(reader["Accuracy"].ToString());

                            PlayerModel newPlayer = new PlayerModel(playerID, score, accuracy);
                            games[uint.Parse(reader["Game"].ToString())].AddPlayer(playerID, score, accuracy);
                        }
                    }

                    return WebViews.GetAllGames(games);
                }
                catch (Exception e)
                {
                    return "Error: " + e.Message;
                }
            }
        }

        /// <summary>
        /// Sends the game that was just played to the database to be stored
        /// </summary>
        /// <param name="tankList">Every tank that was in the game</param>
        /// <param name="duration">Length of the game in milliseconds</param>
        /// <returns></returns>
        public string sendGameToDatabase(Dictionary<int, Tank> tankList, Stopwatch duration)
        {
            // Connect to the DB
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    //Calculate the time from milliseconds into seconds
                    int intTime = (int)duration.ElapsedMilliseconds / 1000;

                    //Insert the duration of the game into the games database
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = "INSERT INTO Games (Duration) VALUES(" + intTime + ")";
                    command.ExecuteNonQuery();

                    //Get the gameID
                    command.CommandText = "SELECT LAST_INSERT_ID()";
                    String ID = command.ExecuteScalar().ToString();

                    //Calculate the accuracy of every tank, then add that tank into players
                    foreach (Tank t in tankList.Values)
                    {
                        String accuracy;

                        if (t.tankTotalShots == 0)
                        {
                            accuracy = "0";
                        }
                        else
                        {
                            accuracy = ((((double)t.tankShotsHit / (double)t.tankTotalShots) * 100)).ToString();
                        }
                        command = conn.CreateCommand();
                        command.CommandText = "INSERT INTO Players (PlayerID, Score, Game, Accuracy) VALUES(" + "'" + t.name + "'" + "," + t.score + "," + ID + "," + accuracy + ")";

                        command.ExecuteNonQuery();
                    }
                    Console.WriteLine("Successfully Written to Database");
                }
                catch (Exception e)
                {
                    return "Error: " + e.Message;
                }
            }
            return "Success";
        }
    }
}
