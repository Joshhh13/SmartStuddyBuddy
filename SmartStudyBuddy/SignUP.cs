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

        private static SignUP instance;
        string connectionString = "SERVER=localhost;DATABASE=studybuddy;UID=root;PASSWORD=;";

        public SignUP()
        {
            InitializeComponent();
        }

        private void SignUP_Load(object sender, EventArgs e)
        {
            roundedTextBox1.Text = "👤 Username";
            roundedTextBox2.Text = "📧 Email";
            roundedTextBox3.Text = "🔒 Password";
            roundedTextBox4.Text = "🔑 Confirm password";

            roundedTextBox3.UseSystemPasswordChar = false;
            roundedTextBox4.UseSystemPasswordChar = false;

            roundedTextBox1.Enter += TextBox_Enter;
            roundedTextBox1.Leave += TextBox_Leave;
            roundedTextBox2.Enter += TextBox_Enter;
            roundedTextBox2.Leave += TextBox_Leave;
            roundedTextBox3.Enter += TextBox_Enter;
            roundedTextBox3.Leave += TextBox_Leave;
            roundedTextBox4.Enter += TextBox_Enter;
            roundedTextBox4.Leave += TextBox_Leave;
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox == roundedTextBox1 && textBox.Text == "👤 Username")
            {
                textBox.Text = "";
                roundedTextBox1.ForeColor = Color.Black;
            }
            else if (textBox == roundedTextBox2 && textBox.Text == "📧 Email")
            {
                textBox.Text = "";
                roundedTextBox2.ForeColor = Color.Black;
            }
            else if (textBox == roundedTextBox3 && textBox.Text == "🔒 Password")
            {
                textBox.Text = "";
                textBox.UseSystemPasswordChar = true; 
                roundedTextBox3.ForeColor = Color.Black;
            }
            else if (textBox == roundedTextBox4 && textBox.Text == "🔑 Confirm password")
            {
                textBox.Text = "";
                textBox.UseSystemPasswordChar = true;
                roundedTextBox4.ForeColor = Color.Black;
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox == roundedTextBox1 && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "👤 Username";
                roundedTextBox1.ForeColor = Color.Gray;
            }
            else if (textBox == roundedTextBox2 && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "📧 Email";
                roundedTextBox2.ForeColor = Color.Gray;
            }
            else if (textBox == roundedTextBox3 && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "🔒 Password";
                textBox.UseSystemPasswordChar = false;
                roundedTextBox3.ForeColor = Color.Gray;
            }
            else if (textBox == roundedTextBox4 && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "🔑 Confirm password";
                textBox.UseSystemPasswordChar = false;
                roundedTextBox4.ForeColor = Color.Gray;
            }
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
        
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

                    string query = "INSERT INTO users (username, email, password) VALUES (@username, @email, @password)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password); 

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Registration Successful! Welcome to Study Buddy!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ClearFields();

                        StudyBuddy studyBuddyForm = new StudyBuddy();
                        studyBuddyForm.Show();

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

        public static SignUP GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new SignUP();
            }
            return instance;
        }
    }
}
