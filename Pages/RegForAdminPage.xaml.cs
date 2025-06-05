using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace MobilePhoneStore.Pages
{
    public partial class RegForAdminPage : Page
    {
        private Users _currentUser = new Users();

        public RegForAdminPage()
        {
            InitializeComponent();
            RoleComboBox.ItemsSource = Entities.GetContext().Roles.ToList();
            DataContext = _currentUser;
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Registration(FIOTextBox.Text, LoginTextBox.Text, PasswordBox.Password, ConfirmPasswordBox.Password, EmailTextBox.Text, PhoneNomberTextBox.Text, _currentUser.RoleID);
        }

        public bool Registration(string fio, string login, string password, string confirmPassword, string email, string phone, int roleId)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword) || string.IsNullOrWhiteSpace(fio) || roleId == 0)
                errors.AppendLine("Поля ФИО, логина, пароля и роли пользователя не должны быть пустыми");

            if (login.Contains(" ")) errors.AppendLine("Поле для логина не должно содержать пробелы");

            if (confirmPassword != password) errors.AppendLine("Пароли не совпадают");

            if (!Regex.IsMatch(password, @"^(?=.*[0-9])(?=.*[a-zA-Z]).{6,}$"))
                errors.AppendLine("Пароль должен содержать минимум 6 символов, хотя бы одну цифру и только латинскую раскладку");

            if (!string.IsNullOrWhiteSpace(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.AppendLine("Неверный формат электронной почты, пример: \"empty@business.com\"");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            using (var db = new Entities())
            {
                var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Login == login);

                if (user != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!");
                    return false;
                }

                Users userObject = new Users
                {
                    FullName = fio,
                    Login = login,
                    Password = password,
                    Email = email,
                    Phone = phone,
                    RoleID = roleId
                };

                db.Users.Add(userObject);
                db.SaveChanges();
                MessageBox.Show("Регистрация прошла успешно!");
                if (NavigationService != null) NavigationService.Navigate(new AuthPage());
                Clear();
                return true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
            Clear();
        }

        private void Clear()
        {
            FIOTextBox.Clear();
            LoginTextBox.Clear();
            PasswordBox.Clear();
            ConfirmPasswordBox.Clear();
            EmailTextBox.Clear();
            PhoneNomberTextBox.Clear();
            RoleComboBox.SelectedIndex = -1;
        }
    }
}
