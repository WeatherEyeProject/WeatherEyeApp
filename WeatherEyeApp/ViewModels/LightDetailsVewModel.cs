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
    public class LightDetailsViewModel : BaseViewModel
    {
        public ObservableCollection<SensorsData> LightUVDB { get; set; }
        public ObservableCollection<SensorsData> LightLuxDB { get; set; }
        private readonly string luxLightSensorUrl = "http://weathereye.pl/api/sensors/s5";
        private readonly string uvLightSensorUrl = "http://weathereye.pl/api/sensors/s6";
        public Command LoadLightCommand { get; }
        public Command LoadLightByDateCommand { get; }
        private readonly SensorService<SensorsData> lightService;
        private readonly LatestDataSensorService latestService;
        private string currentLightUV;
        public string CurrentLightUV
        {
            get => currentLightUV;
            set
            {
                if (currentLightUV != value)
                {
                    currentLightUV = value;
                    OnPropertyChanged(nameof(CurrentLightUV));
                }
            }
        }
        private string currentLightLux;
        public string CurrentLightLux
        {
            get => currentLightLux;
            set
            {
                if (currentLightLux != value)
                {
                    currentLightLux = value;
                    OnPropertyChanged(nameof(CurrentLightLux));
                }
            }
        }
        private PlotModel lightUVPlotModel;
        public PlotModel LightUVPlotModel
        {
            get => lightUVPlotModel;
            set
            {
                if (lightUVPlotModel != value)
                {
                    lightUVPlotModel = value;
                    OnPropertyChanged(nameof(LightUVPlotModel));
                }
            }
        }

        private PlotModel lightLuxPlotModel;
        public PlotModel LightLuxPlotModel
        {
            get => lightLuxPlotModel;
            set
            {
                if (lightLuxPlotModel != value)
                {
                    lightLuxPlotModel = value;
                    OnPropertyChanged(nameof(LightLuxPlotModel));
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

        public LightDetailsViewModel()
        {
            Title = "Light Details";
            lightService = new SensorService<SensorsData>();
            latestService = new LatestDataSensorService();
            LightUVDB = new ObservableCollection<SensorsData>();
            LightLuxDB = new ObservableCollection<SensorsData>();
            LoadLightCommand = new Command(async () => await ExecuteLoadLightByDateCommand());
            LoadLightByDateCommand = new Command(async () => await ExecuteLoadLightByDateCommand());

            LightUVDB.CollectionChanged += OnLightCollectionChanged;
            LightLuxDB.CollectionChanged += OnLightCollectionChanged;
            currentLightUV = "0UV";
            currentLightLux = "0Lux";
        }

        private void OnLightCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                LightUVPlotModel = null;
                LightLuxPlotModel = null;
            }
            else
            {
                if(LightLuxDB.Count() > 0 && LightUVDB.Count() > 0)
                {
                    LightUVPlotModel = GenerateSingleChart(IsDayNightMode, "#ffef5f", "Light UV", LightUVDB);
                    LightLuxPlotModel = GenerateSingleChart(IsDayNightMode, "#fcc111", "Light Lux", LightLuxDB);
                } 
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            LoadLightCommand.Execute(null);
        }

        async Task ExecuteLoadLightByDateCommand()
        {
            IsBusy = true;

            try
            {
                var latest = await latestService.RefreshDataAsync();
                if(latest.s5 != null)
                {
                    CurrentLightLux = latest.s5.value.ToString() + "Lux";
                }
                if(latest.s6 != null)
                {
                    CurrentLightUV = latest.s6.value.ToString() + "UV";
                }

                
                var lightsUV = await lightService.GetDataByDateAsync(uvLightSensorUrl, selectedDate1, selectedDate2);
                var lightsLux = await lightService.GetDataByDateAsync(luxLightSensorUrl, selectedDate1, selectedDate2);
                if(lightsLux != null && lightsUV != null)
                {
                    if (lightsLux.Count() == 0 || lightsUV.Count() == 0)
                    {
                        if (LightLuxDB.Count() == 0 || LightUVDB.Count() == 0)
                        {   //get data from last day available
                            var latestDate = latest.s6.date;
                            lightsUV = await lightService.GetDataByDateAsync(uvLightSensorUrl, latestDate, latestDate);
                            lightsLux = await lightService.GetDataByDateAsync(luxLightSensorUrl, latestDate, latestDate);
                        }
                        else
                        {
                            return;
                        }

                    }
                    else
                    {
                        LightUVDB.Clear();
                        LightLuxDB.Clear();
                    }
                    var sortedListUV = lightsUV.OrderBy(aq => aq.date).ToList();
                    var sortedListLux = lightsLux.OrderBy(aq => aq.date).ToList();
                    foreach (var lght in sortedListUV)
                    {
                        LightUVDB.Add(lght);
                    }
                    foreach (var lght in sortedListLux)
                    {
                        LightLuxDB.Add(lght);
                    }
                }                
                //FillDBWithMockData(LightLuxDB);
                //FillDBWithMockData(LightUVDB);

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
