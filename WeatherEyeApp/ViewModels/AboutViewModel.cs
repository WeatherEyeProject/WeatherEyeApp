using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WeatherEyeApp.Models;
using WeatherEyeApp.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WeatherEyeApp.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public LatestData LatestData { get; set; }
        public Command LoadDataCommand { get; }
        private readonly LatestDataSensorService latestDataService;
        private string currentTemp = "15*C";
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
        private string currentUV = "14Lux";
        public string CurrentUV {
            get => currentUV;
            set
            {
                if (currentUV != value)
                {
                    currentUV = value;
                    OnPropertyChanged(nameof(CurrentUV));
                }
            }
        }
        private string currentRain = "0mm";
        public string CurrentRain
        {
            get => currentRain;
            set
            {
                if (currentRain != value)
                {
                    currentRain = value;
                    OnPropertyChanged(nameof(CurrentRain));
                }
            }
        }
        private string currentPm2_5 = "3µ/m³";
        public string CurrentPm2_5 {
            get => currentPm2_5;
            set
            {
                if (currentPm2_5 != value)
                {
                    currentPm2_5 = value;
                    OnPropertyChanged(nameof(CurrentPm2_5));
                }
            }
        }

        public AboutViewModel()
        {
            Title = "Welcome";
            CurrentTemp = "15*C";
            CurrentUV = "14Lux";
            CurrentRain = "0mm";
            CurrentPm2_5 = "3µ/m³";
            LatestData = new LatestData();
            latestDataService = new LatestDataSensorService();
            LoadDataCommand = new Command(async () => await ExecuteLoadDataCommand());
            
        }

        public void OnAppearing()
        {
            IsBusy = true;
            LoadDataCommand.Execute(null);
        }

        async Task ExecuteLoadDataCommand()
        {
            IsBusy = true;

            try
            {
                var temps = await latestDataService.RefreshDataAsync();
                LatestData = temps;
                CurrentRain = LatestData.s11.value.ToString() + "mm";
                CurrentUV = LatestData.s6.value.ToString() + "UV";
                CurrentPm2_5 = LatestData.s8.value.ToString() + "µ/m³";
                CurrentTemp = LatestData.s1.value.ToString() + "°C";
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