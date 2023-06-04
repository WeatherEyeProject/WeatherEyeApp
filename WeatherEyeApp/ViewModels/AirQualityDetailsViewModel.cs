﻿using Microcharts;
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
    public class AirQualityDetailsViewModel : BaseViewModel
    {
        public ObservableCollection<SensorsData> AirQualityPm2_5 { get; set; }
        public ObservableCollection<SensorsData> AirQualityPm10 { get; set; }
        private readonly string airQualityPm2_5SensorUrl = "http://weathereye.pl/api/sensors/s8";
        private readonly string airQualityPm10SensorUrl = "http://weathereye.pl/api/sensors/s7";
        public Command LoadAirQualityCommand { get; }
        public Command LoadAirQualityByDateCommand { get; }
        private readonly SensorService<SensorsData> airQualityService;
        private readonly LatestDataSensorService latestService;
        private string currentAirQualityPm2_5;
        public string CurrentAirQualityPm2_5
        {
            get => currentAirQualityPm2_5;
            set
            {
                if (currentAirQualityPm2_5 != value)
                {
                    currentAirQualityPm2_5 = value;
                    OnPropertyChanged(nameof(CurrentAirQualityPm2_5));
                }
            }
        }
        private string currentAirQualityPm10;
        public string CurrentAirQualityPm10
        {
            get => currentAirQualityPm10;
            set
            {
                if (currentAirQualityPm10 != value)
                {
                    currentAirQualityPm10 = value;
                    OnPropertyChanged(nameof(CurrentAirQualityPm10));
                }
            }
        }

        private PlotModel airQualityPlotModel;
        public PlotModel AirQualityPlotModel
        {
            get => airQualityPlotModel;
            set
            {
                if(airQualityPlotModel != value)
                {
                    airQualityPlotModel = value;
                    OnPropertyChanged(nameof(AirQualityPlotModel));
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
            AirQualityPm2_5 = new ObservableCollection<SensorsData>();
            AirQualityPm10 = new ObservableCollection<SensorsData>();
            LoadAirQualityCommand = new Command(async () => await ExecuteLoadAirQualityByDateCommand());
            LoadAirQualityByDateCommand = new Command(async () => await ExecuteLoadAirQualityByDateCommand());

            AirQualityPm2_5.CollectionChanged += OnAirQualityCollectionChanged;
            AirQualityPm10.CollectionChanged += OnAirQualityCollectionChanged;
            currentAirQualityPm2_5 = "0µ/m³";
            currentAirQualityPm10 = "0µ/m³";
        }

        private void OnAirQualityCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                AirQualityPlotModel = null;
            }
            else
            {
                AirQualityPlotModel = GenerateLineChart();
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            LoadAirQualityCommand.Execute(null);
        }

        async Task ExecuteLoadAirQualityByDateCommand()
        {
            IsBusy = true;

            try
            {
                var latest = await latestService.RefreshDataAsync();
                if (latest.s8 != null)
                {
                    CurrentAirQualityPm2_5 = latest.s8.value.ToString() + "µ/m³";
                }
                if (latest.s7 != null)
                {
                    CurrentAirQualityPm2_5 = latest.s7.value.ToString() + "µ/m³";
                }

                AirQualityPm2_5.Clear();
                AirQualityPm10.Clear();

                var airQualitysPm2_5 = await airQualityService.GetDataByDateAsync(airQualityPm2_5SensorUrl, selectedDate1, selectedDate2);             
                var airQualitysPm10 = await airQualityService.GetDataByDateAsync(airQualityPm10SensorUrl, selectedDate1, selectedDate2);       
                if (airQualitysPm2_5.Count() == 0 || airQualitysPm10.Count() == 0)
                {
                    var latestDate = latest.s8.date;
                    airQualitysPm2_5 = await airQualityService.GetDataByDateAsync(airQualityPm2_5SensorUrl, latestDate, latestDate);
                    airQualitysPm10 = await airQualityService.GetDataByDateAsync(airQualityPm10SensorUrl, latestDate, latestDate);
                }
                var sortedListPm2_5 = airQualitysPm2_5.OrderBy(aq => aq.date).ToList();
                var sortedListPm10 = airQualitysPm10.OrderBy(aq => aq.date).ToList();
                foreach (var airQuality in sortedListPm2_5)
                {
                    AirQualityPm2_5.Add(airQuality);
                }
                foreach (var airQuality in sortedListPm10)
                {
                    AirQualityPm10.Add(airQuality);
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


        private PlotModel GenerateLineChart()
        {
            var model = new PlotModel();
            var linePm2_5 = new LineSeries()
            {
                Color = OxyColor.Parse("#799eb9"),
                MarkerType = MarkerType.Circle,
                SeriesGroupName = "Pm2.5"
            };

            var linePm10 = new LineSeries()
            {
                Color = OxyColor.Parse("#799ef0"),
                MarkerType = MarkerType.Circle,
                SeriesGroupName = "Pm10"
            };

            foreach(var entry in AirQualityPm2_5)
            {
                var dataPoint = new DataPoint(DateTimeAxis.ToDouble(entry.date), (double) entry.value);
                linePm2_5.Points.Add(dataPoint);
            }

            foreach (var entry in AirQualityPm10)
            {
                var dataPoint = new DataPoint(DateTimeAxis.ToDouble(entry.date), (double)entry.value);
                linePm10.Points.Add(dataPoint);
            }

            model.Series.Add(linePm2_5);
            model.Series.Add(linePm10);

            model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Date" });
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Air Quality µ/m³" });


            return model;
        }

    }
}
