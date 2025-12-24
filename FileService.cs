using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RailwayApp
{
    public static class FileService
    {
        public static void SaveToFile(IEnumerable<Tariff> tariffs, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var tariff in tariffs)
                {
                    string discountType = tariff.Strategy.GetType().Name;
                    string discountValue = "";
                    if (tariff.Strategy is PercentageDiscount discount)
                    {
                        discountValue = discount.DiscountPercent.ToString();
                    }
                    writer.WriteLine($"{tariff.Direction}|{tariff.BaseCost}|{discountType}|{discountValue}");
                }
            }
        }

        public static void SaveToFile(Station station, string filePath)
        {
            SaveToFile(station.GetAllTariffs(), filePath);
        }

        public static void LoadFromFile(Station station, string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден", filePath);

            station.Clear();
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split('|');
                if (parts.Length < 3) continue;

                string direction = parts[0];
                double baseCost = double.Parse(parts[1]);
                string discountType = parts[2];
                string discountValue = parts.Length > 3 ? parts[3] : "";

                DiscountStrategy strategy;

                if (discountType == nameof(PercentageDiscount) &&
                    int.TryParse(discountValue, out int percent))
                {
                    if (percent == 0)
                    {
                        strategy = new NoDiscount();
                    }
                    else
                    {
                        strategy = new PercentageDiscount(percent);
                    }
                }
                else
                {
                    strategy = new NoDiscount();
                }

                try
                {
                    station.AddTariff(direction, baseCost, strategy);
                }
                catch (ArgumentException)
                {

                }
            }
        }
    }
}