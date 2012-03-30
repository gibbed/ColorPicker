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

namespace ColorPicker
{
    [Serializable]
    public struct ColorRgb
    {
        public int R;
        public int G;
        public int B;

        public ColorRgb(int r, int g, int b)
        {
            if (r < 0 || r > 255)
            {
                throw new ArgumentOutOfRangeException("r", r, "r must corrospond to a byte value");
            }

            if (g < 0 || g > 255)
            {
                throw new ArgumentOutOfRangeException("g", g, "g must corrospond to a byte value");
            }

            if (b < 0 || b > 255)
            {
                throw new ArgumentOutOfRangeException("b", b, "b must corrospond to a byte value");
            }

            this.R = r;
            this.G = g;
            this.B = b;
        }

        public static ColorRgb FromHsv(ColorHsv hsv)
        {
            return hsv.ToRgb();
        }

        public Color ToColor()
        {
            return Color.FromArgb(this.R, this.G, this.B);
        }

        public ColorHsv ToHsv()
        {
            // In this function, R, G, and B values must be scaled 
            // to be between 0 and 1.
            // HsvColor.Hue will be a value between 0 and 360, and 
            // HsvColor.Saturation and value are between 0 and 1.

            double r = (double)R / 255;
            double g = (double)G / 255;
            double b = (double)B / 255;

            double h;
            double s;

            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            double v = max;
            double delta = max - min;

            if (Equals(max, 0.0) == true || Equals(delta, 0.0) == true)
            {
                // R, G, and B must be 0, or all the same.
                // In this case, S is 0, and H is undefined.
                // Using H = 0 is as good as any...
                s = 0;
                h = 0;
            }
            else
            {
                s = delta / max;
                if (Equals(r, max) == true)
                {
                    // Between Yellow and Magenta
                    h = (g - b) / delta;
                }
                else if (Equals(g, max) == true)
                {
                    // Between Cyan and Yellow
                    h = 2 + (b - r) / delta;
                }
                else
                {
                    // Between Magenta and Cyan
                    h = 4 + (r - g) / delta;
                }
            }

            // Scale h to be between 0 and 360. 
            // This may require adding 360, if the value
            // is negative.
            h *= 60;

            if (h < 0)
            {
                h += 360;
            }

            // Scale to the requirements of this 
            // application. All values are between 0 and 255.
            return new ColorHsv((int)h, (int)(s * 100), (int)(v * 100));
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", R, G, B);
        }
    }
}
