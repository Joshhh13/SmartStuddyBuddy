using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SmartStudyBuddy
{
    // ══════════════════════════════════════════════════════════════════
    //  DATA MODEL
    // ══════════════════════════════════════════════════════════════════
    public class StudentEntry
    {
        public string Name { get; set; }
        public double QuizAvg { get; set; }
        public int CardsStudied { get; set; }
        public int DayStreak { get; set; }
        public double TotalScore { get; set; }

        public StudentEntry(string name, double quizAvg,
            int cardsStudied, int dayStreak)
        {
            Name = name;
            QuizAvg = quizAvg;
            CardsStudied = cardsStudied;
            DayStreak = dayStreak;
            TotalScore = (quizAvg * 0.5) +
                           (cardsStudied * 0.3) +
                           (dayStreak * 0.2);
        }
    }

    // ══════════════════════════════════════════════════════════════════
    //  LEADERBOARD FORM
    // ══════════════════════════════════════════════════════════════════
    public partial class LeaderBoard : Form
    {
        private Panel pnlSidebar, pnlMain, pnlTopBar;
        private Panel pnlPodium, pnlTable;
        private ComboBox cboFilter;
        private string currentFilter = "Overall";

        public LeaderBoard()
        {
            InitializeComponent();
            this.Text = "SmartStudyBuddy - Leaderboard";
            this.Size = new Size(1280, 800);
            this.BackColor = Color.FromArgb(224, 247, 250);
            this.Font = new Font("Segoe UI", 9f);
            BuildUI();
            LoadLeaderboard();
        }

        // ── Mock Data ─────────────────────────────────────────────────
        private List<StudentEntry> GetLeaderboardData(string filter)
        {
            List<StudentEntry> data = new List<StudentEntry>
            {
                new StudentEntry("Alice Santos",   97, 210, 14),
                new StudentEntry("Ben Reyes",       93, 185, 12),
                new StudentEntry("Carla Mendoza",   90, 200, 11),
                new StudentEntry("Dan Cruz",        88, 175, 10),
                new StudentEntry("Eva Lim",         85, 160,  9),
                new StudentEntry("Felix Tan",       83, 150,  8),
                new StudentEntry("Grace Uy",        80, 140,  7),
                new StudentEntry("Hiro Nakamura",   78, 130,  6),
                new StudentEntry("Iris Flores",     75, 120,  5),
                new StudentEntry("Jake Santos",     72, 110,  4),
            };

            if (filter == "Quiz Score")
                data.Sort((a, b) => b.QuizAvg.CompareTo(a.QuizAvg));
            else if (filter == "Cards Studied")
                data.Sort((a, b) => b.CardsStudied.CompareTo(a.CardsStudied));
            else if (filter == "Day Streak")
                data.Sort((a, b) => b.DayStreak.CompareTo(a.DayStreak));
            else
                data.Sort((a, b) => b.TotalScore.CompareTo(a.TotalScore));

            return data;
        }

        // ── UI Builder ────────────────────────────────────────────────
        private void BuildUI()
        {
            // Sidebar
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = Color.FromArgb(0, 86, 179)
            };
            Label lblNav = new Label
            {
                Text = "MENU",
                ForeColor = Color.FromArgb(179, 214, 255),
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                Location = new Point(16, 16),
                AutoSize = true
            };
            pnlSidebar.Controls.Add(lblNav);
            AddSidebarBtn(pnlSidebar, "Home", 50, () => this.Hide());
            AddSidebarBtn(pnlSidebar, "Study Sets", 100, () => this.Hide());
            AddSidebarBtn(pnlSidebar, "Ai Chat", 150, null);
            AddSidebarBtn(pnlSidebar, "Leader Board", 200, null);
            AddSidebarBtn(pnlSidebar, "Profile", 250, null);

            // Top bar
            pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(0, 86, 179)
            };
            pnlTopBar.Controls.Add(new Label
            {
                Text = "Smart Study Buddy",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 14),
                AutoSize = true
            });

            // Main panel
            pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24)
            };

            Label lblTitle = new Label
            {
                Text = "Leaderboard",
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 86, 179),
                Location = new Point(24, 16),
                AutoSize = true
            };
            Label lblSub = new Label
            {
                Text = "See how you rank among your peers!",
                ForeColor = Color.Gray,
                Location = new Point(26, 50),
                AutoSize = true
            };

            Label lblFilter = new Label
            {
                Text = "Rank by:",
                ForeColor = Color.FromArgb(0, 86, 179),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Location = new Point(24, 80),
                AutoSize = true
            };
            cboFilter = new ComboBox
            {
                Location = new Point(100, 77),
                Size = new Size(160, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.White
            };
            cboFilter.Items.AddRange(new object[]
                { "Overall", "Quiz Score", "Cards Studied", "Day Streak" });
            cboFilter.SelectedIndex = 0;
            cboFilter.SelectedIndexChanged += (s, e) =>
            {
                currentFilter = cboFilter.SelectedItem.ToString();
                LoadLeaderboard();
            };

            // Podium panel
            pnlPodium = new Panel
            {
                Location = new Point(24, 112),
                Size = new Size(1020, 180),
                BackColor = Color.Transparent
            };
            pnlPodium.Paint += PnlPodium_Paint;

            // Table panel
            pnlTable = new Panel
            {
                Location = new Point(24, 304),
                Size = new Size(1020, 420),
                BackColor = Color.White,
                AutoScroll = true
            };
            pnlTable.Paint += (s, e) =>
            {
                Pen pen = new Pen(Color.FromArgb(200, 230, 255), 1);
                e.Graphics.DrawRectangle(pen, 0, 0,
                    pnlTable.Width - 1, pnlTable.Height - 1);
                pen.Dispose();
            };

            pnlMain.Controls.Add(lblTitle);
            pnlMain.Controls.Add(lblSub);
            pnlMain.Controls.Add(lblFilter);
            pnlMain.Controls.Add(cboFilter);
            pnlMain.Controls.Add(pnlPodium);
            pnlMain.Controls.Add(pnlTable);

            this.Controls.Add(pnlMain);
            this.Controls.Add(pnlSidebar);
            this.Controls.Add(pnlTopBar);
        }

        // ── Load Leaderboard ──────────────────────────────────────────
        private void LoadLeaderboard()
        {
            List<StudentEntry> data = GetLeaderboardData(currentFilter);
            pnlPodium.Tag = data;
            pnlPodium.Invalidate();

            pnlTable.Controls.Clear();
            DrawTableHeader();
            for (int i = 3; i < data.Count; i++)
                DrawTableRow(data[i], i + 1, i - 3);
        }

        // ── Podium ────────────────────────────────────────────────────
        private void PnlPodium_Paint(object sender,
            System.Windows.Forms.PaintEventArgs e)
        {
            List<StudentEntry> data = pnlPodium.Tag as List<StudentEntry>;
            if (data == null || data.Count < 3) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int[] xPos = { 60, 360, 660 };
            int[] ranks = { 2, 1, 3 };
            int[] heights = { 120, 150, 100 };

            Color[] podiumColors =
            {
                Color.FromArgb(176, 190, 197),
                Color.FromArgb(255, 193,   7),
                Color.FromArgb(188, 143,  89)
            };
            string[] medals = { "2nd", "1st", "3rd" };

            for (int i = 0; i < 3; i++)
            {
                int rank = ranks[i];
                StudentEntry student = data[rank - 1];
                int x = xPos[i];
                int baseY = pnlPodium.Height - heights[i] - 10;

                SolidBrush br = new SolidBrush(podiumColors[i]);
                FillRoundedRect(g, br,
                    new RectangleF(x, baseY, 260, heights[i]), 8);
                br.Dispose();

                // Rank on block
                Font rankFont = new Font("Segoe UI", 28f, FontStyle.Bold);
                g.DrawString(rank.ToString(), rankFont, Brushes.White,
                    x + 110, baseY + heights[i] / 2 - 22);
                rankFont.Dispose();

                // Medal label
                Font medalFont = new Font("Segoe UI", 11f, FontStyle.Bold);
                g.DrawString(medals[i], medalFont, Brushes.DimGray,
                    x + 10, baseY - 38);
                medalFont.Dispose();

                // Name
                Font nameFont = new Font("Segoe UI", 11f, FontStyle.Bold);
                SolidBrush nameBr = new SolidBrush(Color.FromArgb(20, 20, 60));
                g.DrawString(student.Name, nameFont, nameBr, x + 10, baseY - 60);
                nameFont.Dispose();
                nameBr.Dispose();

                // Score
                Font scoreFont = new Font("Segoe UI", 9f);
                g.DrawString(GetScoreText(student), scoreFont,
                    Brushes.DimGray, x + 10, baseY - 38);
                scoreFont.Dispose();
            }
        }

        // ── Table ─────────────────────────────────────────────────────
        private void DrawTableHeader()
        {
            Panel header = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(pnlTable.Width - 2, 40),
                BackColor = Color.FromArgb(0, 86, 179)
            };

            string[] cols = { "Rank", "Name", "Quiz Avg",
                                 "Cards Studied", "Day Streak", "Total Score" };
            int[] widths = { 60, 280, 150, 150, 150, 150 };
            int x = 10;

            for (int i = 0; i < cols.Length; i++)
            {
                header.Controls.Add(new Label
                {
                    Text = cols[i],
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    Location = new Point(x, 10),
                    Size = new Size(widths[i], 22),
                    TextAlign = ContentAlignment.MiddleLeft
                });
                x += widths[i];
            }
            pnlTable.Controls.Add(header);
        }

        private void DrawTableRow(StudentEntry s, int rank, int rowIndex)
        {
            Color rowBg = rowIndex % 2 == 0
                ? Color.White
                : Color.FromArgb(240, 248, 255);

            Panel row = new Panel
            {
                Location = new Point(0, 40 + rowIndex * 48),
                Size = new Size(pnlTable.Width - 2, 48),
                BackColor = rowBg
            };

            row.MouseEnter += (s2, e) =>
                row.BackColor = Color.FromArgb(207, 226, 255);
            row.MouseLeave += (s2, e) =>
                row.BackColor = rowBg;

            row.Paint += (s2, e) =>
            {
                Pen pen = new Pen(Color.FromArgb(220, 235, 255), 1);
                e.Graphics.DrawLine(pen, 0, row.Height - 1,
                    row.Width, row.Height - 1);
                pen.Dispose();
            };

            int[] widths = { 60, 280, 150, 150, 150, 150 };
            string[] values =
            {
                "#" + rank,
                s.Name,
                s.QuizAvg + "%",
                s.CardsStudied.ToString(),
                s.DayStreak + " days",
                s.TotalScore.ToString("0.#")
            };

            int x = 10;
            for (int i = 0; i < values.Length; i++)
            {
                Color fg = i == 0
                    ? Color.FromArgb(0, 86, 179)
                    : Color.FromArgb(30, 30, 60);

                Label lbl = new Label
                {
                    Text = values[i],
                    ForeColor = fg,
                    Font = new Font("Segoe UI",
                        i == 0 ? 11f : 10f,
                        i == 0 ? FontStyle.Bold : FontStyle.Regular),
                    Location = new Point(x, 12),
                    Size = new Size(widths[i], 24),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                lbl.MouseEnter += (s2, e) =>
                    row.BackColor = Color.FromArgb(207, 226, 255);
                lbl.MouseLeave += (s2, e) =>
                    row.BackColor = rowBg;
                row.Controls.Add(lbl);
                x += widths[i];
            }
            pnlTable.Controls.Add(row);
        }

        private string GetScoreText(StudentEntry s)
        {
            if (currentFilter == "Quiz Score")
                return "Quiz: " + s.QuizAvg + "%";
            if (currentFilter == "Cards Studied")
                return "Cards: " + s.CardsStudied;
            if (currentFilter == "Day Streak")
                return "Streak: " + s.DayStreak + " days";
            return "Score: " + s.TotalScore.ToString("0.#");
        }

        // ── Helpers ───────────────────────────────────────────────────
        private void AddSidebarBtn(Panel sidebar, string text,
            int y, Action onClick)
        {
            Label btn = new Label
            {
                Text = text,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f),
                Location = new Point(0, y),
                Size = new Size(200, 40),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.MouseEnter += (s, e) =>
                btn.BackColor = Color.FromArgb(0, 60, 140);
            btn.MouseLeave += (s, e) =>
                btn.BackColor = Color.Transparent;
            if (onClick != null)
                btn.Click += (s, e) => onClick();
            sidebar.Controls.Add(btn);
        }

        private static void FillRoundedRect(Graphics g, Brush br,
            RectangleF r, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(r.X, r.Y,
                radius * 2, radius * 2, 180, 90);
            path.AddArc(r.Right - radius * 2, r.Y,
                radius * 2, radius * 2, 270, 90);
            path.AddArc(r.Right - radius * 2, r.Bottom - radius * 2,
                radius * 2, radius * 2, 0, 90);
            path.AddArc(r.X, r.Bottom - radius * 2,
                radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            g.FillPath(br, path);
            path.Dispose();
        }
    }
}