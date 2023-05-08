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
                new Item { Id = Guid.NewGuid().ToString(), Text = "1.04.2023", Description="Data from 1.04.2023" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "2.04.2023", Description="Data from 2.04.2023" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "3.04.2023", Description="Data from 3.04.2023" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "4.04.2023", Description="Data from 4.04.2023" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "5.04.2023", Description="Data from 5.04.2023" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "6.04.2023", Description="Data from 6.04.2023" },
                new Item { Id = Guid.NewGuid().ToString(), Text = "7.04.2023", Description="Data from 7.04.2023" }
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