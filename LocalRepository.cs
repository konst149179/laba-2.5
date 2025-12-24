using System;
using System.Collections.Generic;
using System.Linq;

namespace RailwayApp
{
    public class LocalRepository : IDataRepository
    {
        private readonly Station _station = new Station();

        public List<Tariff> GetAllTariffs() => _station.GetAllTariffs();

        public void AddTariff(Tariff tariff)
        {
            _station.AddTariff(tariff.Direction, tariff.BaseCost, tariff.Strategy);
        }

        public void RemoveTariff(string direction)
        {
            var tariffs = _station.GetAllTariffs();
            for (int i = 0; i < tariffs.Count; i++)
            {
                if (tariffs[i].Direction.Equals(direction, StringComparison.OrdinalIgnoreCase))
                {
                    _station.RemoveTariff(i);
                    return;
                }
            }
        }

        public void UpdateTariff(string oldDirection, Tariff updatedTariff)
        {
            var tariffs = _station.GetAllTariffs();
            for (int i = 0; i < tariffs.Count; i++)
            {
                if (tariffs[i].Direction.Equals(oldDirection, StringComparison.OrdinalIgnoreCase))
                {
                    if (!_station.ChangeDirection(i, updatedTariff.Direction))
                    {
                        throw new ArgumentException($"Направление '{updatedTariff.Direction}' уже существует");
                    }
                    tariffs[i].BaseCost = updatedTariff.BaseCost;
                    tariffs[i].SetStrategy(updatedTariff.Strategy);
                    return;
                }
            }
            throw new ArgumentException($"Направление '{oldDirection}' не найдено");
        }

        public void Clear() => _station.Clear();

        public List<string> FindMinCostDirections()
        {
            return _station.FindAllMinCostDirections();
        }
        public string FindMinCostDirection()
        {
            return _station.FindMinCostDirection();
        }
    }
}