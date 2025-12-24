using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace RailwayApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            InitializeDatabaseMode();

            Application.Run(new MainForm());
        }
        private static void InitializeDatabaseMode()
        {
            try
            {
                AppConfig.EnsureDatabaseDirectoryExists();

                using (var connection = new SQLiteConnection($"Data Source={AppConfig.DatabasePath}"))
                {
                    connection.Open();
                    DatabaseService.InitializeDatabase();
                    AppConfig.UseDatabase = true;
                    File.Delete("db_errors.log");
                }
            }
            catch (Exception ex)
            {
                string errorDetails = $"Полная ошибка подключения к БД:\n" +
                    $"Дата: {DateTime.Now}\n" +
                    $"Путь к БД: {AppConfig.DatabasePath}\n" +
                    $"Ошибка: {ex.Message}\n" +
                    $"Стек: {ex.StackTrace}\n" +
                    $"Внутренняя ошибка: {ex.InnerException?.Message}";

                File.WriteAllText("db_errors.log", $"{DateTime.Now}: {ex.Message}\n{ex.StackTrace}");
                AppConfig.UseDatabase = false;
            }
        }
    }
}