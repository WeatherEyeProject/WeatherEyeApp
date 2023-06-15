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
    public partial class PressureDetailsPage : ContentPage
    {
        readonly PressureDetailsViewModel _viewModel;

        public PressureDetailsPage()
        {
            InitializeComponent();
            
            BindingContext = _viewModel = new PressureDetailsViewModel();

        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        public void HandleToggled(object sender, ToggledEventArgs e)
        {
            _viewModel.IsDayNightMode = e.Value;
            _viewModel.PressurePlotModel = _viewModel.GenerateSingleChart(_viewModel.IsDayNightMode, "#e93e3a", "Pressure hPa", _viewModel.PressureDB);
        }


    }
}