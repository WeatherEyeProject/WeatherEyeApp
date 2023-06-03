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
        LightDetailsViewModel _viewModel;
 

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

        
    }
}