using System.Collections.Generic;

namespace RailwayApp
{
    public interface IDataRepository
    {
        List<Tariff> GetAllTariffs();
        void AddTariff(Tariff tariff);
        void RemoveTariff(string direction);
        void UpdateTariff(string oldDirection, Tariff updatedTariff);
        void Clear();
        string FindMinCostDirection();
        List<string> FindMinCostDirections();
    }
}