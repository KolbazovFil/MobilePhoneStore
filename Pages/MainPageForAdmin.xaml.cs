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
        public MainPageForAdmin()
        {
            InitializeComponent();
            ListProducts.ItemsSource = Entities.GetContext().Products.ToList();
            CategoriesComboBox.SelectedIndex = 0;
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
            if (Visibility == Visibility.Visible)
            {
                Entities.GetContext().ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                ListProducts.ItemsSource = Entities.GetContext().Products.ToList();
            }
        }

        private void ApplyPriceFilterButton_Click(object sender, RoutedEventArgs e)
        {
            decimal minPrice = 0;
            decimal maxPrice = decimal.MaxValue;

            if (!string.IsNullOrEmpty(MinPriceTextBox.Text) && decimal.TryParse(MinPriceTextBox.Text, out decimal min))
            {
                minPrice = min;
            }

            if (!string.IsNullOrEmpty(MaxPriceTextBox.Text) && decimal.TryParse(MaxPriceTextBox.Text, out decimal max))
            {
                maxPrice = max;
            }

            FilterProducts();
        }

        private void CategoriesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterProducts();
        }

        private void FilterProducts()
        {
            var selectedCategory = (CategoriesComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            var products = Entities.GetContext().Products.AsQueryable();

            if (selectedCategory != null && selectedCategory != "Все категории")
            {
                products = products.Where(p => p.Categories.CategoryName == selectedCategory);
            }

            decimal minPrice = 0;
            decimal maxPrice = decimal.MaxValue;

            if (!string.IsNullOrEmpty(MinPriceTextBox.Text) && decimal.TryParse(MinPriceTextBox.Text, out decimal min))
            {
                minPrice = min;
            }

            if (!string.IsNullOrEmpty(MaxPriceTextBox.Text) && decimal.TryParse(MaxPriceTextBox.Text, out decimal max))
            {
                maxPrice = max;
            }

            products = products.Where(p => p.Price >= minPrice && p.Price <= maxPrice);

            ListProducts.ItemsSource = products.ToList();
        }

        private void RegUser_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService != null) NavigationService.Navigate(new RegForAdminPage());
        }
    }
}
