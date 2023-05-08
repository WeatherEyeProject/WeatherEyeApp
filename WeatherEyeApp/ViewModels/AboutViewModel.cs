using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WeatherEyeApp.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "Welcome";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("http://weathereye.pl"));
        }

        public ICommand OpenWebCommand { get; }
    }
}