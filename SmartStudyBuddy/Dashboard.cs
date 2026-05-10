using Org.BouncyCastle.Asn1.Cmp;
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
using System.Speech.Recognition;
using System.Speech.Synthesis;


namespace SmartStudyBuddy
{

    
    public partial class Dashboard : Form
    {

        private static Dashboard instance;
        private SpeechRecognitionEngine speechEngine;
        private bool isListening = false;
        private Form currentForm; 

        public Dashboard()
        {
            InitializeComponent();
            MakeButtonCircular(btnMic);


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

            //Parents
            btnMyProfile.Parent = panel15;
            btnSettings.Parent = panel15;
            btnMyProgress.Parent = panel15;
            buttonLogout.Parent = panel15;

            
            /*
            //sa searching para may placeholder
            textBox1.Text = "Search...";
            textBox1.Enter += textBox_Enter;
            textBox1.Leave += textBox_Leave;
            */

            //Styling ito ng 4 buttons
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


            // textBox1 with panel 16
            textBox1.Font = new Font("Segoe UI", 11);
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.TextChanged += textBox1_TextChanged;

            // eto yung ma sesearch
            if (button1 != null) button1.Tag = "home main dashboard";
            if (button2 != null) button2.Tag = "study sets modules";
            if (button3 != null) button3.Tag = "ai chat assistant bot";
            if (button4 != null) button4.Tag = "leader board ranking";
            if (button5 != null) button5.Tag = "profile account user";

        }

        /*
        private void textBox_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Search...")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        
        private void textBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "Search...";
                textBox1.ForeColor = Color.Gray;
            }
        }
        */


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

        //ROUNDED PANELS CODE
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



        //panel15 
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
                ProgressForm progressForm = new ProgressForm();
                progressForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Logout (para iconfirm)
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

        //Logout(papuntang login)
        private void button6_Click(object sender, EventArgs e)
        {
            StudyBuddy studyForm1 = new StudyBuddy();
            studyForm1.Show();
            this.Hide();
        }

        //pupuntang study sets Form
        private void button2_Click(object sender, EventArgs e)
        {
            StudySets studdy = new StudySets();
            studdy.Show();
            this.Hide();
        }

        private void MakeButtonCircular(Button btn)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, btn.Width, btn.Height);
            btn.Region = new Region(path);

            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.FromArgb(231, 76, 60); 
            btn.FlatAppearance.BorderSize = 0;
            //btn.FlatAppearance.BorderColor = Color.White;//
            btn.ForeColor = Color.White;

