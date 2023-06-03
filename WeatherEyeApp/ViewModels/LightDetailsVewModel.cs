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
        private string luxLightSensorUrl = "http://weathereye.pl/api/sensors/s5";
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
                LightUVPlotModel = GenerateLineChart("uv", "#ffef5f");
                LightLuxPlotModel = GenerateLineChart("lux", "#fcc111");
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
                LightUVDB.Clear();
                LightLuxDB.Clear();
                var lightsUV = await lightService.GetDataByDateAsync(uvLightSensorUrl, selectedDate1, selectedDate2);
                var lightsLux = await lightService.GetDataByDateAsync(luxLightSensorUrl, selectedDate1, selectedDate2);
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
                var latest = await latestService.RefreshDataAsync();
                CurrentLightUV = latest.s6.value.ToString() + "UV";
                CurrentLightLux = latest.s5.value.ToString() + "Lux";
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

        private PlotModel GenerateLineChart(string lighttype, string color)
        {
            var model = new PlotModel();
            var linelight = new LineSeries()
            {
                Color = OxyColor.Parse(color),
                MarkerType = MarkerType.Circle,
                SeriesGroupName = "LightUV"
            };

            if(lighttype == "uv")
            {
                foreach (var entry in LightUVDB)
                {
                    var dataPoint = new DataPoint(DateTimeAxis.ToDouble(entry.date), (double)entry.value);
                    linelight.Points.Add(dataPoint);
                }

                model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Light UV" });
            }
            else
            {
                foreach (var entry in LightLuxDB)
                {
                    var dataPoint = new DataPoint(DateTimeAxis.ToDouble(entry.date), (double)entry.value);
                    linelight.Points.Add(dataPoint);
                }

                model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Light Lux" });
            }

            model.Series.Add(linelight);

            model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Date" });


            return model;
        }

    }
}
