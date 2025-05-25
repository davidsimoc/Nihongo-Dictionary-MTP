using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
    /// Interaction logic for ProfileControl.xaml
    /// </summary>
    public partial class ProfileControl : UserControl
    {
        private MainWindow _mainWindow;
        private bool _isEditing = false;
        private const string DbConnectionString = "Data Source=UsersDatabase.db;Version=3;"; // Moved to a constant

        public ProfileControl(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            // Setăm DataContext pentru a accesa CurrentUser
            DataContext = _mainWindow;
            LoadAboutMe(); // Încarcă "About Me" din baza de date
        }

        public void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainContent.Content = new LoginControl(_mainWindow); // Go back to login
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditing = true;
            AboutYouTextBox.IsReadOnly = false;
            EditButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Visible;
            SaveButton.Visibility = Visibility.Visible;
            AboutYouTextBox.Focus();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditing = false;
            AboutYouTextBox.IsReadOnly = true;
            EditButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Collapsed;
            LoadAboutMe();  // Reload the original text
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditing = false;
            AboutYouTextBox.IsReadOnly = true;
            EditButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Collapsed;
            SaveAboutMe();
        }

        private void LoadAboutMe()
        {
            if (_mainWindow.CurrentUser != null)
            {
                using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString)) // Use the constant
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT AboutMe FROM Users WHERE Username = @Username";
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Username", _mainWindow.CurrentUser.Username);
                            object result = command.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                _mainWindow.CurrentUser.AboutMe = result.ToString();
                            }
                            else
                            {
                                _mainWindow.CurrentUser.AboutMe = ""; // Or any default value
                            }
                            AboutYouTextBox.Text = _mainWindow.CurrentUser.AboutMe;
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show($"Error loading About Me: {ex.Message}");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
        private void SaveAboutMe()
        {
            if (_mainWindow.CurrentUser != null)
            {
                using (SQLiteConnection connection = new SQLiteConnection(DbConnectionString)) // Use the constant
                {
                    try
                    {
                        connection.Open();
                        string query = "UPDATE Users SET AboutMe = @AboutMe WHERE Username = @Username";
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@AboutMe", AboutYouTextBox.Text);
                            command.Parameters.AddWithValue("@Username", _mainWindow.CurrentUser.Username);
                            command.ExecuteNonQuery();
                            _mainWindow.CurrentUser.AboutMe = AboutYouTextBox.Text; // Update the property
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show($"Error saving About Me: {ex.Message}");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
    }
}
