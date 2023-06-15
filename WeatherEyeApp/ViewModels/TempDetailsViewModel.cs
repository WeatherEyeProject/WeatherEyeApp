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
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

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

        private PlotModel tempPlotModel;
        public PlotModel TempPlotModel
        {
            get => tempPlotModel;
            set
            {
                if (tempPlotModel != value)
                {
                    tempPlotModel = value;
                    OnPropertyChanged(nameof(TempPlotModel));
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

        private bool isDayNightMode;
        public bool IsDayNightMode
        {
            get => isDayNightMode;
            set
            {
                if(isDayNightMode != value)
                {
                    isDayNightMode = value;
                    OnPropertyChanged(nameof(IsDayNightMode));
                }
            }
        }

        public TempDetailsViewModel()
        {
            Title = "Temp Details";
            tempService = new SensorService<SensorsData>();
            latestService = new LatestDataSensorService();
            TempDB = new ObservableCollection<SensorsData>();
            LoadTempCommand = new Command(async () => await ExecuteLoadTempByDateCommand());
            LoadTempByDateCommand = new Command(async () => await ExecuteLoadTempByDateCommand());

            TempDB.CollectionChanged += OnTempCollectionChanged;
            currentTemp = "0°C";
        }

        private void OnTempCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                TempPlotModel = null;
            }
            else
            {
                if(TempDB.Count() > 0)
                {
                    TempPlotModel = GenerateSingleChart(isDayNightMode, "#FF9900", "Temperature °C", TempDB);
                }
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            LoadTempCommand.Execute(null);
        }

        



        async Task ExecuteLoadTempByDateCommand()
        {
            IsBusy = true;

            try
            {
                var latest = await latestService.RefreshDataAsync();
                if (latest.s1 != null)
                {
                    CurrentTemp = latest.s1.value.ToString() + "°C";
                }

                var temps = await tempService.GetDataByDateAsync(tempSensorUrl, selectedDate1.AddDays(-1), selectedDate2);
                if(temps != null)
                {
                    if (temps.Count() == 0)
                    {
                        if (TempDB.Count() != 0)
                        {
                            return;
                        }
                        else
                        {
                            var latestDate = latest.s1.date;
                            temps = await tempService.GetDataByDateAsync(tempSensorUrl, latestDate, latestDate);
                        }

                    }
                    else
                    {
                        TempDB.Clear();
                    }
                    var sortedtemps = temps.OrderBy(aq => aq.date).ToList();
                    foreach (var temp in sortedtemps)
                    {
                        TempDB.Add(temp);
                    }
                }                
                //FillDBWithMockData(TempDB);

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

        

    }
}
