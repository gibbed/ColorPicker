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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ColorPicker
{
    public partial class ColorBox : UserControl
    {
        public ColorBox()
        {
            this.InitializeComponent();
            this.FillColor = ColorBgra.Transparent;
        }

        private ColorBgra _FillColor;

        public ColorBgra FillColor
        {
            get { return this._FillColor; }

            set
            {
                if (this._FillColor != value)
                {
                    this._FillColor = value;
                    this.OnFillColorChanged();
                    this.Invalidate();
                }
            }
        }

        public event EventHandler FillColorChanged;

        private void OnFillColorChanged()
        {
            if (this.FillColorChanged != null)
            {
                this.FillColorChanged(this, new EventArgs());
            }
        }

        private void DrawFill(Graphics g)
        {
            g.FillRectangle(
                new HatchBrush(
                    HatchStyle.LargeCheckerBoard,
                    Color.White,
                    Color.Gray),
                this.ClientRectangle);
            g.FillRectangle(
                new SolidBrush(
                    this._FillColor.ToColor()),
                this.ClientRectangle);
        }

        protected void OnPaint(object sender, PaintEventArgs e)
        {
            this.DrawFill(e.Graphics);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            this.DrawFill(e.Graphics);
        }
    }
}
