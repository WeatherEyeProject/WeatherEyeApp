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
    public class SensorService<T> //: ISensorService<RainData>
    {
        readonly HttpClient client;

        public SensorService()
        {
            client = new HttpClient();
        }

        public async Task<ObservableCollection<T>> RefreshDataAsync(string url)
        {
            var uri = new Uri(url);
            
            try
            {
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync(); 
                    var sensorData = JsonConvert.DeserializeObject<ObservableCollection<T>>(content);
                    return sensorData;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return null;
        }

        public async Task<ObservableCollection<T>> GetDataByDateAsync(string url, DateTime date1, DateTime date2)
        {
            var WebAPIUrl = url + "/" + date1.ToString("yyyy-MM-dd") + "/" + date2.ToString("yyyy-MM-dd");
            var uri = new Uri(WebAPIUrl);

            try
            {
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var sensorData = JsonConvert.DeserializeObject<ObservableCollection<T>>(content);
                    return sensorData;
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
