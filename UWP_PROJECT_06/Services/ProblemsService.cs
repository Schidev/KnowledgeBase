using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP_PROJECT_06.Models.Notes;
using UWP_PROJECT_06.Models.Problems;
using Windows.Storage;

namespace UWP_PROJECT_06.Services
{
    public class ProblemsService
    {
        static string FileName = "ProblemsDB";

        public async static Task InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);

            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand;
                string commandText;

                // Create Problems table

                commandText = "CREATE TABLE IF NOT EXISTS Problems (Id INTEGER NOT NULL, IsDone BOOLEAN NOT NULL, Problem TEXT NOT NULL, Link TEXT NOT NULL, Week TINYINT NOT NULL, Category NVARCHAR(1) NOT NULL, TimePeriodType NVARCHAR(1) NOT NULL, DueDateTimeBegin DATETIME NOT NULL, DueDateTimeEnd DATETIME NOT NULL, IsMonday BOOLEAN NOT NULL, IsTuesday BOOLEAN NOT NULL, IsWednesday BOOLEAN NOT NULL, IsThursday BOOLEAN NOT NULL, IsFriday BOOLEAN NOT NULL, IsSaturday BOOLEAN NOT NULL, IsSunday BOOLEAN NOT NULL, RepetitionFrequencyWeeks TINYINT NOT NULL, RepetitionFrequencyDays TINYINT NOT NULL, RepetitionDateFrom DATETIME NOT NULL, RepetitionDateTo DATETIME NOT NULL, Hash TEXT NOT NULL UNIQUE, CreatedOn DATETIME NOT NULL, LastModifiedOn DATETIME NOT NULL, PRIMARY KEY(Id AUTOINCREMENT));";
                sqliteCommand = new SqliteCommand(commandText, conn);
                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }

        #region Problems CRUD

        public static void CreateProblem(Problem problem)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "INSERT INTO Problems VALUES (NULL, @IsDone, @Problem1, @Link, @Week, @Category, @TimePeriodType, @DueDateTimeBegin, @DueDateTimeEnd, @IsMonday, @IsTuesday, @IsWednesday, @IsThursday, @IsFriday, @IsSaturday, @IsSunday, @RepetitionFrequencyWeeks, @RepetitionFrequencyDays, @RepetitionDateFrom, @RepetitionDateTo, @Hash, @CreatedOn, @LastModifiedOn);";

                sqliteCommand.Parameters.AddWithValue("@IsDone", problem.IsDone);
                sqliteCommand.Parameters.AddWithValue("@Problem1", problem.Problem1);
                sqliteCommand.Parameters.AddWithValue("@Link", problem.Link);
                sqliteCommand.Parameters.AddWithValue("@Week", problem.Week);
                sqliteCommand.Parameters.AddWithValue("@Category", problem.Category);
                sqliteCommand.Parameters.AddWithValue("@TimePeriodType", problem.TimePeriodType);
                sqliteCommand.Parameters.AddWithValue("@DueDateTimeBegin", problem.DueDateTimeBegin);
                sqliteCommand.Parameters.AddWithValue("@DueDateTimeEnd", problem.DueDateTimeEnd);
                sqliteCommand.Parameters.AddWithValue("@IsMonday", problem.IsMonday);
                sqliteCommand.Parameters.AddWithValue("@IsTuesday", problem.IsTuesday);
                sqliteCommand.Parameters.AddWithValue("@IsWednesday", problem.IsWednesday);
                sqliteCommand.Parameters.AddWithValue("@IsThursday", problem.IsThursday);
                sqliteCommand.Parameters.AddWithValue("@IsFriday", problem.IsFriday);
                sqliteCommand.Parameters.AddWithValue("@IsSaturday", problem.IsSaturday);
                sqliteCommand.Parameters.AddWithValue("@IsSunday", problem.IsSunday);
                sqliteCommand.Parameters.AddWithValue("@RepetitionFrequencyWeeks", problem.RepetitionFrequencyWeeks);
                sqliteCommand.Parameters.AddWithValue("@RepetitionFrequencyDays", problem.RepetitionFrequencyDays);
                sqliteCommand.Parameters.AddWithValue("@RepetitionDateFrom", problem.RepetitionDateFrom);
                sqliteCommand.Parameters.AddWithValue("@RepetitionDateTo", problem.RepetitionDateTo);
                sqliteCommand.Parameters.AddWithValue("@Hash", problem.Hash);
                sqliteCommand.Parameters.AddWithValue("@CreatedOn", problem.CreatedOn);
                sqliteCommand.Parameters.AddWithValue("@LastModifiedOn", problem.LastModifiedOn);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static Problem ReadProblem(int id)
        {
            Problem problem = new Problem();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, IsDone, Problem, Link, Week, Category, TimePeriodType, DueDateTimeBegin, DueDateTimeEnd, IsMonday, IsTuesday, IsWednesday, IsThursday, IsFriday, IsSaturday, IsSunday, RepetitionFrequencyWeeks, RepetitionFrequencyDays, RepetitionDateFrom, RepetitionDateTo, Hash, CreatedOn, LastModifiedOn FROM Problems WHERE Id = {id};";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    problem = new Problem()
                    {
                        Id = query.GetInt32(0),
                        IsDone = query.GetBoolean(1),
                        Problem1 = query.GetString(2),
                        Link = query.GetString(3),
                        Week = query.GetByte(4),
                        Category = query.GetString(5),
                        TimePeriodType = query.GetString(6),
                        DueDateTimeBegin = query.GetDateTime(7),
                        DueDateTimeEnd = query.GetDateTime(8),
                        IsMonday = query.GetBoolean(9),
                        IsTuesday = query.GetBoolean(10),
                        IsWednesday = query.GetBoolean(11),
                        IsThursday = query.GetBoolean(12),
                        IsFriday = query.GetBoolean(13),
                        IsSaturday = query.GetBoolean(14),
                        IsSunday = query.GetBoolean(15),
                        RepetitionFrequencyWeeks = query.GetByte(16),
                        RepetitionFrequencyDays = query.GetByte(17),
                        RepetitionDateFrom = query.GetDateTime(18),
                        RepetitionDateTo = query.GetDateTime(19),
                        Hash = query.GetString(20),
                        CreatedOn = query.GetDateTime(21),
                        LastModifiedOn = query.GetDateTime(22)
                    };
                }

                conn.Close();
            }

            return problem;
        }
        public static List<Problem> ReadProblems()
        {
            List<Problem> problems = new List<Problem>();

            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                string commandText = $"SELECT Id, IsDone, Problem, Link, Week, Category, TimePeriodType, DueDateTimeBegin, DueDateTimeEnd, IsMonday, IsTuesday, IsWednesday, IsThursday, IsFriday, IsSaturday, IsSunday, RepetitionFrequencyWeeks, RepetitionFrequencyDays, RepetitionDateFrom, RepetitionDateTo, Hash, CreatedOn, LastModifiedOn FROM Problems;";
                SqliteCommand sqliteCommand = new SqliteCommand(commandText, conn);

                SqliteDataReader query = sqliteCommand.ExecuteReader();

                while (query.Read())
                {
                    problems.Add(new Problem()
                    {
                        Id = query.GetInt32(0),
                        IsDone = query.GetBoolean(1),
                        Problem1 = query.GetString(2),
                        Link = query.GetString(3),
                        Week = query.GetByte(4),
                        Category = query.GetString(5),
                        TimePeriodType = query.GetString(6),
                        DueDateTimeBegin = query.GetDateTime(7),
                        DueDateTimeEnd = query.GetDateTime(8),
                        IsMonday = query.GetBoolean(9),
                        IsTuesday = query.GetBoolean(10),
                        IsWednesday = query.GetBoolean(11),
                        IsThursday = query.GetBoolean(12),
                        IsFriday = query.GetBoolean(13),
                        IsSaturday = query.GetBoolean(14),
                        IsSunday = query.GetBoolean(15),
                        RepetitionFrequencyWeeks = query.GetByte(16),
                        RepetitionFrequencyDays = query.GetByte(17),
                        RepetitionDateFrom = query.GetDateTime(18),
                        RepetitionDateTo = query.GetDateTime(19),
                        Hash = query.GetString(20),
                        CreatedOn = query.GetDateTime(21),
                        LastModifiedOn = query.GetDateTime(22)
                    });
                }

                conn.Close();
            }

            return problems;
        }
        public static void UpdateProblem(Problem problem)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "UPDATE Problems SET IsDone = @IsDone, Problem = @Problem, Link = @Link, Week = @Week, Category = @Category, TimePeriodType = @TimePeriodType, DueDateTimeBegin = @DueDateTimeBegin, DueDateTimeEnd = @DueDateTimeEnd, IsMonday = @IsMonday, IsTuesday = @IsTuesday, IsWednesday = @IsWednesday, IsThursday = @IsThursday, IsFriday = @IsFriday, IsSaturday = @IsSaturday, IsSunday = @IsSunday, RepetitionFrequencyWeeks = @RepetitionFrequencyWeeks, RepetitionFrequencyDays = @RepetitionFrequencyDays, RepetitionDateFrom = @RepetitionDateFrom, RepetitionDateTo = @RepetitionDateTo, Hash = @Hash, CreatedOn = @CreatedOn, LastModifiedOn = @LastModifiedOn WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", problem.Id);
                sqliteCommand.Parameters.AddWithValue("@IsDone", problem.IsDone);
                sqliteCommand.Parameters.AddWithValue("@Problem", problem.Problem1);
                sqliteCommand.Parameters.AddWithValue("@Link", problem.Link);
                sqliteCommand.Parameters.AddWithValue("@Week", problem.Week);
                sqliteCommand.Parameters.AddWithValue("@Category", problem.Category);
                sqliteCommand.Parameters.AddWithValue("@TimePeriodType", problem.TimePeriodType);
                sqliteCommand.Parameters.AddWithValue("@DueDateTimeBegin", problem.DueDateTimeBegin);
                sqliteCommand.Parameters.AddWithValue("@DueDateTimeEnd", problem.DueDateTimeEnd);
                sqliteCommand.Parameters.AddWithValue("@IsMonday", problem.IsMonday);
                sqliteCommand.Parameters.AddWithValue("@IsTuesday", problem.IsTuesday);
                sqliteCommand.Parameters.AddWithValue("@IsWednesday", problem.IsWednesday);
                sqliteCommand.Parameters.AddWithValue("@IsThursday", problem.IsThursday);
                sqliteCommand.Parameters.AddWithValue("@IsFriday", problem.IsFriday);
                sqliteCommand.Parameters.AddWithValue("@IsSaturday", problem.IsSaturday);
                sqliteCommand.Parameters.AddWithValue("@IsSunday", problem.IsSunday);
                sqliteCommand.Parameters.AddWithValue("@RepetitionFrequencyWeeks", problem.RepetitionFrequencyWeeks);
                sqliteCommand.Parameters.AddWithValue("@RepetitionFrequencyDays", problem.RepetitionFrequencyDays);
                sqliteCommand.Parameters.AddWithValue("@RepetitionDateFrom", problem.RepetitionDateFrom);
                sqliteCommand.Parameters.AddWithValue("@RepetitionDateTo", problem.RepetitionDateTo);
                sqliteCommand.Parameters.AddWithValue("@Hash", problem.Hash);
                sqliteCommand.Parameters.AddWithValue("@CreatedOn", problem.CreatedOn);
                sqliteCommand.Parameters.AddWithValue("@LastModifiedOn", problem.LastModifiedOn);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }
        public static void DeleteProblem(int id)
        {
            string dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, FileName);
            using (SqliteConnection conn = new SqliteConnection($"Filename={dbPath}"))
            {
                conn.Open();

                SqliteCommand sqliteCommand = new SqliteCommand();
                sqliteCommand.Connection = conn;

                sqliteCommand.CommandText = "DELETE FROM Problems WHERE Id = @Id;";
                sqliteCommand.Parameters.AddWithValue("@Id", id);

                sqliteCommand.ExecuteReader();

                conn.Close();
            }
        }

        #endregion
    }
}
