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
        public ObservableCollection<SensorsData> RainDB { get; set; }
        //private string discreteRainSensorUrl = "http://weathereye.pl/api/sensors/s11";
        private string valueRainSensorUrl = "http://weathereye.pl/api/sensors/s10";
        public Command LoadRainCommand { get; }
        public Command LoadRainByDateCommand { get; }
        private SensorService<SensorsData> rainService;
        private LatestDataSensorService latestService;
        private string currentRain;
        public string CurrentRain 
        {
            get => currentRain;
            set
            {
                if(currentRain != value)
                {
                    currentRain = value;
                    OnPropertyChanged(nameof(CurrentRain));
                }
            } 
        }
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

        private DateTime selectedDate1 = DateTime.Now;
        public DateTime SelectedDate1
        {
            get => selectedDate1;
            set
            {
                if (selectedDate1 != value)
                {
                    selectedDate1 = value;
                    OnPropertyChanged(nameof(SelectedDate1));
                }
            }
        }

        private DateTime selectedDate2 = DateTime.Now;
        public DateTime SelectedDate2
        {
            get => selectedDate2;
            set
            {
                if (selectedDate2 != value)
                {
                    selectedDate2 = value;
                    OnPropertyChanged(nameof(SelectedDate2));
                }
            }
        }

        public RainDetailsViewModel()
        {
            Title = "Rain Details";
            rainService = new SensorService<SensorsData>();
            latestService = new LatestDataSensorService();
            RainDB = new ObservableCollection<SensorsData>();
            LoadRainCommand = new Command(async () => await ExecuteLoadRainCommand());
            LoadRainByDateCommand = new Command(async () => await ExecuteLoadRainByDateCommand());

            RainDB.CollectionChanged += OnRainCollectionChanged;
            currentRain = "0mm";
        }

        private void OnRainCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Reset)
            {
                RainChart = null;
            }
            else
            {
                RainChart = GenerateRainChart();
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            LoadRainCommand.Execute(null);
        }


        async Task ExecuteLoadRainCommand()
        {
            IsBusy = true;

            try
            {
                RainDB.Clear();
                var temps = await rainService.RefreshDataAsync(valueRainSensorUrl);
                foreach (var temp in temps)
                {
                    RainDB.Add(temp);
                }
                //CurrentRain = RainDB.Select(r => r.value).ToList().Last().ToString() + "mm";
                var latest = await latestService.RefreshDataAsync();
                CurrentRain = latest.s10.value.ToString() + "mm";
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

        async Task ExecuteLoadRainByDateCommand()
        {
            IsBusy = true;

            try
            {
                RainDB.Clear();
                var temps = await rainService.GetDataByDateAsync(valueRainSensorUrl, selectedDate1, selectedDate2);              
                foreach (var temp in temps)
                {
                    RainDB.Add(temp);
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
                Entries = RainDB.Select(r => new ChartEntry((float)r.value) { Label = r.date.ToString("dd/MM"), ValueLabel = r.value.ToString() + "mm", Color = SKColor.Parse("#0077C0") }),
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
