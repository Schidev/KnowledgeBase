using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.Notes;
using Windows.Storage;

namespace UWP_PROJECT_06.Services
{
    public static class NotesService
    {
        static string FileName = "NotesDB";

        public async static void InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);

            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand;
                string commandText;

                // Create States table and fill it

                commandText = "CREATE TABLE IF NOT EXISTS States (Id INTEGER NOT NULL, State TEXT NOT NULL, PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                commandText = "SELECT State FROM States";
                sqliteCommand = new SqliteCommand(commandText, conn);
                SqliteDataReader query = sqliteCommand.ExecuteReader();

                if (!query.HasRows)
                {
                    commandText = "INSERT INTO States (State) VALUES ('Do'), ('Doing'), ('Done')";
                    sqliteCommand = new SqliteCommand(commandText, conn);
                    sqliteCommand.ExecuteReader();
                }

                // Create Themes table and fill it

                commandText = "CREATE TABLE IF NOT EXISTS Themes (Id INTEGER NOT NULL, Theme TEXT NOT NULL, PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                query = sqliteCommand.ExecuteReader();

                if (!query.HasRows)
                {
                    commandText = "INSERT INTO Themes (Theme) VALUES ('Entertainment'), ('Music'), ('Movie'), ('Education'), ('Sport'), ('Food'), ('Development')";
                    sqliteCommand = new SqliteCommand(commandText, conn);
                    sqliteCommand.ExecuteReader();
                }

                // Create Source Types table and fill it

                commandText = "CREATE TABLE IF NOT EXISTS SourceTypes ( Id INTEGER NOT NULL, SourceType TEXT NOT NULL, PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                query = sqliteCommand.ExecuteReader();

                if (!query.HasRows)
                {
                    commandText = "INSERT INTO SourceTypes (SourceType) VALUES ('VIDEO'), ('SOUND'), ('IMAGE'), ('DOCUMENT')";
                    sqliteCommand = new SqliteCommand(commandText, conn);
                    sqliteCommand.ExecuteReader();
                }
                // Create Sources table

                commandText = "CREATE TABLE IF NOT EXISTS Sources (Id INTEGER NOT NULL, SourceName TEXT NOT NULL, Duration INTEGER NOT NULL, ActualTime INTEGER NOT NULL, State TINYINT NOT NULL,  Theme TINYINT NOT NULL, SourceType TINYINT NOT NULL, IsDownloaded BOOLEAN NOT NULL, Description TEXT NOT NULL, SourceLink TEXT NOT NULL, FOREIGN KEY(Theme) REFERENCES Themes (Id), FOREIGN KEY(SourceType) REFERENCES SourceTypes (Id), FOREIGN KEY(State) REFERENCES States (Id), PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                query = sqliteCommand.ExecuteReader();

                if (!query.HasRows)
                {
                    commandText = "INSERT INTO Sources (SourceName, Duration, ActualTime, State, Theme, SourceType, IsDownloaded, Description, SourceLink) VALUES ('SOURCE_UNKNOWN', '0', '0', '1', '1', '1', '0', 'If you have quote but do not know from where, leave it here', 'SOURCE_UNKNOWN')";
                    sqliteCommand = new SqliteCommand(commandText, conn);
                    sqliteCommand.ExecuteReader();
                }

                // Create Notes table

                commandText = "CREATE TABLE IF NOT EXISTS Notes ( Id INTEGER NOT NULL, SourceID INTEGER NOT NULL, Stamp TEXT NOT NULL, Title TEXT NOT NULL, Note TEXT NOT NULL, FOREIGN KEY(SourceID) REFERENCES Sources (Id), PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                // Create SourceExtras table

                commandText = "CREATE TABLE IF NOT EXISTS SourcesExtras (Id INTEGER NOT NULL, SourceID INTEGER NOT NULL, Key TEXT NOT NULL, Value TEXT NOT NULL, PRIMARY KEY(Id AUTOINCREMENT), FOREIGN KEY(SourceID) REFERENCES Sources);";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                // Create Quotes table

                commandText = "CREATE TABLE IF NOT EXISTS Quotes (Id INTEGER NOT NULL, SourceID INTEGER NOT NULL, QuoteBegin TEXT NOT NULL, QuoteEnd TEXT NOT NULL, OriginalQuote TEXT NOT NULL, TranslatedQuote TEXT NOT NULL, FOREIGN KEY(SourceID) REFERENCES Sources(Id), PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }


        #region Sources CRUD

        public static void CreateSource(Source source)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "INSERT INTO Sources VALUES (NULL, @SourceName, @Duration, @ActualTime, @State, @Theme, @SourceType, @IsDownloaded, @Description, @SourceLink);";
                
