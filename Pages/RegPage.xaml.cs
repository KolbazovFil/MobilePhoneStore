using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace MobilePhoneStore.Pages
{
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
            Clear();
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Registration(FIOTextBox.Text, LoginTextBox.Text, PasswordBox.Password, ConfirmPasswordBox.Password, EmailTextBox.Text, PhoneNomberTextBox.Text);
        }

        public bool Registration(string fio, string login, string password, string confirmPassword, string email, string phone)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(confirmPassword) || string.IsNullOrWhiteSpace(fio)) errors.AppendLine("Все поля должны быть заполнены");

            if (login.Contains(" ")) errors.AppendLine("Поле для логина не должно содержать пробелы");
            if (confirmPassword != password) errors.AppendLine("Пароли не совпадают");

            if (!Regex.IsMatch(password, @"^(?=.*[0-9])(?=.*[a-zA-Z])[A-Za-z0-9]{6,}$")) errors.AppendLine("Пароль должен содержать только хотя бы 6 символов, одну цифру и латинскую раскладку");
            if (!string.IsNullOrWhiteSpace(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) errors.AppendLine("Неверный формат электронной почты");
            
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
                    RoleID = 2
                };

                db.Users.Add(userObject);
                db.SaveChanges();
                MessageBox.Show("Регистрация прошла успешно!");
                if (NavigationService != null) NavigationService.Navigate(new AuthPage());
                Clear();
                return true;
            }
        }

        private void Clear()
        {
            FIOTextBox.Clear();
            LoginTextBox.Clear();
            PasswordBox.Clear();
            ConfirmPasswordBox.Clear();
            EmailTextBox.Clear();
            PhoneNomberTextBox.Clear();
        }

    }
}
