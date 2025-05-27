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
    /// Interaction logic for HiraganaLessonsView.xaml
    /// </summary>
    public partial class HiraganaLessonsView : UserControl
    {
        public HiraganaLessonsView()
        {
            InitializeComponent();
        }
        private void LessonCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                string tag = clickedButton.Tag as string;
                if (tag == "HiraganaFirstRow")
                {
                    if (Window.GetWindow(this) is MainWindow mainWindow)
                    {
                        mainWindow.MainContent.Content = new HiraganaFirstRowPageView(mainWindow);
                    }
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.MainContent.Content = new MainAppView(mainWindow);
            }
        }
    }
    
}
