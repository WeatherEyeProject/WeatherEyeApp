using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using WeatherEyeApp.Models;

namespace WeatherEyeApp.Services
{
    public interface ISensorService<T>
    {
        Task<ObservableCollection<T>> RefreshDataAsync();
        Task<ObservableCollection<T>> GetDataByDateAsync(DateTime date1, DateTime date2);

    }
}
