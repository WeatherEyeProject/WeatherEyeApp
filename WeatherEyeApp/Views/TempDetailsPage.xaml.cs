﻿using System;
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
        RainDetailsViewModel _viewModel;
        public Chart RainChart;

        public RainDetailsPage()
        {
            InitializeComponent();
            
            BindingContext = _viewModel = new RainDetailsViewModel();

            RainChart = _viewModel.RainChart;
        }

        //public object TempChart { get; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        
    }
}