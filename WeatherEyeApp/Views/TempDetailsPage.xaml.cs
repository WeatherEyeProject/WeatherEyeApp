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
        public Chart AirQualityChart;

        public AirQualityDetailsPage()
        {
            InitializeComponent();
            
            BindingContext = _viewModel = new AirQualityDetailsViewModel();

            AirQualityChart = _viewModel.AirQualityChart;
        }

        //public object AirQualityChart { get; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        
    }
}