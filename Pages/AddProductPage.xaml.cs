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
    public partial class AddProductPage : Page
    {
        private Products _currentProduct = new Products();

        public AddProductPage(Products product)
        {
            InitializeComponent();
            if (product != null) _currentProduct = product;
            DataContext = _currentProduct;
            CategoriesComboBox.ItemsSource = Entities.GetContext().Categories.ToList();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentProduct.ProductName)) errors.AppendLine("Введите название товара");
            if (string.IsNullOrWhiteSpace(Convert.ToString(_currentProduct.Price))) errors.AppendLine("Введите цену товара");
            if (_currentProduct.Categories == null) errors.AppendLine("Выберите категорию товара");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (_currentProduct.ID == 0) Entities.GetContext().Products.Add(_currentProduct);

            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Сохранение выполнено успешно!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}
