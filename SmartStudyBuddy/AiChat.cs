using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmartStudyBuddy
{
    public partial class AiChat : Form
    {
        // ── PASTE YOUR GEMINI API KEY HERE ─────────────────────────────
        private const string API_KEY = "YOUR_GEMINI_API_KEY_HERE";
        // ───────────────────────────────────────────────────────────────

        private const string API_URL =
            "https://generativelanguage.googleapis.com/v1beta/models/" +
            "gemini-2.0-flash:generateContent?key=";

        private static readonly HttpClient httpClient = new HttpClient();

        private Panel pnlSidebar, pnlMain, pnlChatArea, pnlInput;
        private FlowLayoutPanel flowMessages;
        private TextBox txtInput;
        private Button btnSend, btnMic, btnVoiceToggle, btnClear;
        private Label lblStatus, lblTitle;
        private Panel pnlTopBar;

        private SpeechRecognitionEngine speechEngine;
        private SpeechSynthesizer synthesizer;
        private bool isListening = false;
        private bool voiceReply = true;

        public AiChat()
        {
            InitializeComponent();
            this.Text = "StudyBuddy - AI Chat";
            this.Size = new Size(1280, 800);
            this.BackColor = Color.FromArgb(224, 247, 250);
            this.Font = new Font("Segoe UI", 9f);

            synthesizer = new SpeechSynthesizer();
            synthesizer.Volume = 80;
            synthesizer.Rate = 0;

            BuildUI();
            AddWelcomeMessage();
        }

        // ══════════════════════════════════════════════════════════════
        //  UI BUILDER
        // ══════════════════════════════════════════════════════════════
        private void BuildUI()
        {
            // ── Sidebar ──────────────────────────────────────────────
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

            AddSidebarItem(pnlSidebar, "Home", 50, () => { this.Hide(); });
            AddSidebarItem(pnlSidebar, "Study Sets", 100, () => { this.Hide(); });
            AddSidebarItem(pnlSidebar, "Ai Chat", 150, null);
            AddSidebarItem(pnlSidebar, "Leader Board", 200, null);
            AddSidebarItem(pnlSidebar, "Profile", 250, null);

            // ── Top bar ───────────────────────────────────────────────
            pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(0, 86, 179)
            };

            lblTitle = new Label
            {
                Text = "Study Hub",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 14),
                AutoSize = true
            };
            pnlTopBar.Controls.Add(lblTitle);

            // ── Main area ─────────────────────────────────────────────
            pnlMain = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            // Chat header
            Label lblChatTitle = new Label
            {
                Text = "AI Study Assistant",
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 86, 179),
                Location = new Point(20, 16),
                AutoSize = true
            };
            Label lblChatSub = new Label
            {
                Text = "Ask me anything about your studies — I can explain topics and help with quizzes!",
                ForeColor = Color.Gray,
                Location = new Point(22, 44),
                AutoSize = true
            };

            // Toolbar buttons
            btnVoiceToggle = MakeToolButton("Voice Reply: ON", 20, 72,
                Color.FromArgb(0, 150, 136));
            btnVoiceToggle.Click += (s, e) =>
            {
                voiceReply = !voiceReply;
                btnVoiceToggle.Text = voiceReply ? "Voice Reply: ON" : "Voice Reply: OFF";
                btnVoiceToggle.BackColor = voiceReply
                    ? Color.FromArgb(0, 150, 136)
                    : Color.FromArgb(120, 120, 120);
            };

            btnClear = MakeToolButton("Clear Chat", 160, 72,
                Color.FromArgb(211, 47, 47));
            btnClear.Click += (s, e) =>
            {
                flowMessages.Controls.Clear();
                AddWelcomeMessage();
            };

            // Messages area
            pnlChatArea = new Panel
            {
                Location = new Point(20, 112),
                Size = new Size(1020, 530),
                BackColor = Color.White,
                AutoScroll = false,
                BorderStyle = BorderStyle.None
            };

            // Rounded border on chat area
            pnlChatArea.Paint += (s, e) =>
            {
                Pen pen = new Pen(Color.FromArgb(200, 230, 255), 2);
                e.Graphics.DrawRectangle(pen, 0, 0,
                    pnlChatArea.Width - 1, pnlChatArea.Height - 1);
                pen.Dispose();
            };

            flowMessages = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(12)
            };
            pnlChatArea.Controls.Add(flowMessages);

            // Input area
            pnlInput = new Panel
            {
                Location = new Point(20, 654),
                Size = new Size(1020, 52),
                BackColor = Color.White
            };
            pnlInput.Paint += (s, e) =>
            {
                Pen pen = new Pen(Color.FromArgb(0, 86, 179), 2);
                e.Graphics.DrawRectangle(pen, 0, 0,
                    pnlInput.Width - 1, pnlInput.Height - 1);
                pen.Dispose();
            };

            txtInput = new TextBox
            {
                Location = new Point(10, 12),
                Size = new Size(820, 28),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 11f),
                ForeColor = Color.FromArgb(30, 30, 30)
            };
            txtInput.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && !e.Shift)
                {
                    e.SuppressKeyPress = true;
                    SendMessage();
                }
            };

            btnSend = new Button
            {
                Text = "Send",
                Location = new Point(840, 8),
                Size = new Size(80, 36),
                BackColor = Color.FromArgb(0, 86, 179),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Click += (s, e) => SendMessage();

            btnMic = new Button
            {
                Text = "🎙",
                Location = new Point(930, 8),
                Size = new Size(80, 36),
                BackColor = Color.FromArgb(211, 47, 47),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Emoji", 14f),
                Cursor = Cursors.Hand
            };
            btnMic.FlatAppearance.BorderSize = 0;
            btnMic.Click += BtnMic_Click;

            pnlInput.Controls.Add(txtInput);
            pnlInput.Controls.Add(btnSend);
            pnlInput.Controls.Add(btnMic);

            // Status label
            lblStatus = new Label
            {
                Text = "Ready",
                ForeColor = Color.Gray,
                Location = new Point(22, 714),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic)
            };

            pnlMain.Controls.Add(lblChatTitle);
            pnlMain.Controls.Add(lblChatSub);
            pnlMain.Controls.Add(btnVoiceToggle);
            pnlMain.Controls.Add(btnClear);
            pnlMain.Controls.Add(pnlChatArea);
            pnlMain.Controls.Add(pnlInput);
            pnlMain.Controls.Add(lblStatus);

            this.Controls.Add(pnlMain);
            this.Controls.Add(pnlSidebar);
            this.Controls.Add(pnlTopBar);
        }

        private Button MakeToolButton(string text, int x, int y, Color color)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(130, 32),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void AddSidebarItem(Panel sidebar, string text, int y, Action onClick)
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
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(0, 60, 140);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.Transparent;
            if (onClick != null) btn.Click += (s, e) => onClick();
            sidebar.Controls.Add(btn);
        }

        // ══════════════════════════════════════════════════════════════
        //  WELCOME MESSAGE
        // ══════════════════════════════════════════════════════════════
        private void AddWelcomeMessage()
        {
            string welcome =
                "Hi! I'm your AI Study Assistant 👋\n\n" +
                "I can help you with:\n" +
                "• 📚 Explaining study topics\n" +
                "• ❓ Answering quiz questions\n" +
                "• 🧠 Creating practice questions\n" +
                "• 💡 Summarizing lessons\n\n" +
                "Try asking: \"Explain what a for loop is in C#\" " +
                "or \"Give me 5 quiz questions about arrays\"";
            AddBotBubble(welcome);
        }

        // ══════════════════════════════════════════════════════════════
        //  SEND MESSAGE
        // ══════════════════════════════════════════════════════════════
        private async void SendMessage()
        {
            string userText = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(userText)) return;

            txtInput.Text = "";
            btnSend.Enabled = false;
            btnMic.Enabled = false;

            AddUserBubble(userText);
            SetStatus("AI is thinking...");

            // Typing indicator
            Label typing = AddBotBubble("...");

            try
            {
                string response = await CallGeminiAsync(userText);
                flowMessages.Controls.Remove(typing);
                AddBotBubble(response);

                if (voiceReply)
                    SpeakAsync(response);

                SetStatus("Ready");
            }
            catch (Exception ex)
            {
                flowMessages.Controls.Remove(typing);
                AddBotBubble("Sorry, I couldn't get a response. Error: " + ex.Message);
                SetStatus("Error — check your API key or internet connection");
            }

            btnSend.Enabled = true;
            btnMic.Enabled = true;
        }

        // ══════════════════════════════════════════════════════════════
        //  GEMINI API CALL
        // ══════════════════════════════════════════════════════════════
        private async Task<string> CallGeminiAsync(string userMessage)
        {
            string systemPrompt =
                "You are a helpful study assistant for a student using StudyBuddy app. " +
                "Focus on explaining topics clearly, helping with C# programming, " +
                "general subjects, and creating quiz questions when asked. " +
                "Keep responses concise and student-friendly.";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = systemPrompt + "\n\nStudent: " + userMessage }
                        }
                    }
                }
            };

            string json = JsonConvert.SerializeObject(requestBody);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response =
                await httpClient.PostAsync(API_URL + API_KEY, content);
            string responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception("API Error: " + responseJson);

            JObject parsed = JObject.Parse(responseJson);
            string aiReply = parsed["candidates"][0]["content"]["parts"][0]["text"]
                               .ToString();
            return aiReply;
        }

        // ══════════════════════════════════════════════════════════════
        //  CHAT BUBBLES
        // ══════════════════════════════════════════════════════════════
        private Label AddUserBubble(string text)
        {
            Panel wrapper = new Panel
            {
                Width = flowMessages.Width - 30,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            Label bubble = new Label
            {
                Text = "You:  " + text,
                AutoSize = true,
                MaximumSize = new Size(600, 0),
                BackColor = Color.FromArgb(0, 86, 179),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f),
                Padding = new Padding(12, 8, 12, 8),
                Cursor = Cursors.Default
            };

            // Right-align
            wrapper.Controls.Add(bubble);
            wrapper.Height = bubble.Height + 10;
            bubble.Location = new Point(wrapper.Width - bubble.Width - 10, 4);

            flowMessages.Controls.Add(wrapper);
            ScrollToBottom();
            return bubble;
        }

        private Label AddBotBubble(string text)
        {
            Panel wrapper = new Panel
            {
                Width = flowMessages.Width - 30,
                AutoSize = true,
                BackColor = Color.Transparent
            };

            Label bubble = new Label
            {
                Text = "AI:  " + text,
                AutoSize = true,
                MaximumSize = new Size(700, 0),
                BackColor = Color.FromArgb(224, 247, 250),
                ForeColor = Color.FromArgb(20, 20, 60),
                Font = new Font("Segoe UI", 10f),
                Padding = new Padding(12, 8, 12, 8),
                Cursor = Cursors.Default
            };

            wrapper.Controls.Add(bubble);
            wrapper.Height = bubble.Height + 10;
            bubble.Location = new Point(10, 4);

            flowMessages.Controls.Add(wrapper);
            ScrollToBottom();
            return bubble;
        }

        private void ScrollToBottom()
        {
            flowMessages.ScrollControlIntoView(
                flowMessages.Controls[flowMessages.Controls.Count - 1]);
        }

        // ══════════════════════════════════════════════════════════════
        //  VOICE INPUT
        // ══════════════════════════════════════════════════════════════
        private void BtnMic_Click(object sender, EventArgs e)
        {
            if (!isListening)
                StartListening();
            else
                StopListening();
        }

        private void StartListening()
        {
            try
            {
                speechEngine = new SpeechRecognitionEngine();
                speechEngine.SetInputToDefaultAudioDevice();

                // Use dictation grammar for free-form input
                speechEngine.LoadGrammar(new DictationGrammar());
                speechEngine.SpeechRecognized += OnSpeechRecognized;
                speechEngine.SpeechHypothesized += OnSpeechHypothesized;
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);

                isListening = true;
                btnMic.BackColor = Color.FromArgb(0, 150, 0);
                btnMic.Text = "🔴";
                SetStatus("Listening... speak now");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Microphone error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StopListening()
        {
            if (speechEngine != null)
            {
                speechEngine.RecognizeAsyncStop();
                speechEngine.SpeechRecognized -= OnSpeechRecognized;
                speechEngine.SpeechHypothesized -= OnSpeechHypothesized;
                speechEngine.Dispose();
                speechEngine = null;
            }
            isListening = false;
            btnMic.BackColor = Color.FromArgb(211, 47, 47);
            btnMic.Text = "🎙";
            SetStatus("Ready");
        }

        private void OnSpeechHypothesized(object sender,
            SpeechHypothesizedEventArgs e)
        {
            if (InvokeRequired)
                Invoke(new Action(() => SetStatus("Hearing: " + e.Result.Text)));
            else
                SetStatus("Hearing: " + e.Result.Text);
        }

        private void OnSpeechRecognized(object sender,
            SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence < 0.5f) return;

            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    txtInput.Text = e.Result.Text;
                    StopListening();
                    SendMessage();
                }));
            }
            else
            {
                txtInput.Text = e.Result.Text;
                StopListening();
                SendMessage();
            }
        }

        // ══════════════════════════════════════════════════════════════
        //  VOICE OUTPUT (Text-to-Speech)
        // ══════════════════════════════════════════════════════════════
        private void SpeakAsync(string text)
        {
            // Strip long responses to avoid very long TTS
            string toSpeak = text.Length > 400
                ? text.Substring(0, 400) + "... and more."
                : text;

            try
            {
                synthesizer.SpeakAsyncCancelAll();
                synthesizer.SpeakAsync(toSpeak);
            }
            catch { /* ignore TTS errors */ }
        }

        private void SetStatus(string msg)
        {
            if (InvokeRequired)
                Invoke(new Action(() => lblStatus.Text = msg));
            else
                lblStatus.Text = msg;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopListening();
            synthesizer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}