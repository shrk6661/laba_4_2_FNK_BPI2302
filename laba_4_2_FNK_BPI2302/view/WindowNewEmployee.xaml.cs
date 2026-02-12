using System;
using System.Windows;
using laba_4_2_FNK_BPI2302.model;
using laba_4_2_FNK_BPI2302.viewmodel;

namespace laba_4_2_FNK_BPI2302.view
{
    public partial class WindowNewEmployee : Window
    {
        public WindowNewEmployee()
        {
            InitializeComponent();

            //должности
            var roleVM = new RoleViewModel();
            CbRole.ItemsSource = roleVM.ListRole;

            //дата
            var person = DataContext as PersonDpo;
            if (person != null && string.IsNullOrEmpty(person.Birthday))
            {
                person.Birthday = DateTime.Now.ToString("dd.MM.yyyy");
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var person = DataContext as PersonDpo;
            if (person == null)
            {
                DialogResult = false;
                return;
            }

            //проверка полей
            if (string.IsNullOrWhiteSpace(person.LastName))
            {
                MessageBox.Show("Введите фамилию!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(person.FirstName))
            {
                MessageBox.Show("Введите имя!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CbRole.SelectedItem == null)
            {
                MessageBox.Show("Выберите должность!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //сохраняем дату
            if (ClBirthday.SelectedDate.HasValue)
            {
                person.Birthday = ClBirthday.SelectedDate.Value.ToString("dd.MM.yyyy");
            }

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}