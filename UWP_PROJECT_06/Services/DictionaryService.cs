using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using Windows.Storage;

namespace UWP_PROJECT_06.Services
{
    public static class DictionaryService
    {
        static string FileName = "DictionaryDB";

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
                
                // Create Statuses table and fill it

                commandText = "CREATE TABLE IF NOT EXISTS Statuses (Id INTEGER NOT NULL, Status TEXT NOT NULL, PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                commandText = "SELECT Status FROM Statuses";
                sqliteCommand = new SqliteCommand(commandText, conn);
                query = sqliteCommand.ExecuteReader();

                if (!query.HasRows)
                {
                    commandText = "INSERT INTO Statuses(Status) VALUES ('BLUE_RARE'), ('RARE'), ('MEDIUM_RARE'), ('MEDIUM'), ('MEDIUM_WELL'), ('WELL_DONE'), ('DOUBLE_WELL_DONE')";
                    sqliteCommand = new SqliteCommand(commandText, conn);
                    sqliteCommand.ExecuteReader();
                }
                
                // Create PartsOfSpeech table and fill it

                commandText = "CREATE TABLE IF NOT EXISTS PartsOfSpeech (Id INTEGER NOT NULL, PartOfSpeech TEXT NOT NULL, PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                commandText = "SELECT PartOfSpeech FROM PartsOfSpeech";
                sqliteCommand = new SqliteCommand(commandText, conn);
                query = sqliteCommand.ExecuteReader();

                if (!query.HasRows)
                {
                    commandText = "INSERT INTO PartsOfSpeech(PartOfSpeech) VALUES ('существительное_мужского_рода'), ('существительное_женского_рода'), ('существительное_среднего_рода'), ('только_множественное_число'), ('множественное_число_от'), ('глагол'), ('прилагательное'), ('наречие'), ('предлог'), ('числительное'), ('местоимение'), ('союз'), ('частица'), ('междометие'), ('притяжательное_местоимение'), ('определяющее_слово'), ('префикс')";
                    sqliteCommand = new SqliteCommand(commandText, conn);
                    sqliteCommand.ExecuteReader();
                }
                
                // Create LinkTypes table and fill it

                commandText = "CREATE TABLE IF NOT EXISTS LinkTypes (Id INTEGER NOT NULL, LinkType TEXT NOT NULL, PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                commandText = "SELECT LinkType FROM LinkTypes";
                sqliteCommand = new SqliteCommand(commandText, conn);
                query = sqliteCommand.ExecuteReader();

                if (!query.HasRows)
                {
                    commandText = "INSERT INTO LinkTypes(LinkType) VALUES ('Plural'), ('Synonym'), ('Antonym'), ('Figurative Meaning'), ('Meaning string'), ('Meaning'), ('Example'), ('Translation into russian'), ('Translation into german'), ('Translation into english'), ('Translation into spanish'), ('Translation into italian'), ('Translation into french')";
                    sqliteCommand = new SqliteCommand(commandText, conn);
                    sqliteCommand.ExecuteReader();
                }

                // Create Words table and fill it

                commandText = "CREATE TABLE IF NOT EXISTS Words (Id INTEGER NOT NULL, Word TEXT NOT NULL, Language INTEGER NOT NULL, Status INTEGER NOT NULL, PartOfSpeech INTEGER NOT NULL, CreatedOn DATE NOT NULL, LastModifiedOn DATETIME NOT NULL, LastRepeatedOn	DATETIME NOT NULL, CONSTRAINT FK_Words_PartsOfSpeech FOREIGN KEY(PartOfSpeech) REFERENCES PartsOfSpeech(Id), CONSTRAINT FK_Words_Statuses FOREIGN KEY(Status) REFERENCES Statuses(Id), CONSTRAINT FK_Words_Languages FOREIGN KEY(Language) REFERENCES Languages(Id), PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                // Create WordsExtras table and fill it

                commandText = "CREATE TABLE IF NOT EXISTS WordsExtras (RowId INTEGER NOT NULL, WordId INTEGER NOT NULL, LinkedWordId INTEGER NOT NULL, LinkType INTEGER NOT NULL, ExtraText TEXT NOT NULL, CONSTRAINT FK_Words_WordsExtra1 FOREIGN KEY(WordId) REFERENCES Words(Id), CONSTRAINT FK_Words_WordsExtra2 FOREIGN KEY(LinkedWordId) REFERENCES Words(Id), CONSTRAINT FK_Words_WordsExtra3 FOREIGN KEY(LinkType) REFERENCES LinkTypes(Id),  PRIMARY KEY(RowId AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }

