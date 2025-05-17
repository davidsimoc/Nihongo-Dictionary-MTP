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
            MainContent.Content = new LoginControl(this);
        }

        public void LoadDictionaryView()
        {
            MainContent.Content = new MainAppView();
        }
    }
}
