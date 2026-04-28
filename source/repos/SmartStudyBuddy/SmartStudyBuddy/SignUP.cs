using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SmartStudyBuddy
{
    public partial class SignUP : Form
    {
        // MySQL Connection String
        string connectionString = "SERVER=localhost;DATABASE=studybuddy;UID=root;PASSWORD=;";

        public SignUP()
        {
            InitializeComponent();
        }

        private void SignUP_Load(object sender, EventArgs e)
        {
            // Optional: Clear all fields on load
            ClearFields();
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            // Get values from textboxes
            string username = roundedTextBox1.Text.Trim();
            string email = roundedTextBox2.Text.Trim();
            string password = roundedTextBox3.Text.Trim();
            string confirmPassword = roundedTextBox4.Text.Trim();

            // Validation
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill in all fields!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate email format
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if passwords match
            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                roundedTextBox3.Clear();
                roundedTextBox4.Clear();
                roundedTextBox3.Focus();
                return;
            }

            // Check if password is strong enough (at least 6 characters)
            if (password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Register user
            RegisterUser(username, email, password);
        }

        private void RegisterUser(string username, string email, string password)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if username or email already exists
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username OR email = @email";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@username", username);
                    checkCmd.Parameters.AddWithValue("@email", email);

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        MessageBox.Show("Username or Email already exists!", "Registration Failed",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Insert new user
                    string query = "INSERT INTO users (username, email, password) VALUES (@username, @email, @password)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password); // Note: You should hash this in production!

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Registration Successful! Welcome to Study Buddy!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Clear fields
                        ClearFields();

                        // Open StudyBuddy form
                        StudyBuddy studyBuddyForm = new StudyBuddy();
                        studyBuddyForm.Show();

                        // Close the SignUp form
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Registration failed. Please try again.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ClearFields()
        {
            roundedTextBox1.Clear();
            roundedTextBox2.Clear();
            roundedTextBox3.Clear();
            roundedTextBox4.Clear();
            roundedTextBox1.Focus();
        }

        // Optional: Add a link to Login form if you have one
        private void linkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // If you have a Login form
            // Login loginForm = new Login();
            // loginForm.Show();
            // this.Hide();
        }
    }
}
