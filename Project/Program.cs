using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    internal class Program
    {
        internal static async Task RouteRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            var path = request.Url.AbsolutePath;
            var method = request.HttpMethod;

            if (path.StartsWith("/api") && path.TrimEnd('/') == "/api")
            {
                var htmlPath = @"C:\Users\ServerPC\source\repos\Project\Documentation\test.html";
                var htmlContent = File.ReadAllText(htmlPath);
                await Settings.SendResponse(response, htmlContent, "text/html");
            }
            else if (path.StartsWith("/api/MedicalCards"))
            {
                switch (method)
                {
                    case "GET":
                        await MedicalCardRequest.HandleGetMedicalCard(request, response);
                        break;
                    case "POST":
                        await MedicalCardRequest.HandlePostMedicalCard(request, response);
                        break;
                    case "PUT":
                        await MedicalCardRequest.HandlePutMedicalCard(request, response);
                        break;
                    case "DELETE":
                        await MedicalCardRequest.HandleDeleteMedicalCard(request, response);
                        break;
                    default:
                        response.StatusCode = 404;
                        break;
                }
            }
            else if (path.StartsWith("/api/Patient"))
            {
                switch (method)
                {
                    case "GET":
                        await PatientRequest.HandleGetPatient(request, response);
                        break;
                    case "POST":
                        await PatientRequest.HandlePostPatient(request, response);
                        break;
                    case "PUT":
                        await PatientRequest.HandlePutPatient(request, response);
                        break;
                    case "DELETE":
                        await PatientRequest.HandleDeletePatient(request, response);
                        break;
                }
            }
            else if(path.StartsWith("/api/DiseaseHistory"))
            {
                switch (method)
                {
                    case "GET":
                        await DiseaseHistoryRequest.HandleGetDiseaseHistory(request, response);
                        break;
                    case "POST":
                        await DiseaseHistoryRequest.HandlePostDiseaseHistory(request, response);
                        break;
                    case "PUT":
                        await DiseaseHistoryRequest.HandlePutDiseaseHistory(request, response);
                        break;
                    case "DELETE":
                        await DiseaseHistoryRequest.HandleDeleteDiseaseHistory(request, response);
                        break;
                }
            }
            else
            {
                response.StatusCode = 404;

            }
            response.Close();
        }

        static void Main(string[] args)
        {
            Task.Run(() => StartServer()).GetAwaiter().GetResult();
        }

        private static async Task StartServer()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/api/");
            listener.Start();
            Settings.Log("Сервер с адресом http://localhost:8080/api/ запущен");

            while (true)
            {
                var context = await listener.GetContextAsync();
                await RouteRequest(context.Request, context.Response);
            }
        }
    }
}
