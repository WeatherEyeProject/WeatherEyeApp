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

        
    }
}