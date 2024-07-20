using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PM2E2Grupo1.Controllers
{
    public class GeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeocodingService(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }

        public async Task<(string? Departamento, string? Ciudad)> GetCoordinateDetailsAsync(double Lat, double Lng)
        {
            string apiUrl = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={Lat},{Lng}&key={_apiKey}";

            string? departamento = null;
            string? ciudad = null;
            string? direccion = null;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage responseBing = await client.GetAsync(apiUrl);

                if (responseBing.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    string jsonResponse = await responseBing.Content.ReadAsStringAsync();

                    if (jsonResponse != null)
                    {
                        // Parse the JSON response using JsonDocument
                        JsonDocument jsonDocument = JsonDocument.Parse(jsonResponse);

                        departamento = jsonDocument.RootElement
                            .GetProperty("results")[0]
                            .GetProperty("address_components")
                            .EnumerateArray()
                            .FirstOrDefault(c => c.GetProperty("types").EnumerateArray().Any(t => t.GetString() == "administrative_area_level_1"))
                            .GetProperty("long_name")
                            .GetString();

                        ciudad = jsonDocument.RootElement
                            .GetProperty("results")[0]
                            .GetProperty("address_components")
                            .EnumerateArray()
                            .FirstOrDefault(c => c.GetProperty("types").EnumerateArray().Any(t => t.GetString() == "administrative_area_level_2"))
                            .GetProperty("long_name")
                            .GetString();
                    }

                    return (departamento, ciudad);
                }
                else
                {
                    return (string.Empty, string.Empty);
                }
            }
        }
    }
}
