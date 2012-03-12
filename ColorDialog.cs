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
using System.Windows.Forms;

namespace ColorPicker
{
    public partial class ColorDialog : Form
    {
        public ColorDialog()
        {
            this.InitializeComponent();
            this.colorWheel1.Color = ColorHsv.FromColor(Color.White);
        }

        public ColorBgra WheelColor
        {
            get
            {
                ColorRgb rgb = this.colorWheel1.Color.ToRgb();
                return ColorBgra.FromBgra(rgb.B.ClampToByte(),
                                          rgb.G.ClampToByte(),
                                          rgb.R.ClampToByte(),
                                          this.alphaColorSlider.Value.ClampToByte());
            }

            set
            {
                this.colorWheel1.Color = ColorHsv.FromColor(value.ToColor());
                this.alphaColorSlider.Value = value.A;
            }
        }

        private int _IgnoreChangedEventCounter;

        private bool IgnoreChangedEvents
        {
            get { return this._IgnoreChangedEventCounter != 0; }
        }

        private void PushIgnoreChangedEvents()
        {
            ++this._IgnoreChangedEventCounter;
        }

        private void PopIgnoreChangedEvents()
        {
            --this._IgnoreChangedEventCounter;
        }

        private void SetColorGradientMinMaxColorsRgb(int r, int g, int b)
        {
            this.redColorSlider.MaxColor = Color.FromArgb(255, g, b);
            this.redColorSlider.MinColor = Color.FromArgb(0, g, b);
            this.greenColorSlider.MaxColor = Color.FromArgb(r, 255, b);
            this.greenColorSlider.MinColor = Color.FromArgb(r, 0, b);
            this.blueColorSlider.MaxColor = Color.FromArgb(r, g, 255);
            this.blueColorSlider.MinColor = Color.FromArgb(r, g, 0);
        }

        // ReSharper disable UnusedParameter.Local
        private void SetColorGradientMinMaxColorsAlpha(int a)
            // ReSharper restore UnusedParameter.Local
        {
            /*
            Color[] colors = new Color[256];

            for (int newA = 0; newA <= 255; ++newA)
            {
                colors[newA] = Color.FromArgb(
                    newA,
                    this.redColorSlider.Value,
                    this.greenColorSlider.Value,
                    this.blueColorSlider.Value);
            }

            this.alphaColorSlider.CustomGradient = colors;
            */

            this.alphaColorSlider.MaxColor = Color.FromArgb(
                this.redColorSlider.Value,
                this.greenColorSlider.Value,
                this.blueColorSlider.Value);
        }

        private void SetColorGradientValuesRgb(int r, int g, int b)
        {
            // ReSharper disable RedundantCheckBeforeAssignment
            if (redColorSlider.Value != r) // ReSharper restore RedundantCheckBeforeAssignment
            {
                redColorSlider.Value = r;
            }

            // ReSharper disable RedundantCheckBeforeAssignment
            if (greenColorSlider.Value != g) // ReSharper restore RedundantCheckBeforeAssignment
            {
                greenColorSlider.Value = g;
            }

            // ReSharper disable RedundantCheckBeforeAssignment
            if (blueColorSlider.Value != b) // ReSharper restore RedundantCheckBeforeAssignment
            {
                blueColorSlider.Value = b;
            }
        }

        private void SetColorGradientValuesHsv(int h, int s, int v)
        {
            // ReSharper disable RedundantCheckBeforeAssignment
            if (hueColorSlider.Value != h) // ReSharper restore RedundantCheckBeforeAssignment
            {
                hueColorSlider.Value = h;
            }

            // ReSharper disable RedundantCheckBeforeAssignment
            if (saturationColorSlider.Value != s) // ReSharper restore RedundantCheckBeforeAssignment
            {
                saturationColorSlider.Value = s;
            }

            // ReSharper disable RedundantCheckBeforeAssignment
            if (valueColorSlider.Value != v) // ReSharper restore RedundantCheckBeforeAssignment
            {
                valueColorSlider.Value = v;
            }
        }

        private void SetColorGradientMinMaxColorsHsv(int h, int s, int v)
        {
            var hueColors = new Color[361];

            for (int newH = 0; newH <= 360; ++newH)
            {
                var hsv = new ColorHsv(newH, 100, 100);
                hueColors[newH] = hsv.ToColor();
            }

            this.hueColorSlider.CustomGradient = hueColors;

            var satColors = new Color[101];

            for (int newS = 0; newS <= 100; ++newS)
            {
                var hsv = new ColorHsv(h, newS, v);
                satColors[newS] = hsv.ToColor();
            }

            this.saturationColorSlider.CustomGradient = satColors;

            this.valueColorSlider.MaxColor = new ColorHsv(h, s, 100).ToColor();
            this.valueColorSlider.MinColor = new ColorHsv(h, s, 0).ToColor();
        }

