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
    public partial class AirQualityDetailsPage : ContentPage
    {
        readonly AirQualityDetailsViewModel _viewModel;

        public AirQualityDetailsPage()
        {
            InitializeComponent();
            
            BindingContext = _viewModel = new AirQualityDetailsViewModel();

        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        public void HandleToggled(object sender, ToggledEventArgs e)
        {
            _viewModel.IsDayNightMode = e.Value;
            _viewModel.AirQualityPlotModel = _viewModel.GenerateDoubleChart(_viewModel.IsDayNightMode, "#5BB325", "#A7E481", "Air Quality µ/m³", _viewModel.AirQualityPm2_5DB, _viewModel.AirQualityPm10DB);
        }


    }
}