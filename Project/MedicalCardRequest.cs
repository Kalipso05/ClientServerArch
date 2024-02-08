using ModelDataBase.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    internal class MedicalCardRequest
    {
        internal static async Task HandleGetMedicalCard(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                using (var db = new dbModel())
                {
                    var medicalCard = db.MedicalCard.ToList();
                    var settings = new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    };
                    await Settings.SendResponse(response, JsonConvert.SerializeObject(medicalCard, settings));
                    Settings.Log("GET запрос на получение мед.карт выполнен");
                }
            }
            catch(Exception ex)
            {
                Settings.Log($"Ошибка: {ex.Message}", HttpStatusCode.BadRequest, ConsoleColor.DarkRed);
            }
        }
        internal static async Task HandlePostMedicalCard(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                string contentBody;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    contentBody = await reader.ReadToEndAsync();
                }
                var medicalCard = JsonConvert.DeserializeObject<MedicalCard>(contentBody);

                if (medicalCard != null)
                {
                    using (var db = new dbModel())
                    {
                        var existingMedicalCard = db.MedicalCard.FirstOrDefault(mc => mc.Code == medicalCard.Code);
                        if(existingMedicalCard == null)
                        {
                            db.MedicalCard.Add(medicalCard);
                            await db.SaveChangesAsync();
                            await Settings.SendResponse(response, $"Добавлена новая медицинская карта", statusCode: HttpStatusCode.Created);
                            Settings.Log($"PUT запрос на добавление мед.карты с кодом {medicalCard.Code} выполнен", statusCode: HttpStatusCode.Created);
                        }
                        else
                        {
                            await Settings.SendResponse(response, "Медицинская карта уже присутствует в БД", statusCode: HttpStatusCode.Conflict);
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

        internal static async Task HandlePutMedicalCard(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                string contentBody;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    contentBody = await reader.ReadToEndAsync();
                }
                var medicalCard = JsonConvert.DeserializeObject<MedicalCard>(contentBody);

                if (medicalCard != null)
                {
                    using (var db = new dbModel())
                    {
                        var existingMedicalCard = await db.MedicalCard.FindAsync(medicalCard.ID);
                        if (existingMedicalCard != null)
                        {
                            var dublicate = db.MedicalCard.Where(mc => mc.Code == medicalCard.Code).FirstOrDefault();
                            if (dublicate != null)
                            {
                                await Settings.SendResponse(response, "Дубликат медицинской карты", statusCode: HttpStatusCode.Conflict);
                                Settings.Log($"При PUT запросе была попытка дубликата", HttpStatusCode.Conflict, ConsoleColor.DarkRed);
                                return;
                            }
                            db.Entry(existingMedicalCard).CurrentValues.SetValues(medicalCard);
                            await db.SaveChangesAsync();
                            await Settings.SendResponse(response, "Данные медицинской карты были обновлены");
                            Settings.Log("PUT запрос на обновление данных мед.карты выполнен");
                        }
                    }
                }
                else
                {
                    await Settings.SendResponse(response, "Указаны неверные данные", statusCode: HttpStatusCode.BadRequest);
                    Settings.Log($"При PUT запросе были указаны неверные данные", HttpStatusCode.BadRequest, ConsoleColor.DarkRed);
                }
            }
            catch(Exception ex)
            {
                Settings.Log($"Ошибка: {ex.Message}", HttpStatusCode.BadRequest, ConsoleColor.DarkRed);
            }
        }
        internal static async Task HandleDeleteMedicalCard(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                var medicalID = Convert.ToInt32(request.QueryString["id"]);

                using (var db = new dbModel())
                {
                    var medical = await db.MedicalCard.FindAsync(medicalID);
                    if (medical != null)
                    {
                        db.MedicalCard.Remove(medical);
                        await db.SaveChangesAsync();
                        await Settings.SendResponse(response, "Медицинская карта удалена");
                        Settings.Log($"DELETE запрос на удалени медицинской карты выполнен");
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
