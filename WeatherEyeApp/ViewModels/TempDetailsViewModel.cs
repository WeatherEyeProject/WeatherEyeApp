using Microcharts;
using SkiaSharp;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using WeatherEyeApp.Models;
using Xamarin.Forms;
using WeatherEyeApp.Services;

namespace WeatherEyeApp.ViewModels
{
    public class TempDetailsViewModel : BaseViewModel
    {
        public ObservableCollection<TemperatureData> Temperatures { get; }
        public ObservableCollection<TemperatureData> Lights { get; }
        public Command LoadTempsCommand { get; }
        public Chart TempChart { get; set; }
        private readonly MockTemps _mockTemps;

        public TempDetailsViewModel()
        {
            Title = "Measurements Details";
            Temperatures = new ObservableCollection<TemperatureData>();
            LoadTempsCommand = new Command(async () => await ExecuteLoadTempsCommand());
            _mockTemps = new MockTemps();
            TempChart = GenerateTempChart();
            
        }
        public void OnAppearing()
        {
            IsBusy = true;
        }

        async Task ExecuteLoadTempsCommand()
        {
            IsBusy = true;

            try
            {
                Temperatures.Clear();
                var temps = await _mockTemps.GetItemsAsync(true);
                foreach(var temp in temps)
                {
                    Temperatures.Add(temp);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private Chart GenerateTempChart()
        {
            var temperatures = _mockTemps.GetItemsAsync().Result;

            var lineChart = new LineChart()
            {
                Entries = temperatures.Select(t => new ChartEntry((float)t.Temp) { Label = t.DateOfReading.ToString("dd/MM"), ValueLabel = t.Temp.ToString()+ "°C", Color = SKColor.Parse("#0077C0") }),
                LineMode = LineMode.Straight,
                PointMode = PointMode.Circle,
                LabelTextSize = 40,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal
            };

            return lineChart;

        }
    }
}
