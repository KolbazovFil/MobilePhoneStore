using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
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

namespace MobilePhoneStore.Pages
{

    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            if (Auth(LoginTextBox.Text, PasswordBox.Password)) Clear();
        }

        public bool Auth(string login, string password)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(login)) errors.AppendLine("Введите логин!");
            if (string.IsNullOrWhiteSpace(password)) errors.AppendLine("Введите пароль!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return false;
            }

            using (var db = new Entities())
            {
                var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Login == login);

                if (user == null)
                {
                    MessageBox.Show("Пользователь с таким логином не существует!");
                    return false;
                }

                if (password != PasswordBox.Password)
                {
                    MessageBox.Show("Неверный пароль!");
                    return false;
                }

                Session.IsAuthorized = true;
                Session.CurrentUserLogin = user.Login;

                if (NavigationService != null)
                {
                    if (user.RoleID == 1)
                    {
                        NavigationService.Navigate(new MainPageForAdmin());
                    }
                    else
                    {
                        NavigationService.Navigate(new MainPage());
                    }
                }
                return true;
            }
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null) NavigationService.Navigate(new RegPage());
            Clear();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if(NavigationService.CanGoBack) NavigationService.GoBack();
            Clear();
        }

        private void Clear()
        {
            LoginTextBox.Clear();
            PasswordBox.Clear();
        }
    }
}