            //MIC-ICON emoji
            btn.Text = "🎙️";
            btn.Font = new Font("Segoe UI Emoji", 20, FontStyle.Regular); 
            btn.TextAlign = ContentAlignment.MiddleCenter; 
        }

        private void btnMic_Click(object sender, EventArgs e)
        {
            if (!isListening)
            {
                // ACTIVATE VOICE RECOGNITION
                StartVoiceRecognition();

                // VISUAL FEEDBACK - GREEN
                isListening = true;
                btnMic.BackColor = Color.Green; // GREEN
                btnMic.FlatAppearance.BorderSize = 0;
                btnMic.ForeColor = Color.White;

                label3.Text = "Voice recognition is Active";
                label3.ForeColor = Color.Green;
                Speak("Voice recognition activated. Say commands like: Go to Dashboard");// Optional
            }
            else
            {
                // DEACTIVATE VOICE RECOGNITION
                StopVoiceRecognition();

                isListening = false;
                btnMic.BackColor = Color.Red; // RED
                btnMic.FlatAppearance.BorderSize = 0;
                btnMic.ForeColor = Color.White;

                label3.Text = "Voice recognition is Inactive";
                label3.ForeColor = Color.Gray;
                Speak("Voice recognition deactivated");// Optional
            }
        }

        private void StartVoiceRecognition()
        {
            try
            {
                speechEngine = new SpeechRecognitionEngine();
                speechEngine.SetInputToDefaultAudioDevice();

                Choices commands = new Choices();

                // Navigation Commands
                commands.Add(new[]
                {
                "Go to Dashboard", "Open Dashboard", "Dashboard",
                "Go to study sets", "Open study sets", "Study sets",
                "Go to Quizresult", "Open Quizresult", "Quizresult",
                "Go to SignUP", "Open SignUP", "SignUP",
                "Go to StudyBuddy", "Open StudyBuddy", "StudyBuddy", "Home",
                "Go back", "Back", "Return",
                "Close form", "Exit", "Quit"
        });

                Grammar grammar = new Grammar(new GrammarBuilder(commands));
                speechEngine.LoadGrammar(grammar);

                //Kapag narinig ang command
                speechEngine.SpeechRecognized += SpeechEngine_SpeechRecognized;

                //Start 
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);

                MessageBox.Show("🎤 Voice commands active!\n\nTry saying:\n• Go to Dashboard\n• Open Study Sets\n• Go to Quiz Result\n• Go back",
                               "Voice Recognition", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting voice recognition: " + ex.Message,
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopVoiceRecognition()
        {
            if (speechEngine != null)
            {
                speechEngine.RecognizeAsyncStop();
                speechEngine.SpeechRecognized -= SpeechEngine_SpeechRecognized;
                speechEngine.Dispose();
                speechEngine = null;
            }
        }

        private void SpeechEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string command = e.Result.Text.ToLower();

            // Invoke sa UI thread para makapag-navigate
            this.Invoke(new Action(() =>
            {
                NavigateByVoiceCommand(command);
            }));
        }

        private void NavigateByVoiceCommand(string command)
        {
            label3.Text = $"Heard: {command}";

            Form targetForm = null;

            if (command.Contains("dashboard"))
            {
                targetForm = Dashboard.GetInstance(); 
            }
            else if (command.Contains("study sets"))
            {
                targetForm = StudySets.GetInstance();
            }
            else if (command.Contains("quiz results"))
            {
                targetForm = QuizResult.GetInstance();
            }
            else if (command.Contains("sign up"))
            {
                targetForm = SignUP.GetInstance();
            }
            else if (command.Contains("study buddy") || command.Contains("home"))// home ang i-cacall
            {
                targetForm = StudyBuddy.GetInstance();
            }

            if (targetForm != null)
            {
                NavigateToForm(targetForm);
            }
        }

        private void NavigateToForm(Form newForm)
        {           
            this.Hide();// Close current form

            // Show target form
            newForm.Show();
            newForm.BringToFront(); // Para nasa harap
            newForm.Focus();
   
            newForm.Tag = this;// I-tag ang new form kung saan galing
        }

        private void Speak(string text)
        {
            // Optional: Text-to-Speech feedback
            try
            {
                using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
                {
                    synthesizer.Volume = 50;
                    synthesizer.Rate = -2; // Slower
                    synthesizer.SpeakAsync(text);
                }
            }
            catch { /* Ignore TTS errors */ }
        }

        public static Dashboard GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new Dashboard();
            }
            return instance;
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.ToLower().Trim();

            //buttons search
            Button[] buttonsToSearch = {
                button1,
                button2,
                button3,
                button4,
                button5
            };

            foreach (Button btn in buttonsToSearch)
            {
                if (btn == null) continue;

                string buttonText = btn.Text.ToLower();
                string buttonTag = btn.Tag?.ToString().ToLower() ?? "";

                if (string.IsNullOrEmpty(searchText))
                {
                    btn.Visible = true;
                    btn.BackColor = Color.MidnightBlue;
                }
                else if (buttonText.Contains(searchText) || buttonTag.Contains(searchText))
                {
                    btn.Visible = true;
                    btn.BackColor = Color.FromArgb(60, 60, 150); // Highlight
                }
                else
                {
                    btn.Visible = false;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AiChat aiChat = new AiChat();
            aiChat.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LeaderBoard lb = new LeaderBoard();
            lb.Show();
        }
    }
}