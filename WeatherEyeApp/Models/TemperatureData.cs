using System;

namespace WeatherEyeApp.Models
{
    public class TemperatureData
    {
        public string Id { get; set; }
        public double Temp { get; set; }
        public DateTime DateOfReading { get; set; }
    }
}
