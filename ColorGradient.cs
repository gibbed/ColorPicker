/* Permission is hereby granted, free of charge, to any person obtaining a copy of 
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
    public sealed class ColorGradient : UserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container _Components;

        private int _Tracking = -1;
        private int _Highlight = -1;

        private const int TriangleSize = 7;
        private const int TriangleHalfLength = (TriangleSize - 1) / 2;

        #region public bool DrawNearNub
        private bool _DrawNearNub = true;

        [Category("Gradient")]
        [DisplayName("Draw Near Nub")]
        public bool DrawNearNub
        {
            get { return this._DrawNearNub; }

            set
            {
                this._DrawNearNub = value;
                this.Invalidate();
            }
        }
        #endregion

        #region public bool DrawFarNub
        private bool _DrawFarNub = true;

        [Category("Gradient")]
        [DisplayName("Draw Far Nub")]
        public bool DrawFarNub
        {
            get { return this._DrawFarNub; }

            set
            {
                this._DrawFarNub = value;
                this.Invalidate();
            }
        }
        #endregion

        private int[] _Values;

        // value from [0,255] that specifies the hsv "value" component
        // where we should draw little triangles that show the value

        #region public int Value
        [Category("Gradient")]
        public int Value
        {
            get { return this.GetValue(0); }

            set { this.SetValue(0, value); }
        }
        #endregion

        #region public Color[] CustomGradient
        private Color[] _CustomGradient;

        [Category("Gradient")]
        [DisplayName("Custom Gradient")]
        public Color[] CustomGradient
        {
            get
            {
                if (this._CustomGradient == null)
                {
                    return null;
                }

                return (Color[])this._CustomGradient.Clone();
            }

            set
            {
                if (value != this._CustomGradient)
                {
                    if (value == null)
                    {
                        this._CustomGradient = null;
                    }
                    else
                    {
                        this._CustomGradient = (Color[])value.Clone();
                    }

                    this.Invalidate();
                }
            }
        }
        #endregion

        #region public Orientation Orientation
        private Orientation _Orientation = Orientation.Vertical;

        [Category("Gradient")]
        public Orientation Orientation
        {
            get { return this._Orientation; }

            set
            {
                if (value != this._Orientation)
                {
                    this._Orientation = value;
                    this.Invalidate();
                }
            }
        }
        #endregion

        #region public int Count
        [Browsable(false)]
        public int Count
        {
            get { return this._Values.Length; }

            set
            {
                if (value < 0 || value > 16)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Count must be between 0 and 16");
                }

                this._Values = new int[value];

                if (value > 1)
                {
                    for (int i = 0; i < value; i++)
                    {
                        this._Values[i] = i * 255 / (value - 1);
                    }
                }
                else if (value == 1)
                {
                    this._Values[0] = 128;
                }

                this.OnValueChanged(0);
                this.Invalidate();
            }
        }
        #endregion

        public int GetValue(int index)
        {
            if (index < 0 || index >= this._Values.Length)
            {
                throw new ArgumentOutOfRangeException("index", index, "Index must be within the bounds of the array");
            }

            return this._Values[index];
        }

        public void SetValue(int index, int val)
        {
            int min = -1;
            int max = 256;

            if (index < 0 || index >= this._Values.Length)
            {
                throw new ArgumentOutOfRangeException("index", index, "Index must be within the bounds of the array");
            }

            if (index - 1 >= 0)
            {
                min = this._Values[index - 1];
            }

            if (index + 1 < this._Values.Length)
            {
                max = this._Values[index + 1];
            }

            if (this._Values[index] != val)
            {
                this._Values[index] = val.Clamp(min + 1, max - 1);
                this.OnValueChanged(index);
                this.Invalidate();
            }

            this.Update();
        }

        public event EventHandler ValueChanged;
        // ReSharper disable UnusedParameter.Local
        private void OnValueChanged(int index)
            // ReSharper restore UnusedParameter.Local
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, new EventArgs());
            }
        }

        #region public Color MinColor
        private Color _MinColor = Color.Red;

        [Category("Gradient")]
        [DisplayName("Min Color")]
        public Color MinColor
        {
            get { return this._MinColor; }

            set
            {
                if (this._MinColor != value)
                {
                    this._MinColor = value;
                    this.Invalidate();
                }
            }
        }
        #endregion

        #region public Color MaxColor
        private Color _MaxColor = Color.Blue;

        [Category("Gradient")]
        [DisplayName("Max Color")]
        public Color MaxColor
        {
            get { return this._MaxColor; }

            set
            {
                if (this._MaxColor != value)
                {
                    this._MaxColor = value;
                    this.Invalidate();
                }
            }
        }
        #endregion

        public ColorGradient()
        {
            this.InitializeComponent();
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.Count = 1;
        }

        private void DrawGradient(Graphics g)
        {
            g.PixelOffsetMode = PixelOffsetMode.Half;

            float gradientAngle;

            switch (this._Orientation)
            {
                case Orientation.Horizontal:
                {
                    gradientAngle = 180.0f;
                    break;
                }

                case Orientation.Vertical:
                {
                    gradientAngle = 90.0f;
                    break;
                }

                default:
                {
                    throw new InvalidEnumArgumentException();
                }
            }

            // draw gradient
            Rectangle gradientRect = this.ClientRectangle;

            switch (this._Orientation)
            {
                case Orientation.Horizontal:
                {
                    gradientRect.Inflate(-TriangleHalfLength, -TriangleSize + 3);
                    break;
                }

                case Orientation.Vertical:
                {
                    gradientRect.Inflate(-TriangleSize + 3, -TriangleHalfLength);
                    break;
                }

                default:
                {
                    throw new InvalidEnumArgumentException();
                }
            }

            g.FillRectangle(new HatchBrush(
                                HatchStyle.LargeCheckerBoard,
                                Color.White,
                                Color.Gray),
                            gradientRect);

            if (this._CustomGradient != null &&
                this._CustomGradient.Length > 2 &&
                gradientRect.Width > 1 &&
                gradientRect.Height > 1)
            {
                var gradientSurface = new Bitmap(
                    gradientRect.Size.Width,
                    gradientRect.Size.Height,
                    PixelFormat.Format32bppArgb);

                if (Orientation == Orientation.Horizontal)
                {
                    for (int x = 0; x < gradientSurface.Width; ++x)
                    {
                        // TODO: refactor, double buffer, save this computation in a bitmap somewhere
                        double index =
                            x * (this._CustomGradient.Length - 1) /
                            (double)(gradientSurface.Width - 1);

                        var indexL = (int)Math.Floor(index);
                        double t = 1.0 - (index - indexL);

                        var indexR = (int)Math.Min(
                            this._CustomGradient.Length - 1,
                            Math.Ceiling(index));

                        Color colorL = this._CustomGradient[indexL];
                        Color colorR = this._CustomGradient[indexR];

                        double a1 = colorL.A / 255.0;
                        double r1 = colorL.R / 255.0;
                        double g1 = colorL.G / 255.0;
                        double b1 = colorL.B / 255.0;

                        double a2 = colorR.A / 255.0;
                        double r2 = colorR.R / 255.0;
                        double g2 = colorR.G / 255.0;
                        double b2 = colorR.B / 255.0;

                        double at = (t * a1) + ((1.0 - t) * a2);

                        double rt;
                        double gt;
                        double bt;

                        if (Equals(at, 0) == true)
                        {
                            rt = 0;
                            gt = 0;
                            bt = 0;
                        }
                        else
                        {
                            rt = ((t * a1 * r1) + ((1.0 - t) * a2 * r2)) / at;
                            gt = ((t * a1 * g1) + ((1.0 - t) * a2 * g2)) / at;
                            bt = ((t * a1 * b1) + ((1.0 - t) * a2 * b2)) / at;
                        }

                        var ap = (int)(Math.Round(at * 255.0)).Clamp(0, 255);
                        var rp = (int)(Math.Round(rt * 255.0)).Clamp(0, 255);
                        var gp = (int)(Math.Round(gt * 255.0)).Clamp(0, 255);
                        var bp = (int)(Math.Round(bt * 255.0)).Clamp(0, 255);

                        for (int y = 0; y < gradientSurface.Height; ++y)
                        {
                            const int srcA = 0;
                            const int srcR = 0;
                            const int srcG = 0;
                            const int srcB = 0;

                            // we are assuming that src.A = 255
                            int ad = ((ap * ap) + (srcA * (255 - ap))) / 255;
                            int rd = ((rp * ap) + (srcR * (255 - ap))) / 255;
                            int gd = ((gp * ap) + (srcG * (255 - ap))) / 255;
                            int bd = ((bp * ap) + (srcB * (255 - ap))) / 255;

                            // TODO: proper alpha blending!
                            gradientSurface.SetPixel(x, y, Color.FromArgb(ad, rd, gd, bd));
                        }
                    }

                    g.DrawImage(
                        gradientSurface,
                        gradientRect,
                        new Rectangle(new Point(0, 0), gradientSurface.Size),
                        GraphicsUnit.Pixel);
                }
                else if (Orientation == Orientation.Vertical)
                {
                    g.FillRectangle(Brushes.Red, gradientRect);
                    /*                    
                    g.DrawImage(
                        gradientSurface,
                        gradientRect,
                        new Rectangle(new Point(0, 0), gradientSurface.Size),
                        GraphicsUnit.Pixel);
                    */
                }
                else
                {
                    throw new InvalidEnumArgumentException();
                }
            }
            else if (this._CustomGradient != null &&
                     this._CustomGradient.Length == 2)
            {
                using (var lgb =
                    new LinearGradientBrush(
                        this.ClientRectangle,
                        this._CustomGradient[1],
                        this._CustomGradient[0],
                        gradientAngle,
                        false))
                {
                    g.FillRectangle(lgb, gradientRect);
                }
            }
            else
            {
                using (var lgb =
                    new LinearGradientBrush(
                        this.ClientRectangle,
                        this._MaxColor,
                        this._MinColor,
                        gradientAngle,
                        false))
                {
                    g.FillRectangle(lgb, gradientRect);
                }
            }

            // fill background
            using (var nonGradientRegion = new Region())
            {
                nonGradientRegion.MakeInfinite();
                nonGradientRegion.Exclude(gradientRect);

                using (var sb = new SolidBrush(this.BackColor))
                {
                    //g.FillRegion(sb, nonGradientRegion.GetRegionReadOnly());
                    g.FillRegion(sb, nonGradientRegion);
                }
            }

            // draw value triangles
            for (int i = 0; i < this._Values.Length; i++)
            {
                int pos = ValueToPosition(this._Values[i]);
                Brush brush;
                Pen pen;

                if (i == this._Highlight)
                {
                    brush = Brushes.Blue;
                    pen = (Pen)Pens.White.Clone();
                }
                else
                {
                    brush = Brushes.Black;
                    pen = (Pen)Pens.Gray.Clone();
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;

                Point a1;
                Point b1;
                Point c1;

                Point a2;
                Point b2;
                Point c2;

                switch (this._Orientation)
                {
                    case Orientation.Horizontal:
                    {
                        a1 = new Point(pos - TriangleHalfLength, 0);
                        b1 = new Point(pos, TriangleSize - 1);
                        c1 = new Point(pos + TriangleHalfLength, 0);

                        a2 = new Point(a1.X, Height - 1 - a1.Y);
                        b2 = new Point(b1.X, Height - 1 - b1.Y);
                        c2 = new Point(c1.X, Height - 1 - c1.Y);
                        break;
                    }

                    case Orientation.Vertical:
                    {
                        a1 = new Point(0, pos - TriangleHalfLength);
                        b1 = new Point(TriangleSize - 1, pos);
                        c1 = new Point(0, pos + TriangleHalfLength);

                        a2 = new Point(Width - 1 - a1.X, a1.Y);
                        b2 = new Point(Width - 1 - b1.X, b1.Y);
                        c2 = new Point(Width - 1 - c1.X, c1.Y);
                        break;
                    }

                    default:
                    {
                        throw new InvalidEnumArgumentException();
                    }
                }

                if (this._DrawNearNub)
                {
                    g.FillPolygon(brush, new[] {a1, b1, c1, a1});
                }

                if (this._DrawFarNub)
                {
                    g.FillPolygon(brush, new[] {a2, b2, c2, a2});
                }

// ReSharper disable ConditionIsAlwaysTrueOrFalse
                if (pen != null)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
                {
                    if (this._DrawNearNub)
                    {
                        g.DrawPolygon(pen, new[] {a1, b1, c1, a1});
                    }

                    if (this._DrawFarNub)
                    {
                        g.DrawPolygon(pen, new[] {a2, b2, c2, a2});
                    }

                    pen.Dispose();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.DrawGradient(e.Graphics);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            this.DrawGradient(pevent.Graphics);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._Components != null)
                {
                    this._Components.Dispose();
                    this._Components = null;
                }
            }

            base.Dispose(disposing);
        }

        private int PositionToValue(int pos)
        {
            int max;

            switch (this._Orientation)
            {
                case Orientation.Horizontal:
                {
                    max = Width;
                    break;
                }

                case Orientation.Vertical:
                {
                    max = Height;
                    break;
                }

                default:
                {
                    throw new InvalidEnumArgumentException();
                }
            }

            int val = (((max - TriangleSize) - (pos - TriangleHalfLength)) * 255) / (max - TriangleSize);

            if (this._Orientation == Orientation.Horizontal)
            {
                val = 255 - val;
            }

            return val;
        }

        private int ValueToPosition(int val)
        {
            int max;

            if (this._Orientation == Orientation.Horizontal)
            {
                val = 255 - val;
            }

            switch (this._Orientation)
            {
                case Orientation.Horizontal:
                {
                    max = Width;
                    break;
                }

                case Orientation.Vertical:
                {
                    max = Height;
                    break;
                }

                default:
                {
                    throw new InvalidEnumArgumentException();
                }
            }

            int pos = TriangleHalfLength + ((max - TriangleSize) - (((val * (max - TriangleSize)) / 255)));
            return pos;
        }

        private int WhichTriangle(int val)
        {
            int bestIndex = -1;
            int bestDistance = int.MaxValue;
            int v = PositionToValue(val);

            for (int i = 0; i < this._Values.Length; i++)
            {
                int distance = Math.Abs(this._Values[i] - v);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                int val = GetOrientedValue(e);
                this._Tracking = WhichTriangle(val);
                Invalidate();
                OnMouseMove(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                OnMouseMove(e);
                this._Tracking = -1;
                Invalidate();
            }
        }

        private int GetOrientedValue(MouseEventArgs me)
        {
            return GetOrientedValue(new Point(me.X, me.Y));
        }

        private int GetOrientedValue(Point pt)
        {
            int pos;

            switch (this._Orientation)
            {
                case Orientation.Horizontal:
                {
                    pos = pt.X;
                    break;
                }

                case Orientation.Vertical:
                {
                    pos = pt.Y;
                    break;
                }

                default:
                {
                    throw new InvalidEnumArgumentException();
                }
            }

            return pos;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int pos = GetOrientedValue(e);

            if (this._Tracking >= 0)
            {
                int val = PositionToValue(pos);
                this.SetValue(this._Tracking, val);
            }
            else
            {
                int oldHighlight = this._Highlight;
                this._Highlight = WhichTriangle(pos);

                if (this._Highlight != oldHighlight)
                {
                    this.InvalidateTriangle(oldHighlight);
                    this.InvalidateTriangle(this._Highlight);
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            int oldhighlight = this._Highlight;
            this._Highlight = -1;
            this.InvalidateTriangle(oldhighlight);
        }

        private void InvalidateTriangle(int index)
        {
            if (index < 0 || index >= this._Values.Length)
            {
                return;
            }

            int value = ValueToPosition(this._Values[index]);
            Rectangle rect;

            switch (this._Orientation)
            {
                case Orientation.Horizontal:
                    rect = new Rectangle(value - TriangleHalfLength, 0, TriangleSize, this.Height);
                    break;

                case Orientation.Vertical:
                    rect = new Rectangle(0, value - TriangleHalfLength, this.Width, TriangleSize);
                    break;

                default:
                    throw new InvalidEnumArgumentException();
            }

            this.Invalidate(rect, true);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._Components = new System.ComponentModel.Container();
        }
        #endregion
    }
}
