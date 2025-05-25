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

namespace Nihongo_Dictionary
{
    /// <summary>
    /// Interaction logic for MainAppView.xaml
    /// </summary>
    public partial class MainAppView : UserControl
    {
        private MainWindow _mainWindow;  // Add a reference to the MainWindow
        public MainAppView(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow; // Store the MainWindow instance
            LoadMainContent(null, null); // Încarcă pagina principală implicit
        }

        private void LoadMainContent(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new DictionaryControl();
        }

        private void LoadLessonsContent(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new LessonsControl();
        }

        private void LoadProfileContent(object sender, RoutedEventArgs e)
        {
            ProfileControl profileControl = new ProfileControl(_mainWindow);
            profileControl.DataContext = _mainWindow; 
            MainContent.Content = profileControl;
        }
    }
}
