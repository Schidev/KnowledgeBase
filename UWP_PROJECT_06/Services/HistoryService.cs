using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.History;
using Windows.Storage;

namespace UWP_PROJECT_06.Services
{
    public static class HistoryService
    {
        static string FileName = "HistoryDB";

        public async static void InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand;
                SqliteDataReader query;
                string commandText;

                // Create Languages table and fill it

                commandText = "CREATE TABLE IF NOT EXISTS Languages ( Id INTEGER NOT NULL, Language TEXT NOT NULL, PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                commandText = "SELECT Language FROM Languages";
                sqliteCommand = new SqliteCommand(commandText, conn);
                query = sqliteCommand.ExecuteReader();

                if (!query.HasRows)
                {
                    commandText = "INSERT INTO Languages (Language) VALUES ('Русский'), ('Deutsch'), ('English'), ('Français'), ('Italiano'), ('Español')";
                    sqliteCommand = new SqliteCommand(commandText, conn);
                    sqliteCommand.ExecuteReader();
                }

                // Create Words table and fill it

                commandText = "CREATE TABLE IF NOT EXISTS UnknownWords (Id INTEGER NOT NULL, Word TEXT NOT NULL, Language INTEGER NOT NULL, LastModifiedOn DATETIME NOT NULL, CONSTRAINT FK_Words_Languages FOREIGN KEY(Language) REFERENCES Languages(Id), PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }


        #region UnknownWords

        public static void CreateUnknownWord(UnknownWord word)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "INSERT INTO UnknownWords VALUES (NULL, @Word, @Language, @LastModifiedOn);";
                sqliteCommand.Parameters.AddWithValue("@Word", word.Word);
                sqliteCommand.Parameters.AddWithValue("@Language", word.Language);
                sqliteCommand.Parameters.AddWithValue("@LastModifiedOn", word.LastModifiedOn);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static UnknownWord ReadUnknownWord(int id)
        {
            UnknownWord word = new UnknownWord();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, Word, Language, LastModifiedOn FROM UnknownWords WHERE Id = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    word = new UnknownWord()
                    {
                        Id = query.GetInt32(0),
                        Word = query.GetString(1),
                        Language = query.GetInt32(2),
                        LastModifiedOn = query.GetDateTime(3)
                    };
                }

                conn.Close();
            }

            return word;
        }
        public static List<UnknownWord> ReadUnknownWords()
        {
            List<UnknownWord> words = new List<UnknownWord>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, Word, Language, LastModifiedOn FROM UnknownWords;";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    words.Add(new UnknownWord()
                    {
                        Id = query.GetInt32(0),
                        Word = query.GetString(1),
                        Language = query.GetInt32(2),
                        LastModifiedOn = query.GetDateTime(3)
                    });
                }

                conn.Close();
            }

            return words;
        }
        public static void UpdateUnknownWord(UnknownWord word)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "UPDATE UnknownWords SET Word = @Word, Language = @Language, LastModifiedOn = @LastModifiedOn WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", word.Id);
                sqliteCommand.Parameters.AddWithValue("@Word", word.Word);
                sqliteCommand.Parameters.AddWithValue("@Language", word.Language);
                sqliteCommand.Parameters.AddWithValue("@LastModifiedOn", word.LastModifiedOn);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static void DeleteUnknownWord(int id)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "DELETE FROM UnknownWords WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", id);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }

        #endregion


    }
}
