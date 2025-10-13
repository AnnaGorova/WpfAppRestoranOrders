using System.Windows;
using WpfAppRestoranOrder.Admin;
using WpfAppRestoranOrder.Services;
using WpfAppRestoranOrder.View.Client;
using WpfAppRestoranOrder.View.Kitchen;

namespace WpfAppRestoranOrder
{
    public partial class MainWindow : Window
    {
        private DataService _dataService;
        private AdminWindow _adminWindow;
        private ClientWindow _clientWindow;
        private KitchenWindow _kitchenWindow;



        public MainWindow()
        {
            InitializeComponent();
            _dataService = new DataService();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                
                _dataService.LoadAllData();

                

              
                _adminWindow = new AdminWindow(_dataService);
                _adminWindow.Title = "Ресторан - Адмін панель";
                _adminWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                _adminWindow.Show();

                
                _clientWindow = new ClientWindow();
                _clientWindow.Title = "Ресторан - Клієнт";
                _clientWindow.Show();

                
                _kitchenWindow = new KitchenWindow();
                _kitchenWindow.Title = "Ресторан - Кухня";
                _kitchenWindow.Show();

                this.Topmost = true;    
                this.Title = "Ресторан - Головне вікно (закрийте для виходу!!!)";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}", "Помилка");
            }
            
        }


        private void MainWindow_Closed(object sender, EventArgs e)
        {
            _adminWindow?.Close();
            _clientWindow?.Close();
            _kitchenWindow?.Close();    
        }

    }
}