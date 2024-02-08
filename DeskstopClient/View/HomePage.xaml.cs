using DeskstopClient.ViewModel;
using ModelDataBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeskstopClient.View
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private Patient _selectedPatient { get; set; }
        private DiseaseHistory _selectedDisease { get; set; }
        private MedicalCard _selectedMedicalCard {  get; set; }
        public HomePage()
        {
            InitializeComponent();
        }

        private async Task LoadData()
        {
            if(listViewPatient.Visibility == Visibility.Visible)
            {
                await ViewModelPatient.GetObjectPatient(listViewPatient);
            }
            else
            {
                await ViewModelDiseaseHistory.GetObjectDiseaseHistory(listViewDesiaseHistory);
                await ViewModelMedicalCard.GetObjectMedicalCard(listViewMedicalCard);
                await ViewModelPatient.GetPatientInComboBox(cmbSelectPatientForDiseaseHistory);
                await ViewModelPatient.GetPatientInComboBox(cmbSelectPatientForMedicalCard);
            }
        }

        private async void btnDiseaseHistory_Click(object sender, RoutedEventArgs e)
        {
            if (listViewPatient.Visibility == Visibility.Visible)
            {
                listViewPatient.Visibility = Visibility.Collapsed;
                listViewDesiaseHistory.Visibility = Visibility.Visible;

                gridPatient.Visibility = Visibility.Collapsed;
                gridDiseaseHistory.Visibility = Visibility.Visible;

                btnDiseaseHistory.Content = "Пациенты";
                btnMedicalcard.Visibility = Visibility.Collapsed;
                await LoadData();
            }
            else
            {
                listViewPatient.Visibility = Visibility.Visible;
                listViewDesiaseHistory.Visibility= Visibility.Collapsed;

                gridPatient.Visibility = Visibility.Visible;
                gridDiseaseHistory.Visibility = Visibility.Collapsed;

                btnDiseaseHistory.Content = "История болезни";
                btnMedicalcard.Visibility = Visibility.Visible;
                await LoadData();
            }
        }

        private async void btnMedicalCard_Click(object sender, RoutedEventArgs e)
        {
            if (listViewPatient.Visibility == Visibility.Visible)
            {
                listViewPatient.Visibility= Visibility.Collapsed;
                listViewMedicalCard.Visibility = Visibility.Visible;

                gridPatient.Visibility = Visibility.Collapsed;
                gridMedicalCard.Visibility= Visibility.Visible;

                btnMedicalcard.Content = "Пациенты";
                btnDiseaseHistory.Visibility = Visibility.Collapsed;
                await LoadData();
            }
            else
            {
                listViewPatient.Visibility = Visibility.Visible;
                listViewMedicalCard.Visibility = Visibility.Collapsed;

                gridPatient.Visibility = Visibility.Visible;
                gridMedicalCard.Visibility = Visibility.Collapsed;

                btnMedicalcard.Content = "Медицинская карта";
                btnDiseaseHistory.Visibility = Visibility.Visible;
                await LoadData();
            }
        }

        private async void btnAddpatient_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                if (txbFirstName.Text == "" && txbLastName.Text == "" && txbPatronymic.Text == "" && txbPhone.Text == "" && txbEmail.Text == "" && dpDateOfBirth.SelectedDate.Value == null)
                {
                    MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (_selectedPatient == null)
                {
                    await ViewModelPatient.PostObjectPatient(txbFirstName.Text, txbLastName.Text, txbPatronymic.Text, txbPhone.Text, txbEmail.Text, dpDateOfBirth.SelectedDate.Value);
                    MessageBox.Show("Добавлен новый пациент", "Добавление пациента", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ViewModelPatient.editPatientID = _selectedPatient.ID;
                    await ViewModelPatient.PutObjectPatient(txbFirstName.Text, txbLastName.Text, txbPatronymic.Text, txbPhone.Text, txbEmail.Text, dpDateOfBirth.SelectedDate.Value);
                    MessageBox.Show("Данные пациента были обновлены", "Обновление данных пациента", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                await LoadData();
                ClearText();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
        }

        private void ClearText()
        {
            txbFirstName.Text = string.Empty;
            txbLastName.Text = string.Empty;
            txbPatronymic.Text = string.Empty;
            txbEmail.Text = string.Empty;
            txbPhone.Text = string.Empty;
            dpDateOfBirth.Text = string.Empty;

            txbNameDisease.Text = string.Empty;
            txbDescriptionDisease.Text= string.Empty;
            dpDateOfDisease.Text = string.Empty;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadData();
        }

        private async void btnAddDiseaseHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var diseaseAdd = cmbSelectPatientForDiseaseHistory.SelectedItem as Patient;

                if(diseaseAdd != null && txbNameDisease.Text == "" && txbDescriptionDisease.Text == "" && dpDateOfDisease.SelectedDate.Value == null)
                {
                    MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (_selectedDisease == null)
                {
                    await ViewModelDiseaseHistory.PostObjectDiseaseHistory(diseaseAdd.ID, txbNameDisease.Text, txbDescriptionDisease.Text, dpDateOfDisease.SelectedDate.Value);
                    ClearText();
                    MessageBox.Show($"Добавлена история болезни для {diseaseAdd.LastName} {diseaseAdd.FirstName} {diseaseAdd.Patronymic}", "Добавление истории болезни", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ViewModelDiseaseHistory.editDiseaseId = _selectedDisease.ID;
                    await ViewModelDiseaseHistory.PutObjectDiseaseHistory(_selectedDisease.ID, txbNameDisease.Text, txbDescriptionDisease.Text, dpDateOfDisease.SelectedDate.Value);
                    MessageBox.Show("Данные истории болезни были обновлены", "Обновление данных истории болезни", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                await LoadData();
                ClearText();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void btnEditPatientButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient != null)
            {
                txbFirstName.Text = _selectedPatient.FirstName;
                txbLastName.Text = _selectedPatient.LastName;
                txbPatronymic.Text = _selectedPatient.Patronymic;
                txbPhone.Text = _selectedPatient.Phone;
                txbEmail.Text= _selectedPatient.Email;
                dpDateOfBirth.SelectedDate = _selectedPatient.DateOfBirth;
            }
        }

        private async void btnDeletePatientButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient != null)
            {
                await ViewModelPatient.DeleteObjectPatient(_selectedPatient.ID);
                await LoadData();
            }
        }

        private void listViewPatient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedPatient = (Patient)listViewPatient.SelectedItem;
        }

        private void listViewDesiaseHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedDisease = (DiseaseHistory)listViewDesiaseHistory.SelectedItem;
        }

        private async void btnDeleteDiseaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDisease != null)
            {
                await ViewModelDiseaseHistory.DeleteObjectDisease(_selectedDisease.ID);
                await LoadData();
            }
        }

        private void btnEditDiseaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDisease != null)
            {
                txbNameDisease.Text = _selectedDisease.NameDisease;
                txbDescriptionDisease.Text = _selectedDisease.Description;
                dpDateOfDisease.SelectedDate = _selectedDisease.DateOfDisease;
            }
        }

        private async void btnDeleteMedicalButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedMedicalCard != null)
            {
                await ViewModelMedicalCard.DeleteObjectMedicalCard(_selectedMedicalCard.ID);
                await LoadData();
            }
        }


        private void listViewMedicalCard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedMedicalCard = (MedicalCard)listViewMedicalCard.SelectedItem;
        }

        private async void btnAddMedicalCard_Click(object sender, RoutedEventArgs e)
        {
            var medicalCardAdd = cmbSelectPatientForMedicalCard.SelectedItem as Patient;
            var viewModel = new ViewModelMedicalCard();
            if(medicalCardAdd != null)
            {
                bool isUnique = await viewModel.TryAddUniqueMedicalCard(medicalCardAdd.ID);
                var code = viewModel.code;
                var image = viewModel.image;
                if (isUnique)
                {
                    viewModel.SaveQRCodeToFile(image, code);
                }
                await LoadData();
            }
        }
    }
}
