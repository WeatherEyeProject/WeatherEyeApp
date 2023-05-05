using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherEyeApp.Models
{
    public class RainData
    {
        public int Id { get; set; }
        public double Rain { get; set; }
        public double IntensityOfRain { get; set; }
        public DateTime DateOfReading { get; set; }
    }
}
