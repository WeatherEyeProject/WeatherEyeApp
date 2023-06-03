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
        AirQualityDetailsViewModel _viewModel;

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

        
    }
}