        #region Words
        
        public static void CreateWord(Word word)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "INSERT INTO Words VALUES (NULL, @Word, @Language, @Status, @PartOfSpeech, @CreatedOn, @LastModifiedOn, @LastRepeatedOn);";
                sqliteCommand.Parameters.AddWithValue("@Word", word.Word1);
                sqliteCommand.Parameters.AddWithValue("@Language", word.Language);
                sqliteCommand.Parameters.AddWithValue("@Status", word.Status);
                sqliteCommand.Parameters.AddWithValue("@PartOfSpeech", word.PartOfSpeech);
                sqliteCommand.Parameters.AddWithValue("@CreatedOn", word.CreatedOn);
                sqliteCommand.Parameters.AddWithValue("@LastModifiedOn", word.LastModifiedOn);
                sqliteCommand.Parameters.AddWithValue("@LastRepeatedOn", word.LastRepeatedOn);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static Word ReadWord(int id)
        {
            Word word = new Word();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, Word, Language, Status, PartOfSpeech, CreatedOn, LastModifiedOn, LastRepeatedOn FROM Words WHERE Id = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    word = new Word()
                    {
                        Id = query.GetInt32(0),
                        Word1 = query.GetString(1),
                        Language = query.GetInt32(2),
                        Status = query.GetInt32(3),
                        PartOfSpeech = query.GetInt32(4),
                        CreatedOn = query.GetDateTime(5),
                        LastModifiedOn = query.GetDateTime(6),
                        LastRepeatedOn = query.GetDateTime(7),
                    };
                }

                conn.Close();
            }

