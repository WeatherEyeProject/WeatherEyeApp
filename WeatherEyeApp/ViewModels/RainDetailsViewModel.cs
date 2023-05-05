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
using System.Collections.Specialized;

namespace WeatherEyeApp.ViewModels
{
    public class RainDetailsViewModel : BaseViewModel
    {
        public ObservableCollection<RainData> Rain { get; set; }
        public Command LoadRainCommand { get; set; }
        private RainService rainService;
        private Chart rainChart;
        public Chart RainChart
        {
            get => rainChart;
            set
            {
                if (rainChart != value)
                {
                    rainChart = value;
                    OnPropertyChanged(nameof(RainChart));
                }
            }
        }

        public RainDetailsViewModel()
        {
            Title = "Rain Details";
            rainService = new RainService();
            Rain = new ObservableCollection<RainData>();
            LoadRainCommand = new Command(async () => await ExecuteLoadRainCommand());
            Rain.CollectionChanged += OnRainCollectionChanged;
            //RainChart = GenerateRainChart();
        }

        private void OnRainCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RainChart = GenerateRainChart();
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }


        async Task ExecuteLoadRainCommand()
        {
            IsBusy = true;

            try
            {
                Rain.Clear();
                var temps = await rainService.RefreshDataAsync();
                //var date = new DateTime(2023, 03, 17);
                //var temps = await rainService.GetRainDataByDateAsync(date);
                foreach (var temp in temps)
                {
                    Rain.Add(temp);
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


        private Chart GenerateRainChart()
        {

            var lineChart = new LineChart()
            {
                Entries = Rain.Select(r => new ChartEntry((float)r.Rain) { Label = r.DateOfReading.ToString("dd/MM"), ValueLabel = r.Rain.ToString() + "mm", Color = SKColor.Parse("#0077C0") }),
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
