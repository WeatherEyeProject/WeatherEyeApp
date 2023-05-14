using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherEyeApp.Models
{
    public class DustData
    {
        public int Id { get; set; }
        public double? IntensityPm2_5 { get; set; }
        public double? IntensityPm10 { get; set; }
        public DateTime DateOfReading { get; set; }
    }
}
