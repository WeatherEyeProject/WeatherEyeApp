using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherEyeApp.Models
{
    public class TemperatureData
    {
        public string Id { get; set; }
        public double Temp { get; set; }
        public DateTime DateOfReading { get; set; }
    }
}
