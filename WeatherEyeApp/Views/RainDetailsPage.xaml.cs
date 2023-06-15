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
    public partial class RainDetailsPage : ContentPage
    {
        readonly RainDetailsViewModel _viewModel;
 

        public RainDetailsPage()
        {
            InitializeComponent();
            
            BindingContext = _viewModel = new RainDetailsViewModel();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        public void HandleToggled(object sender, ToggledEventArgs e)
        {
            _viewModel.IsDayNightMode = e.Value;
            _viewModel.RainmmPlotModel = _viewModel.GenerateSingleChart(_viewModel.IsDayNightMode, "#799eb9", "Rain mm", _viewModel.RainDB);
            _viewModel.RainDiscPlotModel = _viewModel.GenerateSingleChart(_viewModel.IsDayNightMode, "#ceedff", "Rain", _viewModel.RainDiscreteDB);
        }


    }
}