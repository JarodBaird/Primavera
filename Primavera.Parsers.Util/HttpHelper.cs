using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Primavera.Parsers.Util
{
    public static class HttpHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<T> GetAsync<T>(Uri url)
        {
            HttpResponseMessage result = await _httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync()
                    .ConfigureAwait(false));
                return data;
            }

            return default;
        }
    }
}