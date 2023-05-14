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
    public class AirQualityDetailsViewModel : BaseViewModel
    {
        public ObservableCollection<SensorsData> AirQualityDB { get; set; }
        private string airQualitySensorUrl = "http://weathereye.pl/api/sensors/s8";
        public Command LoadAirQualityCommand { get; }
        public Command LoadAirQualityByDateCommand { get; }
        private SensorService<SensorsData> airQualityService;
        private LatestDataSensorService latestService;
        private string currentAirQuality;
        public string CurrentAirQuality
        {
            get => currentAirQuality;
            set
            {
                if (currentAirQuality != value)
                {
                    currentAirQuality = value;
                    OnPropertyChanged(nameof(CurrentAirQuality));
                }
            }
        }
        private Chart airQualityChart;
        public Chart AirQualityChart
        {
            get => airQualityChart;
            set
            {
                if (airQualityChart != value)
                {
                    airQualityChart = value;
                    OnPropertyChanged(nameof(AirQualityChart));
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

        public AirQualityDetailsViewModel()
        {
            Title = "AirQuality Details";
            airQualityService = new SensorService<SensorsData>();
            latestService = new LatestDataSensorService();
            AirQualityDB = new ObservableCollection<SensorsData>();
            LoadAirQualityCommand = new Command(async () => await ExecuteLoadAirQualityCommand());
            LoadAirQualityByDateCommand = new Command(async () => await ExecuteLoadAirQualityByDateCommand());

            AirQualityDB.CollectionChanged += OnAirQualityCollectionChanged;
            currentAirQuality = "0µ/m³";
        }

        private void OnAirQualityCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                AirQualityChart = null;
            }
            else
            {
                AirQualityChart = GenerateAirQualityChart();
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            LoadAirQualityCommand.Execute(null);
        }


        async Task ExecuteLoadAirQualityCommand()
        {
            IsBusy = true;

            try
            {
                AirQualityDB.Clear();
                var airQualitys = await airQualityService.RefreshDataAsync(airQualitySensorUrl);
                foreach (var airQuality in airQualitys)
                {
                    AirQualityDB.Add(airQuality);
                }
                //CurrentAirQuality = AirQualityDB.Select(r => r.value).ToList().Last().ToString() + "mm";
                var latest = await latestService.RefreshDataAsync();
                CurrentAirQuality = latest.s10.value.ToString() + "mm";
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

        async Task ExecuteLoadAirQualityByDateCommand()
        {
            IsBusy = true;

            try
            {
                AirQualityDB.Clear();
                var airQualitys = await airQualityService.GetDataByDateAsync(airQualitySensorUrl, selectedDate1, selectedDate2);
                foreach (var airQuality in airQualitys)
                {
                    AirQualityDB.Add(airQuality);
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

        private Chart GenerateAirQualityChart()
        {
            var lineChart = new LineChart()
            {
                Entries = AirQualityDB.Select(r => new ChartEntry((float)r.value) { Label = r.date.ToString("dd/MM"), ValueLabel = r.value.ToString() + "µ/m³", Color = SKColor.Parse("#799eb9") }),
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
