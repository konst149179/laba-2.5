using System.Data.SQLite;
using System;
using System.IO;

namespace RailwayApp
{
    public static class DatabaseService
    {
        public static void InitializeDatabase()
        {
            AppConfig.EnsureDatabaseDirectoryExists();

            using (var connection = new SQLiteConnection($"Data Source={AppConfig.DatabasePath}"))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Tariffs (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Direction TEXT NOT NULL UNIQUE,
                            BaseCost REAL NOT NULL,
                            DiscountType TEXT NOT NULL,
                            DiscountPercent INTEGER
                        );";
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool TestDatabaseConnection()
        {
            try
            {
                using (var connection = new SQLiteConnection($"Data Source={AppConfig.DatabasePath}"))
                {
                    connection.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}