using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using WeatherEyeApp.Models;
using Xamarin.Forms;

namespace WeatherEyeApp.ViewModels
{
    public class TempDetailsViewModel : BaseViewModel
    {
        public ObservableCollection<TemperatureData> Temperatures { get; }
        public Command LoadTempsCommand { get; }
        public Microcharts.Chart TempChart { get; set; }
        public TempDetailsViewModel()
        {
            Title = "Temperature Details";
            Temperatures = new ObservableCollection<TemperatureData>();
            LoadTempsCommand = new Command(async () => await ExecuteLoadTempsCommand());
            
        }
        public void OnAppearing()
        {
            IsBusy = true;
        }

        async Task ExecuteLoadTempsCommand()
        {
            IsBusy = true;

            try
            {
                Temperatures.Clear();
                var temps = await TempData.GetItemsAsync(true);
                foreach(var temp in temps)
                {
                    Temperatures.Add(temp);
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


    }
}
