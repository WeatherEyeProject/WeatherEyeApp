using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherEyeApp.Models
{
    public class EnvironmentalData
    {
        public int Id { get; set; }
        public double? Dampness { get; set; } //humidity xDD
        public double? Temperature { get; set; }
        public double? Pressure { get; set; }
        public double? IAQuality { get; set; }
        public DateTime DateOfReading { get; set; }
    }
}
