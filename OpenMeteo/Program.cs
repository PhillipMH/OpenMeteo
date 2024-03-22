using System.Data;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;

namespace OpenMeteo
{
    public class Program
    {
        // https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&hourly=temperature_2m
        public static void Main()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("1. Vejr");
                Console.WriteLine("2. Vejr de næste 24 timer");
                Console.WriteLine("3. Vejr de seneste 24 timer");
                Console.WriteLine("4. Vejr den næste uge");
                Console.WriteLine("5. Exit");
                Console.WriteLine("Vælg en menu: ");
                var menu = Console.ReadKey();
                switch (menu.Key)
                {
                    case ConsoleKey.D1:
                        GetSortedWeather().Wait();
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case ConsoleKey.D2:
                        GetHighestTemp().Wait();
                        GetLowestTemp().Wait();
                        GetAverageWindSpeed().Wait();
                        GetSortedWeather().Wait();
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case ConsoleKey.D3:
                        GetWeatherSingleCityJson().Wait();
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case ConsoleKey.D4:
                        GetWeatherSingleCityJson().Wait();
                        Console.ReadKey();
                        Console.Clear();
                        break;
                    case ConsoleKey.D5:
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Ugyldigt valg");
                        Console.Clear();
                        break;
                }
            }
        }

        private async static Task GetWeatherSingleCityJson()
        {
            Console.Clear();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast?latitude=54.909&longitude=9.7892&hourly=temperature_2m,snowfall&timezone=Europe%2FBerlin");
            var response = await client.GetAsync(client.BaseAddress);
            string stringResult = await response.Content.ReadAsStringAsync();

            var weather = JsonSerializer.Deserialize<Root>(stringResult);
                for (int i = 0; i < weather.hourly.time.Count; i++)
                {
                    weather.hourly.time[i] = weather.hourly.time[i].Replace("T", " ");
                    Console.WriteLine($"Time: {weather.hourly.time[i]} - Temperature: {weather.hourly.temperature_2m[i]} Snowfall: {weather.hourly.snowfall[i]}");
                }
        }
        private async static Task GetHighestTemp()
        {
            Console.Clear();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast?latitude=54.909&longitude=9.7892&hourly=temperature_2m,snowfall&timezone=Europe%2FBerlin&forecast_days=1");
            var response = await client.GetAsync(client.BaseAddress);
            string stringResult = await response.Content.ReadAsStringAsync();

            var weather = JsonSerializer.Deserialize<Root>(stringResult);
            if (weather != null)
            {
                for (int i = 0; i < weather.hourly.time.Count; i++)
                {
                    weather.hourly.time[i] = weather.hourly.time[i].Replace("T", " ");
                }
                double maxTemp = weather.hourly.temperature_2m.Max();
                string time = weather.hourly.time[weather.hourly.temperature_2m.IndexOf(maxTemp)];
                Console.WriteLine($"Højeste temperatur de næste 24 timer: {maxTemp} - Klokken: {time}");
            }
        }

        private async static Task GetLowestTemp()
        {
            Console.Clear();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast?latitude=54.909&longitude=9.7892&hourly=temperature_2m,snowfall&timezone=Europe%2FBerlin&forecast_days=1");
            var response = await client.GetAsync(client.BaseAddress);
            string stringResult = await response.Content.ReadAsStringAsync();

            var weather = JsonSerializer.Deserialize<Root>(stringResult);
            if (weather != null)
            {
                for (int i = 0; i < weather.hourly.time.Count; i++)
                {
                    weather.hourly.time[i] = weather.hourly.time[i].Replace("T", " ");
                }
                double minTemp = weather.hourly.temperature_2m.Min();
                string time = weather.hourly.time[weather.hourly.temperature_2m.IndexOf(minTemp)];
                Console.WriteLine($"Laveste temperatur de næste 24 timer: {minTemp} - Klokken: {time}");
            }
        }

        private async static Task GetAverageWindSpeed()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast?latitude=54.909&longitude=9.7892&hourly=temperature_2m,snowfall,wind_speed_10m,wind_direction_10m,wind_gusts_10m&timezone=Europe%2FBerlin&forecast_days=1");
            var response = await client.GetAsync(client.BaseAddress);
            string stringResult = await response.Content.ReadAsStringAsync();

            var weather = JsonSerializer.Deserialize<Root2>(stringResult);
            if (weather != null)
            {
                for (int i = 0; i < weather.hourly.time.Count; i++)
                {
                    weather.hourly.time[i] = weather.hourly.time[i].Replace("T", " ");
                }
                double averageWindSpeed = weather.hourly.wind_speed_10m.Average();
                Console.WriteLine($"Gennemsnitlig vind hastighed de næste 24 timer: {averageWindSpeed}");
            }
        }

        private async static Task GetSortedWeather()
        {
            Console.Clear();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.open-meteo.com/v1/forecast?latitude=54.909&longitude=9.7892&hourly=temperature_2m,snowfall,wind_speed_10m,wind_direction_10m,wind_gusts_10m&timezone=Europe%2FBerlin&forecast_days=1");
            var response = await client.GetAsync(client.BaseAddress);
            string stringResult = await response.Content.ReadAsStringAsync();

            var weather = JsonSerializer.Deserialize<Root2>(stringResult);
            if (weather != null)
            {
                for (int i = 0; i < weather.hourly.time.Count; i++)
                {
                    weather.hourly.time[i] = weather.hourly.time[i].Replace("T", " ");
                }
                List<string> time = weather.hourly.time;
                List<double> temperature = weather.hourly.temperature_2m;
                List<double> windSpeed = weather.hourly.wind_speed_10m;
                List<int> windDirection = weather.hourly.wind_direction_10m;
                List<double> windGusts = weather.hourly.wind_gusts_10m;

                List<HourlyUnits2> weatherList = new List<HourlyUnits2>();
                for (int i = 0; i < time.Count; i++)
                {
                    weatherList.Add(new HourlyUnits2()
                    {
                        time = time[i],
                        temperature_2m = temperature[i].ToString(),
                        wind_speed_10m = windSpeed[i].ToString(),
                        wind_direction_10m = windDirection[i].ToString(),
                        wind_gusts_10m = windGusts[i].ToString()
                    });
                }

                weatherList = weatherList.OrderBy(x => x.time).ToList();
                foreach (var item in weatherList)
                {
                    Console.WriteLine($"Time: {item.time} - Temperature: {item.temperature_2m} - WindSpeed: {item.wind_speed_10m} - WindDirection: {item.wind_direction_10m} - WindGusts: {item.wind_gusts_10m}");
                }
            }
        }

        public class Hourly
        {
            public List<string> time { get; set; }
            public List<double> temperature_2m { get; set; }
            public List<double> snowfall { get; set; }
        }

        public class HourlyUnits
        {
            public string time { get; set; }
            public string temperature_2m { get; set; }
            public string snowfall { get; set; }
        }

        public class Root
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
            public double generationtime_ms { get; set; }
            public int utc_offset_seconds { get; set; }
            public string timezone { get; set; }
            public string timezone_abbreviation { get; set; }
            public double elevation { get; set; }
            public HourlyUnits hourly_units { get; set; }
            public Hourly hourly { get; set; }
        }


        public class Hourly2
        {
            public List<string> time { get; set; }
            public List<double> temperature_2m { get; set; }
            public List<double> snowfall { get; set; }
            public List<double> wind_speed_10m { get; set; }
            public List<int> wind_direction_10m { get; set; }
            public List<double> wind_gusts_10m { get; set; }
        }

        public class HourlyUnits2
        {
            public string time { get; set; }
            public string temperature_2m { get; set; }
            public string snowfall { get; set; }
            public string wind_speed_10m { get; set; }
            public string wind_direction_10m { get; set; }
            public string wind_gusts_10m { get; set; }
        }

        public class Root2
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
            public double generationtime_ms { get; set; }
            public int utc_offset_seconds { get; set; }
            public string timezone { get; set; }
            public string timezone_abbreviation { get; set; }
            public double elevation { get; set; }
            public HourlyUnits2 hourly_units { get; set; }
            public Hourly2 hourly { get; set; }
        }


    }
}