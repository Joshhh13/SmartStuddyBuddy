using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SmartStudyBuddy
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
            this.Text = "StudyBuddy - Progress";
            this.Size = new Size(1280, 800);
            this.BackColor = Color.WhiteSmoke;
            this.Font = new Font("Segoe UI", 9f);
            BuildUI();
            LoadMockData();
        }

        private Panel pnlSidebar, pnlMain;
        private Panel pnlStats, pnlChart, pnlCalendar, pnlSubject, pnlCards;

        // ══════════════════════════════════════════════════════════════
        //  MOCK DATA
        // ══════════════════════════════════════════════════════════════
        private readonly List<Tuple<string, double>> mockQuizHistory =
            new List<Tuple<string, double>>
            {
                Tuple.Create("C# Loops", 85.0), Tuple.Create("Arrays",   90.0),
                Tuple.Create("OOP",      78.0), Tuple.Create("SDF 105",  88.0),
                Tuple.Create("C# Loops", 92.0), Tuple.Create("Arrays",   95.0),
                Tuple.Create("OOP",      80.0), Tuple.Create("SDF 105",  87.0),
                Tuple.Create("C# Loops", 93.0), Tuple.Create("Arrays",   97.0)
            };

        private readonly List<Tuple<string, double, int>> mockSubjects =
            new List<Tuple<string, double, int>>
            {
                Tuple.Create("Arrays",   94.0, 3),
                Tuple.Create("C# Loops", 90.0, 3),
                Tuple.Create("SDF 105",  87.5, 2),
                Tuple.Create("OOP",      79.0, 2)
            };

        private readonly List<Tuple<string, int>> mockCardsPerDay =
            new List<Tuple<string, int>>
            {
                Tuple.Create("Mon", 24), Tuple.Create("Tue", 18),
                Tuple.Create("Wed", 30), Tuple.Create("Thu", 15),
                Tuple.Create("Fri", 22), Tuple.Create("Sat", 28),
                Tuple.Create("Sun", 20)
            };

        private readonly Dictionary<DateTime, int> mockStreakDays =
            new Dictionary<DateTime, int>();

        // ══════════════════════════════════════════════════════════════
        //  UI BUILDER
        // ══════════════════════════════════════════════════════════════
        private void BuildUI()
        {
            // Sidebar
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = Color.FromArgb(26, 35, 126)
            };

            Label lblNav = new Label
            {
                Text = "NAVIGATION",
                ForeColor = Color.FromArgb(159, 168, 218),
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                Location = new Point(16, 16),
                AutoSize = true
            };
            pnlSidebar.Controls.Add(lblNav);
            AddSidebarItem(pnlSidebar, "Home", 50);
            AddSidebarItem(pnlSidebar, "Study Sets", 100);
            AddSidebarItem(pnlSidebar, "AI Chat", 150);
            AddSidebarItem(pnlSidebar, "Leader Board", 200);
            AddSidebarItem(pnlSidebar, "Profile", 250);

            // Main panel
            pnlMain = new Panel { Dock = DockStyle.Fill, Padding = new Padding(28) };

            Label lblTitle = new Label
            {
                Text = "Your Progress",
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 35, 126),
                AutoSize = true,
                Location = new Point(28, 24)
            };

            Label lblSubtitle = new Label
            {
                Text = "Track your study performance over time.",
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(30, 58)
            };

            pnlStats = new Panel { Location = new Point(28, 88), Size = new Size(1020, 90) };
            pnlChart = MakeCard(28, 192, 600, 220, "Quiz Score History");
            pnlCalendar = MakeCard(644, 192, 390, 220, "Study Streak (Last 30 Days)");
            pnlSubject = MakeCard(28, 430, 490, 220, "Per-Subject Breakdown");
            pnlCards = MakeCard(534, 430, 500, 220, "Cards Studied Per Day");

            pnlMain.Controls.Add(lblTitle);
            pnlMain.Controls.Add(lblSubtitle);
            pnlMain.Controls.Add(pnlStats);
            pnlMain.Controls.Add(pnlChart);
            pnlMain.Controls.Add(pnlCalendar);
            pnlMain.Controls.Add(pnlSubject);
            pnlMain.Controls.Add(pnlCards);

            this.Controls.Add(pnlMain);
            this.Controls.Add(pnlSidebar);
        }

        private Panel MakeCard(int x, int y, int w, int h, string title)
        {
            Panel p = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = Color.White,
                Padding = new Padding(12)
            };
            p.Paint += (s, e) =>
            {
                Pen pen = new Pen(Color.FromArgb(220, 220, 220));
                e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
                pen.Dispose();
            };
            p.Controls.Add(new Label
            {
                Text = title,
                AutoSize = true,
                Location = new Point(12, 10),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 35, 126)
            });
            return p;
        }

        private void AddSidebarItem(Panel sidebar, string text, int y)
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
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(57, 73, 171);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.Transparent;
            sidebar.Controls.Add(btn);
        }

        // ══════════════════════════════════════════════════════════════
        //  LOAD MOCK DATA
        // ══════════════════════════════════════════════════════════════
        private void LoadMockData()
        {
            Random rng = new Random();
            for (int i = 0; i < 30; i++)
            {
                DateTime date = DateTime.Today.AddDays(-29 + i);
                if (rng.Next(0, 5) != 0)
                    mockStreakDays[date] = rng.Next(5, 35);
            }
            mockStreakDays[DateTime.Today] = 20;

            LoadSummaryStats();
            LoadQuizHistory();
            LoadStreakCalendar();
            LoadSubjectBreakdown();
            LoadCardsPerDay();
        }

        // ── Summary stat cards ────────────────────────────────────────
        private void LoadSummaryStats()
        {
            int totalCards = 0;
            foreach (Tuple<string, int> d in mockCardsPerDay)
                totalCards += d.Item2;

            double avgScore = 0;
            foreach (Tuple<string, double> q in mockQuizHistory)
                avgScore += q.Item2;
            avgScore /= mockQuizHistory.Count;

            int streak = 0;
            DateTime expected = DateTime.Today;
            while (mockStreakDays.ContainsKey(expected))
            {
                streak++;
                expected = expected.AddDays(-1);
            }

            string[] icons = { "Cards", "Quizzes", "Avg Score", "Streak" };
            string[] vals = { totalCards.ToString(), mockQuizHistory.Count.ToString(), avgScore.ToString("0.#") + "%", streak.ToString() };
            string[] labels = { "Cards Studied", "Quizzes Taken", "Avg Quiz Score", "Day Streak" };
            Color[] bgs = { Color.FromArgb(232, 245, 233), Color.FromArgb(227, 242, 253), Color.FromArgb(255, 243, 224), Color.FromArgb(252, 228, 236) };
            Color[] fgs = { Color.FromArgb(46, 125, 50), Color.FromArgb(21, 101, 192), Color.FromArgb(230, 81, 0), Color.FromArgb(173, 20, 87) };

            int xOff = 0;
            for (int i = 0; i < icons.Length; i++)
            {
                Panel card = new Panel
                {
                    Location = new Point(xOff, 0),
                    Size = new Size(230, 80),
                    BackColor = bgs[i]
                };
                card.Controls.Add(new Label
                {
                    Text = vals[i],
                    Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                    ForeColor = fgs[i],
                    AutoSize = true,
                    Location = new Point(12, 10)
                });
                card.Controls.Add(new Label
                {
                    Text = labels[i],
                    ForeColor = Color.Gray,
                    AutoSize = true,
                    Location = new Point(14, 50)
                });
                pnlStats.Controls.Add(card);
                xOff += 245;
            }
        }

        // ── Quiz score line chart ─────────────────────────────────────
        private void LoadQuizHistory()
        {
            Panel chartArea = new Panel
            {
                Location = new Point(12, 36),
                Size = new Size(pnlChart.Width - 24, pnlChart.Height - 48),
                BackColor = Color.White
            };
            pnlChart.Controls.Add(chartArea);
            chartArea.Paint += (s, e) =>
                DrawLineChart(e.Graphics, chartArea.Size, mockQuizHistory);
        }

        private void DrawLineChart(Graphics g, Size sz,
            List<Tuple<string, double>> data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int pad = 40, w = sz.Width - pad * 2, h = sz.Height - pad * 2;
            float xStep = data.Count > 1 ? w / (float)(data.Count - 1) : w;
            Font font = new Font("Segoe UI", 7f);

            Pen axisPen = new Pen(Color.LightGray, 1);
            g.DrawLine(axisPen, pad, pad, pad, pad + h);
            g.DrawLine(axisPen, pad, pad + h, pad + w, pad + h);

            for (int pct = 0; pct <= 100; pct += 25)
            {
                float y = pad + h - (h * pct / 100f);
                g.DrawLine(axisPen, pad, y, pad + w, y);
                g.DrawString(pct + "%", font, Brushes.Gray, 2, y - 7);
            }
            axisPen.Dispose();

            PointF[] pts = new PointF[data.Count];
            for (int i = 0; i < data.Count; i++)
                pts[i] = new PointF(pad + i * xStep,
                    pad + h - (h * (float)data[i].Item2 / 100f));

            Pen linePen = new Pen(Color.FromArgb(26, 35, 126), 2.5f);
            if (pts.Length > 1) g.DrawCurve(linePen, pts, 0.3f);

            foreach (PointF pt in pts)
            {
                g.FillEllipse(Brushes.White, pt.X - 5, pt.Y - 5, 10, 10);
                g.DrawEllipse(linePen, pt.X - 5, pt.Y - 5, 10, 10);
            }
            linePen.Dispose();

            for (int i = 0; i < data.Count; i++)
                g.DrawString(data[i].Item1, font, Brushes.Gray,
                    pad + i * xStep - 14, pad + h + 4);
        }

        // ── Streak heatmap ────────────────────────────────────────────
        private void LoadStreakCalendar()
        {
            Panel calArea = new Panel
            {
                Location = new Point(12, 36),
                Size = new Size(pnlCalendar.Width - 24, pnlCalendar.Height - 52),
                BackColor = Color.White
            };
            pnlCalendar.Controls.Add(calArea);
            calArea.Paint += (s, e) =>
                DrawHeatmap(e.Graphics, calArea.Size, mockStreakDays);
        }

        private void DrawHeatmap(Graphics g, Size sz,
            Dictionary<DateTime, int> days)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int cols = 10, cellW = 28, cellH = 28, gap = 4;
            int startX = 10, startY = 14;
            Font font = new Font("Segoe UI", 7f);

            for (int i = 0; i < 30; i++)
            {
                DateTime date = DateTime.Today.AddDays(-29 + i);
                int col = i % cols, row = i / cols;
                float x = startX + col * (cellW + gap);
                float y = startY + row * (cellH + gap);

                Color fill;
                if (!days.ContainsKey(date))
                    fill = Color.FromArgb(245, 245, 245);
                else if (days[date] > 20)
                    fill = Color.FromArgb(26, 35, 126);
                else if (days[date] > 10)
                    fill = Color.FromArgb(92, 107, 192);
                else
                    fill = Color.FromArgb(197, 202, 232);

                SolidBrush br = new SolidBrush(fill);
                FillRoundedRect(g, br, new RectangleF(x, y, cellW, cellH), 4);
                br.Dispose();

                Brush textBrush = days.ContainsKey(date) ? Brushes.White : Brushes.LightGray;
                g.DrawString(date.Day.ToString(), font, textBrush, x + 7, y + 8);
            }

            // Legend
            string[] legendLabels = { "None", "Low", "Mid", "High" };
            Color[] legendColors =
            {
                Color.FromArgb(245,245,245), Color.FromArgb(197,202,232),
                Color.FromArgb(92,107,192),  Color.FromArgb(26,35,126)
            };
            int lx = startX;
            for (int i = 0; i < legendLabels.Length; i++)
            {
                SolidBrush br = new SolidBrush(legendColors[i]);
                FillRoundedRect(g, br,
                    new RectangleF(lx, startY + 3 * (cellH + gap) + 4, 14, 14), 3);
                br.Dispose();
                g.DrawString(legendLabels[i], font, Brushes.Gray,
                    lx + 17, startY + 3 * (cellH + gap) + 4);
                lx += 60;
            }
        }

        // ── Per-subject breakdown ─────────────────────────────────────
        private void LoadSubjectBreakdown()
        {
            Panel area = new Panel
            {
                Location = new Point(12, 36),
                Size = new Size(pnlSubject.Width - 24, pnlSubject.Height - 52),
                BackColor = Color.White
            };
            pnlSubject.Controls.Add(area);
            area.Paint += (s, e) =>
                DrawSubjectBars(e.Graphics, area.Size, mockSubjects);
        }

        private void DrawSubjectBars(Graphics g, Size sz,
            List<Tuple<string, double, int>> data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Font font = new Font("Segoe UI", 8.5f);
            Font boldFont = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            int barH = 22, gap = 10, labelW = 110,
                maxBarW = sz.Width - labelW - 60;
            int y = 8;

            Color[] colors =
            {
                Color.FromArgb(26,35,126),   Color.FromArgb(92,107,192),
                Color.FromArgb(197,202,232), Color.FromArgb(21,101,192)
            };

            for (int i = 0; i < data.Count; i++)
            {
                string name = data[i].Item1;
                double avg = data[i].Item2;
                int count = data[i].Item3;

                string displayName = name.Length > 14 ? name.Substring(0, 14) + "..." : name;
                g.DrawString(displayName, font, Brushes.DimGray, 0, y + 4);

                int barW = (int)(maxBarW * avg / 100.0);
                SolidBrush br = new SolidBrush(colors[i % colors.Length]);
                FillRoundedRect(g, br, new RectangleF(labelW, y, barW, barH), 4);
                br.Dispose();

                g.DrawString(avg.ToString("0.#") + "%  (" + count + " quizzes)",
                    boldFont, Brushes.DimGray, labelW + barW + 6, y + 4);
                y += barH + gap;
            }
        }

        // ── Cards per day bar chart ───────────────────────────────────
        private void LoadCardsPerDay()
        {
            Panel area = new Panel
            {
                Location = new Point(12, 36),
                Size = new Size(pnlCards.Width - 24, pnlCards.Height - 52),
                BackColor = Color.White
            };
            pnlCards.Controls.Add(area);
            area.Paint += (s, e) =>
                DrawBarChart(e.Graphics, area.Size, mockCardsPerDay);
        }

        private void DrawBarChart(Graphics g, Size sz,
            List<Tuple<string, int>> data)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int pad = 32, w = sz.Width - pad * 2, h = sz.Height - pad * 2;
            int maxVal = 1;
            foreach (Tuple<string, int> d in data)
                if (d.Item2 > maxVal) maxVal = d.Item2;

            float barW = w / (float)data.Count * 0.6f;
            float step = w / (float)data.Count;
            Font font = new Font("Segoe UI", 7.5f);

            Pen axisPen = new Pen(Color.LightGray);
            g.DrawLine(axisPen, pad, pad, pad, pad + h);
            g.DrawLine(axisPen, pad, pad + h, pad + w, pad + h);
            axisPen.Dispose();

            for (int i = 0; i < data.Count; i++)
            {
                float barH = h * data[i].Item2 / (float)maxVal;
                float x = pad + i * step + (step - barW) / 2;
                float y = pad + h - barH;

                SolidBrush br = new SolidBrush(Color.FromArgb(92, 107, 192));
                FillRoundedRect(g, br, new RectangleF(x, y, barW, barH), 4);
                br.Dispose();

                g.DrawString(data[i].Item1, font, Brushes.Gray,
                    x + barW / 2 - 10, pad + h + 2);
                g.DrawString(data[i].Item2.ToString(), font, Brushes.DimGray,
                    x + barW / 2 - 6, y - 14);
            }
        }

        // ══════════════════════════════════════════════════════════════
        //  Rounded rectangle helper
        // ══════════════════════════════════════════════════════════════
        private static void FillRoundedRect(Graphics g, Brush br,
            RectangleF r, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(r.Right - radius * 2, r.Bottom - radius * 2,
                radius * 2, radius * 2, 0, 90);
            path.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            g.FillPath(br, path);
            path.Dispose();
        }
    }
}