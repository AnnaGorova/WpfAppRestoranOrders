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
using System.Windows.Shapes;

namespace WpfAppRestoranOrder.View.Client
{
    /// <summary>
    /// Interaction logic for CustomerInfoWindow.xaml
    /// </summary>
    public partial class CustomerInfoWindow : Window
    {
        public string CustomerName { get; private set; }
        public string PhoneNumber { get; private set; }
        public string DeliveryAddress { get; private set; }

        public CustomerInfoWindow()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введіть ваше ім'я", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show("Введіть ваш телефон", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CustomerName = NameTextBox.Text.Trim();
            PhoneNumber = PhoneTextBox.Text.Trim();
            DeliveryAddress = AddressTextBox.Text.Trim();

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}