        private void SyncHsvFromRgb(ColorBgra bgra)
        {
            ColorHsv hsv = ColorHsv.FromColor(bgra.ToColor());

            this.SetColorGradientValuesHsv(hsv.Hue, hsv.Saturation, hsv.Value);
            this.SetColorGradientMinMaxColorsHsv(hsv.Hue, hsv.Saturation, hsv.Value);

            colorWheel1.Color = hsv;
        }

        private void SyncRgbFromHsv(ColorHsv hsv)
        {
            ColorRgb rgb = hsv.ToRgb();

            redColorSlider.Value = rgb.R;
            greenColorSlider.Value = rgb.G;
            blueColorSlider.Value = rgb.B;

            this.SetColorGradientValuesRgb(rgb.R, rgb.G, rgb.B);
            this.SetColorGradientMinMaxColorsRgb(rgb.R, rgb.G, rgb.B);
            SetColorGradientMinMaxColorsAlpha(alphaColorSlider.Value);
        }

        private void OnColorSliderValueChanged(object sender, EventArgs e)
        {
            if (this.IgnoreChangedEvents)
            {
                return;
            }

            this.PushIgnoreChangedEvents();

            if (sender == redColorSlider ||
                sender == greenColorSlider ||
                sender == blueColorSlider)
            {
                ColorBgra color = ColorBgra.FromBgra(
                    blueColorSlider.Value.ClampToByte(),
                    greenColorSlider.Value.ClampToByte(),
                    redColorSlider.Value.ClampToByte(),
                    alphaColorSlider.Value.ClampToByte());

                this.SetColorGradientMinMaxColorsRgb(color.R, color.G, color.B);
                this.SetColorGradientMinMaxColorsAlpha(color.A);
                this.SetColorGradientValuesRgb(color.R, color.G, color.B);
                this.SetColorGradientMinMaxColorsAlpha(color.A);

                this.SyncHsvFromRgb(color);
                //OnUserColorChanged(rgbColor);
            }
            else if (
                sender == hueColorSlider ||
                sender == saturationColorSlider ||
                sender == valueColorSlider)
            {
                ColorHsv oldHsv = colorWheel1.Color;
                var hsv = new ColorHsv(
                    hueColorSlider.Value,
                    saturationColorSlider.Value,
                    valueColorSlider.Value);

                if (oldHsv != hsv)
                {
                    colorWheel1.Color = hsv;

                    this.SetColorGradientValuesHsv(
                        hsv.Hue,
                        hsv.Saturation,
                        hsv.Value);

                    this.SetColorGradientMinMaxColorsHsv(
                        hsv.Hue,
                        hsv.Saturation,
                        hsv.Value);

                    this.SyncRgbFromHsv(hsv);
                    //ColorRGB rgbColor = hsv.ToRgb();
                    //OnUserColorChanged(ColorBgra.FromBgra((byte)rgbColor.Blue, (byte)rgbColor.Green, (byte)rgbColor.Red, (byte)alphaUpDown.Value));
                }
            }

            this.PopIgnoreChangedEvents();
        }

        private void OnWheelColorChanged(object sender, EventArgs e)
        {
            if (this.IgnoreChangedEvents)
            {
                return;
            }

            this.PushIgnoreChangedEvents();

            ColorHsv hsvColor = colorWheel1.Color;
            ColorRgb rgbColor = hsvColor.ToRgb();
            ColorBgra color = ColorBgra.FromBgra(
                (byte)rgbColor.B,
                (byte)rgbColor.G,
                (byte)rgbColor.R,
                (byte)alphaColorSlider.Value);

            hueColorSlider.Value = hsvColor.Hue;
            saturationColorSlider.Value = hsvColor.Saturation;
            valueColorSlider.Value = hsvColor.Value;

            redColorSlider.Value = color.R;
            greenColorSlider.Value = color.G;
            blueColorSlider.Value = color.B;

            alphaColorSlider.Value = color.A;

            this.SetColorGradientValuesHsv(
                hsvColor.Hue,
                hsvColor.Saturation,
                hsvColor.Value);

            this.SetColorGradientMinMaxColorsHsv(
                hsvColor.Hue,
                hsvColor.Saturation,
                hsvColor.Value);

            this.SetColorGradientValuesRgb(
                color.R,
                color.G,
                color.B);

            this.SetColorGradientMinMaxColorsRgb(
                color.R,
                color.G,
                color.B);

            SetColorGradientMinMaxColorsAlpha(color.A);

            this.PopIgnoreChangedEvents();
            this.Update();
        }
    }
}
