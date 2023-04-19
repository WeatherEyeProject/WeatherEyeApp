using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherEyeApp.Models;

namespace WeatherEyeApp.Services
{
    public class MockTemps : IDataStore<TemperatureData>
    {
        readonly List<TemperatureData> temperatures;

        public MockTemps()
        {
            temperatures = new List<TemperatureData>()
            {
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 17.7, DateOfReading = new DateTime(2023, 4, 1) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 14.2, DateOfReading = new DateTime(2023, 4, 2) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 12.1, DateOfReading = new DateTime(2023, 4, 3) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 13.2, DateOfReading = new DateTime(2023, 4, 4) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 15.0, DateOfReading = new DateTime(2023, 4, 5) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 14.1, DateOfReading = new DateTime(2023, 4, 6) }
            };
        }

        public async Task<bool> AddItemAsync(TemperatureData td)
        {
            temperatures.Add(td);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(TemperatureData td)
        {
            var oldItem = temperatures.Where((TemperatureData arg) => arg.Id == td.Id).FirstOrDefault();
            temperatures.Remove(oldItem);
            temperatures.Add(td);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = temperatures.Where((TemperatureData arg) => arg.Id == id).FirstOrDefault();
            temperatures.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<TemperatureData> GetItemAsync(string id)
        {
            return await Task.FromResult(temperatures.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<TemperatureData>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(temperatures);
        }
    }
}