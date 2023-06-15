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
    public class HumidityDetailsViewModel : BaseViewModel
    {
        public ObservableCollection<SensorsData> HumidityDB { get; set; }
        private readonly string tempSensorUrl = "http://weathereye.pl/api/sensors/s2";
        public Command LoadHumidityCommand { get; }
        public Command LoadHumidityByDateCommand { get; }
        private readonly SensorService<SensorsData> tempService;
        private readonly LatestDataSensorService latestService;
        private string currentHumidity;
        public string CurrentHumidity
        {
            get => currentHumidity;
            set
            {
                if (currentHumidity != value)
                {
                    currentHumidity = value;
                    OnPropertyChanged(nameof(CurrentHumidity));
                }
            }
        }

        private PlotModel tempPlotModel;
        public PlotModel HumidityPlotModel
        {
            get => tempPlotModel;
            set
            {
                if (tempPlotModel != value)
                {
                    tempPlotModel = value;
                    OnPropertyChanged(nameof(HumidityPlotModel));
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
                if (isDayNightMode != value)
                {
                    isDayNightMode = value;
                    OnPropertyChanged(nameof(IsDayNightMode));
                }
            }
        }

        public HumidityDetailsViewModel()
        {
            Title = "Humidity Details";
            tempService = new SensorService<SensorsData>();
            latestService = new LatestDataSensorService();
            HumidityDB = new ObservableCollection<SensorsData>();
            LoadHumidityCommand = new Command(async () => await ExecuteLoadHumidityByDateCommand());
            LoadHumidityByDateCommand = new Command(async () => await ExecuteLoadHumidityByDateCommand());

            HumidityDB.CollectionChanged += OnHumidityCollectionChanged;
            currentHumidity = "0%";
        }

        private void OnHumidityCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                HumidityPlotModel = null;
            }
            else
            {
                if(HumidityDB.Count() > 0)
                {
                    HumidityPlotModel = GenerateSingleChart(IsDayNightMode, "#799eb9", "Humidity %", HumidityDB);
                }
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            LoadHumidityCommand.Execute(null);
        }



        async Task ExecuteLoadHumidityByDateCommand()
        {
            IsBusy = true;

            try
            {
                var latest = await latestService.RefreshDataAsync();
                if(latest.s2 != null)
                {
                    CurrentHumidity = latest.s2.value.ToString() + "%";
                }

                var hums = await tempService.GetDataByDateAsync(tempSensorUrl, selectedDate1, selectedDate2);
                if(hums != null)
                {
                    if (hums.Count() == 0)
                    {
                        if (HumidityDB.Count() == 0)
                        {
                            var latestDate = latest.s2.date;
                            hums = await tempService.GetDataByDateAsync(tempSensorUrl, latestDate, latestDate);
                        }
                        else
                        {
                            return;
                        }

                    }
                    else
                    {
                        HumidityDB.Clear();
                    }
                    var sortedhums = hums.OrderBy(aq => aq.date).ToList();
                    foreach (var temp in sortedhums)
                    {
                        HumidityDB.Add(temp);
                    }
                }
                //FillDBWithMockData(HumidityDB);

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
