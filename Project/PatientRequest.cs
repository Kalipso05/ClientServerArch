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
    internal class PatientRequest
    {
        internal static async Task HandleGetPatient(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                using (var db = new dbModel())
                {

                    var patient = db.Patient.ToList();
                    var settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };
                    var json = JsonConvert.SerializeObject(patient, settings);
                    await Settings.SendResponse(response, json);
                    Settings.Log("GET запрос на получение пациентов выполнен");
                }
            }
            catch (Exception ex)
            {
                Settings.Log($"Ошибка: {ex.Message}", HttpStatusCode.BadRequest, ConsoleColor.DarkRed);
            }
        }
        internal static async Task HandlePostPatient(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                string contentBody;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    contentBody = await reader.ReadToEndAsync();
                }
                var patient = JsonConvert.DeserializeObject<Patient>(contentBody);

                if (patient != null)
                {
                    using (var db = new dbModel())
                    {
                        var existingPatient = db.Patient.FirstOrDefault(mc => mc.ID == patient.ID);
                        if (existingPatient == null)
                        {
                            db.Patient.Add(patient);
                            await db.SaveChangesAsync();
                            await Settings.SendResponse(response, $"Добавлен новый пациент", statusCode: HttpStatusCode.Created);
                            Settings.Log($"PUT запрос на добавление пациента выполнен", statusCode: HttpStatusCode.Created);
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

        internal static async Task HandlePutPatient(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                string contentBody;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    contentBody = await reader.ReadToEndAsync();
                }
                var updatedPatient = JsonConvert.DeserializeObject<Patient>(contentBody);
                if (updatedPatient == null || updatedPatient.ID == 0)
                {
                    await Settings.SendResponse(response, "Были переданы недопустимые данные", statusCode: HttpStatusCode.BadRequest);
                    Settings.Log("При PUT запросе были переданы допустимые данные", HttpStatusCode.BadRequest, ConsoleColor.DarkRed);
                    return;
                }
                using (var db = new dbModel())
                {
                    var existingPatient = await db.Patient.FindAsync(updatedPatient.ID);
                    if(existingPatient == null)
                    {
                        await Settings.SendResponse(response, "Были переданы недопустимые данные", statusCode: HttpStatusCode.BadRequest);
                        Settings.Log("При PUT запросе были переданы допустимые данные", HttpStatusCode.BadRequest, ConsoleColor.DarkRed);
                        return;
                    }
                    db.Entry(existingPatient).CurrentValues.SetValues(updatedPatient);
                    await db.SaveChangesAsync();
                    await Settings.SendResponse(response, "Данные пациента были обновлены");
                    Settings.Log("PUT запрос на изменение данных пациента выполнен!");
                }
            }
            catch (Exception ex)
            {
                Settings.Log($"Ошибка: {ex.Message}", HttpStatusCode.BadRequest, ConsoleColor.DarkRed);
            }
        }
        internal static async Task HandleDeletePatient(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                var patientID = Convert.ToInt32(request.QueryString["id"]);

                using (var db = new dbModel())
                {
                    var patient = await db.Patient.FindAsync(patientID);
                    if (patient != null)
                    {
                        db.Patient.Remove(patient);
                        await db.SaveChangesAsync();
                        await Settings.SendResponse(response, "Пациент удален из БД");
                        Settings.Log($"DELETE запрос на удалени пациент выполнен");
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
