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
                HumidityPlotModel = GenerateHumidityChart();
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

                HumidityDB.Clear();
                var temps = await tempService.GetDataByDateAsync(tempSensorUrl, selectedDate1, selectedDate2);
                if(temps.Count() == 0)
                {
                    var latestDate = latest.s2.date;
                    temps = await tempService.GetDataByDateAsync(tempSensorUrl, latestDate, latestDate);
                }
                var sortedtemps = temps.OrderBy(aq => aq.date).ToList();
                foreach (var temp in sortedtemps)
                {
                    HumidityDB.Add(temp);
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

        private PlotModel GenerateHumidityChart()
        {
            var model = new PlotModel();
            var linePm2_5 = new LineSeries()
            {
                Color = OxyColor.Parse("#799eb9"),
                MarkerType = MarkerType.Circle,
                SeriesGroupName = "Humidity"
            };

            foreach (var entry in HumidityDB)
            {
                var dataPoint = new DataPoint(DateTimeAxis.ToDouble(entry.date), (double)entry.value);
                linePm2_5.Points.Add(dataPoint);
            }

            model.Series.Add(linePm2_5);

            model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Date" });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Humidity %" });


            return model;

        }

    }
}
