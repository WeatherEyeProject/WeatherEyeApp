using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherEyeApp.Models
{
    public class LatestData
    {
        //public DustData dustSensor;
        //public EnvironmentalData environmentalSensor;
        //public LightData lightSensor;
        //public RainData rainSensor;
        //public UVData uvSensor;
        public SensorsData s1;  //  TEMPERATURE
        public SensorsData s2;  //  HUMIDITY
        public SensorsData s3;  //  PRESSURE
        public SensorsData s4;  //  AIR_QUALITY_IAQ
        public SensorsData s5;  //  LIGHT_ALS
        public SensorsData s6;  //  LIGHT_UV
        public SensorsData s7;  //  AIR_PM10
        public SensorsData s8;  //  AIR_PM2_5
        public SensorsData s10; //  RAIN_DISCRETE
        public SensorsData s11; //  RAIN_VALUE
    }
}
