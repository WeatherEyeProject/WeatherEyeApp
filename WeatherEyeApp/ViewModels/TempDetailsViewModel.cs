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
    public class TempDetailsViewModel : BaseViewModel
    {
        public ObservableCollection<SensorsData> TempDB { get; set; }
        private readonly string tempSensorUrl = "http://weathereye.pl/api/sensors/s1";
        public Command LoadTempCommand { get; }
        public Command LoadTempByDateCommand { get; }
        private readonly SensorService<SensorsData> tempService;
        private readonly LatestDataSensorService latestService;
        private string currentTemp;
        public string CurrentTemp
        {
            get => currentTemp;
            set
            {
                if (currentTemp != value)
                {
                    currentTemp = value;
                    OnPropertyChanged(nameof(CurrentTemp));
                }
            }
        }
        private Chart tempChart;
        public Chart TempChart
        {
            get => tempChart;
            set
            {
                if (tempChart != value)
                {
                    tempChart = value;
                    OnPropertyChanged(nameof(TempChart));
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

        public TempDetailsViewModel()
        {
            Title = "Temp Details";
            tempService = new SensorService<SensorsData>();
            latestService = new LatestDataSensorService();
            TempDB = new ObservableCollection<SensorsData>();
            LoadTempCommand = new Command(async () => await ExecuteLoadTempCommand());
            LoadTempByDateCommand = new Command(async () => await ExecuteLoadTempByDateCommand());

            TempDB.CollectionChanged += OnTempCollectionChanged;
            currentTemp = "0°C";
        }

        private void OnTempCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                TempChart = null;
            }
            else
            {
                TempChart = GenerateTempChart();
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            LoadTempCommand.Execute(null);
        }


        async Task ExecuteLoadTempCommand()
        {
            IsBusy = true;

            try
            {
                TempDB.Clear();
                var temps = await tempService.RefreshDataAsync(tempSensorUrl);
                foreach (var temp in temps)
                {
                    TempDB.Add(temp);
                }
                //CurrentTemp = TempDB.Select(r => r.value).ToList().Last().ToString() + "mm";
                var latest = await latestService.RefreshDataAsync();
                CurrentTemp = latest.s1.value.ToString() + "°C";
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

        async Task ExecuteLoadTempByDateCommand()
        {
            IsBusy = true;

            try
            {
                TempDB.Clear();
                var temps = await tempService.GetDataByDateAsync(tempSensorUrl, selectedDate1, selectedDate2);
                foreach (var temp in temps)
                {
                    TempDB.Add(temp);
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
            var lineChart = new LineChart()
            {
                Entries = TempDB.Select(r => new ChartEntry((float)r.value) { Label = r.date.ToString("dd/MM"), ValueLabel = r.value.ToString() + "°C", Color = SKColor.Parse("#5bb325") }),
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
