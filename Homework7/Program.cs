namespace Homework7
{

    using System.Text;
    using Newtonsoft.Json; // dotnet add package Newtonsoft.Json

    class Program
    {
        private const string AddCitiesUrl = "http://firstgame11.mygamesonline.org/db_upload.php";
        private const string GetCitiesUrl = "http://firstgame11.mygamesonline.org/get_cities.php";
        private static readonly HttpClient client = new();

        static async Task Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            var cities = new List<string>();
            Console.Write("Введіть назву вашого міста: ");
            var myCity = Console.ReadLine();
            cities.Add(myCity!);

            await AddCitiesAsync(cities);

            var allCities = await GetCitiesAsync();

            Console.WriteLine("Список усіх міст:");
            foreach (var city in allCities)
            {
                Console.WriteLine(city);
            }
        }

        static async Task AddCitiesAsync(List<string> cities)
        {
            var json = JsonConvert.SerializeObject(cities);
            Console.WriteLine(json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(AddCitiesUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Міста успішно додано.");
                }
                else
                {
                    Console.WriteLine("Помилка при додаванні міст: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Сталася помилка: " + ex.Message);
            }
        }

        static async Task<List<string>> GetCitiesAsync()
        {
            var response = await client.GetAsync(GetCitiesUrl);
            var responseString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseString))
                return new List<string>();

            // Проверка: если сервер вернул HTML, не парсим как JSON
            if (responseString.TrimStart().StartsWith("<"))
            {
                Console.WriteLine("Сервер повернув HTML замість JSON:");
                Console.WriteLine(responseString);
                return new List<string>();
            }

            return JsonConvert.DeserializeObject<List<string>>(responseString) ?? new List<string>();
        }
    }
}
