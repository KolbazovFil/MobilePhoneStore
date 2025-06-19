using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MobilePhoneStore.Pages
{
    public static class Session 
    {
        public static bool IsAuthorized { get; set; } = false;
        public static string CurrentUserLogin { get; set; } = null;
    }

    public partial class MainPage : Page
    {
        private string currentBrand = null;
        private string currentCategory = null;
        private decimal minPrice = 0;
        private decimal maxPrice = decimal.MaxValue;

        public MainPage()
        {
            InitializeComponent();
            currentCategory = null;
            LoadAllProducts();
        }
        private void LoadAllProducts()
        {
            ListProducts.ItemsSource = Entities.GetContext().Products.ToList();
        }
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            {
                if (Visibility == Visibility.Visible)
                {
                    Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                    ApplyFilters();

                    if (Session.IsAuthorized)
                    {
                        Authorization.Visibility = Visibility.Collapsed;
                        ProfilePanel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Authorization.Visibility = Visibility.Visible;
                        ProfilePanel.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
        private void ApplyFilters()
        {
            var products = Entities.GetContext().Products.AsQueryable();

            if (!string.IsNullOrEmpty(currentBrand))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(currentBrand) ||
                    (currentBrand == "Xiaomi" &&
                     (p.ProductName.Contains("Poco") || p.ProductName.Contains("Redmi"))));
            }

            if (!string.IsNullOrEmpty(currentCategory))
            {
                products = products.Where(p => p.Categories.CategoryName == currentCategory);
            }

            products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice);

            ListProducts.ItemsSource = products.ToList();
        }
        private void ProductFilter_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is string brand)
            {
                currentCategory = null;
                currentBrand = brand;
                ApplyFilters();
            }
        }
        private void CategoryFilter_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is string category)
            {
                currentBrand = null;
                currentCategory = category;
                ApplyFilters();
            }
        }
        private void ApplyPriceFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MinPriceTextBox.Text) && decimal.TryParse(MinPriceTextBox.Text, out decimal min))
            {
                minPrice = min;
            }
            else
            {
                minPrice = 0;
            }

            if (!string.IsNullOrEmpty(MaxPriceTextBox.Text) && decimal.TryParse(MaxPriceTextBox.Text, out decimal max))
            {
                maxPrice = max;
            }
            else
            {
                maxPrice = decimal.MaxValue;
            }

            ApplyFilters();
        }
        private void ProductAll_Click(object sender, RoutedEventArgs e)
        {
            currentBrand = null;
            currentCategory = null;
            minPrice = 0;
            maxPrice = decimal.MaxValue;

            MinPriceTextBox.Text = "";
            MaxPriceTextBox.Text = "";

            LoadAllProducts();
        }
        private void Authorization_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null) NavigationService.Navigate(new AuthPage());
        }
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Session.IsAuthorized || string.IsNullOrEmpty(Session.CurrentUserLogin))
            {
                MessageBox.Show("Вы не авторизованы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var currentUser = Entities.GetContext().Users.FirstOrDefault(u => u.Login == Session.CurrentUserLogin);

            if (currentUser == null)
            {
                MessageBox.Show("Пользователь не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (NavigationService != null) NavigationService.Navigate(new ProfilePage(currentUser));
        }
        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Session.IsAuthorized)
            {
                MessageBox.Show("Пожалуйста, войдите сначало в систему.", "Требуется авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (sender is Button btn && btn.DataContext is Products product)
            {
                MessageBox.Show($"Поздравляю, вы стали обладателем нового {product.ProductName}!");
            }
        }
    }
}