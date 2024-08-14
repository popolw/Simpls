using System.Net;
using System.Text;
using System.Text.Json;

namespace Swellbay.Extensions.HTTPServer
{
    public static class HttpListenerResponseExtensions
    {
        public static void Write(this HttpListenerResponse response, string text, Encoding encoding)
        {
            var buffer = encoding.GetBytes(text);
            response.ContentEncoding = encoding;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }

        public static void WriteJson<T>(this HttpListenerResponse response, T value)
        {
            response.WriteJson(value, Encoding.UTF8, null);
        }

        public static void WriteJson<T>(this HttpListenerResponse response, T value, Encoding encoding, JsonSerializerOptions? options = null)
        {
            response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(value, options);
            response.Write(json, encoding);
        }
    }
}
