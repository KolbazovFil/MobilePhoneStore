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
        private int loginAttempts = 0;
        private string captchaText;

        public AuthPage()
        {
            InitializeComponent();
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            Auth(LoginTextBox.Text, PasswordBox.Password);
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
                    loginAttempts++;
                    CheckForCaptcha();
                    return false;
                }

                if (password!=PasswordBox.Password)
                {
                    MessageBox.Show("Неверный пароль!");
                    loginAttempts++;
                    CheckForCaptcha();
                    return false;
                }

                if (loginAttempts >= 3)
                {
                    if (CaptchaTextBox.Text != captchaText)
                    {
                        MessageBox.Show("Неверная капча!");
                        loginAttempts++;
                        return false;
                    }
                }

                loginAttempts = 0;
                HideCaptcha();

                if (NavigationService != null) NavigationService.Navigate(new AddProductPage(null));

                LoginTextBox.Clear();
                PasswordBox.Clear();
                CaptchaTextBox.Clear();
                return true;
            }
        }

        public void CheckForCaptcha()
        {
            if (loginAttempts >= 3)
            {
                GenerateCaptcha();
                CaptchaLabel.Visibility = Visibility.Visible;
                CaptchaTextBox.Visibility = Visibility.Visible;
                EnterCaptcha.Visibility = Visibility.Visible;
            }
        }

        public void GenerateCaptcha()
        {
            Random random = new Random();
            captchaText = new string(Enumerable.Range(0, 5).Select(x => (char)random.Next('A', 'Z' + 1)).ToArray());

            CaptchaLabel.Text = captchaText;
        }

        public void HideCaptcha()
        {
            EnterCaptcha.Visibility = Visibility.Collapsed;
            CaptchaLabel.Visibility = Visibility.Collapsed;
            CaptchaTextBox.Visibility = Visibility.Collapsed;
            CaptchaTextBox.Clear();
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null) NavigationService.Navigate(new RegPage());
            LoginTextBox.Clear();
            PasswordBox.Clear();
        }

    }
}
