using Microsoft.Win32;
using ModelDataBase.Model;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DeskstopClient.ViewModel
{
    internal class ViewModelMedicalCard
    {
        private static HttpClient client = new HttpClient();
        private static ObservableCollection<MedicalCard> medicalCardList = new ObservableCollection<MedicalCard>();

        public string FilePath { get; set; }
        public string code { get; set; }
        public BitmapImage image { get; set; }

        private async Task<HttpResponseMessage> SendDataToServer(MedicalCard medical)
        {
            using(var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(medical);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:8080/api/MedicalCards", content);
                return response;
            }
        }

        internal static async Task GetObjectMedicalCard(ListView listView)
        {
            try
            {
                var response = await client.GetAsync("http://localhost:8080/api/MedicalCards");
                var contentBody = await response.Content.ReadAsStringAsync();

                var medicalCard = JsonConvert.DeserializeObject<List<MedicalCard>>(contentBody);

                medicalCardList.Clear();
                foreach (var item in medicalCard)
                {
                    medicalCardList.Add(item);
                }
                listView.ItemsSource = medicalCardList;

            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Уведомление об ошибке", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static async Task DeleteObjectMedicalCard(int cardID)
        {
            try
            {
                var url = $"http://localhost:8080/api/MedicalCards/?id={cardID}";
                using( var client = new HttpClient())
                {
                    var response = await client.DeleteAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Медицинская карта удалена из базы данных", "Удаление медицинской карты", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Уведомление об ошибке", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public async Task<bool> TryAddUniqueMedicalCard(int patientID)
        {
            bool isUnique = false;
            HttpResponseMessage response;

            do
            {
                code = GenerateCode();
                image = GenerateQRCode(code);
                string fileName = $"{code}.png";
                var medicalCard = new MedicalCard()
                {
                    Code = code,
                    PathQRCode = fileName,
                    IDPatient = patientID,
                    
                };

                response = await SendDataToServer(medicalCard);

                if (response.IsSuccessStatusCode)
                {
                    isUnique = true;
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    File.Delete(FilePath);
                }
                else
                {
                    throw new HttpRequestException($"Ошибка: {response.ReasonPhrase}");
                }

            } while (!isUnique);
            return isUnique;
        }

        public string GenerateCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
        public BitmapImage GenerateQRCode(string code)
        {
            using(var qrGenerator = new QRCodeGenerator())
            using(var qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q))
            using(var qrCode = new QRCode(qrCodeData))
            using(var qrCodeImage = qrCode.GetGraphic(20))
            using (var ms = new MemoryStream())
            {
                qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        public void SaveQRCodeToFile(BitmapImage bitmapImage, string fileName)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "PNG Files (*.png)|*.png",
                DefaultExt = "png",
                AddExtension = true,
                FileName = fileName
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                using (var fileStream = new FileStream(FilePath, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                    encoder.Save(fileStream);
                }

            }
        }
    }
}
