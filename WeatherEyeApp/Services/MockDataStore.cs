using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherEyeApp.Models;

namespace WeatherEyeApp.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        readonly List<Item> items;

        public MockDataStore()
        {
            items = new List<Item>()
            {
                new Item { Id = Guid.NewGuid().ToString(), Text = "23.03.2023", Description="Additional info from MockDataBase" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "24.03.2023", Description="Additional info from MockDataBase" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "25.03.2023", Description="Additional info from MockDataBase" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "26.03.2023", Description="Additional info from MockDataBase" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "27.03.2023", Description="Additional info from MockDataBase" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "28.03.2023", Description="Additional info from MockDataBase" }
            };

        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}