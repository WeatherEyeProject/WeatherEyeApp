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
    public class PressureDetailsViewModel : BaseViewModel
    {
        public ObservableCollection<SensorsData> PressureDB { get; set; }
        private readonly string tempSensorUrl = "http://weathereye.pl/api/sensors/s3";
        public Command LoadPressureCommand { get; }
        public Command LoadPressureByDateCommand { get; }
        private readonly SensorService<SensorsData> tempService;
        private readonly LatestDataSensorService latestService;
        private string currentPressure;
        public string CurrentPressure
        {
            get => currentPressure;
            set
            {
                if (currentPressure != value)
                {
                    currentPressure = value;
                    OnPropertyChanged(nameof(CurrentPressure));
                }
            }
        }

        private PlotModel tempPlotModel;
        public PlotModel PressurePlotModel
        {
            get => tempPlotModel;
            set
            {
                if (tempPlotModel != value)
                {
                    tempPlotModel = value;
                    OnPropertyChanged(nameof(PressurePlotModel));
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

        public PressureDetailsViewModel()
        {
            Title = "Pressure Details";
            tempService = new SensorService<SensorsData>();
            latestService = new LatestDataSensorService();
            PressureDB = new ObservableCollection<SensorsData>();
            LoadPressureCommand = new Command(async () => await ExecuteLoadPressureByDateCommand());
            LoadPressureByDateCommand = new Command(async () => await ExecuteLoadPressureByDateCommand());

            PressureDB.CollectionChanged += OnPressureCollectionChanged;
            currentPressure = "0 hPa";
        }

        private void OnPressureCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                PressurePlotModel = null;
            }
            else
            {
                PressurePlotModel = GeneratePressureChart();
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            LoadPressureCommand.Execute(null);
        }



        async Task ExecuteLoadPressureByDateCommand()
        {
            IsBusy = true;

            try
            {
                var latest = await latestService.RefreshDataAsync();
                if (latest.s3 != null)
                {
                    CurrentPressure = latest.s3.value.ToString() + "hPa";
                }

                PressureDB.Clear();
                var temps = await tempService.GetDataByDateAsync(tempSensorUrl, selectedDate1, selectedDate2);
                if(temps.Count() == 0)
                {
                    var latestDate = latest.s3.date;
                    temps = await tempService.GetDataByDateAsync(tempSensorUrl, latestDate, latestDate);
                }
                var sortedtemps = temps.OrderBy(aq => aq.date).ToList();
                foreach (var temp in sortedtemps)
                {
                    PressureDB.Add(temp);
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

        private PlotModel GeneratePressureChart()
        {
            var model = new PlotModel();
            var linePm2_5 = new LineSeries()
            {
                Color = OxyColor.Parse("#e93e3a"),
                MarkerType = MarkerType.Circle,
                SeriesGroupName = "Pressure hPa"
            };

            foreach (var entry in PressureDB)
            {
                var dataPoint = new DataPoint(DateTimeAxis.ToDouble(entry.date), (double)entry.value);
                linePm2_5.Points.Add(dataPoint);
            }

            model.Series.Add(linePm2_5);

            model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Date" });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Pressure hPa" });


            return model;

        }

    }
}
