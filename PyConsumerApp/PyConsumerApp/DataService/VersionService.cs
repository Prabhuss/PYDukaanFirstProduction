using Newtonsoft.Json;
using PyConsumerApp.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PyConsumerApp.DataService
{
    class VersionService
    {
        private static VersionService instance;
        public static VersionService Instance => instance ?? (instance = new VersionService());
        public VersionApiData ResponseData { get; set; }
        public async Task<VersionApiData> CheckVersion(string app_name)
        {

            string baseUrl = "https://getpymobileapp.azurewebsites.net/api/v2/auth/";
            string url = string.Format("{0}{1}", baseUrl, "getMessageForVersion");
            using (HttpClient Webclient = new HttpClient())
            {
                var uri = new Uri(url);
                Dictionary<string, dynamic> payload = new Dictionary<string, dynamic>();
                payload.Add("app_name", app_name);
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await Webclient.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    var rcontent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        VersionApiResponse verData = JsonConvert.DeserializeObject<VersionApiResponse>(rcontent);
                        Console.WriteLine("Response of custdetails Status is : " + verData.Status);
                        if (verData.Status.ToLower() != "success")
                        {
                            System.Diagnostics.Debug.WriteLine("[BuildRequestNDisplay] Received non-success " + verData.Status);
                            System.Diagnostics.Debug.WriteLine("[BuildRequestNDisplay] Received Response: " + content);
                            return null;
                        }
                        ResponseData = verData.Data;
                        return ResponseData;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Error in customer API " + e.Message);
                    }
                }
                return null;
            }
        }
    }
}
