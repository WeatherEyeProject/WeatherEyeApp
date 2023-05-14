using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WeatherEyeApp.Models;

namespace WeatherEyeApp.Services
{
    public class LatestDataSensorService
    {
        HttpClient client;

        public LatestDataSensorService()
        {
            client = new HttpClient();
        }

        public async Task<LatestData> RefreshDataAsync()
        {
            var WebAPIUrl = "http://weathereye.pl/api/LatestSensorsData";
            var uri = new Uri(WebAPIUrl);

            try
            {
                var response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var sensorData = JsonConvert.DeserializeObject<LatestData>(content);
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
