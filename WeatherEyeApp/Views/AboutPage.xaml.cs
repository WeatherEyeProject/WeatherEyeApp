using System;
using System.ComponentModel;
using WeatherEyeApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatherEyeApp.Views
{
    public partial class AboutPage : ContentPage
    {
        readonly AboutViewModel _viewModel;
        public AboutPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new AboutViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}