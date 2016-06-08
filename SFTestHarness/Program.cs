using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFTestHarness
{
    class Program
    {
        private const int MillisecondsToDelay = 500;
        static void Main()
        {
             Task.Run(() => Run(40)).Wait();

            Console.WriteLine("Press <enter> to complete...");
            Console.ReadLine();
        }

        private static async Task Run(int numberOfAssets)
        {
            var rand = new Random(DateTime.UtcNow.Millisecond);

            var startModels = new IngestionStartModel[numberOfAssets];


            for (var i =0;i<numberOfAssets;i++)
            {
                var startModel = await PostAsync<AssetRequestModel, IngestionStartModel>("/libraries/49FCFFB0-BAFB-4C36-A525-8F1AE478837D/assets/", new AssetRequestModel {  Filename = i + ".jpg" });
                startModels[i] = startModel;

                await DisplayAssetStatus(startModel);
            }

            Parallel.For(0, numberOfAssets, async i =>
            {
                await Task.Delay(TimeSpan.FromSeconds(rand.Next(5, 20)));

                var startModel = startModels[i];
                await
                    PostAsync<StringMessage>(
                        $"/libraries/49FCFFB0-BAFB-4C36-A525-8F1AE478837D/ingestions/{startModel.ProcessId}/events");
            });

            var assets = new AssetStatusModel[numberOfAssets];
            while (Continue(assets))
            {
                Console.WriteLine("--------");

                for (var i = 0; i < numberOfAssets; i++)
                {
                    assets[i] = await DisplayAssetStatus(startModels[i]);
                }

                await Task.Delay(MillisecondsToDelay);
            }
        }

        private static bool Continue(IEnumerable<AssetStatusModel> models)
        {
            return models.Any(m => m == null || m.State == "Activated");
        }

        private static async Task<AssetStatusModel> DisplayAssetStatus(IngestionStartModel startModel)
        {
            await Task.Delay(MillisecondsToDelay);
            var asset =
                await GetAsync<AssetStatusModel>($"/libraries/49FCFFB0-BAFB-4C36-A525-8F1AE478837D/assets/{startModel.AssetId}");
            Console.WriteLine(JsonConvert.SerializeObject(asset));

            return asset;
        }



        private static async Task<TResponse> PostAsync<TResponse>(string endPoint)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:8753");
                httpClient.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));


                var response = await httpClient.PostAsync(new Uri(endPoint, UriKind.Relative), new StringContent(string.Empty));

                return JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync());
            }
        }

        private static async Task<TResponse> PostAsync<TRequest, TResponse>(string endPoint, TRequest requestBody)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:8753");
                httpClient.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));
    

                var response = await httpClient.PostAsync(new Uri(endPoint, UriKind.Relative), new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));

                return JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync());
            }
        }

        private static async Task<T> GetAsync<T>(string endPoint)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://localhost:8753");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                var response = await httpClient.GetAsync(new Uri(endPoint, UriKind.Relative));

                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
