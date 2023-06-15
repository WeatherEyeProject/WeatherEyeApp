using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using WeatherEyeApp.Models;
using WeatherEyeApp.Services;
using Xamarin.Forms;

namespace WeatherEyeApp.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        
        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public void FillDBWithMockData(ObservableCollection<SensorsData> dataBase)
        {
            dataBase.Clear();

            // Generate mock data for testing
            var random = new Random();
            var startDate = DateTime.Now;


            for (int i = 0; i < 40; i++)
            {
                var currentDate = startDate;
                var randomHour = random.Next(0, 24);
                currentDate = currentDate.AddHours(randomHour);
                var value = random.Next(10, 30); // Generate a random value within the range 10 to 30

                var sensorsData = new SensorsData
                {
                    date = currentDate,
                    value = value
                };

                dataBase.Add(sensorsData);
            }

            // Sort the collection by date in ascending order
            var sortedTemps = dataBase.OrderBy(data => data.date).ToList();
            dataBase.Clear();

            foreach (var temp in sortedTemps)
            {
                dataBase.Add(temp);
            }
        }

        public PlotModel GenerateSingleChart(bool daynight, string defaultColor, string title, ObservableCollection<SensorsData> dataBase)
        {
            var model = new PlotModel();

            if (!daynight)
            {
                var line = new LineSeries()
                {
                    Color = OxyColor.Parse(defaultColor),
                    MarkerType = MarkerType.Circle,
                    SeriesGroupName = "Line Series"
                };

                foreach (var entry in dataBase)
                {
                    var dataPoint = new DataPoint(DateTimeAxis.ToDouble(entry.date), (double)entry.value);
                    line.Points.Add(dataPoint);
                }

                model.Series.Add(line);
            }
            else
            {
                List<ObservableCollection<SensorsData>> DayNightList = new List<ObservableCollection<SensorsData>>
                {
                    new ObservableCollection<SensorsData>()
                };
                var lastEntryDate = dataBase[0].date;
                int i = 0;
                // Iterate over each hour in the date
                foreach (var entry in dataBase)
                {
                    if (entry.date.Hour >= 6 && entry.date.Hour < 22)
                    {
                        //day
                        if (lastEntryDate.Hour < 6 || lastEntryDate.Hour >= 22)
                        {
                            //new list
                            DayNightList.Add(new ObservableCollection<SensorsData>());
                            DayNightList[i].Add(entry);
                            i++;
                        }
                        DayNightList[i].Add(entry); //add entry to list
                    }
                    else
                    {
                        //night
                        if (lastEntryDate.Hour >= 6 && lastEntryDate.Hour < 22)
                        {
                            //new list
                            DayNightList.Add(new ObservableCollection<SensorsData>());
                            DayNightList[i].Add(entry); //add entry to list
                            i++;
                        }
                        DayNightList[i].Add(entry); //add entry to list
                    }

                    lastEntryDate = entry.date;
                }

                foreach (var list in DayNightList)
                {
                    var seriesColor = "#397ACD";    //default night
                    if (list[0].date.Hour >= 6 && list[0].date.Hour < 22)
                    { //change color to day
                        seriesColor = "#fcc111";
                    }

                    var lineSeries = new LineSeries
                    {
                        Color = OxyColor.Parse(seriesColor),
                        MarkerType = MarkerType.Circle
                    };

                    foreach (var entry in list)
                    {
                        var dataPoint = new DataPoint(DateTimeAxis.ToDouble(entry.date), (double)entry.value);
                        lineSeries.Points.Add(dataPoint);
                    }
                    model.Series.Add(lineSeries);

                }
            }


            model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Date", MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = OxyColor.Parse("#D3D3D3")});
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = title, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = OxyColor.Parse("#D3D3D3") });

            return model;

        }

        public PlotModel GenerateDoubleChart(bool daynight, string defaultColor1, string defaultColor2, string title, ObservableCollection<SensorsData> dataBase1, ObservableCollection<SensorsData> dataBase2)
        {
            var model = new PlotModel();

            if (!daynight)
            {
                var line1 = new LineSeries()
                {
                    Color = OxyColor.Parse(defaultColor1),
                    MarkerType = MarkerType.Circle,
                    SeriesGroupName = "Line Series"
                };

                foreach (var entry in dataBase1)
                {
                    var dataPoint = new DataPoint(DateTimeAxis.ToDouble(entry.date), (double)entry.value);
                    line1.Points.Add(dataPoint);
                }

                var line2 = new LineSeries()
                {
                    Color = OxyColor.Parse(defaultColor2),
                    MarkerType = MarkerType.Circle,
                    SeriesGroupName = "Line Series"
                };

                foreach (var entry in dataBase2)
                {
                    var dataPoint = new DataPoint(DateTimeAxis.ToDouble(entry.date), (double)entry.value);
                    line2.Points.Add(dataPoint);
                }

                model.Series.Add(line1);
                model.Series.Add(line2);
            }
            else
            {
                List<ObservableCollection<SensorsData>> DayNightList1 = new List<ObservableCollection<SensorsData>>();
                List<ObservableCollection<SensorsData>> DayNightList2 = new List<ObservableCollection<SensorsData>>();
                DayNightList1.Add(new ObservableCollection<SensorsData>());
                DayNightList2.Add(new ObservableCollection<SensorsData>());
                int i = 0;
                // Iterate over each hour in the date
                if(dataBase1.Count() > 0)
                {
                    var lastEntryDate = dataBase1[0].date;
                    foreach (var entry in dataBase1)
                    {
                        if (entry.date.Hour >= 6 && entry.date.Hour < 22)
                        {
                            //day
                            if (lastEntryDate.Hour < 6 || lastEntryDate.Hour >= 22)
                            {
                                //new list
                                DayNightList1.Add(new ObservableCollection<SensorsData>());
                                DayNightList1[i].Add(entry); //add entry to list
                                i++;
                            }
                            DayNightList1[i].Add(entry); //add entry to list
                        }
                        else
                        {
                            //night
                            if (lastEntryDate.Hour >= 6 && lastEntryDate.Hour < 22)
                            {
                                //new list
                                DayNightList1.Add(new ObservableCollection<SensorsData>());
                                DayNightList1[i].Add(entry); //add entry to list
                                i++;
                            }
                            DayNightList1[i].Add(entry); //add entry to list
                        }

                        lastEntryDate = entry.date;
                    }

                    foreach (var list in DayNightList1)
                    {
                        var seriesColor = "#006fff";    //default night
                        if (list[0].date.Hour >= 6 && list[0].date.Hour < 22)
                        { //change color to day
                            seriesColor = "#ffe900";
                        }

                        var lineSeries = new LineSeries
                        {
                            Color = OxyColor.Parse(seriesColor),
                            MarkerType = MarkerType.Circle
                        };

                        foreach (var entry in list)
                        {
                            var dataPoint = new DataPoint(DateTimeAxis.ToDouble(entry.date), (double)entry.value);
                            lineSeries.Points.Add(dataPoint);
                        }
                        model.Series.Add(lineSeries);

                    }
                }

                if(dataBase2.Count() > 0)
                {
                    var lastEntryDate = dataBase2[0].date;

                    i = 0;

                    foreach (var entry in dataBase2)
                    {
                        if (entry.date.Hour >= 6 && entry.date.Hour < 22)
                        {
                            //day
                            if (lastEntryDate.Hour < 6 || lastEntryDate.Hour >= 22)
                            {
                                //new list
                                DayNightList2.Add(new ObservableCollection<SensorsData>());
                                DayNightList2[i].Add(entry); //add entry to list
                                i++;
                            }
                            DayNightList2[i].Add(entry); //add entry to list
                        }
                        else
                        {
                            //night
                            if (lastEntryDate.Hour >= 6 && lastEntryDate.Hour < 22)
                            {
                                //new list
                                DayNightList2.Add(new ObservableCollection<SensorsData>());
                                DayNightList2[i].Add(entry); //add entry to list
                                i++;
                            }
                            DayNightList2[i].Add(entry); //add entry to list
                        }

                        lastEntryDate = entry.date;
                    }

                    foreach (var list in DayNightList2)
                    {
                        var seriesColor = "#0700d4";    //default night2
                        if (list[0].date.Hour >= 6 && list[0].date.Hour < 22)
                        { //change color to day
                            seriesColor = "#c4b300";
                        }

                        var lineSeries = new LineSeries
                        {
                            Color = OxyColor.Parse(seriesColor),
                            MarkerType = MarkerType.Circle
                        };

                        foreach (var entry in list)
                        {
                            var dataPoint = new DataPoint(DateTimeAxis.ToDouble(entry.date), (double)entry.value);
                            lineSeries.Points.Add(dataPoint);
                        }
                        model.Series.Add(lineSeries);

                    }
                }

                
            }


            model.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Title = "Date", MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = OxyColor.Parse("#D3D3D3")});
            model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = title, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = OxyColor.Parse("#D3D3D3") });

            return model;

        }
    }
}
