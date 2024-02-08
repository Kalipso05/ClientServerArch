using ModelDataBase.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DeskstopClient.ViewModel
{
    internal class ViewModelPatient
    {
        static HttpClient client = new HttpClient();
        static ObservableCollection<Patient> patientList = new ObservableCollection<Patient>();
        internal static async Task GetObjectPatient(ListView listView)
        {
            try
            {
                var response = await client.GetAsync("http://localhost:8080/api/Patient");

                var responseBody = await response.Content.ReadAsStringAsync();

                var patient = JsonConvert.DeserializeObject<List<Patient>>(responseBody);

                patientList.Clear();
                foreach (var pat in patient)
                {
                    patientList.Add(pat);
                }
                listView.ItemsSource = patientList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Уведомление об ошибке", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static async Task PostObjectPatient(string firstname, string lastname, string patronymic, string phone, string email, DateTime dateOfBirth)
        {
            try
            {
                var patient = new Patient()
                {
                    FirstName = firstname,
                    LastName = lastname,
                    Patronymic = patronymic,
                    Phone = phone,
                    Email = email,
                    DateOfBirth = dateOfBirth
                };

                var json = JsonConvert.SerializeObject(patient);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("http://localhost:8080/api/Patient", content);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Уведомление об ошибке", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static int? editPatientID {  get; set; }
        internal static async Task PutObjectPatient(string firstname, string lastname, string patronymic, string phone, string email, DateTime dateOfBirth)
        {
            try
            {
                var patient = new Patient()
                {
                    FirstName = firstname,
                    LastName = lastname,
                    Patronymic = patronymic,
                    Phone = phone,
                    Email = email,
                    DateOfBirth = dateOfBirth
                };
                patient.ID = editPatientID.Value;
                var json = JsonConvert.SerializeObject(patient);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"http://localhost:8080/api/Patient/?id={editPatientID.Value}", content);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Уведомление об ошибке", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        internal static async Task GetPatientInComboBox(ComboBox comboBox)
        {
            var response = await client.GetAsync("http://localhost:8080/api/Patient");
            
            var contentBody = await response.Content.ReadAsStringAsync();
            
            var patient = JsonConvert.DeserializeObject<List<Patient>>(contentBody);
            comboBox.ItemsSource = patient;
        }
        internal static async Task DeleteObjectPatient(int patientID)
        {
            string url = $"http://localhost:8080/api/Patient/?id={patientID}";
            using( var client = new HttpClient())
            {
                try
                {
                    var response = await client.DeleteAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Пациент удален из базы данных", "Удаление пациента", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Уведомление об ошибке", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
