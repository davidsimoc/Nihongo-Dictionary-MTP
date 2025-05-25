using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Text;
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
    public partial class LoginControl : UserControl
    {
        private MainWindow _mainWindow;
        private const string SessionFileName = "session.txt"; // Numele fișierului în care salvăm sesiunea

        
        public LoginControl(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            LoadSession(); // Încărcăm sesiunea la initializarea controlului Login
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            bool rememberMe = chkRememberMe.IsChecked ?? false; // Obtinem starea checkbox-ului

            if (AuthenticateUser(username, password))
            {
                System.Diagnostics.Debug.WriteLine($"CurrentUserRole after login: {_mainWindow.CurrentUserRole}");
                if (rememberMe)
                {
                    SaveSession(username); // Salvăm sesiunea dacă utilizatorul a bifat "Ține-mă minte"
                }

                // Creează o instanță a MainAppView
                var mainAppView = new MainAppView(_mainWindow);

                // Setează MainAppView ca conținut al MainContent în MainWindow
                if (Window.GetWindow(this) is MainWindow mainWindow)
                {
                    mainWindow.MainContent.Content = mainAppView;
                }
            }
            else
            {
                lblErrorMessage.Text = "Nume de utilizator sau parolă incorecte.";
            }
        }

        private void btnSwitchToRegister_Click(object sender, RoutedEventArgs e)
        {
            loginPanel.Visibility = Visibility.Collapsed;
            registerPanel.Visibility = Visibility.Visible;
            lblErrorMessage.Text = "";
            lblRegisterErrorMessage.Text = "";
            txtNewUsername.Text = "";
            txtNewPassword.Password = "";
            txtConfirmPassword.Password = "";
        }

        private void btnSwitchToLogin_Click(object sender, RoutedEventArgs e)
        {
            registerPanel.Visibility = Visibility.Collapsed;
            loginPanel.Visibility = Visibility.Visible;
            lblRegisterErrorMessage.Text = "";
            lblErrorMessage.Text = "";
            txtUsername.Text = "";
            txtPassword.Password = "";
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = txtNewUsername.Text;
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                lblRegisterErrorMessage.Text = "Toate câmpurile sunt obligatorii.";
                return;
            }

            if (newPassword != confirmPassword)
            {
                lblRegisterErrorMessage.Text = "Parolele nu se potrivesc.";
                return;
            }

            if (RegisterNewUser(newUsername, newPassword))
            {
                MessageBox.Show("Înregistrare reușită!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                btnSwitchToLogin_Click(sender, e);
            }
            else
            {
                lblRegisterErrorMessage.Text = "Numele de utilizator există deja.";
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            string connectionString = "Data Source=UsersDatabase.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT Username, Password, Role, AboutMe FROM Users WHERE Username = @Username";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string? storedPassword = reader["Password"].ToString();
                                if (password.Equals(storedPassword))
                                {
                                    string? role = reader["Role"].ToString();
                                    string? retrievedUsername = reader["Username"].ToString(); // Get username from DB
                                    string? aboutMe = reader["AboutMe"].ToString();
                                    if (Window.GetWindow(this) is MainWindow mainWindow)
                                    {
                                        mainWindow.CurrentUserRole = role; // Use the property setter
                                    }

                                    User user = new User(retrievedUsername, role, aboutMe);
                                    _mainWindow.CurrentUser = user; // Store user in MainWindow
                                    return true;
                                }
                            }
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    lblErrorMessage.Text = $"Eroare la baza de date: {ex.Message}";
                }
            }
            return false;
        }

        private bool RegisterNewUser(string username, string password)
        {
            string connectionString = "Data Source=UsersDatabase.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    using (SQLiteCommand checkCommand = new SQLiteCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Username", username);
                        long userCount = (long)checkCommand.ExecuteScalar();
                        if (userCount > 0)
                        {
                            return false;
                        }
                    }
                    string insertQuery = "INSERT INTO Users (Username, Password, Role) VALUES (@Username, @Password, 'user')"; //added role
                    using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Username", username);
                        insertCommand.Parameters.AddWithValue("@Password", password);
                        insertCommand.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (SQLiteException ex)
                {
                    lblRegisterErrorMessage.Text = $"Eroare la baza de date: {ex.Message}";
                    return false;
                }
            }
        }

        private void SaveSession(string username)
        {
            try
            {
                File.WriteAllText(SessionFileName, username);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Eroare la salvarea sesiunii: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadSession()
        {
            if (File.Exists(SessionFileName))
            {
                try
                {
                    string username = File.ReadAllText(SessionFileName);
                    txtUsername.Text = username;
                    chkRememberMe.IsChecked = true;

                    // Simulate login and get user data
                    if (AuthenticateUser(username, "")) // Pass empty password since we only have username
                    {
                        // Creează o instanță a MainAppView și o setează ca conținut
                        var mainAppView = new MainAppView(_mainWindow);
                        if (Window.GetWindow(this) is MainWindow mainWindow)
                        {
                            mainWindow.MainContent.Content = mainAppView;
                        }
                    }
                    else
                    {
                        // Clear session if authentication fails
                        File.Delete(SessionFileName);
                    }

                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Eroare la încărcarea sesiunii: {ex.Message}", "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            loginPanel.Visibility = Visibility.Collapsed;
            registerPanel.Visibility = Visibility.Visible;
        }
    }
}
