using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartStudyBuddy
{
    public partial class QuizResult : Form
    {



        private static QuizResult instance;

        private List<StudySets.Question> questions;
        private int reviewIndex = 0;
        private int totalScore = 0;

        // galing itong dashboard na cinocommand ko
        public QuizResult()
        {
            InitializeComponent();
            
        }

        public QuizResult(List<StudySets.Question> quizQuestions)
        {
            InitializeComponent();
            this.questions = quizQuestions;
            CalculateScore();
            UpdateNavigationColors();
            DisplayCurrentQuestion(); 
        }

        private void CalculateScore()
        {
            int score = 0;
            if (questions != null)
            {
                foreach (var q in questions)
                {
                    if (q.UserAnswer == q.CorrectAnswer)
                    {
                        score++;
                    }
                }
            }
            totalScore = score;
        }

        private void DisplayCurrentQuestion()
        {
            if (questions == null || questions.Count == 0) return;
            if (reviewIndex < 0) reviewIndex = 0;
            if (reviewIndex >= questions.Count) reviewIndex = questions.Count - 1;

            var q = questions[reviewIndex];

            Label lblScore = this.Controls.Find("lblScore", true).FirstOrDefault() as Label;
            if (lblScore != null)
                lblScore.Text = $"Score: {totalScore} / {questions.Count}";

            ProgressBar pb = this.Controls.Find("progressBar1", true).FirstOrDefault() as ProgressBar;
            if (pb != null)
            {
                int percentage = (questions.Count > 0) ? (totalScore * 100) / questions.Count : 0;
                pb.Value = percentage;
            }

            Panel panel = this.Controls.Find("panel3", true).FirstOrDefault() as Panel;
            if (panel == null) return;

            panel.Controls.Clear();

            Label lblQ = new Label
            {
                Text = $"Question {reviewIndex + 1} of {questions.Count}",
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 25, 112),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            panel.Controls.Add(lblQ);

            Label lblQuestionText = new Label
            {
                Text = q.QuestionText,
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(panel.Width - 30, 60),
                Location = new Point(10, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lblQuestionText);

            string[] letters = { "A", "B", "C", "D" };
            int panelWidth = (panel.Width - 50) / 2;
            int startY = 100;

            for (int i = 0; i < 4; i++)
            {
                int row = i / 2;
                int col = i % 2;

                Panel optPanel = new Panel
                {
                    Size = new Size(panelWidth, 60),
                    Location = new Point(10 + col * (panelWidth + 30), startY + row * 70),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label lblOpt = new Label
                {

                    Text = System.Text.RegularExpressions.Regex.IsMatch(q.Options[i], @"^[A-D]\.\s")
                                    ? q.Options[i]
                                    : $"{letters[i]}. {q.Options[i]}",
                    AutoSize = false,
                    Size = new Size(panelWidth - 50, 40),
                    Location = new Point(10, 15),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                bool isUserAnswer = (i == q.UserAnswer);
                bool isCorrect = (i == q.CorrectAnswer);

                if (isUserAnswer && isCorrect)
                {

                    optPanel.BackColor = Color.LightGreen;
                    Label lblMark = new Label
                    {
                        Text = "✓",
                        Font = new Font("Arial", 14, FontStyle.Bold),
                        ForeColor = Color.Green,
                        AutoSize = true,
                        Location = new Point(panelWidth - 30, 15)
                    };
                    optPanel.Controls.Add(lblMark);
                }
                else if (isCorrect)
                {

                    optPanel.BackColor = Color.LightGreen;
                    Label lblMark = new Label
                    {
                        Text = "✓ Correct",
                        Font = new Font("Arial", 8, FontStyle.Bold),
                        ForeColor = Color.Green,
                        AutoSize = true,
                        Location = new Point(panelWidth - 80, 20)
                    };
                    optPanel.Controls.Add(lblMark);
                }
                else if (isUserAnswer)
                {

                    optPanel.BackColor = Color.LightCoral;
                    Label lblMark = new Label
                    {
                        Text = "✗",
                        Font = new Font("Arial", 14, FontStyle.Bold),
                        ForeColor = Color.Red,
                        AutoSize = true,
                        Location = new Point(panelWidth - 30, 15)
                    };
                    optPanel.Controls.Add(lblMark);
                }

                optPanel.Controls.Add(lblOpt);
                panel.Controls.Add(optPanel);
            }
        }

        private void QuizResult_Load(object sender, EventArgs e)
        {
        }

        private void UpdateNavigationColors()
        {
            Button btnFlash = this.Controls.Find("btnFlashCards", true).FirstOrDefault() as Button;
            Button btnQuiz = this.Controls.Find("btnQuiz", true).FirstOrDefault() as Button;
            Button btnAi = this.Controls.Find("btnAiChat", true).FirstOrDefault() as Button;
            Button btnMatch = this.Controls.Find("btnMatch", true).FirstOrDefault() as Button;

            if (btnFlash != null) { btnFlash.BackColor = Color.LightGray; btnFlash.ForeColor = Color.Black; }
            if (btnQuiz != null) { btnQuiz.BackColor = Color.FromArgb(25, 25, 112); btnQuiz.ForeColor = Color.White; }
            if (btnAi != null) { btnAi.BackColor = Color.LightGray; btnAi.ForeColor = Color.Black; }
            if (btnMatch != null) { btnMatch.BackColor = Color.LightGray; btnMatch.ForeColor = Color.Black; }
        }

        // ✅ PREVIOUS BUTTON
        private void btnReviewPrev_Click(object sender, EventArgs e)
        {
            if (reviewIndex > 0)
            {
                reviewIndex--;
                DisplayCurrentQuestion();
            }
        }

        // ✅ NEXT BUTTON
        private void btnReviewNext_Click(object sender, EventArgs e)
        {
            if (reviewIndex < questions.Count - 1)
            {
                reviewIndex++;
                DisplayCurrentQuestion();
            }
        }

        public static QuizResult GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new QuizResult(); // Or with parameters kung kailangan
            }
            return instance;
        }

        private void buttonHome_Click(object sender, EventArgs e)
        {
            Dashboard dash = new Dashboard();
            dash.Show();
            this.Hide();
        }

     
        private void buttonStudySets_Click(object sender, EventArgs e)
        {
            StudySets sets = new StudySets();
            sets.Show();
            this.Hide();
        }
        private void buttonAiChat_Click(object sender, EventArgs e)
        {

        }

        private void buttonLeaderBoard_Click(object sender, EventArgs e)
        {

        }

        private void buttonProfile_Click(object sender, EventArgs e)
        {

        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {

        }

        private void btnFlashCard_Click(object sender, EventArgs e)
        {

            

            StudySets study = new StudySets();
            study.Show();
            this.Hide();
        }
        

    }
}