                sqliteCommand.Parameters.AddWithValue("@SourceName", source.SourceName);
                sqliteCommand.Parameters.AddWithValue("@Duration", source.Duration);
                sqliteCommand.Parameters.AddWithValue("@ActualTime", source.ActualTime);
                sqliteCommand.Parameters.AddWithValue("@State", source.State);
                sqliteCommand.Parameters.AddWithValue("@Theme", source.Theme);
                sqliteCommand.Parameters.AddWithValue("@SourceType", source.SourceType);
                sqliteCommand.Parameters.AddWithValue("@IsDownloaded", source.IsDownloaded);
                sqliteCommand.Parameters.AddWithValue("@Description", source.Description);
                sqliteCommand.Parameters.AddWithValue("@SourceLink", source.SourceLink);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static Source ReadSource(int id)
        {
            Source source = new Source();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, SourceName, Duration, ActualTime, State, Theme, SourceType, IsDownloaded, Description, SourceLink FROM Sources WHERE Id = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    source = new Source()
                    {
                        Id = query.GetInt32(0),
                        SourceName = query.GetString(1),
                        Duration = query.GetInt32(2),
                        ActualTime = query.GetInt32(3),
                        State = query.GetByte(4),
                        Theme = query.GetByte(5),
                        SourceType = query.GetByte(6),
                        IsDownloaded = query.GetBoolean(7),
                        Description = query.GetString(8),
                        SourceLink = query.GetString(9),
                    };
                }

                conn.Close();
            }

            return source;
        }
        public static List<Source> ReadSources()
        {
            List<Source> sources = new List<Source>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, SourceName, Duration, ActualTime, State, Theme, SourceType, IsDownloaded, Description, SourceLink FROM Sources;";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    sources.Add(new Source()
                    {
                        Id = query.GetInt32(0),
                        SourceName = query.GetString(1),
                        Duration = query.GetInt32(2),
                        ActualTime = query.GetInt32(3),
                        State = query.GetByte(4),
                        Theme = query.GetByte(5),
                        SourceType = query.GetByte(6),
                        IsDownloaded = query.GetBoolean(7),
                        Description = query.GetString(8),
                        SourceLink = query.GetString(9),
                    });
                }

                conn.Close();
            }

            return sources;
        }
        public static void UpdateSource(Source source)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "UPDATE Sources SET SourceName = @SourceName, Duration = @Duration, ActualTime = @ActualTime, State = @State, Theme = @Theme, SourceType = @SourceType, IsDownloaded = @IsDownloaded, Description = @Description, SourceLink = @SourceLink WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", source.Id);
                sqliteCommand.Parameters.AddWithValue("@SourceName", source.SourceName);
                sqliteCommand.Parameters.AddWithValue("@Duration", source.Duration);
                sqliteCommand.Parameters.AddWithValue("@ActualTime", source.ActualTime);
                sqliteCommand.Parameters.AddWithValue("@State", source.State);
                sqliteCommand.Parameters.AddWithValue("@Theme", source.Theme);
                sqliteCommand.Parameters.AddWithValue("@SourceType", source.SourceType);
                sqliteCommand.Parameters.AddWithValue("@IsDownloaded", source.IsDownloaded);
                sqliteCommand.Parameters.AddWithValue("@Description", source.Description);
                sqliteCommand.Parameters.AddWithValue("@SourceLink", source.SourceLink);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static void DeleteSource(int id)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "UPDATE SourceExtras SET SourceID = 1 WHERE SourceID = @Id;UPDATE Notes SET SourceID = 1 WHERE SourceID = @Id; UPDATE Quotes SET SourceID = 1 WHERE SourceID = @Id; DELETE FROM Sources WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", id);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }

        #endregion

        #region SourcesExtras

        public static void CreateSourceExtra(SourceExtra extra)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "INSERT INTO SourcesExtras VALUES (NULL, @SourceID, @Key, @Value);";

                sqliteCommand.Parameters.AddWithValue("@SourceID", extra.SourceID);
                sqliteCommand.Parameters.AddWithValue("@Key", extra.Key);
                sqliteCommand.Parameters.AddWithValue("@Value", extra.Value);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static SourceExtra ReadSourceExtra(int id)
        {
            SourceExtra extra = new SourceExtra();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, SourceID, Key, Value FROM SourcesExtras WHERE Id = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    extra = new SourceExtra()
                    {
                        Id = query.GetInt32(0),
                        SourceID = query.GetInt32(1),
                        Key = query.GetString(2),
                        Value = query.GetString(3)
                    };
                }

                conn.Close();
            }

            return extra;
        }
        public static List<SourceExtra> ReadSourceExtras(int id)
        {
            List<SourceExtra> extras = new List<SourceExtra>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, SourceID, Key, Value FROM SourcesExtras WHERE SourceID = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    extras.Add(new SourceExtra()
                    {
                        Id = query.GetInt32(0),
                        SourceID = query.GetInt32(1),
                        Key = query.GetString(2),
                        Value = query.GetString(3)
                    });
                }

                conn.Close();
            }

            return extras;
        }
        public static void UpdateSourceExtra(SourceExtra extra)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "UPDATE SourcesExtras SET SourceID = @SourceID, Key = @Key, Value = @Value WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", extra.Id);
                sqliteCommand.Parameters.AddWithValue("@SourceID", extra.SourceID);
                sqliteCommand.Parameters.AddWithValue("@Key", extra.Key);
                sqliteCommand.Parameters.AddWithValue("@Value", extra.Value);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static void DeleteSourceExtra(int id)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "DELETE FROM SourceExtras WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", id);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }

        #endregion

    }
}
