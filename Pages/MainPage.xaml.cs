using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MobilePhoneStore.Pages
{
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

        // Метод загрузки списка продуктов
        private void LoadAllProducts()
        {
            ListProducts.ItemsSource = Entities.GetContext().Products.ToList();
        }

        // Метод обновления страницы
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                ApplyFilters();
            }
        }

        // Метод применения текущих фильтров к списку продуктов
        private void ApplyFilters()
        {
            var products = Entities.GetContext().Products.AsQueryable();

            if (!string.IsNullOrEmpty(currentBrand))
            {
                products = currentBrand == "Xiaomi"
                    ? products.Where(p => p.ProductName.Contains("Xiaomi") ||
                                          p.ProductName.Contains("Poco") ||
                                          p.ProductName.Contains("Redmi"))
                    : products.Where(p => p.ProductName.Contains(currentBrand));
            }

            if (!string.IsNullOrEmpty(currentCategory))
            {
                products = products.Where(p => p.Categories.CategoryName == currentCategory);
            }

            products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice);

            ListProducts.ItemsSource = products.ToList();
        }

        // Обработчик клика по элементу меню (Продукция)
        private void ProductFilter_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is string brand)
            {
                currentCategory = null;
                currentBrand = brand;
                ApplyFilters();
            }
        }

        // Обработчик клика по элементу меню (Категории)
        private void CategoryFilter_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is string category)
            {
                currentBrand = null;
                currentCategory = category;
                ApplyFilters();
            }
        }

        // Обрабодчик кнопки "применить фильтр"
        private void ApplyPriceFilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MinPriceTextBox.Text) && decimal.TryParse(MinPriceTextBox.Text, out decimal min))
                minPrice = min;
            else
                minPrice = 0;

            if (!string.IsNullOrEmpty(MaxPriceTextBox.Text) && decimal.TryParse(MaxPriceTextBox.Text, out decimal max))
                maxPrice = max;
            else
                maxPrice = decimal.MaxValue;

            ApplyFilters();
        }

        // Обработчик клика по элементу меню (DВсе)
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

        // Переход на страницу авторизации
        private void Authorization_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null) NavigationService.Navigate(new AuthPage());
            LogInProfile();
        }

        // Переход на страницу регистрации
        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null) NavigationService.Navigate(new RegPage());
        }

        private void AddToShopCartButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LogInProfile()
        {
            Authorization.Visibility = Visibility.Collapsed;
            ProfilePanel.Visibility = Visibility.Visible;
        }


    }
}
