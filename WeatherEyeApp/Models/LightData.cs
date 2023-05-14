using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherEyeApp.Models
{
    public class LightData
    {
        public int Id { get; set; }
        public double IlluminanceLux { get; set; }
        public DateTime DateOfReading { get; set; }
    }
}
