﻿using System;
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
    /// Interaction logic for LessonsControl.xaml
    /// </summary>
    public partial class LessonsControl : UserControl
    {
        public LessonsControl()
        {
            InitializeComponent();
        }

        private void HiraganaLessons_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                string tag = clickedButton.Tag as string;
                if (tag == "HiraganaRows")
                {
                    if (Window.GetWindow(this) is MainWindow mainWindow)
                    {
                        mainWindow.MainContent.Content = new HiraganaLessonsView();
                    }
                }
            }
        }

        private void KatakanaLessons_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                string tag = clickedButton.Tag as string;
                if (tag == "HiraganaRows")
                {
                    if (Window.GetWindow(this) is MainWindow mainWindow)
                    {
                        
                    }
                }
            }
        }

    }
}
