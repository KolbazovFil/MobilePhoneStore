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
using MobilePhoneStore;

namespace MobilePhoneStore.Pages
{
    public partial class ProfilePage : Page
    {
        private Users _currentUser;
        public ProfilePage(Users user)
        {
            InitializeComponent();
            if (user != null)
                _currentUser = user;
            else
                _currentUser = new Users();

            DataContext = _currentUser;

            SetEditMode(false);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if(NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SetEditMode(true);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentUser.Login))
                errors.AppendLine("Поле для логина не может быть пустым");
            if (string.IsNullOrWhiteSpace(_currentUser.FullName))
                errors.AppendLine("Поле для ФИО не может быть пустым");
            if (string.IsNullOrWhiteSpace(_currentUser.Email))
                errors.AppendLine("Введите корректный email");


            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_currentUser.ID == 0)
                Entities.GetContext().Users.Add(_currentUser);

            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Профиль успешно сохранён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                SetEditMode(false);
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetEditMode(bool isEditable)
        {
            LoginTextBox.IsReadOnly = !isEditable;
            FIOTextBox.IsReadOnly = !isEditable;
            EmailTextBox.IsReadOnly = !isEditable;
            PhoneNumberTextBox.IsReadOnly = !isEditable;

            SaveButton.Visibility = isEditable ? Visibility.Visible : Visibility.Collapsed;
            EditButton.Visibility = isEditable ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Exit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Session.IsAuthorized = false;
            Session.CurrentUserLogin = null;

            if (NavigationService != null)
            {
                NavigationService.Navigate(new MainPage());
            }
        }

    }
}
