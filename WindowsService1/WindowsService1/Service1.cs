using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;)  

        public object JsonConvert { get; private set; }

        public Service1()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 300000; //number in milisecinds  
            timer.Enabled = true;

        }
        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {


            Process();
            WriteToFile("Service is recall at " + DateTime.Now);
        }

        internal void Process()
        {
            YWSample yahooWeather = new YWSample();

            var json = yahooWeather.GetWeather();

            JObject obj = JObject.Parse(json);
            JObject location = JObject.Parse(obj["location"].ToString());
            JObject current =JObject.Parse(obj["current_observation"].ToString());
            JObject condition = JObject.Parse(current["condition"].ToString());

            WeatherLog logWeather = new WeatherLog();
            //JObject city = JObject.Parse(location["city"].ToString());
            //JObject temperature = JObject.Parse(condition["temperature"].ToString());
            //JObject code = JObject.Parse(condition["code"].ToString());

            logWeather.Location = location.SelectToken(@"city").Value<string>();
            logWeather.Temperature = condition.SelectToken(@"temperature").Value<string>();
            logWeather.Code = int.Parse(condition.SelectToken(@"code").Value<string>());

            logWeather.Unit = "F";
            switch (logWeather.Code)
            {
                case 0: case 1: case 2: case 3: case 4: case 5: case 6: case 7: case 8: case 9: case 10:
                case 11: case 12: case 13: case 14: case 15: case 16: case 17: case 18: case 35: case 37:
                case 38: case 39: case 40: case 41: case 42: case 43: case 45: case 46: case 47:
                    logWeather.Precipitation = true;
                    break;

                default:
                    logWeather.Precipitation = false;
                    break;

            };

            logWeather.Time = DateTime.Now;

            WriteToCSV(logWeather);



    }

        public void WriteToCSV(WeatherLog weatherLog)
        {

            string message = weatherLog.Location + "," + weatherLog.Temperature + "," + weatherLog.Unit + "," + weatherLog.Precipitation + "," + weatherLog.Time.ToString();
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Weather";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Weather\\CurrentWeather.csv";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(message.ToString());
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(message.ToString());
                }
            }
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}