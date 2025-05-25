using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Nihongo_Dictionary
{
    /// <summary>
    /// Interaction logic for ApplicationMainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private User _currentUser;
        public User CurrentUser
        {
            get { return _currentUser; }
            set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    OnPropertyChanged(nameof(CurrentUser)); // Notify about CurrentUser
                }
            }
        }
        private string? _currentUserRole;
        public string? CurrentUserRole
        {
            get { return _currentUserRole; }
            set
            {
                if (_currentUserRole != value)
                {
                    _currentUserRole = value;
                    OnPropertyChanged(nameof(CurrentUserRole));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoginControl loginControl = new LoginControl(this); // Pass 'this' (the MainWindow instance)
            MainContent.Content = loginControl;
        }

        public void LoadDictionaryView()
        {
            MainContent.Content = new MainAppView(mainWindow);
        }
    }
}
