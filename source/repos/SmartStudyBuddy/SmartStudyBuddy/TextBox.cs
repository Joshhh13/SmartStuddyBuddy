using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SmartStudyBuddy
{
    public class RoundedTextBox : TextBox
    {
        private int borderRadius = 15;
        private Color borderColor = Color.White;
        private int borderSize = 2;

        // Custom Properties na lalabas sa Properties Window
        public int BorderRadius
        {
            get => borderRadius;
            set { borderRadius = value; Invalidate(); }
        }

        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; Invalidate(); }
        }

        public int BorderSize
        {
            get => borderSize;
            set { borderSize = value; Invalidate(); }
        }

        public RoundedTextBox()
        {
            // Automatic na i-remove ang default border para makita ang custom border
            this.BorderStyle = BorderStyle.None;
            this.Height = 35; // Standard height para maayos ang curve
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // 0x000F = WM_PAINT (kapag nagre-render ang control)
            if (m.Msg == 0x000F)
            {
                DrawRoundedBorder();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate(); // I-refresh ang border kapag nag-resize ang form
        }

        private void DrawRoundedBorder()
        {
            using (Graphics g = Graphics.FromHwnd(this.Handle))
            using (GraphicsPath path = new GraphicsPath())
            {
                int r = borderRadius;

                // Iwasan ang error kapag maliit ang textbox
                if (this.Width < r * 2) r = this.Width / 2;
                if (this.Height < r * 2) r = this.Height / 2;

                // Gumawa ng rounded rectangle path
                path.AddArc(0, 0, r, r, 180, 90);
                path.AddArc(this.Width - r, 0, r, r, 270, 90);
                path.AddArc(this.Width - r, this.Height - r, r, r, 0, 90);
                path.AddArc(0, this.Height - r, r, r, 90, 90);
                path.CloseFigure();

                // I-draw ang border
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(borderColor, borderSize))
                {
                    g.DrawPath(pen, path);
                }
            }
        }
    }
}