using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1
{
    public class WeatherLog
    {
        public string Location { get; set; }
        public string Temperature { get; set; }
        public string Unit { get; set; }
        public int Code { get; set; }
        public bool Precipitation { get; set; }
        public DateTime Time { get; set; }

    }
}
