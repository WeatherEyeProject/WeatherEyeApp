using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherEyeApp.Models;
using WeatherEyeApp.ViewModels;
using WeatherEyeApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microcharts;

namespace WeatherEyeApp.Views
{
    public partial class HumidityDetailsPage : ContentPage
    {
        readonly HumidityDetailsViewModel _viewModel;

        public HumidityDetailsPage()
        {
            InitializeComponent();
            
            BindingContext = _viewModel = new HumidityDetailsViewModel();

        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        public void HandleToggled(object sender, ToggledEventArgs e)
        {
            _viewModel.IsDayNightMode = e.Value;
            _viewModel.HumidityPlotModel = _viewModel.GenerateSingleChart(_viewModel.IsDayNightMode, "#799eb9", "Humidity %", _viewModel.HumidityDB);
        }


    }
}