            return word;
        }
        public static Word ReadWord(string word1)
        {
            Word word = new Word();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, Word, Language, Status, PartOfSpeech, CreatedOn, LastModifiedOn, LastRepeatedOn FROM Words WHERE Word = '{word1}';";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    word = new Word()
                    {
                        Id = query.GetInt32(0),
                        Word1 = query.GetString(1),
                        Language = query.GetInt32(2),
                        Status = query.GetInt32(3),
                        PartOfSpeech = query.GetInt32(4),
                        CreatedOn = query.GetDateTime(5),
                        LastModifiedOn = query.GetDateTime(6),
                        LastRepeatedOn = query.GetDateTime(7),
                    };
                }

                conn.Close();
            }

            return word;
        }

        public static List<Word> ReadWords()
        {
            List<Word> words = new List<Word>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, Word, Language, Status, PartOfSpeech, CreatedOn, LastModifiedOn, LastRepeatedOn FROM Words;";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    words.Add(new Word()
                    {
                        Id = query.GetInt32(0),
                        Word1 = query.GetString(1),
                        Language = query.GetInt32(2),
                        Status = query.GetInt32(3),
                        PartOfSpeech = query.GetInt32(4),
                        CreatedOn = query.GetDateTime(5),
                        LastModifiedOn = query.GetDateTime(6),
                        LastRepeatedOn = query.GetDateTime(7),
                    });
                }

                conn.Close();
            }

            return words;
        }
        public static void UpdateWord(Word word)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "UPDATE Words SET Word = @Word, Language = @Language, Status = @Status, PartOfSpeech = @PartOfSpeech, CreatedOn = @CreatedOn, LastModifiedOn = @LastModifiedOn, LastRepeatedOn = @LastRepeatedOn WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", word.Id);
                sqliteCommand.Parameters.AddWithValue("@Word", word.Word1);
                sqliteCommand.Parameters.AddWithValue("@Language", word.Language);
                sqliteCommand.Parameters.AddWithValue("@Status", word.Status);
                sqliteCommand.Parameters.AddWithValue("@PartOfSpeech", word.PartOfSpeech);
                sqliteCommand.Parameters.AddWithValue("@CreatedOn", word.CreatedOn);
                sqliteCommand.Parameters.AddWithValue("@LastModifiedOn", word.LastModifiedOn);
                sqliteCommand.Parameters.AddWithValue("@LastRepeatedOn", word.LastRepeatedOn);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static void DeleteWord(int id)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "DELETE FROM WordsExtras WHERE WordId = @Id; DELETE FROM Words WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", id);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }

        #endregion

        #region WordsExtras

        public static void CreateWordExtra(WordExtra wordExtra)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "INSERT INTO WordsExtras VALUES (NULL, @WordId, @LinkedWordId, @LinkType, @ExtraText);";
                sqliteCommand.Parameters.AddWithValue("@WordId", wordExtra.WordId);
                sqliteCommand.Parameters.AddWithValue("@LinkedWordId", wordExtra.LinkedWordId);
                sqliteCommand.Parameters.AddWithValue("@LinkType", wordExtra.LinkType);
                sqliteCommand.Parameters.AddWithValue("@ExtraText", wordExtra.ExtraText);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static WordExtra ReadWordExtra(int rowId)
        {
            WordExtra wordExtra = new WordExtra();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT RowId, WordId, LinkedWordId, LinkType, ExtraText FROM WordsExtras WHERE RowId = {rowId};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    wordExtra = new WordExtra()
                    {
                        RowID = query.GetInt32(0),
                        WordId = query.GetInt32(1),
                        LinkedWordId = query.GetInt32(2),
                        LinkType = query.GetInt32(3),
                        ExtraText = query.GetString(4),
                    };
                }

                conn.Close();
            }

            return wordExtra;
        }
        public static List<WordExtra> ReadWordExtras(int wordId)
        {
            List<WordExtra> wordExtras = new List<WordExtra>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT RowId, WordId, LinkedWordId, LinkType, ExtraText FROM WordsExtras WHERE WordId = {wordId};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    wordExtras.Add(new WordExtra()
                    {
                        RowID = query.GetInt32(0),
                        WordId = query.GetInt32(1),
                        LinkedWordId = query.GetInt32(2),
                        LinkType = query.GetInt32(3),
                        ExtraText = query.GetString(4),
                    });
                }

                conn.Close();
            }

            return wordExtras;
        }
        public static void UpdateWordExtra(WordExtra wordExtra)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "UPDATE WordsExtras SET WordId = @WordId, LinkedWordId = @LinkedWordId, LinkType = @LinkType, ExtraText = @ExtraText WHERE RowId = @RowId;";
                sqliteCommand.Parameters.AddWithValue("@RowId", wordExtra.RowID);
                sqliteCommand.Parameters.AddWithValue("@WordId", wordExtra.WordId);
                sqliteCommand.Parameters.AddWithValue("@LinkedWordId", wordExtra.LinkedWordId);
                sqliteCommand.Parameters.AddWithValue("@LinkType", wordExtra.LinkType);
                sqliteCommand.Parameters.AddWithValue("@ExtraText", wordExtra.ExtraText);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static void DeleteWordExtra(int rowId)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "DELETE FROM WordsExtras WHERE RowId = @RowId;";
                sqliteCommand.Parameters.AddWithValue("@RowId", rowId);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }

        #endregion

        #region Languages
        public static string ReadLanguage(int id)
        {
            string language = "";

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Language FROM Languages WHERE Id = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                    language = query.GetString(0);
                
                conn.Close();
            }

            return language;
        }
        public static int ReadLanguageId(string language)
        {
            int id = 1;

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id FROM Languages WHERE Language = {language};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                    id = query.GetInt32(0);

                conn.Close();
            }

            return id;
        }
        public static List<string> ReadLanguages()
        {
            List<string> languages = new List<string>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Language FROM Languages;";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                    languages.Add(query.GetString(0));

                conn.Close();
            }

            return languages;
        }


        #endregion

        #region Statuses

        public static string ReadStatus(int id)
        {
            string status = "";

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Status FROM Statuses WHERE Id = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                    status = query.GetString(0);

                conn.Close();
            }

            return status;
        }

        #endregion

        #region Statuses

        public static string ReadPartOfSpeech(int id)
        {
            string partOfSpeech = "";

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT PartOfSpeech FROM PartsOfSpeech WHERE Id = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                    partOfSpeech = query.GetString(0);

                conn.Close();
            }

            return partOfSpeech;
        }

        #endregion

    }
}
