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

namespace SmartStudyBuddy
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            
            this.AutoScaleMode = AutoScaleMode.None;
            this.PerformLayout();

           
            ApplyRoundedCorners();
            SetRoundedPanel(panel3, 30);
            panel3.BackColor = Color.FromArgb(25, 25, 100);

            
            panel15.BackColor = Color.MidnightBlue; 
            panel15.BorderStyle = BorderStyle.None;
            panel15.Visible = false; 
            panel15.Size = new Size(175, 185); 
            panel15.BringToFront();

            // 3. Set Parents
            btnMyProfile.Parent = panel15;
            btnSettings.Parent = panel15;
            btnMyProgress.Parent = panel15;
            buttonLogout.Parent = panel15;

            // 4. Apply Exact Styling
            StyleExactButton(btnMyProfile, "MyProfile");
            StyleExactButton(btnSettings, "Settings");
            StyleExactButton(btnMyProgress, "MyProgress");
            StyleExactButton(buttonLogout, "Logout");

            
            btnMyProfile.Location = new Point(7, 6);
            btnSettings.Location = new Point(7, 49);  
            btnMyProgress.Location = new Point(7, 92);  
            buttonLogout.Location = new Point(7, 135); 

            
            btnMyProfile.BringToFront();
            btnSettings.BringToFront();
            btnMyProgress.BringToFront();
            buttonLogout.BringToFront();
        }


        private void StyleExactButton(Button btn, string text)
        {
            
            btn.AutoSize = false;
            btn.Size = new Size(160, 38); 

            btn.Text = text;
            btn.BackColor = Color.MidnightBlue;
            btn.ForeColor = Color.White;  
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;            
            btn.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.Cursor = Cursors.Hand;

            
            btn.MouseEnter += (s, e) => btn.BackColor = Color.MidnightBlue;
            btn.MouseLeave += (s, e) => btn.BackColor = Color.MidnightBlue;
        }

        // 🔹 ROUNDED PANELS CODE
        private void SetRoundedPanel(Panel panel, int radius)
        {
            try
            {
                if (panel.Width <= 0 || panel.Height <= 0) return;

                GraphicsPath path = new GraphicsPath();
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(panel.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(panel.Width - radius, panel.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, panel.Height - radius, radius, radius, 90, 90);
                path.CloseFigure();

                panel.Region = new Region(path);
                panel.BorderStyle = BorderStyle.None;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error applying rounded corners: " + ex.Message);
            }
        }

        private void ApplyRoundedCorners()
        {
            int borderRadius = 20;
            Panel[] panels = { panel5, panel7, panel8, panel9, panel13, panel12, panel11 };

            foreach (Panel p in panels)
            {
                if (p == null) continue;
                p.BackColor = Color.White;
                SetRoundedPanel(p, borderRadius);

                foreach (Control ctrl in p.Controls)
                {
                    if (ctrl is Label) ctrl.BackColor = Color.Transparent;
                }
            }

            if (label17 != null) label17.BackColor = Color.Transparent;
        }



        //Toggle panel15 
        private void btnLogout_Click(object sender, EventArgs e)
        {
            panel15.Visible = !panel15.Visible;

            
        }


        private void panel15_Paint(object sender, PaintEventArgs e)
        {
            
        }

        //MyProfile
        private void btnMyProfile_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Welcome to your Profile!\n\n" +
                               "📧 Email: user@example.com\n" +
                               "🎓 Role: Student\n" +
                               "🏫 School: Taguig City University",
                               "My Profile",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Settings
        private void btnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show("⚙️ Open Settings Panel?\n\n" +
                                           "• Change Password\n" +
                                           "• Notifications\n" +
                                           "• Theme & Language",
                                           "Settings",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Settings panel opened!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //MyProgress
        private void btnMyProgress_Click(object sender, EventArgs e)
        {
            try
            {
                string progressMsg = " Your Learning Progress:\n\n" +
                                   "✅ Modules Completed: 12/20\n" +
                                   "⭐ Total Points: 1,450\n" +
                                   "🏆 Badges Earned: 5\n" +
                                   "📈 Accuracy Rate: 87%\n\n" +
                                   "Keep up the great work! 🎉";
                MessageBox.Show(progressMsg, "My Progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Logout (Confirmation)
        private void buttonLogout_Click(object sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show("️ Are you sure you want to logout?\n\n" +
                                           "Any unsaved progress will be lost.",
                                           "Confirm Logout",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Warning,
                                           MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("You have been logged out successfully. 👋", "Logout", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit(); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            StudyBuddy studyForm1 = new StudyBuddy();
            studyForm1.Show();
            this.Hide();
        }
    }
}