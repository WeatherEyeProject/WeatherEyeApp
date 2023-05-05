using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeatherEyeApp.Models;

namespace WeatherEyeApp.Services
{
    public class RainService : IRainService
    {
        HttpClient client;
        ObservableCollection<RainData> rainData;

        public RainService()
        {
            client = new HttpClient();
        }

        public async Task<ObservableCollection<RainData>> RefreshDataAsync()
        {
            var WebAPIUrl = "http://weathereye.pl/api/controller";
            var uri = new Uri(WebAPIUrl);
            
            try
            {
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(); 
                    rainData = JsonConvert.DeserializeObject<ObservableCollection<RainData>>(content);
                    return rainData;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return null;
        }

        // nie działa, do naprawienia I guess
        public async Task<ObservableCollection<RainData>> GetRainDataByDateAsync(DateTime date)
        {
            var WebAPIUrl = $"http://weathereye.pl/api/controller?dateOfReading={date:yyyy-MM-dd}";
            var uri = new Uri(WebAPIUrl);

            try
            {
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var rainData = JsonConvert.DeserializeObject<ObservableCollection<RainData>>(content);
                    return rainData;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return null;
        }
    }
}
