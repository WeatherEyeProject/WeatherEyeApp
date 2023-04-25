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
        //public Chart LightChart { get; set; }
        private readonly MockTemps _mockTemps;

        public TempDetailsViewModel()
        {
            Title = "Measurements Details";
            Temperatures = new ObservableCollection<TemperatureData>();
            LoadTempsCommand = new Command(async () => await ExecuteLoadTempsCommand());
            _mockTemps = new MockTemps();
            TempChart = GenerateTempChart();
            
            //LightChart = GenerateLightChart();
            
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

        /*
        public Chart TChart
        {
            get => TempChart;
            set => SetProperty(ref TempChart, value);
        }

        public Chart LChart
        {
            get => LightChart;
            set => SetProperty(ref LightChart, value);
        }

        

        public void ChartViewModel()
        {
            GenerateTempChart();
            GenerateLightChart();
        }
        
        */
        private Chart GenerateTempChart()
        {
            //var temperatures = TempData.GetItemsAsync().Result;
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

            /*
            var chartData = new List<ChartEntry>();

            // Mock database with sample data
            var mockData = new List<TemperatureData>
            {
               new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 17.7, DateOfReading = new DateTime(2023, 4, 1) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 14.2, DateOfReading = new DateTime(2023, 4, 2) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 12.1, DateOfReading = new DateTime(2023, 4, 3) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 13.2, DateOfReading = new DateTime(2023, 4, 4) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 15.0, DateOfReading = new DateTime(2023, 4, 5) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 14.1, DateOfReading = new DateTime(2023, 4, 6) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 10.1, DateOfReading = new DateTime(2023, 4, 7) }
            };
            
            // Iterate through the mock data and add it to the chart data list
            foreach (var tempData in mockData)
            {
                chartData.Add(new ChartEntry((float)tempData.Temp)
                {
                    Color = SKColor.Parse("#0077C0"), // Optional: set the color of the chart line
                    Label = tempData.DateOfReading.ToShortDateString(),
                    ValueLabel = tempData.Temp.ToString()
                });
            }

            // Create a line chart with the chart data
            var chart = new LineChart
            {
                Entries = chartData,
                LineMode = LineMode.Straight, // Optional: set the line mode (straight or curved)
                PointMode = PointMode.Circle, // Optional: set the point mode (none, circle, square, diamond, or triangle)                
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelTextSize = 40
            };
           

            return chart;
             */
        }

        /*
        private Chart GenerateLightChart()
        {
            var chartData = new List<ChartEntry>();

            // Mock database with sample data
            var mockData = new List<TemperatureData>
            {
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 200, DateOfReading = new DateTime(2023, 4, 1) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 150, DateOfReading = new DateTime(2023, 4, 2) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 175, DateOfReading = new DateTime(2023, 4, 3) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 160, DateOfReading = new DateTime(2023, 4, 4) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 180, DateOfReading = new DateTime(2023, 4, 5) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 170, DateOfReading = new DateTime(2023, 4, 6) },
                new TemperatureData { Id = Guid.NewGuid().ToString(), Temp = 190, DateOfReading = new DateTime(2023, 4, 7) }
            };

            // Iterate through the mock data and add it to the chart data list
            foreach (var lightData in mockData)
            {
                chartData.Add(new ChartEntry((float)lightData.Temp)
                {
                    Color = SKColor.Parse("#FFCE00"), // Optional: set the color of the chart line
                    Label = lightData.DateOfReading.ToShortDateString(),
                    ValueLabel = lightData.Temp.ToString()
                });
            }

            // Create a line chart with the chart data
            var chart = new LineChart
            {
                Entries = chartData,
                LineMode = LineMode.Straight, // Optional: set the line mode (straight or curved)
                PointMode = PointMode.Circle, // Optional: set the point mode (none, circle, square, diamond, or triangle)
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelTextSize = 40
            };

            return chart;
        }
        */
    }
}
