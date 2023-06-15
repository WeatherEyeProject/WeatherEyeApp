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
    public partial class LightDetailsPage : ContentPage
    {
        readonly LightDetailsViewModel _viewModel;
 

        public LightDetailsPage()
        {
            InitializeComponent();
            
            BindingContext = _viewModel = new LightDetailsViewModel();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        public void HandleToggled(object sender, ToggledEventArgs e)
        {
            _viewModel.IsDayNightMode = e.Value;
            _viewModel.LightUVPlotModel = _viewModel.GenerateSingleChart(_viewModel.IsDayNightMode, "#ffef5f", "Light UV", _viewModel.LightUVDB);
            _viewModel.LightLuxPlotModel = _viewModel.GenerateSingleChart(_viewModel.IsDayNightMode, "#fcc111", "Light LUX", _viewModel.LightLuxDB);
        }


    }
}