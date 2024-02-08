using ModelDataBase.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    internal class DiseaseHistoryRequest
    {
        internal static async Task HandleGetDiseaseHistory(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                using (var db = new dbModel())
                {
                    var disease = db.DiseaseHistory.ToList();
                    var settings = new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    };
                    await Settings.SendResponse(response, JsonConvert.SerializeObject(disease, settings));
                    Settings.Log("GET запрос на получение истории болезни выполнен");
                }
            }
            catch (Exception ex)
            {
                Settings.Log($"Ошибка: {ex.Message}", HttpStatusCode.BadRequest, ConsoleColor.DarkRed);
            }
        }
        internal static async Task HandlePostDiseaseHistory(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                string contentBody;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    contentBody = await reader.ReadToEndAsync();
                }
                var disease = JsonConvert.DeserializeObject<DiseaseHistory>(contentBody);

                if (disease != null)
                {
                    using (var db = new dbModel())
                    {
                        var existingDisease = db.DiseaseHistory.FirstOrDefault(mc => mc.ID == disease.ID);
                        if (existingDisease == null)
                        {
                            db.DiseaseHistory.Add(disease);
                            await db.SaveChangesAsync();
                            await Settings.SendResponse(response, $"Добавлена новая история болезней", statusCode: HttpStatusCode.Created);
                            Settings.Log($"PUT запрос на добавление истории болезни выполнен", statusCode: HttpStatusCode.Created);
                        }
                        else
                        {
                            await Settings.SendResponse(response, "Пациент уже присутствует в БД", statusCode: HttpStatusCode.Conflict);
                            Settings.Log($"При POST запросе была попытка дубликата", HttpStatusCode.Conflict, ConsoleColor.DarkRed);
                        }
                    }
                }
                else
                {
                    await Settings.SendResponse(response, "Указаны неверные данные", statusCode: HttpStatusCode.BadRequest);
                    Settings.Log($"При POST запросе были указаны неверные данные", HttpStatusCode.BadRequest, ConsoleColor.DarkRed);
                }
            }
            catch (Exception ex)
            {
                Settings.Log($"Ошибка: {ex.Message}", HttpStatusCode.BadRequest, ConsoleColor.DarkRed);
            }
        }

        internal static async Task HandlePutDiseaseHistory(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                string contentBody;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    contentBody = await reader.ReadToEndAsync();
                }
                var updatedDisease = JsonConvert.DeserializeObject<DiseaseHistory>(contentBody);
                if (updatedDisease == null || updatedDisease.ID == 0)
                {
                    await Settings.SendResponse(response, "Были переданы недопустимые данные", statusCode: HttpStatusCode.BadRequest);
                    Settings.Log("При PUT запросе были переданы допустимые данные", HttpStatusCode.BadRequest, ConsoleColor.DarkRed);
                    return;
                }
                using (var db = new dbModel())
                {
                    var existingDisease = await db.DiseaseHistory.FindAsync(updatedDisease.ID);
                    if(existingDisease == null)
                    {
                        await Settings.SendResponse(response, "Были переданы недопустимые данные", statusCode: HttpStatusCode.BadRequest);
                        Settings.Log("При PUT запросе были переданы допустимые данные", HttpStatusCode.BadRequest, ConsoleColor.DarkRed);
                        return;
                    }
                    db.Entry(existingDisease).CurrentValues.SetValues(updatedDisease);
                    await db.SaveChangesAsync();
                    await Settings.SendResponse(response, "Данные истории болезни были обновлены");
                    Settings.Log("PUT запрос на изменение данных истории болезни выполнен!");
                }
            }
            catch (Exception ex)
            {
                Settings.Log($"Ошибка: {ex.Message}", HttpStatusCode.BadRequest, ConsoleColor.DarkRed);
            }
        }
        internal static async Task HandleDeleteDiseaseHistory(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                var diseaseID = Convert.ToInt32(request.QueryString["id"]);

                using (var db = new dbModel())
                {
                    var disease = await db.DiseaseHistory.FindAsync(diseaseID);
                    if (disease != null)
                    {
                        db.DiseaseHistory.Remove(disease);
                        await db.SaveChangesAsync();
                        await Settings.SendResponse(response, "История болезни удалена");
                        Settings.Log($"DELETE запрос на удалени истории болезни выполнен");
                    }
                    else
                    {
                        await Settings.SendResponse(response, "Указаны неверные данные", statusCode: HttpStatusCode.BadRequest);
                        Settings.Log($"При DELETE запросе были указаны неверные данные", HttpStatusCode.BadRequest, ConsoleColor.DarkRed);
                    }
                }
            }
            catch (Exception ex)
            {
                Settings.Log($"Ошибка: {ex.Message}", HttpStatusCode.BadRequest, ConsoleColor.DarkRed);
            }
        }
    }
}
