using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Assets.Catalog;
using Assets.Other;
using Newtonsoft.Json;

namespace Backend.ShowcaseRequests
{
    public class Transport
    {
        private static readonly Uri BaseAddress = new Uri("http://localhost:3002/");
        private static readonly HttpClient Client = new HttpClient {BaseAddress = BaseAddress};

        public static ProductInfo GetProductInfo(int id)
        {
            var response = Client.GetAsync($"api/catalog/products/{id}").Result;
            var json = response.StatusCode != HttpStatusCode.OK ? null : response.Content.ReadAsStringAsync().Result;
            return json == null ? null : JsonConvert.DeserializeObject<ProductInfo>(json);
        }

        public static MessageInfo UpdateProductCount(int id, int count, string token)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"http://localhost:3002/api/catalog/products/{id}"),
                Headers = {{HttpRequestHeader.Authorization.ToString(), token}},
                Method = HttpMethod.Put,
                Content = new StringContent(JsonConvert.SerializeObject(new {Count = count}), Encoding.UTF8,
                    "application/json")
            };
            var response = Client.SendAsync(request).Result;
            var json = response.StatusCode != HttpStatusCode.OK ? null : response.Content.ReadAsStringAsync().Result;
            return json == null ? null : JsonConvert.DeserializeObject<MessageInfo>(json);
        }
    }
}