using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Bookmarks;
using UWP_PROJECT_06.Models.Dictionary;
using UWP_PROJECT_06.Models.Notes;
using Windows.Storage;

namespace UWP_PROJECT_06.Services
{
    public static class BookmarksService
    {
        static string FileName = "BookmarksDB";

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

                commandText = "CREATE TABLE IF NOT EXISTS Bookmarks (Id INTEGER NOT NULL, Date DATETIME NOT NULL, Content TEXT NOT NULL, PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                // Create Notes table

                commandText = "CREATE TABLE IF NOT EXISTS DailyTasks (Id INTEGER NOT NULL, BookmarkID INTEGER NOT NULL, TimeBegin DATETIME NOT NULL, TimeEnd DATETIME NOT NULL, Task TEXT NOT NULL, PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }

        #region Bookmarks
        public static void CreateBookmark(Bookmark bookmark)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "INSERT INTO Bookmarks VALUES (NULL, @Date, @Content);";

                sqliteCommand.Parameters.AddWithValue("@Date", bookmark.Date);
                sqliteCommand.Parameters.AddWithValue("@Content", bookmark.Content);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static Bookmark ReadBookmark(int id)
        {
            Bookmark bookmark = new Bookmark();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, Date, Content FROM Bookmarks WHERE Id = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    bookmark = new Bookmark()
                    {
                        Id = query.GetInt32(0),
                        Date = query.GetDateTime(1),
                        Content = query.GetString(2)
                    };
                }

                conn.Close();
            }

            return bookmark;
        }
        public static List<Bookmark> ReadBookmarks()
        {
            List<Bookmark> bookmarks = new List<Bookmark>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, Date, Content FROM Bookmarks;";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    bookmarks.Add(new Bookmark()
                    {
                        Id = query.GetInt32(0),
                        Date = query.GetDateTime(1),
                        Content = query.GetString(2)
                    });
                }

                conn.Close();
            }

            return bookmarks;
        }
        public static void UpdateBookmark(Bookmark bookmark)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "UPDATE Bookmarks SET Date = @Date, Content = @Content WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", bookmark.Id);
                sqliteCommand.Parameters.AddWithValue("@Date", bookmark.Date);
                sqliteCommand.Parameters.AddWithValue("@Content", bookmark.Content);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static void DeleteBookmark(int id)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "DELETE DailyTasks WHERE BookmarkID = @Id; DELETE FROM Bokmarks WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", id);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }

        #endregion

        #region DailyTasks

        public static void CreateDailyTask(DailyTask dailyTask)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "INSERT INTO DailyTasks VALUES (NULL, @BookmarkID, @TimeBegin, @TimeEnd, @Task);";
                sqliteCommand.Parameters.AddWithValue("@BookmarkID", dailyTask.BookmarkID);
                sqliteCommand.Parameters.AddWithValue("@TimeBegin", dailyTask.TimeBegin);
                sqliteCommand.Parameters.AddWithValue("@TimeEnd", dailyTask.TimeEnd);
                sqliteCommand.Parameters.AddWithValue("@Task", dailyTask.Task);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static DailyTask ReadDailyTask(int id)
        {
            DailyTask dailyTask = new DailyTask();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, BookmarkID, TimeBegin, TimeEnd, Task FROM DailyTasks WHERE Id = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    dailyTask = new DailyTask()
                    {
                        Id = query.GetInt32(0),
                        BookmarkID = query.GetInt32(1),
                        TimeBegin = query.GetDateTime(2),
                        TimeEnd = query.GetDateTime(3),
                        Task = query.GetString(4)
                    };
                }

                conn.Close();
            }

            return dailyTask;
        }
        public static List<DailyTask> ReadDailyTasks(int bookmarkId)
        {
            List<DailyTask> dailyTasks = new List<DailyTask>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, BookmarkID, TimeBegin, TimeEnd, Task FROM DailyTasks WHERE WordId = {bookmarkId};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    dailyTasks.Add(new DailyTask()
                    {
                        Id = query.GetInt32(0),
                        BookmarkID = query.GetInt32(1),
                        TimeBegin = query.GetDateTime(2),
                        TimeEnd = query.GetDateTime(3),
                        Task = query.GetString(4)
                    });
                }

                conn.Close();
            }

            return dailyTasks;
        }
        public static void UpdateDailyTask(DailyTask dailyTask)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "UPDATE DailyTasks SET BookmarkID = @BookmarkID, TimeBegin = @TimeBegin, TimeEnd = @TimeEnd, Task = @Task WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", dailyTask.Id);
                sqliteCommand.Parameters.AddWithValue("@BookmarkID", dailyTask.BookmarkID);
                sqliteCommand.Parameters.AddWithValue("@TimeBegin", dailyTask.TimeBegin);
                sqliteCommand.Parameters.AddWithValue("@TimeEnd", dailyTask.TimeEnd);
                sqliteCommand.Parameters.AddWithValue("@Task", dailyTask.Task);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static void DeleteDailyTask(int Id)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "DELETE FROM DailyTasks WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", Id);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }

        #endregion

    }
}
