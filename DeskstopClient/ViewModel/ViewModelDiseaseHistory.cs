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
    internal class ViewModelDiseaseHistory
    {
        public static HttpClient client = new HttpClient();
        public static ObservableCollection<DiseaseHistory> diseasesList = new ObservableCollection<DiseaseHistory>();
        internal static async Task GetObjectDiseaseHistory(ListView listView)
        {
            try
            {
                var response = await client.GetAsync("http://localhost:8080/api/DiseaseHistory/");
                var contentBody = await response.Content.ReadAsStringAsync();

                var disease = JsonConvert.DeserializeObject<List<DiseaseHistory>>(contentBody);

                diseasesList.Clear();
                foreach (var d in disease)
                {
                    diseasesList.Add(d);
                }
                listView.ItemsSource = diseasesList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Уведомление об ошибке", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        internal static async Task PostObjectDiseaseHistory(int patientID, string nameDisease, string desc, DateTime dateOfDisease)
        {
            try
            {
                var disease = new DiseaseHistory()
                {
                    IDPatient = patientID,
                    NameDisease = nameDisease,
                    Description = desc,
                    DateOfDisease = dateOfDisease
                };
                var json = JsonConvert.SerializeObject(disease);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("http://localhost:8080/api/DiseaseHistory/", content);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Уведомление об ошибке", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static int? editDiseaseId { get; set; }
        internal static async Task PutObjectDiseaseHistory(int patientID, string nameDisease, string desc, DateTime dateOfDisease)
        {
            try
            {
                var disease = new DiseaseHistory()
                {
                    IDPatient = patientID,
                    NameDisease = nameDisease,
                    Description = desc,
                    DateOfDisease = dateOfDisease
                };
                disease.ID = editDiseaseId.Value;
                var json = JsonConvert.SerializeObject(disease);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"http://localhost:8080/api/DiseaseHistory/?id={editDiseaseId.Value}", content);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Уведомление об ошибке", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static async Task DeleteObjectDisease(int diseaseID)
        {
            string url = $"http://localhost:8080/api/DiseaseHistory/?id={diseaseID}";
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.DeleteAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("История болезни удалена из базы данных", "Удаление истории болезни", MessageBoxButton.OK, MessageBoxImage.Information);
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
