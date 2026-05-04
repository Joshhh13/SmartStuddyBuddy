using Microsoft.VisualBasic;
using Mysqlx;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Management.Instrumentation;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SmartStudyBuddy
{
    public partial class StudySets : Form
    {

        private static StudySets instance;

        // Quiz Data Structure
        public class Question
        {
            public string QuestionText { get; set; }
            public string[] Options { get; set; }
            public int CorrectAnswer { get; set; }
            public int UserAnswer { get; set; } = -1;  // ← IDAGDAG MO ITO
        }

        private List<Question> questions = new List<Question>();
        private int currentQuestionIndex = 0;
        private SpeechRecognitionEngine speechEngine;
        private bool isVoiceOn = false;

        public StudySets()
        {
            InitializeComponent();
            InitializeQuestions();
            InitializeSpeechRecognition();
            DisplayQuestion();

            
        }

        private void InitializeQuestions()
        {
            // 24 C# Questions
            questions = new List<Question>
            {
                new Question {
                    QuestionText = "What is a for Loop in C#?",
                    Options = new string[] {
                        "A. A loop that executes based on a condition",
                        "B. A loop that iterates a specific number of times",
                        "C. A loop that only works with arrays",
                        "D. A loop that runs infinitely"
                    },
                    CorrectAnswer = 1
                },
                new Question {
                    QuestionText = "What keyword is used to declare a variable in C#?",
                    Options = new string[] {
                        "A. var",
                        "B. variable",
                        "C. dim",
                        "D. let"
                    },
                    CorrectAnswer = 0
                },
                new Question {
                    QuestionText = "Which symbol is used for single-line comments in C#?",
                    Options = new string[] {
                        "A. //",
                        "B. /*",
                        "C. #",
                        "D. --"
                    },
                    CorrectAnswer = 0
                },
                new Question {
                    QuestionText = "What is the correct way to create an array in C#?",
                    Options = new string[] {
                        "A. int[] arr = new int[5];",
                        "B. array int arr = new array[5];",
                        "C. int arr[] = new int[5];",
                        "D. int arr = new int[5];"
                    },
                    CorrectAnswer = 0
                },
                new Question {
                    QuestionText = "Which loop is guaranteed to execute at least once?",
                    Options = new string[] {
                        "A. for loop",
                        "B. while loop",
                        "C. do-while loop",
                        "D. foreach loop"
                    },
                    CorrectAnswer = 2
                },
                new Question {
                    QuestionText = "What is the default value of an int in C#?",
                    Options = new string[] {
                        "A. null",
                        "B. 1",
                        "C. 0",
                        "D. undefined"
                    },
                    CorrectAnswer = 2
                },
                new Question {
                    QuestionText = "Which keyword is used to inherit a class in C#?",
                    Options = new string[] {
                        "A. extends",
                        "B. inherits",
                        "C. :",
                        "D. implements"
                    },
                    CorrectAnswer = 2
                },
                new Question {
                    QuestionText = "What does CLR stand for in C#?",
                    Options = new string[] {
                        "A. Common Language Runtime",
                        "B. Code Language Runner",
                        "C. Class Library Reference",
                        "D. Common Library Runtime"
                    },
                    CorrectAnswer = 0
                },
                new Question {
                    QuestionText = "Which access modifier is most restrictive?",
                    Options = new string[] {
                        "A. public",
                        "B. protected",
                        "C. internal",
                        "D. private"
                    },
                    CorrectAnswer = 3
                },
                new Question {
                    QuestionText = "What is boxing in C#?",
                    Options = new string[] {
                        "A. Converting value type to reference type",
                        "B. Converting reference type to value type",
                        "C. Creating a new object",
                        "D. Deleting an object"
                    },
                    CorrectAnswer = 0
                },
                new Question {
                    QuestionText = "Which method is used to read a line from console?",
                    Options = new string[] {
                        "A. Console.Read()",
                        "B. Console.ReadLine()",
                        "C. Console.Input()",
                        "D. Console.GetLine()"
                    },
                    CorrectAnswer = 1
                },
                new Question {
                    QuestionText = "What is the size of int in C#?",
                    Options = new string[] {
                        "A. 2 bytes",
                        "B. 4 bytes",
                        "C. 8 bytes",
                        "D. 16 bytes"
                    },
                    CorrectAnswer = 1
                },
                new Question {
                    QuestionText = "Which keyword is used to handle exceptions?",
                    Options = new string[] {
                        "A, try-catch",
                        "B. handle-error",
                        "C. exception",
                        "D. error-handler"
                    },
                    CorrectAnswer = 0
                },
                new Question {
                    QuestionText = "What is the purpose of 'this' keyword?",
                    Options = new string[] {
                        "A. Refers to current instance",
                        "B. Refers to base class",
                        "C. Creates new instance",
                        "D. Deletes instance"
                    },
                    CorrectAnswer = 0
                },
                new Question {
                    QuestionText = "Which collection is key-value based?",
                    Options = new string[] {
                        "A. Array",
                        "B. List",
                        "C. Dictionary",
                        "D. Queue"
                    },
                    CorrectAnswer = 2
                },
                new Question {
                    QuestionText = "What does 'static' mean in C#?",
                    Options = new string[] {
                        "A. Belongs to instance",
                        "B. Belongs to class itself",
                        "C. Cannot be modified",
                        "D. Can be inherited"
                    },
                    CorrectAnswer = 1
                },
                new Question {
                    QuestionText = "Which operator is used for null-coalescing?",
                    Options = new string[] {
                        "A. ??",
                        "B. ??=",
                        "C. ?:",
                        "D. ??"
                    },
                    CorrectAnswer = 0
                },
                new Question {
                    QuestionText = "What is method overloading?",
                    Options = new string[] {
                        "A. Same method name, different parameters",
                        "B. Same method name, same parameters",
                        "C. Different method name, same parameters",
                        "D. Different method name, different parameters"
                    },
                    CorrectAnswer = 0
                },
                new Question {
                    QuestionText = "Which namespace contains Console class?",
                    Options = new string[] {
                        "A. System.IO",
                        "B. System",
                        "C. System.Console",
                        "D. System.Text"
                    },
                    CorrectAnswer = 1
                },
                new Question {
                    QuestionText = "What is the base class of all classes in C#?",
                    Options = new string[] {
                        "A. Base",
                        "B. Object",
                        "C. Class",
                        "D. Root"
                    },
                    CorrectAnswer = 1
                },
                new Question {
                    QuestionText = "Which keyword prevents inheritance?",
                    Options = new string[] {
                        "A. abstract",
                        "B. sealed",
                        "C. final",
                        "D. static"
                    },
                    CorrectAnswer = 1
                },
                new Question {
                    QuestionText = "What is garbage collection?",
                    Options = new string[] {
                        "A. Manual memory management",
                        "B. Automatic memory management",
                        "C. File deletion",
                        "D. Code optimization"
                    },
                    CorrectAnswer = 1
                },
                new Question {
                    QuestionText = "Which type is a value type?",
                    Options = new string[] {
                        "A. Class",
                        "B. Interface",
                        "C. Struct",
                        "D. Delegate"
                    },
                    CorrectAnswer = 2
                },
                new Question {
                    QuestionText = "What does async keyword do?",
                    Options = new string[] {
                        "A. Makes method synchronous",
                        "B. Makes method asynchronous",
                        "C. Stops method execution",
                        "D. Starts new thread"
                    },
                    CorrectAnswer = 1
                }
            };
        }

        private void InitializeSpeechRecognition()
        {
            try
            {
                speechEngine = new SpeechRecognitionEngine();
                speechEngine.SetInputToDefaultAudioDevice();

                // Create choices grammar
                Choices choices = new Choices();
                choices.Add(new string[] { "A", "B", "C", "D",
                                            "option A", "option B", "option C", "option D",
                                            "NEXT", "SUNOD",
                                            "PREVIOUS", "BACK", "BALIK"});
            

                Grammar grammar = new Grammar(new GrammarBuilder(choices));
                speechEngine.LoadGrammar(grammar);

                speechEngine.SpeechRecognized += SpeechEngine_SpeechRecognized;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Speech recognition not available: " + ex.Message);
            }
        }

        private void SpeechEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (!isVoiceOn) return;

            string text = e.Result.Text.ToUpper().Trim();

            // ✅ COMMAND: NEXT (may confirmation popup)
            if (text.Contains("NEXT") || text.Contains("SUNOD"))
            {
                this.Invoke((Action)(() =>
                {
                    DialogResult result = MessageBox.Show(
                        "Are you sure you want to go to the next question?",
                        "Confirm Next",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        NavigateNext();
                    }
                }));
                return;
            }

            // ✅ COMMAND: PREVIOUS (direct go back)
            if (text.Contains("PREVIOUS") || text.Contains("BACK") || text.Contains("BALIK"))
            {
                this.Invoke((Action)(() => NavigatePrevious()));
                return;
            }

            // ✅ COMMANDS: A, B, C, D (select + auto next)
            char answer = ' ';
            if (text.Contains("A")) answer = 'A';
            else if (text.Contains("B")) answer = 'B';
            else if (text.Contains("C")) answer = 'C';
            else if (text.Contains("D")) answer = 'D';

            if (answer != ' ')
            {
                // 1. I-highlight muna ang napili
                SelectOption(answer);

                // 2. Mag-delay ng 0.5 seconds para makita ng user, tapos auto-next
                System.Threading.Tasks.Task.Delay(500).ContinueWith(_ =>
                {
                    this.Invoke((Action)(() => NavigateNext()));
                });
            }
        }


        private void NavigateNext()
        {
            currentQuestionIndex++;
            if (currentQuestionIndex >= questions.Count)
            {
                MessageBox.Show("Quiz Completed! Great job!", "Completed",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                currentQuestionIndex = 0;
            }
            DisplayQuestion();
        }

        private void NavigatePrevious()
        {
            currentQuestionIndex--;
            if (currentQuestionIndex < 0) currentQuestionIndex = 0;
            DisplayQuestion();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dashboard dashForm = new Dashboard();
            dashForm.Show();    
            this.Hide();
        }


        private void btnVoice_Click(object sender, EventArgs e)
        {
            isVoiceOn = !isVoiceOn;
            Button btn = (Button)sender;

            // Hanapin ang lblListening sa loob ng panel13
            Label lblListening = this.Controls.Find("lblListening", true)[0] as Label;

            if (isVoiceOn)
            {
                btn.Text = "Voice Off";
                btn.BackColor = Color.Green;
                if (lblListening != null) lblListening.ForeColor = Color.Green;

                if (speechEngine != null) speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                btn.Text = "Voice On";
                btn.BackColor = Color.DarkBlue; // Original color
                if (lblListening != null) lblListening.ForeColor = Color.Gray;

                if (speechEngine != null) speechEngine.RecognizeAsyncStop();
            }
        }

        private void SelectOption(char option)
        {
            // ✅ GINAMIT ang "panel3" (hindi "panelQuestion")
            Panel panel = this.Controls.Find("panel3", true).FirstOrDefault() as Panel;
            if (panel == null) return;

            switch (option)
            {
                case 'A':
                    var optA = panel.Controls.Find("optA", true).FirstOrDefault() as RadioButton;
                    if (optA != null) optA.Checked = true;
                    var btnA = panel.Controls.Find("btnA", true).FirstOrDefault() as Button;
                    if (btnA != null) btnA.BackColor = Color.LightBlue;
                    break;
                case 'B':
                    var optB = panel.Controls.Find("optB", true).FirstOrDefault() as RadioButton;
                    if (optB != null) optB.Checked = true;
                    var btnB = panel.Controls.Find("btnB", true).FirstOrDefault() as Button;
                    if (btnB != null) btnB.BackColor = Color.LightBlue;
                    break;
                case 'C':
                    var optC = panel.Controls.Find("optC", true).FirstOrDefault() as RadioButton;
                    if (optC != null) optC.Checked = true;
                    var btnC = panel.Controls.Find("btnC", true).FirstOrDefault() as Button;
                    if (btnC != null) btnC.BackColor = Color.LightBlue;
                    break;
                case 'D':
                    var optD = panel.Controls.Find("optD", true).FirstOrDefault() as RadioButton;
                    if (optD != null) optD.Checked = true;
                    var btnD = panel.Controls.Find("btnD", true).FirstOrDefault() as Button;
                    if (btnD != null) btnD.BackColor = Color.LightBlue;
                    break;
            }
        }

        private void DisplayQuestion()
        {
            if (currentQuestionIndex < 0) currentQuestionIndex = 0;
            if (currentQuestionIndex >= questions.Count) currentQuestionIndex = questions.Count - 1;

            Question currentQ = questions[currentQuestionIndex];

            Panel panel3 = this.Controls.Find("panel3", true).FirstOrDefault() as Panel;

            if (panel3 != null)
            {
                Label lblQuestion = panel3.Controls.Find("lblQuestion", true).FirstOrDefault() as Label;
                if (lblQuestion != null)
                    lblQuestion.Text = currentQ.QuestionText;

                Button btnA = panel3.Controls.Find("btnChoiceA", true).FirstOrDefault() as Button;
                Button btnB = panel3.Controls.Find("btnChoiceB", true).FirstOrDefault() as Button;
                Button btnC = panel3.Controls.Find("btnChoiceC", true).FirstOrDefault() as Button;
                Button btnD = panel3.Controls.Find("btnChoiceD", true).FirstOrDefault() as Button;

                if (btnA != null) { btnA.Text = currentQ.Options[0]; btnA.BackColor = Color.White; }
                if (btnB != null) { btnB.Text = currentQ.Options[1]; btnB.BackColor = Color.White; }
                if (btnC != null) { btnC.Text = currentQ.Options[2]; btnC.BackColor = Color.White; }
                if (btnD != null) { btnD.Text = currentQ.Options[3]; btnD.BackColor = Color.White; }

                Label lblProgress = panel3.Controls.Find("lblProgress", true).FirstOrDefault() as Label;
                if (lblProgress != null)
                {
                    lblProgress.Text = $"QUESTION - CARD {currentQuestionIndex + 1} / 24";
                }
            }

            Label lblCounter = this.Controls.Find("lblCounter", true).FirstOrDefault() as Label;
            if (lblCounter != null)
            {
                lblCounter.Text = $"{currentQuestionIndex + 1} / 24";
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            currentQuestionIndex--;
            DisplayQuestion();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            currentQuestionIndex++;
            if (currentQuestionIndex >= questions.Count)
            {
                MessageBox.Show("Quiz Completed!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                currentQuestionIndex = 0;
            }
            DisplayQuestion();
        }

        private void btnQuiz_Click(object sender, EventArgs e)
        {
            QuizResult quiz = new QuizResult(this.questions);
            quiz.Show();
            this.Hide();
        }

        




        private void btnFlashCard_Click(object sender, EventArgs e)
        {
            // 1. Ang Flash Cards Button (na pinindot mo) ay gagawing BLUE
            Button btnFlash = sender as Button;
            if (btnFlash != null)
            {
                btnFlash.BackColor = Color.DarkBlue; // Blue
                btnFlash.ForeColor = Color.White;    // White Text
            }

            // 2. Ang Quiz Button ay gagawing GRAY
            Button btnQuiz = this.Controls.Find("btnQuiz", true).FirstOrDefault() as Button;
            if (btnQuiz != null)
            {
                btnQuiz.BackColor = Color.LightGray; // Gray
                btnQuiz.ForeColor = Color.Black;     // Black Text
            }

            // 3. Ang Ai Chat Button ay gagawing GRAY
            Button btnAi = this.Controls.Find("btnAiChat", true).FirstOrDefault() as Button;
            if (btnAi != null)
            {
                btnAi.BackColor = Color.LightGray;
                btnAi.ForeColor = Color.Black;
            }

            // 4. Ang Match Button ay gagawing GRAY
            Button btnMatch = this.Controls.Find("btnMatch", true).FirstOrDefault() as Button;
            if (btnMatch != null)
            {
                btnMatch.BackColor = Color.LightGray;
                btnMatch.ForeColor = Color.Black;
            }

            // (Optional) Kung may form ka pang FlashCards na bubuksan, ilagay mo dito:
            // FlashCardsForm form = new FlashCardsForm();
            // form.Show();
        }

        private void btnChoiceA_Click(object sender, EventArgs e)
        {
            questions[currentQuestionIndex].UserAnswer = 0; // 0 = A
            HighlightSelectedButton((Button)sender);
        }

        private void btnChoiceB_Click(object sender, EventArgs e)
        {
            questions[currentQuestionIndex].UserAnswer = 1; // 1 = B
            HighlightSelectedButton((Button)sender);
        }

        private void btnChoiceC_Click(object sender, EventArgs e)
        {
            questions[currentQuestionIndex].UserAnswer = 2; // 2 = C
            HighlightSelectedButton((Button)sender);
        }

        private void btnChoiceD_Click(object sender, EventArgs e)
        {
            questions[currentQuestionIndex].UserAnswer = 3; // 3 = D
            HighlightSelectedButton((Button)sender);
        }

        private void HighlightSelectedButton(Button clickedBtn)
        {
            Panel panel3 = this.Controls.Find("panel3", true).FirstOrDefault() as Panel;
            if (panel3 == null) return;

            // Reset all buttons to white
            foreach (Control ctrl in panel3.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.BackColor = Color.White;
                }
            }

            // Highlight clicked button
            clickedBtn.BackColor = Color.LightBlue;
        }


        private void StudySets_Load(object sender, EventArgs e)
        {
            this.AutoScaleMode = AutoScaleMode.None;
            this.PerformLayout();


            panel6.BackColor = Color.MidnightBlue;
            panel6.BorderStyle = BorderStyle.None;
            panel6.Visible = false;
            panel6.Size = new Size(175, 185);
            panel6.BringToFront();

            // 3. Set Parents
            btnMyProfile.Parent = panel6;
            btnSettings.Parent = panel6;
            btnMyProgress.Parent = panel6;
            buttonLogout.Parent = panel6;

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

            // textBox1 with panel 16
            textBox1.Font = new Font("Segoe UI", 11);
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.TextChanged += textBox1_TextChanged; // Add event handler

            // eto yung ma sesearch
            if (button2 != null) button2.Tag = "home main dashboard";
            if (button3 != null) button3.Tag = "study sets modules";
            if (button4 != null) button4.Tag = "ai chat assistant bot";
            if (button5 != null) button5.Tag = "leader board ranking";
            if (button6 != null) button6.Tag = "profile account user";

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

        private void btnLogout_Click(object sender, EventArgs e)
        {
            panel6.Visible = !panel6.Visible;
        }

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

        private void button12_Click(object sender, EventArgs e)
        {
            StudyBuddy studyForm1 = new StudyBuddy();
            studyForm1.Show();
            this.Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.ToLower().Trim();

            // List of buttons to search
            Button[] buttonsToSearch = {
                button2,
                button3,
                button4,
                button5,
                button6
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

        public static StudySets GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new StudySets();
            }
            return instance;
        }
    }
}
    