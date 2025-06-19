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

namespace MobilePhoneStore.Pages
{
    public partial class MainPageForAdmin : Page
    {
        private string currentBrand = null;
        private string currentCategory = null;
        private decimal minPrice = 0;
        private decimal maxPrice = decimal.MaxValue;

        public MainPageForAdmin()
        {
            InitializeComponent();
            currentCategory = null;
            LoadAllProducts();
        }

        private void LoadAllProducts()
        {
            ListProducts.ItemsSource = Entities.GetContext().Products.ToList();
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddProductPage((sender as Button).DataContext as Products));
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddProductPage(null));
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            var productsForRemoving = ListProducts.SelectedItems.Cast<Products>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить запись в количестве {productsForRemoving.Count()} элементов?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Entities.GetContext().Products.RemoveRange(productsForRemoving);
                    Entities.GetContext().SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");

                    ListProducts.ItemsSource = Entities.GetContext().Products.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
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

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null) NavigationService.Navigate(new RegForAdminPage());
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
    }
}
