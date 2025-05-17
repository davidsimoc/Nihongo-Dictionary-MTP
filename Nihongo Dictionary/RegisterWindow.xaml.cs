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
using System.Windows.Shapes;

namespace Nihongo_Dictionary
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string newUsername = txtNewUsername.Text;
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (string.IsNullOrEmpty(newUsername) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                lblErrorMessage.Text = "Toate câmpurile sunt obligatorii.";
                return;
            }

            if (newPassword != confirmPassword)
            {
                lblErrorMessage.Text = "Parolele nu se potrivesc.";
                return;
            }

            if (RegisterNewUser(newUsername, newPassword)) 
            {
                MessageBox.Show("Înregistrare reușită!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); 
            }
            else
            {
                lblErrorMessage.Text = "Numele de utilizator există deja.";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool RegisterNewUser(string username, string password)
        {
            string connectionString = "Data Source=UsersDatabase.db;Version=3;"; // Ajustați calea dacă este necesar

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

                    string insertQuery = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
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
                    lblErrorMessage.Text = $"Eroare la baza de date: {ex.Message}";
                    return false;
                }
            }
        }
    }
}
