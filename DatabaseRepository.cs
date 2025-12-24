using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RailwayApp
{
    public class DatabaseRepository : IDataRepository
    {
        public List<Tariff> GetAllTariffs()
        {
            var tariffs = new List<Tariff>();

            using (var connection = new SQLiteConnection($"Data Source={AppConfig.DatabasePath}"))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Direction, BaseCost, DiscountType, DiscountPercent FROM Tariffs";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string direction = reader.GetString(0);
                            double baseCost = reader.GetDouble(1);
                            string discountType = reader.GetString(2);
                            object discountPercentObj = reader.GetValue(3);

                            DiscountStrategy strategy;

                            if (discountType == "PercentageDiscount" && !reader.IsDBNull(3))
                            {
                                int percent = Convert.ToInt32(discountPercentObj);
                                strategy = new PercentageDiscount(percent);
                            }
                            else
                            {
                                strategy = new NoDiscount();
                            }

                            tariffs.Add(new Tariff(direction, baseCost, strategy));
                        }
                    }
                }
            }

            return tariffs;
        }

        public void AddTariff(Tariff tariff)
        {
            using (var connection = new SQLiteConnection($"Data Source={AppConfig.DatabasePath}"))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = @"
                            INSERT INTO Tariffs (Direction, BaseCost, DiscountType, DiscountPercent) 
                            VALUES (@direction, @baseCost, @discountType, @discountPercent)";

                        command.Parameters.AddWithValue("@direction", tariff.Direction);
                        command.Parameters.AddWithValue("@baseCost", tariff.BaseCost);
                        command.Parameters.AddWithValue("@discountType", tariff.Strategy.GetType().Name);

                        if (tariff.Strategy is PercentageDiscount pd)
                        {
                            command.Parameters.AddWithValue("@discountPercent", pd.DiscountPercent);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@discountPercent", DBNull.Value);
                        }

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        public void RemoveTariff(string direction)
        {
            using (var connection = new SQLiteConnection($"Data Source={AppConfig.DatabasePath}"))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Tariffs WHERE Direction = @direction";
                    command.Parameters.AddWithValue("@direction", direction);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateTariff(string oldDirection, Tariff updatedTariff)
        {
            using (var connection = new SQLiteConnection($"Data Source={AppConfig.DatabasePath}"))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    using (var deleteCmd = connection.CreateCommand())
                    {
                        deleteCmd.Transaction = transaction;
                        deleteCmd.CommandText = "DELETE FROM Tariffs WHERE Direction = @oldDirection";
                        deleteCmd.Parameters.AddWithValue("@oldDirection", oldDirection);
                        deleteCmd.ExecuteNonQuery();
                    }

                    using (var insertCmd = connection.CreateCommand())
                    {
                        insertCmd.Transaction = transaction;
                        insertCmd.CommandText = @"
                            INSERT INTO Tariffs (Direction, BaseCost, DiscountType, DiscountPercent) 
                            VALUES (@direction, @baseCost, @discountType, @discountPercent)";

                        insertCmd.Parameters.AddWithValue("@direction", updatedTariff.Direction);
                        insertCmd.Parameters.AddWithValue("@baseCost", updatedTariff.BaseCost);
                        insertCmd.Parameters.AddWithValue("@discountType", updatedTariff.Strategy.GetType().Name);

                        if (updatedTariff.Strategy is PercentageDiscount pd)
                        {
                            insertCmd.Parameters.AddWithValue("@discountPercent", pd.DiscountPercent);
                        }
                        else
                        {
                            insertCmd.Parameters.AddWithValue("@discountPercent", DBNull.Value);
                        }

                        insertCmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        public void Clear()
        {
            using (var connection = new SQLiteConnection($"Data Source={AppConfig.DatabasePath}"))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Tariffs";
                    command.ExecuteNonQuery();
                }
            }
        }

        public string FindMinCostDirection()
        {
            var all = GetAllTariffs();
            if (all.Count == 0)
                throw new InvalidOperationException("Список тарифов пуст");

            double minCost = all.Min(t => t.FinalCost);
            return all.First(t => Math.Abs(t.FinalCost - minCost) < 0.01).Direction;
        }

        public List<string> FindMinCostDirections()
        {
            var minDirections = new List<string>();
            using (var connection = new SQLiteConnection($"Data Source={AppConfig.DatabasePath}"))
            {
                connection.Open();
                double minCost;
                using (var minCostCmd = connection.CreateCommand())
                {
                    minCostCmd.CommandText = @"
                        SELECT MIN(
                            CASE DiscountType 
                                WHEN 'PercentageDiscount' THEN BaseCost * (1 - CAST(DiscountPercent AS REAL) / 100)
                                ELSE BaseCost 
                            END
                        ) AS MinCost
                        FROM Tariffs";

                    var result = minCostCmd.ExecuteScalar();
                    if (result == null || result == DBNull.Value)
                    {
                        throw new InvalidOperationException("Список тарифов пуст");
                    }
                    minCost = Convert.ToDouble(result);
                }

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Direction 
                        FROM Tariffs 
                        WHERE 
                            CASE DiscountType 
                                WHEN 'PercentageDiscount' THEN BaseCost * (1 - CAST(DiscountPercent AS REAL) / 100)
                                ELSE BaseCost 
                            END = @minCost";

                    cmd.Parameters.AddWithValue("@minCost", minCost);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            minDirections.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return minDirections;
        }
    }
}