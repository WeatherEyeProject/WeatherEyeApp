using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherEyeApp.Models
{
    public class UVData
    {
        public int Id { get; set; }
        public double IlluminanceUV { get; set; }
        public DateTime DateOfReading { get; set; }
    }
}
