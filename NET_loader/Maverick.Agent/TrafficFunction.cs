using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Maverick.Agent
{
    public class TrafficFunction
    {
        private static readonly HttpClient _http;
        private static readonly string HeaderHashCheck;

        static TrafficFunction()
        {
            HeaderHashCheck = CryptoFunction.GetRequestHeaderVallue();
            _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("X-Request-Hash", HeaderHashCheck);
        }

        public static async Task<byte[]> SendDataAsync(string url, byte[] data)
        {
            try
            {
                using (var content = new ByteArrayContent(data))
                {
                    var response = await _http.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
