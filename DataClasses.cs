using System;
using System.Collections.Generic;
using System.Linq;

namespace RailwayApp
{
    public abstract class DiscountStrategy
    {
        public abstract double CalculateCost(double baseCost);
        public abstract string GetStrategyType();
    }

    public class NoDiscount : DiscountStrategy
    {
        public override double CalculateCost(double baseCost) => baseCost;
        public override string GetStrategyType() => "Без скидки";
    }

    public class PercentageDiscount : DiscountStrategy
    {
        public int DiscountPercent { get; set; }

        public PercentageDiscount(int percent)
        {
            if (percent < 0 || percent > 100)
                throw new ArgumentException("Скидка должна быть от 0 до 100%");
            DiscountPercent = percent;
        }

        public override double CalculateCost(double baseCost) =>
            baseCost * (1 - DiscountPercent / 100.0);

        public override string GetStrategyType() => $"Скидка {DiscountPercent}%";
    }

    public class Tariff
    {
        public string Direction { get; set; }
        public double BaseCost { get; set; }
        public DiscountStrategy Strategy { get; private set; }

        public Tariff(string direction, double baseCost, DiscountStrategy strategy)
        {
            if (baseCost <= 0)
                throw new ArgumentException("Стоимость не может быть отрицательной. или 0, кто за бесплатно повезет?");
            Direction = direction;
            BaseCost = baseCost;
            SetStrategy(strategy);
        }
        public void SetStrategy(DiscountStrategy strategy)
        {
            PercentageDiscount pd = strategy as PercentageDiscount;
            if (pd != null && pd.DiscountPercent == 0)
            {
                Strategy = new NoDiscount();
            }
            else Strategy = strategy ?? new NoDiscount();
        }

        public double FinalCost => Strategy.CalculateCost(BaseCost);
        public string DiscountType => Strategy.GetStrategyType();

        public int GetDiscountPercentForSorting()
        {
            PercentageDiscount percentageDiscount = Strategy as PercentageDiscount;
            if (percentageDiscount != null)
            {
                return percentageDiscount.DiscountPercent;
            }
            return 0;
        }
    }

    public class Station
    {
        private readonly List<Tariff> _tariffs = new List<Tariff>();
        private readonly HashSet<string> _usedDirections = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private string Normalize(string str) => str.Trim().ToLower();

        public void AddTariff(string direction, double baseCost)
        {
            AddTariff(direction, baseCost, new NoDiscount());
        }

        public void AddTariff(string direction, double baseCost, int discountPercent)
        {
            AddTariff(direction, baseCost, new PercentageDiscount(discountPercent));
        }

        public void AddTariff(string direction, double baseCost, DiscountStrategy strategy)
        {
            string normalized = Normalize(direction);
            if (_usedDirections.Contains(normalized))
                throw new ArgumentException($"Направление '{direction}' уже существует!");

            _tariffs.Add(new Tariff(direction, baseCost, strategy));
            _usedDirections.Add(normalized);
        }

        public List<Tariff> GetAllTariffs() => _tariffs;

        public void RemoveTariff(int index)
        {
            if (index < 0 || index >= _tariffs.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            string direction = _tariffs[index].Direction;
            _usedDirections.Remove(Normalize(direction));
            _tariffs.RemoveAt(index);
        }

        public bool ChangeDirection(int index, string newDirection)
        {
            if (index < 0 || index >= _tariffs.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            Tariff tariff = _tariffs[index];
            string oldNormalized = Normalize(tariff.Direction);
            string newNormalized = Normalize(newDirection);

            if (oldNormalized == newNormalized)
            {
                tariff.Direction = newDirection;
                return true;
            }

            if (_usedDirections.Contains(newNormalized))
                return false;

            _usedDirections.Remove(oldNormalized);
            tariff.Direction = newDirection;
            _usedDirections.Add(newNormalized);
            return true;
        }

        public string FindMinCostDirection()
        {
            return FindAllMinCostDirections().FirstOrDefault() ??
                throw new InvalidOperationException("Список тарифов пуст");
        }

        public List<string> FindAllMinCostDirections()
        {
            if (_tariffs.Count == 0)
                throw new InvalidOperationException("Список тарифов пуст");
            double minCost = _tariffs.Min(t => t.FinalCost);
            return _tariffs
                .Where(t => Math.Abs(t.FinalCost - minCost) < 0.01)
                .Select(t => t.Direction)
                .ToList();
        }

        public void Clear()
        {
            _tariffs.Clear();
            _usedDirections.Clear();
        }
    }
}