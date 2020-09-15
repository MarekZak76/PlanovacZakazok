using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JobManager.UI
{
    public class BingLocator : Observable, IGeolocator
    {
        readonly string bingMapsKey;
        private bool isAwaiting;
        private string errorMessage;

        public BingLocator()
        {
            HttpClient = new HttpClient();
            bingMapsKey = ConfigurationManager.AppSettings["BingMapsKey"];
        }

        public HttpClient HttpClient { get; set; }
        public bool IsAwaiting
        {
            get => isAwaiting;
            private set => SetProperty(ref isAwaiting, value);
        }
        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        public async Task<Tuple<string, double[]>> GetPointAsync(Adress adress)
        {
            if (adress == null) return null;

            ErrorMessage = string.Empty;
            IsAwaiting = true;        
            HttpResponseMessage httpResponseMessage;
            string name;
            double[] coordinates;
            string addressLine = adress.Street + " " + adress.Number;
            string content;
            JObject json;

            try
            {
                httpResponseMessage = await HttpClient.GetAsync($"http://dev.virtualearth.net/REST/v1/Locations?countryRegion=sk&adminDisctrict={adress.City}&locality={adress.City}&addressLine={addressLine}&key={bingMapsKey}");
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(ex.Message);
                IsAwaiting = false;
                ErrorMessage = "Nie je možné sa spojit s lokačnou službou.";
                return null;
            }
            if (httpResponseMessage.IsSuccessStatusCode == false)
            {
                ErrorMessage = "Adresa sa nenašla.";
                IsAwaiting = false;
                return null;
            }
            
            content = await httpResponseMessage.Content.ReadAsStringAsync();
            json = (JObject)JsonConvert.DeserializeObject(content);
            name = (string)json["resourceSets"][0]["resources"][0]["name"];
            coordinates = json["resourceSets"][0]["resources"][0]["point"]["coordinates"].ToObject<double[]>();
            IsAwaiting = false;
            return Tuple.Create(name, coordinates);
        }
        public async Task<Stream> GetStaticMapAsync(int width, int height, Location main, params Location[] secondary)
        {
            string pushpinMain = string.Empty;
            string pushpins = string.Empty;
            HttpResponseMessage httpResult;
            ErrorMessage = string.Empty;
            IsAwaiting = true;
            Stream content;

            if (main != null)
            {
                pushpinMain = $"&pp={main.LocationPoint[0].ToString(CultureInfo.GetCultureInfo("en-US"))}, {main.LocationPoint[1].ToString(CultureInfo.GetCultureInfo("en-US"))}; 92; P";
            }

            if (secondary != null)
            {
                pushpins = string.Empty;
                foreach (Location place in secondary)
                {
                    pushpins += $"&pp={place.LocationPoint[0].ToString(CultureInfo.GetCultureInfo("en-US"))}, {place.LocationPoint[1].ToString(CultureInfo.GetCultureInfo("en-US"))}; 92; {null}";
                }
            }

            string query = string.Concat($"https://dev.virtualearth.net/REST/v1/Imagery/Map/Road?mapSize={width},{height}", pushpinMain, pushpins, $"&key={bingMapsKey}");

            try
            {
                httpResult = await HttpClient.GetAsync(query);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(ex.Message);
                ErrorMessage = "Nie je možné sa spojit s mapovou službou.";
                IsAwaiting = false;
                return null;
            }
            if (httpResult.IsSuccessStatusCode == false)
            {
                ErrorMessage = "Mapa nie je k dispozícii.";
                IsAwaiting = false;
                return null;
            }

            IsAwaiting = false;
            content = await httpResult.Content.ReadAsStreamAsync();

            return content;
        }
    }
}
