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
    public struct ColorHsv
    {
        public readonly int Hue; // 0-360
        public readonly int Saturation; // 0-100
        public readonly int Value; // 0-100

        public static bool operator ==(ColorHsv lhs, ColorHsv rhs)
        {
            return
                lhs.Hue == rhs.Hue &&
                lhs.Saturation == rhs.Saturation &&
                lhs.Value == rhs.Value;
        }

        public static bool operator !=(ColorHsv lhs, ColorHsv rhs)
        {
            return (lhs == rhs) == false;
        }

        public override bool Equals(object obj)
        {
            return this == (ColorHsv)obj;
        }

        public override int GetHashCode()
        {
            return (Hue + (Saturation << 8) + (Value << 16)).GetHashCode();
        }

        public ColorHsv(int hue, int saturation, int value)
        {
            if (hue < 0 || hue > 360)
            {
                throw new ArgumentOutOfRangeException("hue", "must be in the range [0, 360]");
            }

            if (saturation < 0 || saturation > 100)
            {
                throw new ArgumentOutOfRangeException("saturation", "must be in the range [0, 100]");
            }

            if (value < 0 || value > 100)
            {
                throw new ArgumentOutOfRangeException("value", "must be in the range [0, 100]");
            }

            this.Hue = hue;
            this.Saturation = saturation;
            this.Value = value;
        }

        public static ColorHsv FromColor(Color color)
        {
            return new ColorRgb(color.R, color.G, color.B).ToHsv();
        }

        public Color ToColor()
        {
            ColorRgb rgb = ToRgb();
            return Color.FromArgb(rgb.R, rgb.G, rgb.B);
        }

        public ColorRgb ToRgb()
        {
            // HsvColor contains values scaled as in the color wheel:

            double r = 0;
            double g = 0;
            double b = 0;

            // Scale Hue to be between 0 and 360. Saturation
            // and value scale to be between 0 and 1.
            double h = (double)this.Hue % 360;
            double s = (double)this.Saturation / 100;
            double v = (double)this.Value / 100;

            if (Equals(s, 0.0) == true)
            {
                // If s is 0, all colors are the same.
                // This is some flavor of gray.
                r = v;
                g = v;
                b = v;
            }
            else
            {
                // The color wheel consists of 6 sectors.
                // Figure out which sector you're in.
                double sectorPos = h / 60;
                var sectorNumber = (int)(Math.Floor(sectorPos));

                // get the fractional part of the sector.
                // That is, how many degrees into the sector
                // are you?
                double fractionalSector = sectorPos - sectorNumber;

                // Calculate values for the three axes
                // of the color. 
                double p = v * (1 - s);
                double q = v * (1 - (s * fractionalSector));
                double t = v * (1 - (s * (1 - fractionalSector)));

                // Assign the fractional colors to r, g, and b
                // based on the sector the angle is in.
                switch (sectorNumber)
                {
                    case 0:
                    {
                        r = v;
                        g = t;
                        b = p;
                        break;
                    }

                    case 1:
                    {
                        r = q;
                        g = v;
                        b = p;
                        break;
                    }

                    case 2:
                    {
                        r = p;
                        g = v;
                        b = t;
                        break;
                    }

                    case 3:
                    {
                        r = p;
                        g = q;
                        b = v;
                        break;
                    }

                    case 4:
                    {
                        r = t;
                        g = p;
                        b = v;
                        break;
                    }

                    case 5:
                    {
                        r = v;
                        g = p;
                        b = q;
                        break;
                    }
                }
            }

            // return an RgbColor structure, with values scaled
            // to be between 0 and 255.
            return new ColorRgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", Hue, Saturation, Value);
        }
    }
}
