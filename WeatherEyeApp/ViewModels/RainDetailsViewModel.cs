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
    public class RainDetailsViewModel : BaseViewModel
    {
        public ObservableCollection<SensorsData> RainDB { get; set; }
        public ObservableCollection<SensorsData> RainDiscreteDB { get; set; }
        private string discreteRainSensorUrl = "http://weathereye.pl/api/sensors/s11";
        private readonly string valueRainSensorUrl = "http://weathereye.pl/api/sensors/s10";
        public Command LoadRainCommand { get; }
        public Command LoadRainByDateCommand { get; }
        private readonly SensorService<SensorsData> rainService;
        private readonly LatestDataSensorService latestService;
        private string currentRainmm;
        public string CurrentRainmm 
        {
            get => currentRainmm;
            set
            {
                if(currentRainmm != value)
                {
                    currentRainmm = value;
                    OnPropertyChanged(nameof(CurrentRainmm));
                }
            } 
        }
        private string currentRainDisc;
        public string CurrentRainDisc
        {
            get => currentRainDisc;
            set
            {
                if (currentRainDisc != value)
                {
                    currentRainDisc = value;
                    OnPropertyChanged(nameof(CurrentRainDisc));
                }
            }
        }

        private PlotModel rainmmPlotModel;
        public PlotModel RainmmPlotModel
        {
            get => rainmmPlotModel;
            set
            {
                if (rainmmPlotModel != value)
                {
                    rainmmPlotModel = value;
                    OnPropertyChanged(nameof(RainmmPlotModel));
                }
            }
        }

        private PlotModel rainDiscPlotModel;
        public PlotModel RainDiscPlotModel
        {
            get => rainDiscPlotModel;
            set
            {
                if (rainDiscPlotModel != value)
                {
                    rainDiscPlotModel = value;
                    OnPropertyChanged(nameof(RainDiscPlotModel));
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
            RainDiscreteDB = new ObservableCollection<SensorsData>();
            LoadRainCommand = new Command(async () => await ExecuteLoadRainByDateCommand());
            LoadRainByDateCommand = new Command(async () => await ExecuteLoadRainByDateCommand());

            RainDB.CollectionChanged += OnRainCollectionChanged;
            RainDiscreteDB.CollectionChanged += OnRainCollectionChanged;
            currentRainmm = "0mm";
            currentRainDisc = "0";
        }

        private void OnRainCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Reset)
            {
                RainmmPlotModel = null;
                RainDiscPlotModel = null;
            }
            else
            {
                RainmmPlotModel = GenerateRainChart("mm", "#799eb9");
                RainDiscPlotModel = GenerateRainChart("disc", "#ceedff");
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            LoadRainCommand.Execute(null);
        }


        async Task ExecuteLoadRainByDateCommand()
        {
            IsBusy = true;

            try
            {
                var latest = await latestService.RefreshDataAsync();
                if(latest.s10 != null)
                {
                    CurrentRainmm = latest.s10.value.ToString() + "mm";
                }
                if (latest.s11 != null)
                {
                    CurrentRainDisc = latest.s11.value.ToString();
                }

                RainDB.Clear();
                RainDiscreteDB.Clear();
                var rainsmm = await rainService.GetDataByDateAsync(valueRainSensorUrl, selectedDate1, selectedDate2);
                var rainsdisc = await rainService.GetDataByDateAsync(discreteRainSensorUrl, selectedDate1, selectedDate2);
                if(rainsmm.Count() == 0 || rainsdisc.Count() == 0)
                {
                    var latestDate = latest.s10.date;
                    rainsmm = await rainService.GetDataByDateAsync(valueRainSensorUrl, latestDate, latestDate);
                    rainsdisc = await rainService.GetDataByDateAsync(discreteRainSensorUrl, latestDate, latestDate);
                }
                var sortedListmm = rainsmm.OrderBy(aq => aq.date).ToList();
                var sortedListdisc = rainsdisc.OrderBy(aq => aq.date).ToList();
                foreach (var rain in sortedListmm)
                {
                    RainDB.Add(rain);
                }
                foreach (var rain in sortedListdisc)
                {
                    RainDiscreteDB.Add(rain);
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

        private PlotModel GenerateRainChart(string raintype, string color)
        {
            var model = new PlotModel();
            var linerain = new LineSeries()
            {
                Color = OxyColor.Parse(color),
                MarkerType = MarkerType.Circle,
                SeriesGroupName = "Rain"
            };

            if (raintype == "mm")
            {
                foreach (var entry in RainDB)
                {
                    var dataPoint = new DataPoint(DateTimeAxis.ToDouble(entry.date), (double)entry.value);
                    linerain.Points.Add(dataPoint);
                }

                model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Rain mm" });
            }
            else
            {
                foreach (var entry in RainDiscreteDB)
                {
                    var dataPoint = new DataPoint(DateTimeAxis.ToDouble(entry.date), (double)entry.value);
                    linerain.Points.Add(dataPoint);
                }

                model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Rain" });
            }

            model.Series.Add(linerain);

            model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Date" });



            return model;

        }

    }
}
