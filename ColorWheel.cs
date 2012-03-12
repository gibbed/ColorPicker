﻿/* Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS 
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ColorPicker
{
    public class ColorWheel
        : UserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container _Components;

        private Bitmap _RenderBitmap;
        private bool _Tracking;

        /// <summary>
        /// This number controls what you might call the tesselation of 
        /// the color wheel. Higher value = slower, lower value = looks worse.
        /// </summary>
        private const int ColorTesselation = 80;

        private PictureBox _PictureBox;

        #region public ColorHSV Color
        private ColorHsv _Color;

        public ColorHsv Color
        {
            get { return this._Color; }

            set
            {
                if (this._Color != value)
                {
                    this._Color = value;
                    this.OnColorChanged();
                    this.Refresh();
                }
            }
        }
        #endregion

        public ColorWheel()
        {
            this.InitializeComponent();
            this._Color = new ColorHsv(0, 0, 0);
        }

        private static PointF SphericalToCartesian(float r, float theta)
        {
            float x = r * (float)Math.Cos(theta);
            float y = r * (float)Math.Sin(theta);

            return new PointF(x, y);
        }

        private static PointF[] GetCirclePoints(float r, PointF center)
        {
            var points = new PointF[ColorTesselation];

            for (int i = 0; i < ColorTesselation; i++)
            {
                float theta = (i / (float)ColorTesselation) * 2 * (float)Math.PI;
                points[i] = SphericalToCartesian(r, theta);
                points[i].X += center.X;
                points[i].Y += center.Y;
            }

            return points;
        }

        private Color[] GetColors()
        {
            var colors = new Color[ColorTesselation];

            for (int i = 0; i < ColorTesselation; i++)
            {
                int hue = (i * 360) / ColorTesselation;
                colors[i] = new ColorHsv(hue, 100, 100).ToColor();
            }

            return colors;
        }

        protected override void OnLoad(EventArgs e)
        {
            this.InitRendering();
            base.OnLoad(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this.InitRendering();
            base.OnPaint(e);
        }

        private void InitRendering()
        {
            if (this._RenderBitmap == null)
            {
                this.InitRenderSurface();
                this._PictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                var size = (int)Math.Ceiling(ComputeDiameter(this.Size));
                this._PictureBox.Size = new Size(size, size);
                this._PictureBox.Image = this._RenderBitmap;
            }
        }

        private void OnWheelPaint(object sender, PaintEventArgs e)
        {
            float radius = ComputeRadius(this.Size);
            float theta = (this.Color.Hue / 360.0f) * 2.0f * (float)Math.PI;
            float alpha = (this.Color.Saturation / 100.0f);
            float x = (alpha * (radius - 1) * (float)Math.Cos(theta)) + radius;
            float y = (alpha * (radius - 1) * (float)Math.Sin(theta)) + radius;
            var ix = (int)x;
            var iy = (int)y;

            // Draw the 'target rectangle'
            GraphicsContainer container = e.Graphics.BeginContainer();
            e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.DrawRectangle(Pens.Black, ix - 1, iy - 1, 3, 3);
            e.Graphics.DrawRectangle(Pens.White, ix, iy, 1, 1);
            e.Graphics.EndContainer(container);
        }

        private void InitRenderSurface()
        {
            if (this._RenderBitmap != null)
            {
                this._RenderBitmap.Dispose();
            }

            var wheelDiameter = (int)ComputeDiameter(Size);

            this._RenderBitmap = new Bitmap(
                Math.Max(1, (wheelDiameter * 4) / 3),
                Math.Max(1, (wheelDiameter * 4) / 3),
                PixelFormat.Format24bppRgb);

            using (Graphics g = Graphics.FromImage(this._RenderBitmap))
            {
                g.Clear(this.BackColor);
                this.DrawWheel(
                    g,
                    this._RenderBitmap.Width,
                    this._RenderBitmap.Height);
            }
        }

        private void DrawWheel(Graphics g, int width, int height)
        {
            float radius = ComputeRadius(new Size(width, height));
            PointF[] points = GetCirclePoints(
                Math.Max(1.0f, radius - 1),
                new PointF(radius, radius));

            using (var pgb = new PathGradientBrush(points))
            {
                pgb.CenterColor = new ColorHsv(0, 0, 100).ToColor();
                pgb.CenterPoint = new PointF(radius, radius);
                pgb.SurroundColors = GetColors();

                g.FillEllipse(pgb, 0, 0, radius * 2, radius * 2);
            }
        }

        private static float ComputeRadius(Size size)
        {
            return Math.Min((float)size.Width / 2, (float)size.Height / 2);
        }

        private static float ComputeDiameter(Size size)
        {
            return Math.Min(size.Width, (float)size.Height);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this._RenderBitmap != null &&
                Equals(ComputeRadius(this.Size), ComputeRadius(this._RenderBitmap.Size)) == false)
            {
                this._RenderBitmap.Dispose();
                this._RenderBitmap = null;
            }

            this.Invalidate();
        }

        [Category("Action")]
        [Description("Occurs when the selected color changes.")]
        public event EventHandler ColorChanged;

        protected virtual void OnColorChanged()
        {
            if (this.ColorChanged != null)
            {
                this.ColorChanged(this, EventArgs.Empty);
            }
        }

        private void GrabColor(Point point)
        {
            // center our coordinate system so the middle is (0,0), and positive Y is facing up
            int cx = point.X - (Width / 2);
            int cy = point.Y - (Height / 2);

            double theta = Math.Atan2(cy, cx);

            if (theta < 0)
            {
                theta += 2 * Math.PI;
            }

            double alpha = Math.Sqrt((cx * cx) + (cy * cy));

            var h = (int)((theta / (Math.PI * 2)) * 360.0);
// ReSharper disable PossibleLossOfFraction
            var s = (int)Math.Min(100.0, (alpha / (this.Width / 2)) * 100);
// ReSharper restore PossibleLossOfFraction
            const int v = 100;

            this._Color = new ColorHsv(h, s, v);
            this.OnColorChanged();
            this.Invalidate(true);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                this._Tracking = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (this._Tracking == true)
            {
                this.GrabColor(new Point(e.X, e.Y));
                this._Tracking = false;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            new Point(e.X, e.Y);
            if (this._Tracking == true)
            {
                this.GrabColor(new Point(e.X, e.Y));
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                if (this._Components != null)
                {
                    this._Components.Dispose();
                    this._Components = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._PictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._PictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this._PictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._PictureBox.Location = new System.Drawing.Point(0, 0);
            this._PictureBox.Name = "_PictureBox";
            this._PictureBox.Size = new System.Drawing.Size(150, 150);
            this._PictureBox.TabIndex = 0;
            this._PictureBox.TabStop = false;
            this._PictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnWheelMouseMove);
            this._PictureBox.Click += new System.EventHandler(this.OnWheelClick);
            this._PictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnWheelMouseDown);
            this._PictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.OnWheelPaint);
            this._PictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnWheelMouseUp);
            // 
            // ColorWheel
            // 
            this.Controls.Add(this._PictureBox);
            this.Name = "ColorWheel";
            ((System.ComponentModel.ISupportInitialize)(this._PictureBox)).EndInit();
            this.ResumeLayout(false);
        }
        #endregion

        private void OnWheelMouseMove(object sender, MouseEventArgs e)
        {
            this.OnMouseMove(e);
        }

        private void OnWheelMouseUp(object sender, MouseEventArgs e)
        {
            this.OnMouseUp(e);
        }

        private void OnWheelMouseDown(object sender, MouseEventArgs e)
        {
            this.OnMouseDown(e);
        }

        private void OnWheelClick(object sender, EventArgs e)
        {
            this.OnClick(e);
        }
    }
}
