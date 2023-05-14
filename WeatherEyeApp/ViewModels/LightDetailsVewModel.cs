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
    public class LightDetailsViewModel : BaseViewModel
    {
        public ObservableCollection<SensorsData> LightDB { get; set; }
        //private string luxLightSensorUrl = "http://weathereye.pl/api/sensors/s5";
        private string uvLightSensorUrl = "http://weathereye.pl/api/sensors/s6";
        public Command LoadLightCommand { get; }
        public Command LoadLightByDateCommand { get; }
        private SensorService<SensorsData> lightService;
        private LatestDataSensorService latestService;
        private string currentLight;
        public string CurrentLight
        {
            get => currentLight;
            set
            {
                if (currentLight != value)
                {
                    currentLight = value;
                    OnPropertyChanged(nameof(CurrentLight));
                }
            }
        }
        private Chart lightChart;
        public Chart LightChart
        {
            get => lightChart;
            set
            {
                if (lightChart != value)
                {
                    lightChart = value;
                    OnPropertyChanged(nameof(LightChart));
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

        public LightDetailsViewModel()
        {
            Title = "Light Details";
            lightService = new SensorService<SensorsData>();
            latestService = new LatestDataSensorService();
            LightDB = new ObservableCollection<SensorsData>();
            LoadLightCommand = new Command(async () => await ExecuteLoadLightCommand());
            LoadLightByDateCommand = new Command(async () => await ExecuteLoadLightByDateCommand());

            LightDB.CollectionChanged += OnLightCollectionChanged;
            currentLight = "0UV";
        }

        private void OnLightCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                LightChart = null;
            }
            else
            {
                LightChart = GenerateLightChart();
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            LoadLightCommand.Execute(null);
        }


        async Task ExecuteLoadLightCommand()
        {
            IsBusy = true;

            try
            {
                LightDB.Clear();
                //var temps = await lightService.GetDataByDateAsync(uvLightSensorUrl, selectedDate1, selectedDate2);
                var temps = await lightService.RefreshDataAsync(uvLightSensorUrl);
                foreach (var temp in temps)
                {
                    LightDB.Add(temp);
                }
                //CurrentLight = LightDB.Select(r => r.value).ToList().Last().ToString() + "mm";
                var latest = await latestService.RefreshDataAsync();
                CurrentLight = latest.s10.value.ToString() + "mm";
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

        async Task ExecuteLoadLightByDateCommand()
        {
            IsBusy = true;

            try
            {
                LightDB.Clear();
                var temps = await lightService.GetDataByDateAsync(uvLightSensorUrl, selectedDate1, selectedDate2);
                foreach (var temp in temps)
                {
                    LightDB.Add(temp);
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

        private Chart GenerateLightChart()
        {
            var lineChart = new LineChart()
            {
                Entries = LightDB.Select(r => new ChartEntry((float)r.value) { Label = r.date.ToString("dd/MM"), ValueLabel = r.value.ToString() + "UV", Color = SKColor.Parse("#ffef5f") }),
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
