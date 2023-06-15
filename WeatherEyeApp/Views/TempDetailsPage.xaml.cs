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
    public partial class TempDetailsPage : ContentPage
    {
        readonly TempDetailsViewModel _viewModel;

        public TempDetailsPage()
        {
            InitializeComponent();
            
            BindingContext = _viewModel = new TempDetailsViewModel();

        }

        public void HandleToggled(object sender, ToggledEventArgs e)
        {
            _viewModel.IsDayNightMode = e.Value;
            _viewModel.TempPlotModel = _viewModel.GenerateSingleChart(_viewModel.IsDayNightMode, "#d300a0", "Temperature °C", _viewModel.TempDB);
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

    }
}