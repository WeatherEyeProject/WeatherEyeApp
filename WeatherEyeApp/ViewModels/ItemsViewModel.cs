﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WeatherEyeApp.Models;
using WeatherEyeApp.Views;
using Xamarin.Forms;
using Microcharts;

namespace WeatherEyeApp.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private Item _selectedItem;

        public ObservableCollection<Item> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Item> ItemTapped { get; }
        public List<ChartEntry> Temperatures { get; }
        public Chart TempChart { get; private set; }

        public ItemsViewModel()
        {
            Title = "Archive";
            Items = new ObservableCollection<Item>();
            Temperatures = new List<ChartEntry>();
            
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Item>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);


        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }

            try
            {
                Temperatures.Clear();
                var temps = await TempData.GetItemsAsync(true);
                foreach (var temp in temps)
                {
                    Temperatures.Add(new ChartEntry((float)temp.Temp)
                    {
                        Label = temp.DateOfReading.ToString(),
                        ValueLabel = temp.Temp.ToString(),
                        Color = SkiaSharp.SKColor.Parse("#77d065")
                    });
                }

                TempChart = new LineChart()
                {
                    Entries = Temperatures
                };


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            //var temps = await TempData.GetItemsAsync(true);
            /*
            foreach (var temp in temps)
            {
                Temperatures.Add(new ChartEntry((float)temp.Temp)
                {
                    Label = temp.DateOfReading.ToString(),
                    ValueLabel = temp.Temp.ToString(),
                    Color = SkiaSharp.SKColor.Parse("#77d065")
                });
            }

            //TempChart = new LineChart()
            //{
            //    Entries = Temperatures
           // };
            */
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        async void OnItemSelected(Item item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }

    }
}