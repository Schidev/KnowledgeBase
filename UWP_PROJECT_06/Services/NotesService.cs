using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
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

        public async static Task InitializeDatabase()
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
                sqliteCommand.ExecuteReader();
                
                commandText = "SELECT Theme FROM Themes";
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
                sqliteCommand.ExecuteReader();
                
                commandText = "SELECT SourceType FROM SourceTypes";
                sqliteCommand = new SqliteCommand(commandText, conn);
                query = sqliteCommand.ExecuteReader();

                if (!query.HasRows)
                {
                    commandText = "INSERT INTO SourceTypes (SourceType) VALUES ('UNKNOWN'), ('VIDEO'), ('SOUND'), ('IMAGE'), ('DOCUMENT')";
                    sqliteCommand = new SqliteCommand(commandText, conn);
                    sqliteCommand.ExecuteReader();
                }
                // Create Sources table

                commandText = "CREATE TABLE IF NOT EXISTS Sources (Id INTEGER NOT NULL, SourceName TEXT NOT NULL, Duration INTEGER NOT NULL, ActualTime INTEGER NOT NULL, State TINYINT NOT NULL,  Theme TINYINT NOT NULL, SourceType TINYINT NOT NULL, IsDownloaded BOOLEAN NOT NULL, Description TEXT NOT NULL, SourceLink TEXT NOT NULL, CreatedOn DATETIME NOT NULL, LastModifiedOn DATETIME NOT NULL, PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                commandText = "SELECT SourceName FROM Sources";
                sqliteCommand = new SqliteCommand(commandText, conn);
                query = sqliteCommand.ExecuteReader();

                if (!query.HasRows)
                {
                    commandText = "INSERT INTO Sources (SourceName, Duration, ActualTime, State, Theme, SourceType, IsDownloaded, Description, SourceLink, CreatedOn, LastModifiedOn) VALUES ('UNKNOWN_SOURCE_UNKNOWN', '0', '0', '1', '1', '1', '0', 'If you have quote but do not know from where, leave it here', 'SOURCE_UNKNOWN', DATETIME('now'), DATETIME('now'))";
                    sqliteCommand = new SqliteCommand(commandText, conn);
                    sqliteCommand.ExecuteReader();
                }

                // Create Notes table

                commandText = "CREATE TABLE IF NOT EXISTS Notes ( Id INTEGER NOT NULL, SourceID INTEGER NOT NULL, Stamp TEXT NOT NULL, Title TEXT NOT NULL, Note TEXT NOT NULL, CreatedOn DATETIME NOT NULL, LastModifiedOn DATETIME NOT NULL, FOREIGN KEY(SourceID) REFERENCES Sources (Id), PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                // Create SourceExtras table

                commandText = "CREATE TABLE IF NOT EXISTS SourcesExtras (Id INTEGER NOT NULL, SourceID INTEGER NOT NULL, Key TEXT NOT NULL, Value TEXT NOT NULL, PRIMARY KEY(Id AUTOINCREMENT), FOREIGN KEY(SourceID) REFERENCES Sources);";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                // Create Quotes table

                commandText = "CREATE TABLE IF NOT EXISTS Quotes (Id INTEGER NOT NULL, SourceID INTEGER NOT NULL, QuoteBegin TEXT NOT NULL, QuoteEnd TEXT NOT NULL, OriginalQuote TEXT NOT NULL, TranslatedQuote TEXT NOT NULL, CreatedOn DATETIME NOT NULL, LastModifiedOn DATETIME NOT NULL, FOREIGN KEY(SourceID) REFERENCES Sources(Id), PRIMARY KEY(Id AUTOINCREMENT));";
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

                sqliteCommand.CommandText = "INSERT INTO Sources VALUES (NULL, @SourceName, @Duration, @ActualTime, @State, @Theme, @SourceType, @IsDownloaded, @Description, @SourceLink, @CreatedOn, @LastModifiedOn);";
                
                sqliteCommand.Parameters.AddWithValue("@SourceName", source.SourceName);
                sqliteCommand.Parameters.AddWithValue("@Duration", source.Duration);
                sqliteCommand.Parameters.AddWithValue("@ActualTime", source.ActualTime);
                sqliteCommand.Parameters.AddWithValue("@State", source.State);
                sqliteCommand.Parameters.AddWithValue("@Theme", source.Theme);
                sqliteCommand.Parameters.AddWithValue("@SourceType", source.SourceType);
                sqliteCommand.Parameters.AddWithValue("@IsDownloaded", source.IsDownloaded);
                sqliteCommand.Parameters.AddWithValue("@Description", source.Description);
                sqliteCommand.Parameters.AddWithValue("@SourceLink", source.SourceLink);
                sqliteCommand.Parameters.AddWithValue("@CreatedOn", source.CreatedOn);
                sqliteCommand.Parameters.AddWithValue("@LastModifiedOn", source.LastModifiedOn);

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

                string commandText = $"SELECT Id, SourceName, Duration, ActualTime, State, Theme, SourceType, IsDownloaded, Description, SourceLink, CreatedOn, LastModifiedOn FROM Sources WHERE Id = {id};";
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
                        CreatedOn = query.GetDateTime(10),
                        LastModifiedOn = query.GetDateTime(11)
                    };
                }

                conn.Close();
            }

            return source;
        }
        public static Source ReadSource(string name)
        {
            Source source = new Source();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, SourceName, Duration, ActualTime, State, Theme, SourceType, IsDownloaded, Description, SourceLink, CreatedOn, LastModifiedOn FROM Sources WHERE SourceName = \"{name}\";";
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
                        CreatedOn = query.GetDateTime(10),
                        LastModifiedOn = query.GetDateTime(11)
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

                string commandText = $"SELECT Id, SourceName, Duration, ActualTime, State, Theme, SourceType, IsDownloaded, Description, SourceLink, CreatedOn, LastModifiedOn FROM Sources;";
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
                        CreatedOn = query.GetDateTime(10),
                        LastModifiedOn = query.GetDateTime(11)
                    });
                }

                conn.Close();
            }

            return sources;
        }
        public static List<Source> ReadSources(byte sourceTypeId)
        {
            List<Source> sources = new List<Source>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, SourceName, Duration, ActualTime, State, Theme, SourceType, IsDownloaded, Description, SourceLink, CreatedOn, LastModifiedOn FROM Sources WHERE SourceType = {sourceTypeId};";
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
                        CreatedOn = query.GetDateTime(10),
                        LastModifiedOn = query.GetDateTime(11)
                    });
                }

                conn.Close();
            }

            return sources;
        }
        public static void UpdateSource(Source source)
        {
            if (source.Id == 1)
                return;

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "UPDATE Sources SET SourceName = @SourceName, Duration = @Duration, ActualTime = @ActualTime, State = @State, Theme = @Theme, SourceType = @SourceType, IsDownloaded = @IsDownloaded, Description = @Description, SourceLink = @SourceLink, CreatedOn = @CreatedOn, LastModifiedOn = @LastModifiedOn WHERE Id = @Id;";
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
                sqliteCommand.Parameters.AddWithValue("@CreatedOn", source.CreatedOn);
                sqliteCommand.Parameters.AddWithValue("@LastModifiedOn", source.LastModifiedOn);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static void DeleteSource(int id)
        {
            if (id == 1)
                return;

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "UPDATE SourcesExtras SET SourceID = 1 WHERE SourceID = @Id; UPDATE Notes SET SourceID = 1 WHERE SourceID = @Id; UPDATE Quotes SET SourceID = 1 WHERE SourceID = @Id; DELETE FROM Sources WHERE Id = @Id;";
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

                sqliteCommand.CommandText = "DELETE FROM SourcesExtras WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", id);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }

        #endregion

        #region Notes

        public static void CreateNote(Note note)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "INSERT INTO Notes VALUES (NULL, @SourceID, @Stamp, @Title, @Note, @CreatedOn, @LastModifiedOn);";
                sqliteCommand.Parameters.AddWithValue("@SourceID", note.SourceID);
                sqliteCommand.Parameters.AddWithValue("@Stamp", note.Stamp);
                sqliteCommand.Parameters.AddWithValue("@Title", note.Title);
                sqliteCommand.Parameters.AddWithValue("@Note", note.Note1);
                sqliteCommand.Parameters.AddWithValue("@CreatedOn", note.CreatedOn);
                sqliteCommand.Parameters.AddWithValue("@LastModifiedOn", note.LastModifiedOn);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static Note ReadNote(int id)
        {
            Note note = new Note();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, SourceID, Stamp, Title, Note, CreatedOn, LastModifiedOn FROM Notes WHERE Id = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    note = new Note()
                    {
                        Id = query.GetInt32(0),
                        SourceID = query.GetInt32(1),
                        Stamp = query.GetString(2),
                        Title = query.GetString(3),
                        Note1 = query.GetString(4),
                        CreatedOn = query.GetDateTime(5),
                        LastModifiedOn = query.GetDateTime(6)
                    };
                }

                conn.Close();
            }

            return note;
        }
        public static List<Note> ReadNotes()
        {
            List<Note> notes = new List<Note>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, SourceID, Stamp, Title, Note, CreatedOn, LastModifiedOn FROM Notes;";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    notes.Add(new Note()
                    {
                        Id = query.GetInt32(0),
                        SourceID = query.GetInt32(1),
                        Stamp = query.GetString(2),
                        Title = query.GetString(3),
                        Note1 = query.GetString(4),
                        CreatedOn = query.GetDateTime(5),
                        LastModifiedOn = query.GetDateTime(6)
                    });
                }

                conn.Close();
            }

            return notes;
        }
        public static List<Note> ReadNotes(int id)
        {
            List<Note> notes = new List<Note>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, SourceID, Stamp, Title, Note, CreatedOn, LastModifiedOn FROM Notes WHERE SourceID = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    notes.Add(new Note()
                    {
                        Id = query.GetInt32(0),
                        SourceID = query.GetInt32(1),
                        Stamp = query.GetString(2),
                        Title = query.GetString(3),
                        Note1 = query.GetString(4),
                        CreatedOn = query.GetDateTime(5),
                        LastModifiedOn = query.GetDateTime(6)
                    });
                }

                conn.Close();
            }

            return notes;
        }
        public static void UpdateNote(Note note)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "UPDATE Notes SET SourceID = @SourceID, Stamp = @Stamp, Title = @Title, Note = @Note, CreatedOn = @CreatedOn, LastModifiedOn = @LastModifiedOn WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", note.Id);
                sqliteCommand.Parameters.AddWithValue("@SourceID", note.SourceID);
                sqliteCommand.Parameters.AddWithValue("@Stamp", note.Stamp);
                sqliteCommand.Parameters.AddWithValue("@Title", note.Title);
                sqliteCommand.Parameters.AddWithValue("@Note", note.Note1);
                sqliteCommand.Parameters.AddWithValue("@CreatedOn", note.CreatedOn);
                sqliteCommand.Parameters.AddWithValue("@LastModifiedOn", note.LastModifiedOn);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static void DeleteNote(int id)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "DELETE FROM Notes WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", id);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }

        #endregion

        #region Quotes

        public static void CreateQuote(Quote quote)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "INSERT INTO Quotes VALUES (NULL, @SourceID, @QuoteBegin, @QuoteEnd, @OriginalQuote, @TranslatedQuote, @CreatedOn, @LastModifiedOn);";
                sqliteCommand.Parameters.AddWithValue("@SourceID", quote.SourceID);
                sqliteCommand.Parameters.AddWithValue("@QuoteBegin", quote.QuoteBegin);
                sqliteCommand.Parameters.AddWithValue("@QuoteEnd", quote.QuoteEnd);
                sqliteCommand.Parameters.AddWithValue("@OriginalQuote", quote.OriginalQuote);
                sqliteCommand.Parameters.AddWithValue("@TranslatedQuote", quote.TranslatedQuote);
                sqliteCommand.Parameters.AddWithValue("@CreatedOn", quote.CreatedOn);
                sqliteCommand.Parameters.AddWithValue("@LastModifiedOn", quote.LastModifiedOn);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static Quote ReadQuote(int id)
        {
            Quote quote = new Quote();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, SourceID, QuoteBegin, QuoteEnd, OriginalQuote, TranslatedQuote, CreatedOn, LastModifiedOn FROM Quotes WHERE Id = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    quote = new Quote()
                    {
                        Id = query.GetInt32(0),
                        SourceID = query.GetInt32(1),
                        QuoteBegin = query.GetString(2),
                        QuoteEnd = query.GetString(3),
                        OriginalQuote = query.GetString(4),
                        TranslatedQuote = query.GetString(5),
                        CreatedOn = query.GetDateTime(6),
                        LastModifiedOn = query.GetDateTime(7)
                    };
                }

                conn.Close();
            }

            return quote;
        }

        public static List<Quote> ReadQuotes()
        {
            List<Quote> quotes = new List<Quote>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, SourceID, QuoteBegin, QuoteEnd, OriginalQuote, TranslatedQuote, CreatedOn, LastModifiedOn FROM Quotes;";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    quotes.Add(new Quote()
                    {
                        Id = query.GetInt32(0),
                        SourceID = query.GetInt32(1),
                        QuoteBegin = query.GetString(2),
                        QuoteEnd = query.GetString(3),
                        OriginalQuote = query.GetString(4),
                        TranslatedQuote = query.GetString(5),
                        CreatedOn = query.GetDateTime(6),
                        LastModifiedOn = query.GetDateTime(7)
                    });
                }

                conn.Close();
            }

            return quotes;
        }

        public static List<Quote> ReadQuotes(int id)
        {
            List<Quote> quotes = new List<Quote>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, SourceID, QuoteBegin, QuoteEnd, OriginalQuote, TranslatedQuote, CreatedOn, LastModifiedOn FROM Quotes WHERE SourceID = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    quotes.Add(new Quote()
                    {
                        Id = query.GetInt32(0),
                        SourceID = query.GetInt32(1),
                        QuoteBegin = query.GetString(2),
                        QuoteEnd = query.GetString(3),
                        OriginalQuote = query.GetString(4),
                        TranslatedQuote = query.GetString(5),
                        CreatedOn = query.GetDateTime(6),
                        LastModifiedOn = query.GetDateTime(7)
                    });
                }

                conn.Close();
            }

            return quotes;
        }
        public static void UpdateQuote(Quote quote)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "UPDATE Quotes SET SourceID = @SourceID, QuoteBegin = @QuoteBegin, QuoteEnd = @QuoteEnd, OriginalQuote = @OriginalQuote, TranslatedQuote = @TranslatedQuote, CreatedOn = @CreatedOn, LastModifiedOn = @LastModifiedOn WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", quote.Id);
                sqliteCommand.Parameters.AddWithValue("@SourceID", quote.SourceID);
                sqliteCommand.Parameters.AddWithValue("@QuoteBegin", quote.QuoteBegin);
                sqliteCommand.Parameters.AddWithValue("@QuoteEnd", quote.QuoteEnd);
                sqliteCommand.Parameters.AddWithValue("@OriginalQuote", quote.OriginalQuote);
                sqliteCommand.Parameters.AddWithValue("@TranslatedQuote", quote.TranslatedQuote);
                sqliteCommand.Parameters.AddWithValue("@CreatedOn", quote.CreatedOn);
                sqliteCommand.Parameters.AddWithValue("@LastModifiedOn", quote.LastModifiedOn);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static void DeleteQuote(int id)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "DELETE FROM Quotes WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", id);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }

        #endregion

        #region State

        public static string ReadState(int id)
        {
            string state = "";

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT State FROM States WHERE Id = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    state = query.GetString(0);
                }

                conn.Close();
            }

            return state;
        }
        public static List<string> ReadStates()
        {
            List<string> states = new List<string>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT State FROM States;";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    states.Add(query.GetString(0));
                }

                conn.Close();
            }

            return states;
        }

        #endregion

        #region Themes

        public static string ReadTheme(int id)
        {
            string theme = "";

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Theme FROM Themes WHERE Id = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    theme = query.GetString(0);
                }

                conn.Close();
            }

            return theme;
        }
        public static List<string> ReadThemes()
        {
            List<string> themes = new List<string>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Theme FROM Themes;";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    themes.Add(query.GetString(0));
                }

                conn.Close();
            }

            return themes;
        }

        #endregion

        #region Source types

        public static void CreateSourceType(SourceType sourceType)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "INSERT INTO SourceTypes VALUES (NULL, @SourceType);";

                sqliteCommand.Parameters.AddWithValue("@SourceType", sourceType.SourceType1);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static string ReadSourceType(int id)
        {
            string sourceType = "";

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT SourceType FROM SourceTypes WHERE Id = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    sourceType = query.GetString(0);
                }

                conn.Close();
            }

            return sourceType;
        }
        public static int ReadSourceType(string sourceType)
        {
            int id = 0;

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id FROM SourceTypes WHERE SourceType = '{sourceType}';";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                    id = query.GetInt32(0);
                
                conn.Close();
            }

            return id;
        }

        public static List<string> ReadSourceTypes()
        {
            List<string> sourceTypes = new List<string>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT SourceType FROM SourceTypes;";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    sourceTypes.Add(query.GetString(0));
                }

                conn.Close();
            }

            return sourceTypes;
        }
        public static void UpdateSourceType(SourceType sourceType)
        {
            if (sourceType.Id < 6)
                return;

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                SourceType curSourceType = new SourceType { Id = sourceType.Id, SourceType1 = ReadSourceType(sourceType.Id)};
                List<Source> sources = ReadSources(curSourceType.Id);
                
                foreach (Source source in sources)
                {
                    string[] temp = source.SourceName.Split("_");
                    temp[0] = sourceType.SourceType1;
                    source.SourceName = String.Join("_", temp);
                    
                    UpdateSource(source);
                }

                sqliteCommand.CommandText = "UPDATE SourceTypes SET SourceType = @SourceType WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", sourceType.Id);
                sqliteCommand.Parameters.AddWithValue("@SourceType", sourceType.SourceType1);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static void DeleteSourceType(int id)
        {
            if (id < 6)
                return;

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                List<Source> sources = ReadSources((byte)id);

                foreach (Source source in sources)
                    DeleteSource(source.Id);
                
                sqliteCommand.CommandText = "DELETE FROM SourceTypes WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", id);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }


        #endregion

    }
}
