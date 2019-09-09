using System.Collections.Generic;
using System.IO;
using CsvHelper;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHostingEnvironment _hostingEnvirnoment;

        public string MapboxApiKey { get; }

        public IndexModel(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvirnoment = hostingEnvironment;
            MapboxApiKey = configuration["Mapbox:ApiKey"];
        }

        public void OnGet()
        {
        }

        public IActionResult OnGetAirports()
        {
            using (var sr = new StreamReader(Path.Combine(_hostingEnvirnoment.WebRootPath, "airports.dat")))
            using (var reader = new CsvReader(sr))
            {
                var featureCollection = new FeatureCollection();
                reader.Configuration.HasHeaderRecord = false;
                reader.Configuration.BadDataFound = null;
                reader.Configuration.Delimiter = ",";

                while (reader.Read())
                {
                    // todo refactor this and load csv data using model(airport).
                    // https://joshclose.github.io/CsvHelper/getting-started
                    string name = reader.GetField<string>(2);
                    string iataCode = reader.GetField<string>(4);
                    double latidiute = reader.GetField<double>(6);
                    double longitude = reader.GetField<double>(7);

                    featureCollection.Features.Add(new Feature(
                        new Point(
                            new Position(latidiute, longitude)),
                            new Dictionary<string, object>
                            {
                                {"name", name},
                                {"iataCode", iataCode}
                            }));
                }


                return new JsonResult(featureCollection);
            }
        }
    }
}
