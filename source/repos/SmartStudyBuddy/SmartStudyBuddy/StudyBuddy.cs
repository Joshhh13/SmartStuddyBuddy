using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace SmartStudyBuddy
{
    public partial class StudyBuddy : Form
    {
        // MySQL Connection String
        string connectionString = "SERVER=localhost;DATABASE=studybuddy;UID=root;PASSWORD=;";

        public StudyBuddy()
        {
            InitializeComponent();
        }

        private void StuddyBuddy_Load(object sender, EventArgs e)
        {
            // Setup RichTextBox
            richTextBox1.ReadOnly = true;
            SetupRoundedTextBox();

            // Setup Email TextBox
            SetupEmailTextBox();

            // Setup Password TextBox
            SetupPasswordTextBox();

            // Check for saved credentials (Remember Me)
            CheckRememberMe();
        }

        private void SetupRoundedTextBox()
        {
            GraphicsPath path = new GraphicsPath();
            int radius = 50;

            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(richTextBox1.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(richTextBox1.Width - radius, richTextBox1.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, richTextBox1.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();

            richTextBox1.Region = new Region(path);
            richTextBox1.BorderStyle = BorderStyle.None;
        }

        private void SetupEmailTextBox()
        {
            roundedTextBox1.ForeColor = Color.Gray;
            roundedTextBox1.Text = "Email Address";

            roundedTextBox1.Enter += (object s, EventArgs args) =>
            {
                if (roundedTextBox1.Text == "Email Address")
                {
                    roundedTextBox1.Text = "";
                    roundedTextBox1.ForeColor = Color.Black;
                }
            };

            roundedTextBox1.Leave += (object s, EventArgs args) =>
            {
                if (string.IsNullOrWhiteSpace(roundedTextBox1.Text))
                {
                    roundedTextBox1.Text = "Email Address";
                    roundedTextBox1.ForeColor = Color.Gray;
                }
            };
        }

        private void SetupPasswordTextBox()
        {
            roundedTextBox2.ForeColor = Color.Gray;
            roundedTextBox2.Text = "Password";
            roundedTextBox2.PasswordChar = '\0';

            roundedTextBox2.Enter += (object s, EventArgs args) =>
            {
                if (roundedTextBox2.Text == "Password")
                {
                    roundedTextBox2.Text = "";
                    roundedTextBox2.ForeColor = Color.Black;
                    roundedTextBox2.PasswordChar = '*';
                }
            };

            roundedTextBox2.Leave += (object s, EventArgs args) =>
            {
                if (string.IsNullOrWhiteSpace(roundedTextBox2.Text))
                {
                    roundedTextBox2.Text = "Password";
                    roundedTextBox2.ForeColor = Color.Gray;
                    roundedTextBox2.PasswordChar = '\0';
                }
            };
        }

        private void CheckRememberMe()
        {
            // Check if there are saved credentials
            if (Properties.Settings.Default.RememberMe &&
                !string.IsNullOrEmpty(Properties.Settings.Default.SavedEmail))
            {
                roundedTextBox1.Text = Properties.Settings.Default.SavedEmail;
                roundedTextBox1.ForeColor = Color.Black;
                roundedTextBox2.Text = Properties.Settings.Default.SavedPassword;
                roundedTextBox2.ForeColor = Color.Black;
                roundedTextBox2.PasswordChar = '*';
                checkBox1.Checked = true;
            }
        }

        private void SaveCredentials()
        {
            if (checkBox1.Checked)
            {
                // Save credentials
                Properties.Settings.Default.RememberMe = true;
                Properties.Settings.Default.SavedEmail = roundedTextBox1.Text;
                Properties.Settings.Default.SavedPassword = roundedTextBox2.Text;
                Properties.Settings.Default.Save();
            }
            else
            {
                // Clear saved credentials
                Properties.Settings.Default.RememberMe = false;
                Properties.Settings.Default.SavedEmail = "";
                Properties.Settings.Default.SavedPassword = "";
                Properties.Settings.Default.Save();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = roundedTextBox1.Text.Trim();
            string password = roundedTextBox2.Text.Trim();

            // Validation
            if (email == "Email Address" || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter your email address!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                roundedTextBox1.Focus();
                return;
            }

            if (password == "Password" || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter your password!", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                roundedTextBox2.Focus();
                return;
            }

            // Validate credentials
            if (ValidateUser(email, password))
            {
                // Save credentials if Remember Me is checked
                SaveCredentials();

                MessageBox.Show("Login Successful! Welcome to Study Buddy!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Open Dashboard form
                Dashboard dashboardForm = new Dashboard();
                dashboardForm.Show();

                // Hide login form
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid email or password. Please try again.", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                roundedTextBox2.Clear();
                roundedTextBox2.Focus();
            }
        }

        private bool ValidateUser(string email, string password)
        {
            // ✅ DEFAULT ADMIN CREDENTIALS CHECK
            if (email.ToLower() == "admin" && password == "12345")
            {
                return true;
            }

            // Database validation
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM users WHERE email = @email AND password = @password";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Connection Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnGoogle_Click(object sender, EventArgs e)
        {
            // Google Sign-In Implementation
            // Note: This requires Google Cloud Console setup and OAuth 2.0 configuration

            try
            {
                MessageBox.Show("Google Sign-In feature requires additional setup.\n\n" +
                    "To implement this:\n" +
                    "1. Create a project in Google Cloud Console\n" +
                    "2. Enable Google+ API\n" +
                    "3. Create OAuth 2.0 credentials\n" +
                    "4. Install Google.Apis.Auth NuGet package",
                    "Google Sign-In Setup",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // For now, you can use a simple browser-based approach
                System.Diagnostics.Process.Start("https://accounts.google.com");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening Google Sign-In: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Forgot Password
            string email = Microsoft.VisualBasic.Interaction.InputBox(
                "Please enter your email address to reset your password:",
                "Forgot Password",
                "");

            if (!string.IsNullOrEmpty(email) && email != "Email Address")
            {
                // Check if email exists in database
                if (EmailExists(email))
                {
                    MessageBox.Show($"A password reset link has been sent to {email}\n\n" +
                        "Note: Email functionality requires SMTP server configuration.",
                        "Password Reset",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // TODO: Implement actual email sending functionality
                    // SendPasswordResetEmail(email);
                }
                else
                {
                    MessageBox.Show("Email address not found. Please check or create an account.",
                        "Email Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            else if (!string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool EmailExists(string email)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM users WHERE email = @email";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@email", email);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        

        // Optional: Add password visibility toggle
        private void roundedTextBox2_MouseClick(object sender, MouseEventArgs e)
        {
            // You can add an eye icon button to toggle password visibility
        }

        // Optional: Allow Enter key to trigger login
        private void roundedTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        private void roundedTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                roundedTextBox2.Focus();
                e.SuppressKeyPress = true;
            }
        }

        // Form closing event
        private void StudyBuddy_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SignUP sign = new SignUP();
            sign.Show();
            this.Hide();
        }
    }
}