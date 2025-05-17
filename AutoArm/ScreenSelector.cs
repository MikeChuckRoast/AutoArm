using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoArm
{
    public class ScreenSelector : Form
    {
        private Point startPoint;
        private Rectangle selection;
        private bool isDragging = false;

        public Rectangle SelectedRegion { get; private set; }

        public ScreenSelector()
        {
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;

            // Set the bounds to cover the entire virtual screen (all monitors)
            Rectangle virtualBounds = SystemInformation.VirtualScreen;
            this.Bounds = virtualBounds;

            this.BackColor = Color.White;
            this.Opacity = 0.2;
            this.Cursor = Cursors.Cross;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            isDragging = true;
            startPoint = e.Location;
            selection = new Rectangle(e.Location, Size.Empty);
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isDragging)
            {
                selection = new Rectangle(
                    Math.Min(startPoint.X, e.X),
                    Math.Min(startPoint.Y, e.Y),
                    Math.Abs(startPoint.X - e.X),
                    Math.Abs(startPoint.Y - e.Y)
                );
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isDragging = false;
            SelectedRegion = selection;
            DialogResult = DialogResult.OK;
            Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (selection != Rectangle.Empty)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, selection);
                }
            }
        }
    }
}
