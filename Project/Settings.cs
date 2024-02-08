using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    internal class Settings
    {
        internal static async Task SendResponse(HttpListenerResponse response, string content, string contentType = "application/json", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            byte[] data = Encoding.UTF8.GetBytes(content);
            response.ContentLength64 = data.Length;
            response.ContentType = contentType;
            response.StatusCode = (int)statusCode;

            await response.OutputStream.WriteAsync(data, 0, data.Length);
            response.Close();
        }
        internal static void Log(string message, HttpStatusCode statusCode = HttpStatusCode.OK, ConsoleColor color = ConsoleColor.Green)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now}]: {message} status code: {statusCode}");
        }
    }
}
