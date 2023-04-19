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
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel _viewModel;

        public ItemsPage()
        {
            InitializeComponent();
            
            BindingContext = _viewModel = new ItemsViewModel();

            //TempChart.Chart = new LineChart { Entries = _viewModel.Temperatures };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}