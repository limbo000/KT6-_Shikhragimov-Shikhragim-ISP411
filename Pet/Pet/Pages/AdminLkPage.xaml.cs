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

namespace Pet.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminLkPage.xaml
    /// </summary>
    public partial class AdminLkPage : Page
    {
        public AdminLkPage()
        {
            InitializeComponent();
            ProductListView.ItemsSource = Data.PetShopEntities.GetContext().Product.ToList();
            Init();
        }

        public void Init()
        {
            ProductListView.ItemsSource = Data.PetShopEntities.GetContext().Product.ToList();
            var manufactList = Data.PetShopEntities.GetContext().Manufacturer.ToList();
            manufactList.Insert(0, new Data.Manufacturer { Name = "Все производители" });
            ManufacturerComboBox.ItemsSource = manufactList;
            ManufacturerComboBox.SelectedIndex = 0;

            if (Classes.Manager.CurrentUser != null)
            {
                FIOLabel.Visibility = Visibility.Visible;
                FIOLabel.Content = $"{Classes.Manager.CurrentUser.UserSurname} " +
                    $"{Classes.Manager.CurrentUser.UserName} " +
                    $"{Classes.Manager.CurrentUser.UserPatronymic}";
            }
            else
            {
                FIOLabel.Visibility = Visibility.Hidden;

            }
            CountOfLabel.Content = $"{Data.PetShopEntities.GetContext().Product.Count()}/" +
                $"{Data.PetShopEntities.GetContext().Product.Count()}";
        }

        public List<Data.Product> _currentProduct = Data.PetShopEntities.GetContext().Product.ToList();
        public void Update()
        {
            try
            {
                _currentProduct = Data.PetShopEntities.GetContext().Product.ToList();
                _currentProduct = (from item in Data.PetShopEntities.GetContext().Product
                                   where item.ProductName1.Name.ToLower().Contains(SearchTextBox.Text) ||
                                   item.ProductDescription.ToLower().Contains(SearchTextBox.Text) ||
                                   item.Manufacturer.Name.ToLower().Contains(SearchTextBox.Text) ||
                                   item.ProductCost.ToString().ToLower().Contains(SearchTextBox.Text) ||
                                   item.ProductQuantityInStock.ToString().ToLower().Contains(SearchTextBox.Text)
                                   select item).ToList();
                if (SortRadioButton.IsChecked == true)
                {
                    _currentProduct = _currentProduct.OrderBy(d => d.ProductCost).ToList();
                }
                if (SortDownRadioButton.IsChecked == true)
                {
                    _currentProduct = _currentProduct.OrderByDescending(d => d.ProductCost).ToList();
                }
                var selected = ManufacturerComboBox.SelectedItem as Data.Manufacturer;
                if (selected != null && selected.Name != "Все производители")
                {
                    _currentProduct = _currentProduct.Where(d => d.IdManufacturer == selected.Id).ToList();

                }
                CountOfLabel.Content = $"{_currentProduct.Count}/{Data.PetShopEntities.GetContext().Product.Count()}";
                ProductListView.ItemsSource = _currentProduct;
            }
            catch (Exception ex)
            {

            }
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update();
        }

        private void SortRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void SortDownRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Update();
        }



        private void BackButon_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new LoginPage());
        }

        private void ManufacturerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }




        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new Pages.AddEditPage((sender as Button).DataContext as Data.Product));
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.Manager.MainFrame.Navigate(new Pages.AddEditPage(null));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
         
            var productToDelete = (sender as Button).DataContext as Data.Product;

            if (productToDelete == null)
            {
                MessageBox.Show("Продукт не выбран.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

     
            var isProductOrdered = Data.PetShopEntities.GetContext().OrderProduct
                .Any(orderItem => orderItem.Id == productToDelete.Id);

            if (isProductOrdered)
            {
                MessageBox.Show("Невозможно удалить продукт, так как он уже был заказан.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

          
            var result = MessageBox.Show($"Вы уверены, что хотите удалить продукт '{productToDelete.ProductName1.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                
                    Data.PetShopEntities.GetContext().Product.Remove(productToDelete);
                    Data.PetShopEntities.GetContext().SaveChanges();

                    MessageBox.Show("Продукт успешно удалён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

           
                    Update();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
